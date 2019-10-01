using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Controller : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private float m_moveSpeed = 8.0f;
    [SerializeField] private float m_jumpForce = 20.0f;
    [SerializeField] private float m_groundedDist = 1.05f;
    [SerializeField] private bool m_isGrounded = false;

    private CharacterController m_controller;

    [SerializeField] private Transform m_visual;
    [SerializeField] private float m_visualLerp = 0.5f;

    [Header("Interactions")]

    [SerializeField] private List<Interact_Trigger> m_sceneInteractions;
    public void AddInteraction(Interact_Trigger _interaction) { m_sceneInteractions.Add(_interaction); }
    [SerializeField] private Interact_Trigger m_closestInteraction;
    public KeyCode InteractKey = KeyCode.E;

    [Header("Dialogue")]

    [SerializeField] private Canvas m_dialogueCanvas;
    [SerializeField] private Text m_dialogueText;
    [SerializeField] private Transform m_dialogueOptions;
    [SerializeField] private Dialogue m_currentDialogue;

    [Header("Camera")]
    [SerializeField] private Transform m_cameraPivotY;
    [SerializeField] private Transform m_cameraPivotX;
    [SerializeField] private Transform m_cameraTarget;
    [SerializeField] private Camera m_camera;
    [SerializeField] private float m_cameraCloseDist = 4.0f;
    [SerializeField] private float m_cameraFarDist = 15.0f;
    [SerializeField] private float m_cameraMinAngle = 0.0f;
    [SerializeField] private float m_cameraMaxAngle = 80.0f;
    [SerializeField] private float m_cameraLerpSpeed = 10.0f;
    private float m_camXAngle = 45.0f;

    private void Awake()
    {
        Ai_Manager.SetPlayerTransform(transform);
        GatherComponents();
        m_visual.parent = null;
        m_cameraPivotY.parent = null;
        m_camera.transform.parent = null;
    }

    private void GatherComponents()
    {
        if (m_visual == null) m_visual = transform.GetChild(0);
        m_controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        GatherInput();
        GroundCheck();
        UpdateCamera();
        UpdateAnimations();
        Movement();
        HandleInteractionTriggers();
    }

    private void LateUpdate()
    {
        m_visual.position = Vector3.Lerp(m_visual.position, transform.position, m_visualLerp * Time.deltaTime);
        m_visual.localScale = Vector3.Lerp(m_visual.localScale, transform.localScale, m_visualLerp * Time.deltaTime);
        m_visual.rotation = Quaternion.Lerp(m_visual.rotation, transform.rotation, m_visualLerp * Time.deltaTime);
    }

    private void HandleInteractionTriggers()
    {
        m_closestInteraction = null;
        float closestDist = 1000;
        if (m_sceneInteractions.Count >= 1)
        {
            foreach(Interact_Trigger trigger in m_sceneInteractions)
            {
                float dist = Vector3.Distance(transform.position, trigger.transform.position);
                if(dist <= trigger.InteractionDistance())
                {
                    if (dist < closestDist)
                    {
                        m_closestInteraction = trigger;
                        closestDist = dist;
                    }
                    else
                    {
                        trigger.SetAsClosest(false);
                    }
                }
                else
                {
                    trigger.SetAsClosest(false);
                }
            }
        }
        m_closestInteraction?.SetAsClosest(true);
        if (PlayerInput.Interact) { m_closestInteraction?.TriggerInteraction(); }
    }

    private void GroundCheck()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, m_groundedDist, LayerTools.Default());
        m_isGrounded = false;
        if(hit.collider)
        {
            m_isGrounded = true;
        }
    }

    private void GatherInput()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        PlayerInput.X = x;
        PlayerInput.Y = y;
        PlayerInput.XY = new Vector2(x, y);
        PlayerInput.XYNormalized = PlayerInput.XY.normalized;
        PlayerInput.XYZ = new Vector3(x, 0.0f, y);
        PlayerInput.XYZNormalized = new Vector3(PlayerInput.XYNormalized.x, 0.0f, PlayerInput.XYNormalized.y);
        PlayerInput.Jump = Input.GetButton("Jump");
        PlayerInput.Crouch = Input.GetKey(KeyCode.LeftShift);
        PlayerInput.Sprint = Input.GetKey(KeyCode.LeftControl);
        PlayerInput.Interact = Input.GetKeyDown(KeyCode.E);
        PlayerInput.MouseVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    private void Movement()
    {
        if(m_isGrounded)
        {
            m_controller.Move(PlayerInput.XYZNormalized * Time.deltaTime * m_moveSpeed);
        }
        else
        {
            transform.position += Vector3.down * 10.0f * Time.deltaTime;
        }
        //m_body.AddForce(Vector3.down * 10.0f, ForceMode.Force);

    }

    private void UpdateAnimations()
    {

    }

    private void UpdateCamera()
    {
        m_camXAngle = Mathf.Clamp(m_camXAngle - PlayerInput.MouseVector.y, m_cameraMinAngle, m_cameraMaxAngle);
        float newDist = Mathf.Lerp(m_cameraCloseDist, m_cameraFarDist, (m_camXAngle / m_cameraMaxAngle));
        m_cameraPivotY.position = transform.position;
        float newYAngle = m_cameraPivotY.localEulerAngles.y + PlayerInput.MouseVector.x;
        if (newYAngle >= 360.0f) newYAngle -= 360.0f;
        if (newYAngle <= -360.0f) newYAngle += 360.0f;
        m_cameraPivotY.eulerAngles = new Vector3(0.0f, newYAngle, 0.0f);
        m_cameraPivotX.localEulerAngles = new Vector3(m_camXAngle, 0.0f, 0.0f);
        m_cameraTarget.localPosition = new Vector3(0.0f, 0.0f, -newDist);
        m_camera.transform.position = Vector3.Slerp(m_camera.transform.position, m_cameraTarget.position, m_cameraLerpSpeed * Time.deltaTime);
        m_camera.transform.rotation = Quaternion.Slerp(m_camera.transform.rotation, m_cameraPivotX.rotation, m_cameraLerpSpeed * Time.deltaTime);
    }

    //  DIALOGUE STUFF
    #region

    public void DisplayDialogue(Dialogue _dialogue, bool _pauseGame)
    {
        if (_pauseGame) Time.timeScale = 0;
        m_dialogueCanvas.gameObject.SetActive(true);

        DisplayDialogueRecursive(_dialogue);
    }

    private void DisplayDialogueRecursive(Dialogue _dialogue)
    {
        m_currentDialogue = _dialogue;
        foreach (Transform t in m_dialogueOptions.GetComponentInChildren<Transform>())
        {
            t.GetComponent<Button>().onClick.RemoveAllListeners();
            t.gameObject.SetActive(false);
        }
        m_dialogueText.text = _dialogue.GetContents();
        if (_dialogue.GetPaths().Count >= 1)
        {
            for (int i = 0; i < _dialogue.GetPaths().Count; i++)
            {
                m_dialogueOptions.GetChild(i).gameObject.SetActive(true);
                m_dialogueOptions.GetChild(i).GetChild(0).GetComponent<Text>().text = _dialogue.GetPaths()[i].GetTitle();
            }
        }
        else
        {
            m_dialogueOptions.GetChild(0).gameObject.SetActive(true);
            m_dialogueOptions.GetChild(0).GetChild(0).GetComponent<Text>().text = "Close";
        }
    }

    public void DisplayDialogFromChoice(Button _button)
    {
        if(m_currentDialogue != null)
        {
            if(m_currentDialogue.GetPaths().Count >= 1)
            {
                Dialogue newDialogue = m_currentDialogue.GetPaths()[_button.transform.GetSiblingIndex()];
                if (newDialogue != null) DisplayDialogueRecursive(newDialogue);
            }
            else CloseCurrentDialogue();

        }
    }

    public void CloseCurrentDialogue()
    {
        m_dialogueCanvas.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    #endregion
}