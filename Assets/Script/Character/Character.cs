using UnityEngine.Events;
abstract class Character
{
    public string name;
    public int playerId;
    public UnityEvent<KeyLog> channel;
    
}
