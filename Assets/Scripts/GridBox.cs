using UnityEngine;
using System.Collections;

public class GridBox : MonoBehaviour {

	private GameManager Game;
    public bool isWall;
    public bool isWatter;

    void Start () {

        Game = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

    }
}
