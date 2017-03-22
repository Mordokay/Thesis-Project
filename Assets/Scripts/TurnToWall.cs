using UnityEngine;
using System.Collections;

public class TurnToWall : MonoBehaviour {

	private GameManager Game;

	// Use this for initialization
	void Start () {

        Game = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

    }

	public bool isWall;
	void OnMouseDown()
	{
		string [] splitter = this.gameObject.name.Split (',');
		if(!isWall)
		{
			Game.addWall(int.Parse(splitter[0]),int.Parse(splitter[1]));
			isWall = true;
			this.GetComponent<SpriteRenderer>().color = Color.red;
		}
		else
		{
			Game.removeWall(int.Parse(splitter[0]),int.Parse(splitter[1]));
			isWall = false;
			this.GetComponent<SpriteRenderer>().color = Color.white;
		}
	}

    public void AddWall()
    {

    }
}
