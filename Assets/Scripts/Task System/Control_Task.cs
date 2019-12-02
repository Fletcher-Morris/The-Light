using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control_Task : MonoBehaviour
{
    [SerializeField] private GameTask m_activateTask;
    [SerializeField] private GameTask m_completeTask;
    [SerializeField] private GameTask m_modifyTask;
    [SerializeField] private string m_requirementName;

    public void ActivateTask()
    {
        if (m_activateTask == null) return;
        Task_Manager.ActivateTask(m_activateTask);
    }

    public void Complete()
    {
        if (m_completeTask == null) return;
        Task_Manager.CompleteTask(m_completeTask.TaskName);
    }

    public void SetRequirementInt(int _int)
    {
        if (m_modifyTask == null) return;
        Task_Manager.ModifyTaskRequirement(m_modifyTask.TaskName, m_requirementName, _int, false);
    }
    public void ReduceRequirementInt(int _int)
    {
        if (m_modifyTask == null) return;
        Task_Manager.ModifyTaskRequirement(m_modifyTask.TaskName, m_requirementName, _int, true);
    }

    public void SetRequirementBool(bool _bool)
    {
        if (m_modifyTask == null) return;
        Task_Manager.ModifyTaskRequirement(m_modifyTask.TaskName, m_requirementName, _bool.Inv().ToInt(), false);
    }
}
