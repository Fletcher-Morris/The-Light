using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 1)]
[SerializeField]
public class Dialogue : ScriptableObject
{
    [SerializeField]
    [Multiline]
    [Tooltip("The text that will be displayed to the player.")]
    private string m_contents;
    public string GetContents() { return m_contents; }

    [SerializeField]
    [Tooltip("The title will be displayed as the option for this dialogue.")]
    private string m_dialogueTitle;
    public string GetTitle() { return m_dialogueTitle; }

    [SerializeField]
    [Tooltip("These dialogue options will be displayed at the end of the text.")]
    private List<Dialogue> m_paths;
    public List<Dialogue> GetOptions() { return m_paths; }

    [Tooltip("This dialogue will be saved to the player's journal when opened.")]
    private bool m_saveToJournal;
    public void SaveToJournal() { if (m_saveToJournal) { } }

    [Tooltip("These methods will be invoked when the dialogue is opened.")]
    private UnityEvent OnOpen;
}
