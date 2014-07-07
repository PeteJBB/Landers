using UnityEngine;
using System.Collections;

public class GameBrain : MonoBehaviour
{
    public static GameView CurrentView = GameView.Internal;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public enum GameView
{
    Internal,
    External,
    Map,
    Menu
}