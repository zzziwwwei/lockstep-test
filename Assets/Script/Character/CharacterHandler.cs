using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
    public GameObject enchanter;
    public GameObject asdasdas;

    public GameObject ChooseCharacter(string characterName)
    {
        GameObject character;
        switch (characterName)
        {
            case "enchanter":
                character = enchanter;
                break;
        }
        return enchanter;
    }
}
