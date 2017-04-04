using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Social : MonoBehaviour {

    RaycastHit2D hit;
    public float viewDistance;
    public float talkDistance;

    public GameObject MessageBalloon;

    [SerializeField]
    public List<Aquaintance> myAquaintances;
    [SerializeField]
    public List<Interest> interests;
    [SerializeField]
    public List<Message> information;

    public GameObject line;
    public List<GameObject> linesNpcToNpc;
    public GameObject lineTalk;
    public List<GameObject> linesNpcToNpcTalk;

    public GameObject berrieBalloonIcon;
    public GameObject treeBalloonIcon;

    public float talkCooldownTime;
    float lastTalkTime;

    public float messageForgetTime;

    public enum interest
    {
        gatheringWood,
        gatheringBerries,
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
        public List<interest> messageTags;
        [SerializeField]
        public string message;
        [SerializeField]
        public int id;
        [SerializeField]
        public ObjectGathererController.objectType actionObject;
        [SerializeField]
        public float timeOfLife;

        public Message() { }

        public Message(List<interest> tags, string message, int id, ObjectGathererController.objectType objectType, float lifeTime)
        {
            this.messageTags = tags;
            this.message = message;
            this.id = id;
            this.actionObject = objectType;
            this.timeOfLife = lifeTime;
        }
    };

    [Serializable]
    public class Interest
    {
        [SerializeField]
        public interest interest;
        [SerializeField]
        public float weight;
        
        public Interest() { }

        public Interest(interest myInterest, float weight)
        {
            this.interest = myInterest;
            this.weight = weight;
        }
    };

    void Start()
    {
        for (int i = 0; i < myAquaintances.Count; i++)
        {
            GameObject lineToNPC = Instantiate(line);
            lineToNPC.transform.parent = line.transform.parent;
            linesNpcToNpc.Add(lineToNPC);

            GameObject lineToNPCTalk = Instantiate(lineTalk);
            lineToNPCTalk.transform.parent = lineTalk.transform.parent;
            linesNpcToNpcTalk.Add(lineToNPCTalk);
        }
        MessageBalloon.SetActive(false);
        InvokeRepeating("Talk", 0.0f, 0.5f);
    }

    public IEnumerator ShowBalloon(ObjectGathererController.objectType type)
    {
        switch (type)
        {
            case ObjectGathererController.objectType.simpleTree:
                treeBalloonIcon.SetActive(true);
                break;
            case ObjectGathererController.objectType.berries:
                berrieBalloonIcon.SetActive(true);
                break;
        }
        MessageBalloon.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        treeBalloonIcon.SetActive(false);
        berrieBalloonIcon.SetActive(false);
        MessageBalloon.SetActive(false);
        StopCoroutine("ShowBalloon");
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

    public bool CanTalk()
    {
        return Time.timeSinceLevelLoad - lastTalkTime > talkCooldownTime;
    }
    void Talk()
    {
        if (CanTalk())
        {
            foreach (Aquaintance aquaintaince in myAquaintances)
            {
                //If giver of information can talk
                if (LineOfSight(aquaintaince.npc.transform) && 
                    Vector3.Distance(transform.position, aquaintaince.npc.transform.position) < talkDistance)
                    //&& aquaintaince.npc.GetComponent<Social>().CanTalk())
                {
                    lastTalkTime = Time.timeSinceLevelLoad;

                    float biggestMessageWeight = 0.0f;
                    Message mostImportantMessage = new Message();
                    foreach (Message message in information)
                    {
                        //check if aquaintance already has that message
                        bool alreadyHasMessage = false;
                        foreach (Message aquaintanceMessage in aquaintaince.npc.GetComponent<Social>().information)
                        {
                            if (aquaintanceMessage.id == message.id)
                            {
                                alreadyHasMessage = true;
                            }
                        }
                        if (!alreadyHasMessage)
                        {
                            //Check aquaintance interests with message tags to determine how important is that message
                            float messageTotalWeight = 0.0f;
                            foreach (Interest aquaintanceInterest in aquaintaince.npc.GetComponent<Social>().interests)
                            {
                                if (message.messageTags.Contains(aquaintanceInterest.interest))
                                {
                                    messageTotalWeight += aquaintanceInterest.weight;
                                }
                            }
                            //Check if this is the most important message to send
                            if (messageTotalWeight > biggestMessageWeight)
                            {
                                biggestMessageWeight = messageTotalWeight;
                                mostImportantMessage = message;
                            }
                        }
                    }
                    //Check if there is an important message to send
                    if (biggestMessageWeight > 0.0f)
                    {

                        Debug.Log("NPC " + this.name + " told " + aquaintaince.npc.name + " about " + mostImportantMessage.actionObject.ToString());

                        //NPC remembers the message again if he talks about it. It resets the message time of life.
                        mostImportantMessage.timeOfLife = messageForgetTime;

                        aquaintaince.npc.GetComponent<Social>().information.Add(
                            new Message(mostImportantMessage.messageTags, mostImportantMessage.message, mostImportantMessage.id,
                            mostImportantMessage.actionObject, messageForgetTime));

                        StartCoroutine(ShowBalloon(mostImportantMessage.actionObject));
                        StartCoroutine(aquaintaince.npc.GetComponent<Social>().ShowBalloon(mostImportantMessage.actionObject));
                    }
                }
            }
        }
    }

    void MessageDecay()
    {
        foreach(Message myMessage in information)
        {
            myMessage.timeOfLife -= Time.deltaTime;
        }
        for (int i = information.Count - 1; i >= 0; i--)
        {
            if (information[i].timeOfLife < 0.0f)
            {
                information.RemoveAt(i);
            }
        }
    }

    void Update()
    {
        MessageDecay();
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
