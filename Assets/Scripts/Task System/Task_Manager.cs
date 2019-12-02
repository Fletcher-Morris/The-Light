using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class Task_Manager : MonoBehaviour
{
    private static Task_Manager m_singleton;
    public static Task_Manager Singleton() { return m_singleton; }

    [SerializeField] private List<GameTask> m_activateOnStart;

    [SerializeField] private List<GameTask> m_activeTasks;
    [SerializeField] private List<GameTask> m_completedTasks;


    private void Awake()
    {
        if (m_singleton == null) m_singleton = this;
    }

    private void Start()
    {
        foreach(GameTask task in m_activateOnStart)
        {
            ActivateTask(task);
        }
    }

    public static void ActivateTask(GameTask _task)
    {
        RemoveTask(_task.name);
        Singleton().m_activeTasks.Add(new GameTask(_task));
        Debug.Log("Activated Task '" + _task.TaskName + "'!");
        _task.OnTaskStarted();
        UpdateTasks();
    }

    public static void UpdateTasks()
    {
        foreach(GameTask task in Singleton().m_activeTasks)
        {
            if (task.CheckRequirements()) CompleteTask(task);
        }

        if(Singleton().m_removeFromActive.Count >= 1)
        {
            Singleton().m_activeTasks = Singleton().m_activeTasks.Except(Singleton().m_removeFromActive).ToList<GameTask>();
            Singleton().m_removeFromActive = new List<GameTask>();
        }
        if(Singleton().m_removeFromCompleted.Count >= 1)
        {
            Singleton().m_completedTasks = Singleton().m_completedTasks.Except(Singleton().m_removeFromCompleted).ToList<GameTask>();
            Singleton().m_removeFromCompleted = new List<GameTask>();
        }
        
    }

    public static GameTask GetTask(string _taskName)
    {
        GameTask task;
        task = GetActiveTask(_taskName);
        if (task != null) return task;
        task = GetCompletedTask(_taskName);
        if (task != null) return task;
        return null;
    }
    public static GameTask GetActiveTask(string _taskName)
    {
        foreach (GameTask task in Singleton().m_activeTasks)
        {
            if (task.TaskName == _taskName) return task;
        }
        return null;
    }
    public static GameTask GetCompletedTask(string _taskName)
    {
        foreach (GameTask task in Singleton().m_completedTasks)
        {
            if (task.TaskName == _taskName) return task;
        }
        return null;
    }

    private List<GameTask> m_removeFromActive = new List<GameTask>();
    private List<GameTask> m_removeFromCompleted = new List<GameTask>();

    public static void RemoveTask(string _taskName)
    {
        RemoveTaskFromActive(_taskName);
        RemoveTaskFromCompleted(_taskName);
        UpdateTasks();
    }
    public static void RemoveTaskFromActive(string _taskName)
    {
        foreach (GameTask task in Singleton().m_activeTasks)
        {
            if (task.TaskName == _taskName)
            {
                RemoveTaskFromActive(task);
            }
        }
    }
    private static void RemoveTaskFromActive(GameTask _task)
    {
        if (_task == null) return;
        if (Singleton().m_removeFromActive.Contains(_task)) return;
        Singleton().m_removeFromActive.Add(_task);
        Debug.Log("Removed Task '" + _task.TaskName + " From Active Tasks!");
    }
    public static void RemoveTaskFromCompleted(string _taskName)
    {
        foreach (GameTask task in Singleton().m_activeTasks)
        {
            if (task.TaskName == _taskName)
            {
                RemoveTaskFromCompleted(task);
            }
        }
    }
    private static void RemoveTaskFromCompleted(GameTask _task)
    {
        if (_task == null) return;
        if (Singleton().m_removeFromCompleted.Contains(_task)) return;
        Singleton().m_removeFromCompleted.Add(_task);
        Debug.Log("Removed Task '" + _task.TaskName + " From Completed Tasks!");
    }

    public static void CompleteTask(string _taskName)
    {
        GameTask task = GetActiveTask(_taskName);
        if(task != null)
        {
            CompleteTask(task);
        }
    }

    private static void CompleteTask(GameTask _task)
    {
        Debug.Log("Completed Task '" + _task.TaskName + "!");
        Singleton().m_completedTasks.Add(_task);
        RemoveTaskFromActive(_task);
        Singleton().m_completedTasks.Add(_task);
        _task.OnTaskCompleted();
    }

    public static bool TaskIsActive(string _taskName)
    {
        return Singleton().m_activeTasks.Contains(GetTask(_taskName));
    }
    public static bool TaskIsCompleted(string _taskName)
    {
        return Singleton().m_completedTasks.Contains(GetTask(_taskName));
    }
    public static bool TaskExists(string _taskName)
    {
        return GetTask(_taskName) != null;
    }

    public static void ModifyTaskRequirement(string _taskName, string _requirement, int _value, bool _reduce)
    {
        GameTask task = GetTask(_taskName);
        if(task != null)
        {
            ModifyTaskRequirement(task, _requirement, _value, _reduce);
        }
    }
    private static void ModifyTaskRequirement(GameTask _task, string _requirement, int _value, bool _reduce)
    {
        if(_reduce)
        {
            _task.ReduceRequirementValue(_requirement, _value);
        }
        else
        {
            _task.SetRequirementValue(_requirement, _value);
        }

        UpdateTasks();
    }
}