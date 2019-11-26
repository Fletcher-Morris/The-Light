using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuestManager : MonoBehaviour
{
    private static QuestManager instance;
    public static QuestManager Instance { get => instance; set => instance = value; }

    //Run Quests& Manage Conditions

    private List<Quest> quests=new List<Quest>();
    private List<string> finishedquests = new List<string>();
    //it is not safe to add/remove elements during foreach, so use buffer to add/remove later 
    private List<Quest> add_buffer = new List<Quest>();
    private List<Quest> remove_buffer = new List<Quest>();

    private Text m_questText;
    private string st;

    bool quest_check_running=false;


    static QuestManager()
    {
        GameObject ga = new GameObject("~~~~QuestManager~~~~");
        DontDestroyOnLoad(ga);
        Instance=ga.AddComponent<QuestManager>();
    }

    private void Awake()
    {
        m_questText = GameObject.FindGameObjectWithTag("QuestLayer").GetComponent<Text>();
    }
    /// <summary>
    /// add quest to the quest list
    /// </summary>
    /// <param name="qu"></param>
    public void AddQuest(Quest qu)
    {
        qu.OnAdd();
        if (!quest_check_running)
        {
           
            quests.Add(qu);
        }
        else
        {
            add_buffer.Add(qu);
        }
        UpdateQuests();
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
        UpdateQuests();
    }
    /// <summary>
    /// Set quest active by quest name
    /// </summary>
    /// <param name="qu_name"></param>
    public void SetQuestActive(string qu_name)
    {
        foreach (Quest q in quests)
        {
            if (q.Getname() == qu_name)
            {
                q.quest_active = true;
                q.OnQuestSetActive();
            }
        }
        UpdateQuests();
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
        finishedquests.Add(qu.Getname());
        qu.OnFinish();
        if (!quest_check_running)
        {
           
            quests.Remove(qu);
           
            Destroy(qu.gameObject);
        }
        else
        {
            remove_buffer.Add(qu);
        }
        UpdateQuests();
    }

  
    /// <summary>
    /// Run to fail the quest
    /// </summary>
    /// <param name="qu"></param>
    public void FailQuest(Quest qu)
    {
        qu.quest_active = false;
        qu.Onfail();
        if (!quest_check_running)
        {
            quests.Remove(qu);         
            Destroy(qu.gameObject);
        }
        else
        {
            remove_buffer.Add(qu);
        }
        UpdateQuests();
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

    public bool CheckQuestFinished(string q_name)
    {
        bool finished = false;
        foreach(string st in finishedquests)
        {
            if (st == q_name)
            {
                finished = true;
            }
        }
        return finished;
    }

    public void ResetQuest(Quest q)
    {
        q.OnReset();
        UpdateQuests();
    }
    public void ClearQuestList()
    {
        add_buffer.Clear();
        remove_buffer.Clear();
        quests.Clear();
        finishedquests.Clear();
        UpdateQuests();
    }

    private void UpdateQuests()
    {
        st = "";
        //add quests from addbuffer
        foreach (Quest q in add_buffer)
        {
            quests.Add(q);
        }
        add_buffer.Clear();

        //remove quests in removebuffer
        foreach (Quest q in remove_buffer)
        {
            quests.Remove(q);
            Destroy(q.gameObject);
        }
        remove_buffer.Clear();



        //update each quest
        quest_check_running = true;
        foreach (Quest q in quests)
        {
            if(q != null)
            {
                if (q.quest_active)
                {
                    q.Check();
                    st += q.Getname();
                    st += "\n";
                    st += q.Getdesc();
                    st += "\n";
                }
            }

        }
        quest_check_running = false;

        m_questText.text = st;

        //  Debug.Log(quests.Count);
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(quests.Count);

        }
    }
}
