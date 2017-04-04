using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KillController : MonoBehaviour {


    RaycastHit2D hit;

    public float viewDistance;
    List<GameObject> npcThatCanSee;
    public GameObject lineNpcEvent;

    List<GameObject> linesToNpc;

    public List<Social.interest> interestTypes;

    public string actionMessage;
    public string talkMessage;
    GameManager gameManager;

    public GameManager.objectType whoAmI;

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

    void RemoveKilledObject()
    {
        if (whoAmI.Equals(GameManager.objectType.rabbit))
        {
            GameObject blood = Instantiate(Resources.Load("BloodSpatter")) as GameObject;
            blood.transform.position = this.transform.position;
        }
        for (int i = linesToNpc.Count - 1; i >= 0; i--)
        {
            //Debug.Log("Destroy line: " + linesToNpc[i].name);
            Destroy(linesToNpc[i]);
        }
        Destroy(this.gameObject);
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
        killObject();
    }

    void killObject()
    {
        //Event still happens even if no NPC is around to see it
        int eventId = gameManager.getNewId();

        foreach (GameObject npc in npcThatCanSee)
        {
            if (LineOfSight(npc.transform) && Vector3.Distance(transform.position, npc.transform.position) < npc.GetComponent<Social>().viewDistance)
            {
                //Warn villager of event
                Debug.Log(npc.name + actionMessage);

                //Check all action tags with the NPC interests
                List<Social.interest> interests = new List<Social.interest>();
                foreach (Social.interest objectInterest in interestTypes)
                {
                    foreach (Social.Interest npcInterest in npc.GetComponent<Social>().interests)
                    {
                        //If object interest is the same as the NPC interest .... have to check the weight
                        if (npcInterest.interest.Equals(objectInterest))
                        {
                            interests.Add(objectInterest);
                        }
                    }
                }
                npc.GetComponent<Social>().information.Add(new Social.Message(interests, talkMessage, eventId, whoAmI, npc.GetComponent<Social>().messageForgetTime));
                npc.GetComponent<Social>().showBalloonCorotine(whoAmI);
            }
        }

        RemoveKilledObject();
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
            }
            else
            {
                linesToNpc[i].SetActive(false);
            }
        }

    }
}
