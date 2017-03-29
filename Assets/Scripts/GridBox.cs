using UnityEngine;
using System.Collections;

public class GridBox : MonoBehaviour {

	private GameManager Game;
    public bool isWall;

    void Start () {

        Game = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

    }
}
