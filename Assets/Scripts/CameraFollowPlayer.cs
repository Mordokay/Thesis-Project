using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {

    Vector3 startPos;
    Vector3 endPos;
    bool isMoving;
    float t;
    public float moveSpeed;
    GameManager gameManager;

    void Start()
    {
        this.transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
        this.transform.Translate(new Vector3(0, 0, -10));
        startPos = this.transform.position;
        isMoving = false;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public IEnumerator move(float x, float y)
    {
        isMoving = true;
        startPos = this.transform.position;
        t = 0;

        endPos = new Vector3(startPos.x + x, startPos.y + y, this.transform.position.z);

        while (t <= 1f)
        {
            t += Time.deltaTime * (moveSpeed / gameManager.gridSize);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        isMoving = false;
        yield return 0;
    }

    void Update()
    {
        /*
        if (Input.GetKey(KeyCode.W) && !isMoving && !gameManager.grid[(int)this.transform.position.x, (int)(this.transform.position.y + 1.0f)].IsWall)
        {
            StartCoroutine(move(0f, 1.0f));
        }
        if (Input.GetKey(KeyCode.A) && !isMoving && !gameManager.grid[(int)(this.transform.position.x - 1), (int)this.transform.position.y].IsWall)
        {
            StartCoroutine(move(-1.0f, 0.0f));
        }
        if (Input.GetKey(KeyCode.S) && !isMoving && !gameManager.grid[(int)this.transform.position.x, (int)(this.transform.position.y - 1.0f)].IsWall)
        {
            StartCoroutine(move(0f, -1.0f));
        }
        else if (Input.GetKey(KeyCode.D) && !isMoving && !gameManager.grid[(int)(this.transform.position.x + 1), (int)this.transform.position.y].IsWall)
        {
            StartCoroutine(move(1.0f, 0.0f));
        }
        */
    }
}
