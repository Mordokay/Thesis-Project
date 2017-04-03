using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {

    public int messageId = 0;
	public MyPathNode[,] grid;
    public GameObject gridBox;

    public List<Sprite> groundSprites;
    
    public GameObject map;
    public int gridWidth;
	public int gridHeight;
	public float gridSize;
	public static string distanceType;

	public static int distance = 2;

	void Start () {
        bakeObstacles();
    }

    public int getNewId()
    {
        messageId++;
        return messageId;
    }
	public void createGrid()
	{
        removeGrid();
        for (int i = 0; i<gridHeight; i++) {
			for (int j =0; j<gridWidth; j++) {
				GameObject nobj = (GameObject)GameObject.Instantiate(gridBox);
				nobj.transform.position = new Vector2(gridBox.transform.position.x + (gridSize*j), gridBox.transform.position.y + (gridSize * i));
				nobj.name = j+","+i;

				nobj.gameObject.transform.parent = map.transform;
				nobj.SetActive(true);
                PaintBlock(nobj);
			}
		}
	}

    void PaintBlock(GameObject myGridBox)
    {
        if(groundSprites.Count > 0)
        {
            myGridBox.GetComponent<SpriteRenderer>().sprite = groundSprites[UnityEngine.Random.Range(0, groundSprites.Count)];
        }
    }

    public void removeGrid()
    {
        while(map.transform.childCount > 0)
        {
            //Debug.Log(map.transform.childCount);
            Transform child = map.transform.GetChild(0);
            child.parent = null;
            DestroyImmediate(child.gameObject);
        }
    }

    public void bakeObstacles()
    {
        grid = new MyPathNode[gridWidth, gridHeight];

        foreach (Transform gridBox in map.transform)
        {
            string[] entries = gridBox.gameObject.name.Split(',');
            grid[int.Parse(entries[0]), int.Parse(entries[1])] = new MyPathNode()
            {
                IsWall = gridBox.gameObject.GetComponent<GridBox>().isWall,
                IsWater = gridBox.gameObject.GetComponent<GridBox>().isWatter,
                X = int.Parse(entries[0]),
                Y = int.Parse(entries[1]),
            };
        }
    }

	public void addWall (int x, int y)
	{
		grid [x, y].IsWall = true;
	}
	
	public void removeWall (int x, int y)
	{
		grid [x, y].IsWall = false;
	}

}
