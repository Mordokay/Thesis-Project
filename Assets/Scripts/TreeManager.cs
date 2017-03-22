using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour {

    RaycastHit2D hit;
    public float viewDistance;

    public List<GameObject> npcThatCanSee;

    public GameObject treeFull;
    public GameObject treeCut;

    public GameObject lineNpcEvent;
    public List<GameObject> linesToNpc;

    GameManager gameManager;

    void Start()
    {
        for(int i = 0; i < npcThatCanSee.Count; i++)
        {
            GameObject lineToNPC = Instantiate(lineNpcEvent);
            linesToNpc.Add(lineToNPC);
        }
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    bool LineOfSight(Transform target)
    {
        hit = Physics2D.Linecast(transform.position + (target.position - transform.position).normalized, target.position);
        if (hit && hit.collider.transform == target)
            {

                return true;
            }
        return false;
    }

    void OnMouseDown()
    {
        Debug.Log("Touching tree");
        if (treeFull.activeSelf)
        {
            breakTree();
        }
    }

    void breakTree()
    {
        treeFull.SetActive(false);
        treeCut.SetActive(true);

        //Event still happens even if no NPC is around to see it
        int eventId = gameManager.getNewId();

        foreach (GameObject npc in npcThatCanSee)
        {
            if (LineOfSight(npc.transform) && Vector3.Distance(transform.position, npc.transform.position) < npc.GetComponent<Social>().viewDistance)
            {
                //Warn villager of event
                Debug.Log(npc.name + " saw player breaking a tree!");
                if (npc.GetComponent<Social>().interests.Contains(Social.interest.gatheringWood)){
                    npc.GetComponent<Social>().information.Add(new Social.Message(Social.interest.gatheringWood, 1, "Player gathered some wood", eventId));
                    StartCoroutine(npc.GetComponent<Social>().ShowBaloon());
                }
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < npcThatCanSee.Count; i++)
        {
            if (LineOfSight(npcThatCanSee[i].transform) && Vector3.Distance(transform.position, npcThatCanSee[i].transform.position) < npcThatCanSee[i].GetComponent<Social>().viewDistance)
            {
                linesToNpc[i].SetActive(true);
                linesToNpc[i].GetComponent<LineRenderer>().SetPosition(0, new Vector3(transform.position.x, transform.position.y, -1));
                linesToNpc[i].GetComponent<LineRenderer>().SetPosition(1, new Vector3(npcThatCanSee[i].transform.position.x, npcThatCanSee[i].transform.position.y, -1));
                //Debug.DrawRay(transform.position, npcThatCanSee[i].transform.position - transform.position, Color.red);
            }
            else
            {
                linesToNpc[i].SetActive(false);
            }
        }
            
    }
}
