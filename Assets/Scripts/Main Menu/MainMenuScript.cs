using System;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Reset game statics
        GameStatics.NewGame = true;
        GameStatics.LoadedGame = new GameData();
        GameStatics.GameGuid = Guid.Empty.ToString();
    }
}
