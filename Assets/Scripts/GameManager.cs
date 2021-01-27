using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DIRECTION
{
    
    DOWN,
    LEFT,
    RIGHT,
    UP,

}
public class GameManager : MonoBehaviour
{
    
    public StageManager stage = default;
    //public PlayerManager player = default;
    // Start is called before the first frame update
    void Start()
    {
        stage.LoadTileData(0);
        stage.CreateStage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
