using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectGathererController : MonoBehaviour {

    RaycastHit2D hit;

    public float viewDistance;
    public List<GameObject> npcThatCanSee;
    public GameObject ObjectFull;
    public GameObject ObjectGathered;
    public GameObject lineNpcEvent;

    List<GameObject> linesToNpc;

    public List<Social.interest> interestTypes;

    public string actionMessage;
    public string talkMessage;
    GameManager gameManager;

    public objectType whoAmI;

    public enum objectType
    {
        simpleTree,
        berries
    };

    void Start()
    {
        linesToNpc = new List<GameObject>();
        npcThatCanSee = GameObject.FindGameObjectsWithTag("NPC").ToList<GameObject>();
        for (int i = 0; i < npcThatCanSee.Count; i++)
        {
            GameObject lineToNPC = Instantiate(lineNpcEvent);
            lineToNPC.transform.parent = lineNpcEvent.transform.parent;
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
        //Debug.Log("Touching tree");
        if (ObjectFull.activeSelf)
        {
            gatherObject();
        }
    }

    void gatherObject()
    {
        ObjectFull.SetActive(false);
        ObjectGathered.SetActive(true);

        //Event still happens even if no NPC is around to see it
        int eventId = gameManager.getNewId();

        foreach (GameObject npc in npcThatCanSee)
        {
            if (LineOfSight(npc.transform) && Vector3.Distance(transform.position, npc.transform.position) < npc.GetComponent<Social>().viewDistance)
            {
                //Warn villager of event
                Debug.Log(npc.name + actionMessage);
                //Chesk all action tags with the NPC interests

                List<Social.interest> interests = new List<Social.interest>();
                foreach (Social.interest objectInterest in interestTypes)
                {
                    foreach (Social.Interest npcInterest in npc.GetComponent<Social>().interests) {
                        //If object interest is the same as the NPC interest .... have to check the weight
                        if (npcInterest.interest.Equals(objectInterest))
                        {
                            interests.Add(objectInterest);
                        }
                    }
                }
                npc.GetComponent<Social>().information.Add(new Social.Message(interests, talkMessage, eventId, whoAmI, npc.GetComponent<Social>().messageForgetTime));
                StartCoroutine(npc.GetComponent<Social>().ShowBalloon(whoAmI));
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
