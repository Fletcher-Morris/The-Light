using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    
    //Run Quests& Manage Conditions

    List<Quest> quests=new List<Quest>();
    List<string> finishedquests = new List<string>();
    //it is not safe to add/remove elements during foreach, so use buffer to add/remove later 
    List<Quest> add_buffer = new List<Quest>();
    List<Quest> remove_buffer = new List<Quest>();

    GameObject questtext;
    string st;

    bool quest_check_running=false;
    static QuestManager()
    {
        GameObject ga = new GameObject("~~~~QuestManager~~~~");
        DontDestroyOnLoad(ga);
        instance=ga.AddComponent<QuestManager>();
    }

    private void Awake()
    {
        questtext = GameObject.FindGameObjectWithTag("QuestLayer");
    }
    /// <summary>
    /// add quest to the quest list
    /// </summary>
    /// <param name="qu"></param>
    public void AddQuest(Quest qu)
    {
        if (!quest_check_running)
        {
            qu.OnAdd();
            quests.Add(qu);
        }
        else
        {
            add_buffer.Add(qu);
        }
     
    }
    /// <summary>
    /// Set quest active
    /// </summary>
    /// <param name="qu"></param>
    public void SetQuestActive(Quest qu)
    {
        foreach(Quest q in quests)
        {
            if (q == qu)
            {
             q.quest_active = true;
             q.OnQuestSetActive();
            }
        }
    }
    /// <summary>
    /// Set quest active by quest name
    /// </summary>
    /// <param name="qu_name"></param>
    public void SetQuestActive(string qu_name)
    {
        foreach (Quest q in quests)
        {
            if (q.name == qu_name)
            {
                q.quest_active = true;
                q.OnQuestSetActive();
            }
        }
    }
    /// <summary>
    /// Get Quest by quest name
    /// </summary>
    /// <param name="qu_name"></param>
    /// <returns></returns>
    public Quest GetQuest(string qu_name)
    {
        foreach(Quest q in quests)
        {
            if (q.Getname() == qu_name)
            {
                return q;
            }
        }
        return null;
    }
    /// <summary>
    ///Run to finish the quest
    /// </summary>
    /// <param name="qu"></param>
    public void FinishQuest(Quest qu)
    {
        qu.quest_active = false;
        if (!quest_check_running)
        {
           
            quests.Remove(qu);
            qu.OnFinish();
            finishedquests.Add(qu.Getname());
            Destroy(qu.gameObject);
        }
        else
        {
            remove_buffer.Add(qu);
        }
    }

  
    /// <summary>
    /// Run to fail the quest
    /// </summary>
    /// <param name="qu"></param>
    public void FailQuest(Quest qu)
    {
        qu.quest_active = false;
        if (!quest_check_running)
        {

            quests.Remove(qu);
            qu.Onfail();
            Destroy(qu.gameObject);
        }
        else
        {
            remove_buffer.Add(qu);
        }
    }
 
 
    /// <summary>
    /// check if the quest is in the quest list
    /// </summary>
    /// <param name="q_name"></param>
    /// <returns></returns>
    public bool ChenkQuest(string q_name)
    {
        bool found = false;
        foreach(Quest q in quests)
        {
            if (q.name == q_name)
            {
                found = true;
            }
        }
        return found;
    }
    /// <summary>
    /// check if the quest is active
    /// </summary>
    /// <param name="q_name"></param>
    /// <returns></returns>
    public bool ChenkQuestActive(string q_name)
    {
        bool active = false;
        foreach (Quest q in quests)
        {
            if (q.name == q_name)
            {
                if (q.quest_active == true)
                {
                    active = true;
                }
            }
        }
        return active;
    }

    public void ResetQuest(Quest q)
    {
        q.OnReset();
    }
    public void ClearQuestList()
    {
        add_buffer.Clear();
        remove_buffer.Clear();
        quests.Clear();
    }


    void Update()
    {
        st = "";
        //add quests from addbuffer
        foreach(Quest q in add_buffer)
        {
           q.OnAdd();
           quests.Add(q);
 
        }
        add_buffer.Clear();

        //remove quests in removebuffer
        foreach(Quest q in remove_buffer)
        {
            quests.Remove(q); 
            q.OnFinish(); 
            Destroy(q.gameObject);
        }
        remove_buffer.Clear();


        
        //update each quest
        quest_check_running = true;
        foreach(Quest q in quests)
        {
            if(q.quest_active)
            q.Check();
            st += q.Getname();
            st += "\n";
            st += q.Getdesc();
            st += "\n";
            //  Debug.Log("111");
        }
        quest_check_running = false;
       
        questtext.GetComponent<Text>().text = st;

        //  Debug.Log(quests.Count);
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(quests.Count);
            
        }
    }
}
