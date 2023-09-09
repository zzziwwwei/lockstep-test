using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
public class PlayerData
{
    public int clientID;
    public int playerID;
    public string playerName;
    public string character;
}

public class PlayerHandler : MonoBehaviour
{
    public CharacterHandler characterHandler;
    public GameObject player;
    //public List<GameObject> players = new List<GameObject>();
    public List<GameObject> characters = new List<GameObject>();



    public void CreatPlayer(PlayerData playerData)
    {

        GameObject newcharacter = CreatCharacter(playerData.character);
        characters.Add(newcharacter);

        GameObject CreatCharacter(string characterName)
        {
            var character = characterHandler.ChooseCharacter(characterName);
            GameObject newCharacter = Instantiate(character, this.transform.position, Quaternion.identity);
            newCharacter.transform.SetParent(this.transform);
            return newCharacter;
        }
        Debug.Log("CreatPlayer");
    }

}

