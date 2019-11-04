using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    [SerializeField]protected string questname;
    [SerializeField]protected string describe;
   

    public bool quest_active = false;

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
