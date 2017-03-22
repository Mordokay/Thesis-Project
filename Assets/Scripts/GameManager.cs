using UnityEngine;
using System;
using System.Text;
using System.IO;

public class GameManager : MonoBehaviour {

    public int messageId = 0;
	public MyPathNode[,] grid;
    public GameObject npc;
    public GameObject gridBox;
    public GameObject map;
    public int gridWidth;
	public int gridHeight;
	public float gridSize;
	public GUIStyle lblStyle;

    public string mapToLoad;

	public static string distanceType;
	

	//This is what you need to show in the inspector.
	public static int distance = 2;


	void Start () {

        //LoadMap();
        bakeObstacles();

        /*
        //Generate a grid - nodes according to the specified size
        grid = new MyPathNode[gridWidth, gridHeight];

		for (int x = 0; x < gridWidth; x++) {
			for (int y = 0; y < gridHeight; y++) {
				//Boolean isWall = ((y % 2) != 0) && (rnd.Next (0, 10) != 8);
				Boolean isWall = false;
				grid [x, y] = new MyPathNode ()
				{
					IsWall = isWall,
					X = x,
					Y = y,
				};
			}
		}
        */
        //instantiate grid gameobjects to display on the scene
        //createGrid ();

        //instantiate enemy object
        //createEnemy ();


    }

    /*
	void OnGUI()
	{
		if(GUI.Button(new Rect(0f,0f,200f,50f),"Create Enemy"))
		{
			createEnemy();
		}
		if(GUI.Button(new Rect(0f,60f,200f,50f),"Reload"))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		GUI.Label(new Rect(5f,120f,200f,200f),"Click on the grid to place a wall/tower.\nYou can change the distance formula of the path to Euclidean, " +
			"Manhattan etc\nYou can also change the Grid size in the GameManager variables from the inspector",lblStyle);
	}
    */

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

			}
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

    public void LoadMap()
    {
        removeGrid();
        int[,] myMap;

        try
        {
            StreamReader theReader = new StreamReader("Assets/Resources/" + mapToLoad + ".txt", Encoding.Default);
            string line = theReader.ReadLine();
            string[] entries = line.Split(',');

            gridWidth = int.Parse(entries[0]);
            gridHeight = int.Parse(entries[1]);

            myMap = new int[gridWidth, gridHeight];
            grid = new MyPathNode[gridWidth, gridHeight];

            using (theReader)
            {
                int currentLine = gridHeight - 1;
                while ((line = theReader.ReadLine()) != null)
                {
                    entries = line.Split(',');
                    for(int i = 0; i < gridWidth; i++)
                    {
                        myMap[i, currentLine] = int.Parse(entries[i]);
                    }
                    currentLine--;
                }

                theReader.Close();
            }

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    GameObject box = (GameObject)GameObject.Instantiate(gridBox);
                    box.transform.position = new Vector2(gridBox.transform.position.x + (gridSize * x), gridBox.transform.position.y + (gridSize * y));
                    box.name = x + "," + y;
                    box.gameObject.transform.parent = map.transform;

                    switch (myMap[x, y])
                    {
                        case 0:
                            box.GetComponent<TurnToWall>().isWall = false;
                            box.GetComponent<SpriteRenderer>().color = Color.white;
                            grid[x, y] = new MyPathNode()
                            {
                                IsWall = false,
                                X = x,
                                Y = y,
                            };
                            break;
                        case 1:
                            box.GetComponent<TurnToWall>().isWall = true;
                            box.GetComponent<SpriteRenderer>().color = Color.red;
                            grid[x, y] = new MyPathNode()
                            {
                                IsWall = true,
                                X = x,
                                Y = y,
                            };
                            break;
                    }
                }
            }

        }
        catch (Exception e)
        {
            Console.WriteLine("{0}\n", e.Message);
        }

        createEnemy();
    }

    
    public void bakeObstacles()
    {
        grid = new MyPathNode[gridWidth, gridHeight];

        foreach (Transform gridBox in map.transform)
        {
            string[] entries = gridBox.gameObject.name.Split(',');
            grid[int.Parse(entries[0]), int.Parse(entries[1])] = new MyPathNode()
            {
                IsWall = gridBox.gameObject.GetComponent<TurnToWall>().isWall,
                X = int.Parse(entries[0]),
                Y = int.Parse(entries[1]),
            };
        }
        /*
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {

                //Boolean isWall = ((y % 2) != 0) && (rnd.Next (0, 10) != 8);
                Boolean isWall = false;
                grid[x, y] = new MyPathNode()
                {
                    IsWall = isWall,
                    X = x,
                    Y = y,
                };
            }
        }*/
    }

    public void createEnemy()
	{
		GameObject nb = (GameObject)GameObject.Instantiate (npc);
		nb.SetActive (true);
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
