using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "New Task", menuName = "Task", order = 1)]
[System.Serializable]
public class GameTask : ScriptableObject
{
    [SerializeField] private string taskName;
    public string TaskName { get => taskName; set => taskName = value; }
    [Multiline]
    [SerializeField] private string taskDescription;
    public string TaskDescription { get => taskDescription; set => taskDescription = value; }

    [SerializeField] private List<TaskRequirement> requirements;
    public List<TaskRequirement> Requirements { get => requirements; set => requirements = value; }

    public GameTask (GameTask _task)
    {
        Set(_task);
    }
    public GameTask Set(GameTask _task)
    {
        this.TaskName = _task.TaskName;
        this.TaskDescription = _task.TaskDescription;
        this.Requirements = _task.Requirements;
        return this;
    }


    public bool CheckRequirements()
    {
        foreach(TaskRequirement req in requirements)
        {
            if (req.RequirementData >= 1) return false;
        }
        return true;
    }

    public void SetRequirementValue(string _requirementName, int _value)
    {
        GetRequirement(_requirementName)?.SetValue(_value);
    }
    public void SetRequirementValue(string _requirementName, bool _value)
    {
        SetRequirementValue(_requirementName, 1 - _value.ToInt());
    }
    public void ReduceRequirementValue(string _requirementName)
    {
        ReduceRequirementValue(_requirementName, 1);
    }
    public void ReduceRequirementValue(string _requirementName, int _reduceValue)
    {
        GetRequirement(_requirementName)?.ReduceValue(_reduceValue);
    }

    TaskRequirement GetRequirement(string _requirementName)
    {
        foreach(TaskRequirement req in requirements)
        {
            if (req.RequirementName == _requirementName) return req;
        }
        Debug.LogWarning("Could Not Find Requirement '" + _requirementName + "' In GameTask '" + TaskName + "'!");
        return null;
    }

    public virtual void OnTaskStarted()
    {

    }

    public virtual void OnTaskCompleted()
    {

    }

}


[System.Serializable]
public class TaskRequirement
{
    public string RequirementName = "";
    public int RequirementData = 1;

    public void ReduceValue(int _value)
    {
        RequirementData -= _value;
        Debug.Log("Reduced Value Of Task Requirement '" + RequirementName + "' By " + _value + "!");
    }
    public void SetValue(int _value)
    {
        RequirementData = _value;
        Debug.Log("Set Value Of Task Requirement '" + RequirementName + "' To " + _value + "!");
    }
}