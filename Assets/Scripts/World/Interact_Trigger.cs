using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Interact_Trigger : MonoBehaviour
{
    [SerializeField] private float m_interactDistance = 2.0f;
    public float InteractionDistance() { return m_interactDistance; }

    [SerializeField] private bool m_isClosest;
    public void SetAsClosest(bool _closest) { m_isClosest = _closest; }

    [SerializeField] private string m_text = "Interact";

    [SerializeField] private UnityEvent OnInteract;

    [SerializeField] private bool m_interactable = true;
    public bool IsInteractable() { return m_interactable; }
    public void SetInteractable(bool _interactable) { m_interactable = _interactable; }

    [SerializeField] private bool m_onlyOnce;

    [SerializeField] private GameObject m_interactUiPrefab;
    [SerializeField] private GameObject m_interactUi;
    [SerializeField] private Transform m_uiAnchor;
    [SerializeField] private Vector3 m_uiOffset = Vector3.zero;

    private Player_Controller m_player;

    private Camera m_mainCam;

    private void Start()
    {
        m_player = Ai_Manager.GetPlayerTransform().GetComponent<Player_Controller>();
        m_player.AddInteraction(this);
        m_mainCam = Camera.main;

        if (m_uiAnchor == null) m_uiAnchor = transform;
        m_interactUi = GameObject.Instantiate(m_interactUiPrefab);
        m_interactUi.transform.SetParent(m_uiAnchor);
        Text text = m_interactUi.GetComponentInChildren<Text>();
        text.text = m_text + " [" + m_player.InteractKey.ToString() + "]";
    }

    private void Update()
    {
        float s = 0.25f * ((Mathf.Sin(Time.time * 1.5f)));
        m_interactUi.transform.position = m_uiAnchor.position + m_uiOffset + new Vector3(0,s,0);
        m_interactUi.SetActive(m_isClosest);
        m_interactUi.transform.rotation = m_mainCam.transform.rotation;
    }

    public void TriggerInteraction()
    {
        Debug.Log("Interaction Triggered");
        OnInteract.Invoke();
        if (m_onlyOnce) SetInteractable(false);
    }

    public void DestroyTrigger()
    {
        GameObject.Destroy(m_interactUi);
        Destroy(this);
    }
}
