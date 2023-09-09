using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

class WorldSetting
{

}
public class InGame : MonoBehaviour
{
    public PlayerHandler playerHandler;
    public GameLog gameLog;

    Queue<GameObject> saboteurPool = new();
    List<Saboteur> saboteurs = new();

    public GameObject saboteur;
    int creat_saboteur_time;
    public GameObject block;

    List<UnityEvent<OnHit>> onHit_channel = new();

    public class Saboteur
    {
        InGame inGame;
        public GameObject saboteur;
        Vector3 position;
        RaycastHit hit;
        public int survivaltime;
        int direction;
        float attackLength = 20f;

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
            if (this.survivaltime > 30)
            {
                this.saboteur.transform.position = p + new Vector3(this.direction * 0.1f, 0, 0);
            }
            else if (this.survivaltime <= 30 && this.survivaltime > 20)
            {

            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(p, Vector2.down, 10);
                if (hit.collider != null)
                {
                    Line(p, new Vector3(hit.point.x, hit.point.y, p.z), this.survivaltime);
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
        timer = createTimer();
        for (int i = 0; i < 10; i++)//預載入物件池大小
        {
            AddObjectInPool();
        }


    }
    bool down;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            block.SetActive(true);
            down = true;
        }
        if (down)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            block.transform.position = new Vector3(worldMousePosition.x, worldMousePosition.y, 0);
        }
        if (Input.GetMouseButtonUp(0))
        {
            block.SetActive(false);
            down = false;
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
            onHit_channel.Add(new UnityEvent<OnHit>());
            player.GetComponent<Controller>().onHit_channel.AddListener(On_Hit);

        }


        Test_SpawnPoint();

    }

    void On_Hit(OnHit arg0)
    {
        Debug.Log("onhit" + arg0.onHitID);
        if (arg0.on)
        {
            ChangeColor(players[arg0.onHitID].transform.GetChild(0), 255, 0, 0, 200);
            ChangeColor(players[arg0.onHitID].transform.GetChild(1), 255, 0, 0, 200);
        }
        else
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
