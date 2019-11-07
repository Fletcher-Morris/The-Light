using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    [SerializeField] protected string questname;
    [SerializeField] protected string describe;

    List<Condition> conditions = new List<Condition>();
    [SerializeField] protected List<GameObject> nextquests = new List<GameObject>();
    public bool quest_active = false;

    /// <summary>
    /// set condition value
    /// </summary>
    /// <param name="con_name">condition name</param>
    /// <param name="con_value">target value</param>
    public void SetCondition(string con_name, int con_value)
    {
        bool found = false;
        foreach (Condition con in conditions)
        {
            if (con.name == con_name)
            {
                found = true;
                con.value = con_value;
            }
        }
        if (found == false)
        {
            Condition co = new Condition();
            co.name = con_name;
            co.value = con_value;
            conditions.Add(co);
        }


    }
    /// <summary>
    /// Change condition
    /// </summary>
    /// <param name="con_name">
    /// condition name
    /// </param>
    /// <param name="con_value">
    /// offset value
    /// </param>
    public void ChangeCondition(string con_name, int con_value)
    {
        bool found = false;
        foreach (Condition con in conditions)
        {
            if (con.name == con_name)
            {
                found = true;
                con.value += con_value;
            }
        }
        if (found == false)
        {
            Condition co = new Condition();
            co.name = con_name;
            co.value = con_value;
            conditions.Add(co);
        }


    }
    /// <summary>
    /// get condition value
    /// </summary>
    /// <param name="con_name">condition name</param>
    /// <returns>get value,return -999 if not exist</returns>
    public int ReadCondition(string con_name)
    {
        int v = -999;

        foreach (Condition con in conditions)
        {
            if (con.name == con_name)
            {
                v = con.value;


            }
        }

        return v;
    }

    public string Getname()
    {
        return this.questname;
    }
    public string Getdesc()
    {
        return this.describe;
    }
    ///<summary>
    ///run when quest is added to the quest list(as soon as the quest is in the scene)
    ///</summary>
    public virtual void OnAdd()
    {


    } 
    
    ///<summary>
    ///run when the quest is done
    ///</summary>
    public virtual void OnFinish()
    {
        if (nextquests.Count > 0)
        {
            foreach(GameObject ga in nextquests)
            {
                Instantiate(ga);
            }
        }
    }

    ///<summary>
    ///Run when the Quest is set active(Do not run automatic)
    ///</summary>
    public virtual void OnQuestSetActive()
    {

    }
    ///<summary>
    ///run when the quest fails
    ///</summary>
    public virtual void Onfail()
    {

    }
    /// <summary>
    /// Run when reset the quest
    /// </summary>
    public virtual void OnReset()
    {

    }
    ///<summary>
    ///check if the quest is done or failed(run per frame,can be used as update)
    ///</summary>
    public virtual void Check()
    {

    }
}
