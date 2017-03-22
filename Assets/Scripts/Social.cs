using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Social : MonoBehaviour {

    RaycastHit2D hit;
    public float viewDistance;
    public float talkDistance;

    public GameObject MessageBallon;
    GameManager gameManager;

    [SerializeField]
    public List<Aquaintance> myAquaintances;
    [SerializeField]
    public List<interest> interests;
    [SerializeField]
    public List<Message> information;

    public GameObject line;
    public List<GameObject> linesNpcToNpc;
    public GameObject lineTalk;
    public List<GameObject> linesNpcToNpcTalk;

    public enum interest
    {
        gatheringWood,
        gathering,
        wood,
        killing,
        farming,
        weather
    };

    [Serializable]
    public class Aquaintance
    {
        [SerializeField]
        public GameObject npc;
        [SerializeField]
        public float friendshipLevel;
    };

    [Serializable]
    public class Message
    {
        [SerializeField]
        public interest typeOfMessage;
        [SerializeField]
        public float weight;
        [SerializeField]
        public string message;
        [SerializeField]
        public int id;

        public Message() { }

        public Message(interest myInterest, float weight, string message, int id)
        {
            this.typeOfMessage = myInterest;
            this.weight = weight;
            this.message = message;
            this.id = id;
        }
    };

    void Start()
    {
        for (int i = 0; i < myAquaintances.Count; i++)
        {
            GameObject lineToNPC = Instantiate(line);
            linesNpcToNpc.Add(lineToNPC);
            GameObject lineToNPCTalk = Instantiate(lineTalk);
            linesNpcToNpcTalk.Add(lineToNPCTalk);
        }
        MessageBallon.SetActive(false);
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        InvokeRepeating("Talk", 0.0f, 0.5f);
    }

    public IEnumerator ShowBaloon()
    {
        MessageBallon.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        MessageBallon.SetActive(false);
        StopCoroutine("ShowBaloon");
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

    void Talk()
    {
        foreach (Aquaintance aquaintaince in myAquaintances)
        {
            if (LineOfSight(aquaintaince.npc.transform) && Vector3.Distance(transform.position, aquaintaince.npc.transform.position) < talkDistance)
            {
                foreach(Message message in information)
                {
                    //Check if they share an interest
                    //TODO add a minimum friendship level for message to be sent
                    if (aquaintaince.npc.GetComponent<Social>().interests.Contains(message.typeOfMessage))
                    {
                        //check if aquaintance already has that message
                        bool alreadyHasMessage = false;
                        foreach(Message aquaintanceMessage in aquaintaince.npc.GetComponent<Social>().information)
                        {
                            if(aquaintanceMessage.id == message.id)
                            {
                                alreadyHasMessage = true;
                            }
                        }
                        if (!alreadyHasMessage)
                        {
                            Debug.Log("NPC " + this.name + " told " + aquaintaince.npc.name + " about " + message.typeOfMessage.ToString());
                            aquaintaince.npc.GetComponent<Social>().information.Add(new Social.Message(message.typeOfMessage, 1, "Player gathered some wood", message.id));
                            StartCoroutine(ShowBaloon());
                            StartCoroutine(aquaintaince.npc.GetComponent<Social>().ShowBaloon());
                        }
                    }
                }
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < myAquaintances.Count; i++)
        {
            if (LineOfSight(myAquaintances[i].npc.transform) && Vector3.Distance(transform.position, myAquaintances[i].npc.transform.position) < talkDistance)
            {
                linesNpcToNpcTalk[i].SetActive(true);
                linesNpcToNpc[i].SetActive(false);
                linesNpcToNpcTalk[i].GetComponent<LineRenderer>().SetPosition(0, new Vector3(transform.position.x, transform.position.y, -1));
                linesNpcToNpcTalk[i].GetComponent<LineRenderer>().SetPosition(1, new Vector3(myAquaintances[i].npc.transform.position.x, myAquaintances[i].npc.transform.position.y, -1));
                //Debug.DrawRay(transform.position, aquaintaince.npc.transform.position - transform.position, Color.white);
            }
            else if (LineOfSight(myAquaintances[i].npc.transform) && Vector3.Distance(transform.position, myAquaintances[i].npc.transform.position) < viewDistance)
            {
                linesNpcToNpc[i].SetActive(true);
                linesNpcToNpcTalk[i].SetActive(false);
                linesNpcToNpc[i].GetComponent<LineRenderer>().SetPosition(0, new Vector3(transform.position.x, transform.position.y, -1));
                linesNpcToNpc[i].GetComponent<LineRenderer>().SetPosition(1, new Vector3(myAquaintances[i].npc.transform.position.x, myAquaintances[i].npc.transform.position.y, -1));
                //Debug.DrawRay(transform.position, aquaintaince.npc.transform.position - transform.position, Color.blue);
            }
            else
            {
                linesNpcToNpcTalk[i].SetActive(false);
                linesNpcToNpc[i].SetActive(false);
            }
        }
        
    }

}
