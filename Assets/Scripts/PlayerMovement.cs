using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    Vector3 startPos;
    Vector3 endPos;
    bool isMoving;
    float t;
    public float moveSpeed;
    GameManager gameManager;

    void Start () {
        startPos = this.transform.position;
        isMoving = false;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public IEnumerator move(float x, float y)
    {
        isMoving = true;
        startPos = transform.position;
        t = 0;

        endPos = new Vector2(startPos.x +  x, startPos.y + y);

        while (t <= 1f)
        {
            t += Time.deltaTime * (moveSpeed / gameManager.gridSize);
            transform.position = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        isMoving = false;
        this.GetComponent<Animator>().SetTrigger("Idle");
        yield return 0;
    }

    void Update() {
        //!gameManager.grid[(int)this.transform.position.x, (int)(this.transform.position.y + 1.0f)].IsWater
        if (Input.GetKey(KeyCode.W) && !isMoving && !gameManager.grid[(int)this.transform.position.x, (int)(this.transform.position.y + 1.0f)].IsWall &&
            !gameManager.grid[(int)this.transform.position.x, (int)(this.transform.position.y + 1.0f)].IsWater)
        {
            StartCoroutine(move(0f, 1.0f));
            StartCoroutine(Camera.main.GetComponent<CameraFollowPlayer>().move(0f, 1.0f));
            this.GetComponent<Animator>().SetTrigger("WalkUp");
        }
        if (Input.GetKey(KeyCode.A) && !isMoving && !gameManager.grid[(int)(this.transform.position.x - 1), (int)this.transform.position.y].IsWall &&
            !gameManager.grid[(int)(this.transform.position.x - 1), (int)this.transform.position.y].IsWater)
        {
            StartCoroutine(move(-1.0f, 0.0f));
            StartCoroutine(Camera.main.GetComponent<CameraFollowPlayer>().move(-1.0f, 0.0f));
            this.GetComponent<Animator>().SetTrigger("WalkLeft");
        }
        if (Input.GetKey(KeyCode.S) && !isMoving && !gameManager.grid[(int)this.transform.position.x, (int)(this.transform.position.y - 1.0f)].IsWall &&
            !gameManager.grid[(int)this.transform.position.x, (int)(this.transform.position.y - 1.0f)].IsWater)
        {
            StartCoroutine(move(0f, -1.0f));
            StartCoroutine(Camera.main.GetComponent<CameraFollowPlayer>().move(0f, -1.0f));
            this.GetComponent<Animator>().SetTrigger("WalkDown");
        }
        else if (Input.GetKey(KeyCode.D) && !isMoving && !gameManager.grid[(int)(this.transform.position.x + 1), (int)this.transform.position.y].IsWall &&
            !gameManager.grid[(int)(this.transform.position.x + 1), (int)this.transform.position.y].IsWater)
        {
            StartCoroutine(move(1.0f, 0.0f));
            StartCoroutine(Camera.main.GetComponent<CameraFollowPlayer>().move(1.0f, 0.0f));
            this.GetComponent<Animator>().SetTrigger("WalkRight");
        }
    }
}