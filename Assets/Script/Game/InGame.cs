using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;


interface IOnHit_Raycast
{
    public void OnTrigger();
    public void LeaveTrigger();
    void EnterTrigger();
}


public class InGame : MonoBehaviour
{
    public PlayerHandler playerHandler;
    public GameLog gameLog;

    Queue<GameObject> saboteurPool = new();
    List<Saboteur> saboteurs = new();
    bool gameStart;

    public GameObject saboteur;
    int creat_saboteur_time;
    public GameObject block;
    Queue<GameObject> blockPool = new();
    List<UnityEvent<OnHitEventData>> onHit_channel = new();

    public class Saboteur
    {
        InGame inGame;
        public GameObject saboteur;
        Vector3 position;
        RaycastHit hit;
        public int survivaltime;
        int direction;
        float attackLength = 20f;
        bool onHit;
        IOnHit_Raycast onHit_Raycast;
        public Saboteur(GameObject saboteur, Vector3 position)
        {
            this.saboteur = saboteur;
            this.position = position;
            Init();
        }
        void Init()
        {
            saboteur.transform.position = position;
            this.survivaltime = (int)UnityEngine.Random.Range(60, 120);
            this.direction = (int)UnityEngine.Random.Range(-1, 2);
            this.saboteur.SetActive(true);
        }
        public void ReSet() //物件銷毀
        {
            this.saboteur.SetActive(false);
            onHit_Raycast?.LeaveTrigger();
        }
        void Line(Vector3 s, Vector3 e, int t)
        {
            Vector3[] line = new Vector3[2];
            line[0] = s;
            line[1] = e;
            LineRenderer lineRenderer = this.saboteur.GetComponent<LineRenderer>();
            lineRenderer.SetPositions(line);
            lineRenderer.startWidth = 0.05f * t;
            lineRenderer.endWidth = 0.05f * t;
        }
        public void Update()
        {
            var p = this.saboteur.transform.position;
            if (this.survivaltime > 40)
            {
                this.saboteur.transform.position = p + new Vector3(this.direction * 0.1f, 0, 0);
            }
            else if (this.survivaltime <= 40 && this.survivaltime > 20)
            {
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(p, Vector2.down, 10);
                if (hit.collider != null)
                {
                    var current = hit.collider.gameObject.GetComponent<IOnHit_Raycast>();
                    Line(p, new Vector3(hit.point.x, hit.point.y, p.z), this.survivaltime);
                    if (current != onHit_Raycast)
                    {
                        current?.EnterTrigger();
                        onHit_Raycast?.LeaveTrigger();
                    }
                    onHit_Raycast = current;
                    current?.OnTrigger();
                }
                else
                {
                    onHit_Raycast = null;
                }

            }

            Debug.DrawRay(p, Vector2.down * 10, Color.red);
            //Debug.Log("Hit object: " + hit.collider.gameObject.name);
            this.survivaltime--;
        }
    }
    public float UseRandom(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    List<GameObject> players = new();

    void Start()
    {
        GameData.startGame += () => gameStart = true;
        timer = createTimer();
        for (int i = 0; i < 10; i++)//預載入物件池大小
        {
            AddObjectInPool();
            blockPool.Enqueue(Instantiate(block, new Vector3(0, 0, 0), Quaternion.identity));
        }


    }
    bool down;
    Dictionary<int, GameObject> toucheToBlock = new();
    void Update()
    {
        Touch[] touches = Input.touches;
        foreach (Touch touch in touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                var o = blockPool.Dequeue();
                toucheToBlock.Add(touch.fingerId, o);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchPosition = touch.position;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, 0));
                toucheToBlock[touch.fingerId].SetActive(true);
                toucheToBlock[touch.fingerId].transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {

                toucheToBlock[touch.fingerId].SetActive(false);
                blockPool.Enqueue(toucheToBlock[touch.fingerId]);
                toucheToBlock.Remove(touch.fingerId);
            }
        }

    }
    void CreatSaboteur()
    {
        if (saboteurPool.Count <= 0)
        {
            AddObjectInPool();
        }
        GameObject o = saboteurPool.Dequeue();
        saboteurs.Add(new Saboteur(o, new Vector3(UseRandom(-8, 8), 5f, 0)));
    }

    void AddObjectInPool()
    {
        GameObject @object = Instantiate(saboteur, new Vector3(0, 0, 0), Quaternion.identity);
        @object.SetActive(false);
        saboteurPool.Enqueue(@object);
    }
    // Update is called once per frame
    public void Init()
    {
        foreach (var player in playerHandler.characters)
        {
            this.players.Add(player);
            onHit_channel.Add(new UnityEvent<OnHitEventData>());
            player.GetComponent<Controller>().onHit_channel.AddListener(OnHit);

        }

        Test_SpawnPoint();

    }

    void OnHit(OnHitEventData arg0)
    {
        Debug.Log("onhit" + arg0.onHitID);
        if (arg0.trigger == OnHitEventData.Trigger.Enter)
        {
            ChangeColor(players[arg0.onHitID].transform.GetChild(0), 255, 0, 0, 200);
            ChangeColor(players[arg0.onHitID].transform.GetChild(1), 255, 0, 0, 200);
        }
        if (arg0.trigger == OnHitEventData.Trigger.Leave)
        {
            ChangeColor(players[arg0.onHitID].transform.GetChild(0), 0, 227, 255, 200);
            ChangeColor(players[arg0.onHitID].transform.GetChild(1), 0, 227, 255, 200);
        }
    }


    void On_Hit()
    {

    }
    Func<int> timer;

    void FixedUpdate()
    {
        if (gameStart)
        {


            if (timer() > 0)
            {
                timer();
            }
            else
            {
                timer = createTimer();
                CreatSaboteur();
            }


            for (int i = 0; i < saboteurs.Count; i++)
            {
                saboteurs[i].Update();
                if (saboteurs[i].survivaltime < 0)
                {
                    saboteurs[i].ReSet();
                    saboteurPool.Enqueue(saboteurs[i].saboteur);
                    saboteurs.Remove(saboteurs[i]);
                    i--;
                }
            }
        }
    }

    Func<int> createTimer()
    {
        int time = UnityEngine.Random.Range(30, 120);
        Func<int> reduce = () =>
            {
                time--;
                return time;
            };
        return reduce;
    }

    void Test_SpawnPoint()
    {
        players[0].transform.position = players[0].transform.position + new Vector3(0, -5.25f, 0);
        players[1].transform.position = players[1].transform.position + new Vector3(0, -5.25f, 1);
        players[1].transform.GetComponent<BoxCollider2D>().enabled = false;
        ChangeColor(players[1].transform.GetChild(1), 0, 38, 255, 200);
        ChangeColor(players[1].transform.GetChild(0), 0, 38, 255, 200);

    }
    void ChangeColor(Transform o, float r, float g, float b, float a)
    {
        float red = (float)r / 255;
        float green = (float)g / 255;
        float blue = (float)b / 255;
        float alpha = (float)a / 255;
        SpriteRenderer renderer = o.GetComponent<SpriteRenderer>();
        renderer.color = new Color(red, green, blue, alpha);
    }


}
