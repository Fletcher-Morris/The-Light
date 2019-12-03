using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Interact_Trigger : MonoBehaviour
{
    private float m_interactDistance = 3.0f;
    public float InteractionDistance() { return m_interactDistance; }

    [SerializeField] private bool m_isClosest;
    public void SetAsClosest(bool _closest)
    {
        m_isClosest = _closest;
    }

    [SerializeField] private string m_text = "Interact";

    [SerializeField] private UnityEvent OnInteract;

    [SerializeField] private bool m_interactable = true;
    public bool IsInteractable() { return m_interactable; }
    public void SetInteractable(bool _interactable)
    {
        if (_interactable != m_interactable) UpdateShaderGlow(_interactable);
        m_interactable = _interactable;
    }

    [SerializeField] private bool m_autoFindRenderers = true;

    private void UpdateShaderGlow(bool _enabled)
    {
        if (m_renderers.Count >= 1)
        {
            foreach (Renderer rend in m_renderers)
            {
                foreach (Material mat in rend.materials)
                {
                    if (_enabled) mat.SetInt("_InteractableGlow", 1);
                    else mat.SetInt("_InteractableGlow", 0);
                }
            }
        }
    }

    [SerializeField] private bool m_onlyOnce;

    [SerializeField] private GameObject m_interactUiPrefab;
    [SerializeField] private GameObject m_interactUi;
    [SerializeField] private Transform m_uiAnchor;
    [SerializeField] private Vector3 m_uiOffset = new Vector3(0, 1, 0);

    private Camera m_mainCam;

    [SerializeField] private List<Renderer> m_renderers = new List<Renderer>();

    private void Start()
    {
        Player_Controller.Singleton().AddInteraction(this);
        m_mainCam = Camera.main;

        if (m_uiAnchor == null) m_uiAnchor = transform;
        m_interactUi = GameObject.Instantiate(m_interactUiPrefab);
        m_interactUi.transform.SetParent(m_uiAnchor);
        Text text = m_interactUi.GetComponentInChildren<Text>();
        text.text = m_text + " [" + Player_Controller.Singleton().InteractKey.ToString() + "]";

        if(m_autoFindRenderers)
        {
            if (GetComponent<Renderer>()) m_renderers.AddRange(GetComponents<Renderer>());
            foreach(Renderer rend in GetComponentsInChildren<Renderer>())
            {
                m_renderers.Add(rend);
            }
        }

        SetInteractable(!m_interactable);
        SetInteractable(!m_interactable);
    }

    private void Update()
    {
        float s = 0.25f * ((Mathf.Sin(Time.time * 1.5f)));
        m_interactUi?.transform.SetPositionAndRotation(m_uiAnchor.position + m_uiOffset + new Vector3(0, s, 0), m_mainCam.transform.rotation);
        m_interactUi.SetActive(m_isClosest);
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
