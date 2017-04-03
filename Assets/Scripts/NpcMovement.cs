using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NpcMovement : MonoBehaviour {
	

	private GameManager gameManager;
    
	public MyPathNode nextNode;
    public MyPathNode[,] grid;

    public Sprite npcUp;
    public Sprite npcDown;
    public Sprite npcFront;
    public Sprite npcBack;

    public List<Action> myActions;

    public gridPosition currentGridPosition = new gridPosition();
	public gridPosition startGridPosition = new gridPosition();
	public gridPosition endGridPosition = new gridPosition();
	
	private Orientation gridOrientation = Orientation.Vertical;
	private bool allowDiagonals = false;
	private bool correctDiagonalSpeed = true;
	private Vector2 input;
	//private bool isMoving = false;
	private Vector3 startPosition;
	private Vector3 endPosition;
	private float t;
	private float factor;
	public Color myColor;

    public int currentAction = 0;
    public float currentStayTime = 0.0f;
    private bool configuredWalking = false;
    public bool isWaiting;

    IEnumerable<MyPathNode> currentPath;

    public enum actionType
    {
        walk,
        stay
    };

    [Serializable]
    public class Action
    {
        //[SerializeField]
        //public actionType type;
        //[SerializeField]
        //public float duration;
        //[SerializeField]
        //public Vector2 startPos;
        //[SerializeField]
        //public Vector2 endPos;

        [SerializeField]
        public Vector2 pos;
        [SerializeField]
        public float wait;
    }

	public class MySolver<TPathNode, TUserContext> : SettlersEngine.SpatialAStar<TPathNode, 
	TUserContext> where TPathNode : SettlersEngine.IPathNode<TUserContext>
	{
		protected override Double Heuristic(PathNode inStart, PathNode inEnd)
		{


			int formula = GameManager.distance;
			int dx = Math.Abs (inStart.X - inEnd.X);
			int dy = Math.Abs(inStart.Y - inEnd.Y);

			if(formula == 0)
				return Math.Sqrt(dx * dx + dy * dy); //Euclidean distance

			else if(formula == 1)
				return (dx * dx + dy * dy); //Euclidean distance squared

			else if(formula == 2)
				return Math.Min(dx, dy); //Diagonal distance

			else if(formula == 3)
				return (dx*dy)+(dx + dy); //Manhatten distance

		

			else 
				return Math.Abs (inStart.X - inEnd.X) + Math.Abs (inStart.Y - inEnd.Y);

			//return 1*(Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y) - 1); //optimized tile based Manhatten
			//return ((dx * dx) + (dy * dy)); //Khawaja distance
		}
		
		protected override Double NeighborDistance(PathNode inStart, PathNode inEnd)
		{
			return Heuristic(inStart, inEnd);
		}

		public MySolver(TPathNode[,] inGrid)
			: base(inGrid)
		{
		}
	} 



	// Use this for initialization
	void Start () {

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        isWaiting = false;

        //myColor = getRandomColor();
        //this.GetComponent<Renderer>().material.color = myColor;

        /*
		startGridPosition = new gridPosition(0,UnityEngine.Random.Range(0, gameManager.gridHeight-1));
		endGridPosition = new gridPosition(gameManager.gridWidth-1,UnityEngine.Random.Range(0, gameManager.gridHeight-1));
		initializePosition ();


		MySolver<MyPathNode, System.Object> aStar = new MySolver<MyPathNode, System.Object>(gameManager.grid);
        IEnumerable<MyPathNode> path = aStar.Search(new Vector2(startGridPosition.x, startGridPosition.y), new Vector2(endGridPosition.x, endGridPosition.y), null);
        
		foreach(GameObject g in GameObject.FindGameObjectsWithTag("GridBox"))
		{
			g.GetComponent<Renderer>().material.color = Color.white;
		}
        

        updatePath();

		*/
    }

    void ResetPathColor() {
        if (currentPath != null)
        {
            foreach (MyPathNode node in currentPath)
            {
                //Debug.Log("Resetting color on " + node.X + "," + node.Y);
                //GameObject.Find(node.X + "," + node.Y).GetComponent<Renderer>().material.color = Color.white;
            }
        }
    }

    public void findUpdatedPath(int currentX,int currentY)
	{


		MySolver<MyPathNode, System.Object> aStar = new MySolver<MyPathNode, System.Object>(gameManager.grid);
		currentPath = aStar.Search(new Vector2(currentX, currentY), new Vector2(endGridPosition.x, endGridPosition.y), null);


		int x = 0;

		if (currentPath != null) {
		
			foreach (MyPathNode node in currentPath)
			{
				if(x==1)
				{
					nextNode = node;
					break;
				}

				x++;

			}

            
			foreach(GameObject g in GameObject.FindGameObjectsWithTag("GridBox"))
			{
				if(g.GetComponent<Renderer>().material.color == myColor)
					g.GetComponent<Renderer>().material.color = Color.white;
			}

			foreach (MyPathNode node in currentPath)
			{
                //GameObject.Find(node.X + "," + node.Y).GetComponent<Renderer>().material.color = myColor;
            }
		}
		
	}


	Color getRandomColor()
	{
		Color tmpCol = new Color(UnityEngine.Random.Range(0f,1f),UnityEngine.Random.Range(0f,1f),UnityEngine.Random.Range(0f,1f));
		return tmpCol;

	}

    void Update()
    {
        if (currentAction == myActions.Count)
        {
            currentAction = 0;
        }
        else if (myActions.Count > 0)

            if (isWaiting == true)
            {
                currentStayTime += Time.deltaTime;
                if (currentStayTime >= myActions[currentAction].wait)
                {
                    //After player moves to endGridPosition he waits X seconds before starting next action
                    currentAction++;

                    Debug.Log(currentAction);
                    currentStayTime = 0.0f;
                    isWaiting = false;
                }
            }
            else if (!configuredWalking && !isWaiting)
            {
                configuredWalking = true;

                startGridPosition = new gridPosition((int)this.transform.position.x, (int)this.transform.position.y);
                endGridPosition = new gridPosition((int)myActions[currentAction].pos.x, (int)myActions[currentAction].pos.y);
                initializePosition();
                updatePath();
                getNextMovement();
                //StartCoroutine(move());

                //Debug.Log("currentAction: " + currentAction + " startGridPosition ( " + startGridPosition.x + " , " + startGridPosition.y + " )" + " endGridPosition ( " + endGridPosition.x + " , " + endGridPosition.y + " )");
            }
            else if (this.transform.position.x == endGridPosition.x && this.transform.position.y == endGridPosition.y)
            {
                //Debug.Log("terminatedWalk ...  startGridPosition ( " + currentGridPosition.x + " , " + currentGridPosition.y + " )" + " endGridPosition ( " + endGridPosition.x + " , " + endGridPosition.y + " )");
                isWaiting = true;
                configuredWalking = false;
                StopAllCoroutines();
            }
        /*
        if (currentAction == myActions.Count)
        {
            currentAction = 0;
        }
        else if (myActions.Count > 0)
        {
            switch (myActions[currentAction].type)
            {
                case actionType.stay:
                    //Debug.Log("currentAction: " + currentAction);
                    if (currentStayTime >= myActions[currentAction].duration)
                    {
                        currentAction++;
                        currentStayTime = 0.0f;
                    }
                    else
                    {
                        currentStayTime += Time.deltaTime;
                    }
                    break;
                case actionType.walk:

                    if (!configuredWalking)
                    {
                        configuredWalking = true;

                        startGridPosition = new gridPosition((int)myActions[currentAction].startPos.x, (int)myActions[currentAction].startPos.y);
                        endGridPosition = new gridPosition((int)myActions[currentAction].endPos.x, (int)myActions[currentAction].endPos.y);
                        initializePosition();
                        updatePath();
                        getNextMovement();
                        //StartCoroutine(move());

                        //Debug.Log("currentAction: " + currentAction + " startGridPosition ( " + startGridPosition.x + " , " + startGridPosition.y + " )" + " endGridPosition ( " + endGridPosition.x + " , " + endGridPosition.y + " )");
                    }

                    if (this.transform.position.x == endGridPosition.x && this.transform.position.y == endGridPosition.y)
                    {
                        //Debug.Log("terminatedWalk ...  startGridPosition ( " + currentGridPosition.x + " , " + currentGridPosition.y + " )" + " endGridPosition ( " + endGridPosition.x + " , " + endGridPosition.y + " )");
                        currentAction++;
                        configuredWalking = false;
                        StopAllCoroutines();
                    }
                    break;
            }
        }
        */
    }
    
	public float moveSpeed;
	
    [Serializable]
	public class gridPosition{
        [SerializeField]
        public int x =0;
        [SerializeField]
        public int y=0;

		public gridPosition()
		{
		}

		public gridPosition (int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	};
	
	
	private enum Orientation {
		Horizontal,
		Vertical
	};

	
	public IEnumerator move() {
		//isMoving = true;
		startPosition = transform.position;
		t = 0;

		if(gridOrientation == Orientation.Horizontal) {
			endPosition = new Vector2(startPosition.x + System.Math.Sign(input.x) * gameManager.gridSize,
			                          startPosition.y);
			currentGridPosition.x += System.Math.Sign(input.x);
		} else {
			endPosition = new Vector2(startPosition.x + System.Math.Sign(input.x) * gameManager.gridSize,
			                          startPosition.y + System.Math.Sign(input.y) * gameManager.gridSize);
			
			currentGridPosition.x += System.Math.Sign(input.x);
			currentGridPosition.y += System.Math.Sign(input.y);
		}
		
		if(allowDiagonals && correctDiagonalSpeed && input.x != 0 && input.y != 0) {
			factor = 0.9071f;
		} else {
			factor = 1f;
		}

	
		while (t <= 1f) {
			t += Time.deltaTime * (moveSpeed/ gameManager.gridSize) * factor;
			transform.position = Vector2.Lerp(startPosition, endPosition, t);
            //Debug.Log("startPosition: " + startPosition + " endPosition: " + endPosition);
			yield return null;
		}

		//isMoving = false;
		getNextMovement ();
		
		yield return 0;
		
	}
	
	void updatePath()
	{
        ResetPathColor();
        findUpdatedPath (currentGridPosition.x, currentGridPosition.y);
	}
	
	void getNextMovement()
	{
		updatePath();
		
		input.x = 0;
		input.y = 0;
		if (nextNode.X > currentGridPosition.x) {
			input.x = 1;
			this.GetComponent<SpriteRenderer>().sprite = npcFront;
		}
		if (nextNode.Y > currentGridPosition.y) {
			input.y = 1;
			this.GetComponent<SpriteRenderer>().sprite = npcUp;
		}
		if (nextNode.Y < currentGridPosition.y) {
			input.y = -1;
			this.GetComponent<SpriteRenderer>().sprite = npcDown;
		}
		if (nextNode.X < currentGridPosition.x) {
			input.x = -1;
			this.GetComponent<SpriteRenderer>().sprite = npcBack;
		}
		
		StartCoroutine (move ());
	}

    public Vector2 getGridPosition(int x, int y)
	{
		float posX = gameManager.gridBox.transform.position.x + (gameManager.gridSize*x);
		float posY = gameManager.gridBox.transform.position.y + (gameManager.gridSize*y);
		return new Vector2(posX,posY);	
		
	}
	
	
	public void initializePosition()
	{
		this.gameObject.transform.position = getGridPosition (startGridPosition.x, startGridPosition.y);
		currentGridPosition.x = startGridPosition.x;
		currentGridPosition.y = startGridPosition.y;
		//isMoving = false;
		//GameObject.Find(startGridPosition.x + "," + startGridPosition.y).GetComponent<Renderer>().material.color = Color.black; 

	}
}
