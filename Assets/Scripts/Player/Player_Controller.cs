using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Controller : MonoBehaviour
{
    private static Player_Controller m_singleton;
    public static Player_Controller Singleton() { return m_singleton; }


    [Header("Movement Settings")]
    [SerializeField] private float m_runSpeed = 8.0f;
    [SerializeField] private float m_walkSpeed = 3.0f;
    [SerializeField] private float m_jumpForce = 20.0f;
    [SerializeField] private float m_groundedDist = 1.05f;
    [SerializeField] private bool m_isGrounded = false;
    private CharacterController m_controller;
    [SerializeField] private Transform m_visual;
    [SerializeField] private float m_visualLerp = 0.5f;
    [SerializeField] private float m_visualRotationLerp = 0.5f;
    private Vector3 m_moveDirection = Vector3.forward;
    private Vector3 m_storedMoveDirection = Vector3.forward;
    [SerializeField] private float m_gravity = 10.0f;
    [SerializeField] private KeyCode m_walkKey = KeyCode.LeftShift;
    [SerializeField] private Animator m_animator;
    private int m_animatorMoveHash;
    private int m_animatorLanternHash;
    private int m_animatorFlipHash;
    private int m_animatorDeadHash;
    private bool m_hasLamp = false;
    [SerializeField] private GameObject m_lampObject;

    [SerializeField] private bool m_inCutscene = false;
    public bool InCutscene { get => m_inCutscene; set => m_inCutscene = value; }
    private bool m_camMovementEnabled = true;
    public void EnterCutscene(bool _disableCam) { InCutscene = true; m_camMovementEnabled = !_disableCam; }
    public void EnterCutscene() { EnterCutscene(true); }
    public void ExitCutscene() { InCutscene = false; m_camMovementEnabled = true; }

    [SerializeField] private Transform m_moveToPosition;
    public Transform MoveToPos { get => m_moveToPosition; set => m_moveToPosition = value; }

    [Header("Interactions")]

    [SerializeField] private List<Interact_Trigger> m_sceneInteractions;
    public void AddInteraction(Interact_Trigger _interaction) { m_sceneInteractions.Add(_interaction); }
    [SerializeField] private Interact_Trigger m_closestInteraction;
    public KeyCode InteractKey = KeyCode.E;

    [Header("Dialogue")]

    [SerializeField] private Transform m_dialoguePanel;
    [SerializeField] private Text m_dialogueText;
    [SerializeField] private Transform m_dialogueOptions;
    [SerializeField] private Dialogue m_currentDialogue;

    [Header("Camera")]
    [SerializeField] private Transform m_cameraPivotY;
    public Transform CamYAxis() { return m_cameraPivotY; }
    [SerializeField] private Transform m_cameraPivotX;
    [SerializeField] private Transform m_cameraTarget;
    [SerializeField] private Camera m_camera;
    [SerializeField] private float m_cameraCloseDist = 4.0f;
    [SerializeField] private float m_cameraFarDist = 15.0f;
    [SerializeField] private float m_cameraMinAngle = 0.0f;
    [SerializeField] private float m_cameraMaxAngle = 80.0f;
    [SerializeField] private float m_cameraPositionLerp = 5.0f;
    [SerializeField] private float m_cameraRotationLerp = 10.0f;
    [SerializeField] private float m_camBlockerLerp = 10.0f;
    private float m_camXAngle = 0.0f;
    [SerializeField] float m_pivotHeight = 1.0f;
    [SerializeField] private Image m_healthOverlay;
    [SerializeField] private CanvasGroup m_gameOverGroup;

    [Header("Audio")]
    private AudioSource m_footstepSource;
    [SerializeField] private float m_runningStepMultiplier = 1.0f;
    [SerializeField] private float m_walkingStepMultiplier = 1.0f;
    private int m_footstepCounter = 0;
    private float m_footstepTimer = 0.0f;

    private Inventory_Controller m_inventory;

    private Health m_health;
    public Health Health { get => m_health; set => m_health = value; }

    [SerializeField] private Transform m_pauseMenu;
    private int m_shaderPausedIntId;
    private int m_shaderHealthIntId;

    [SerializeField] private GameObject m_fogObject;


    private void Awake()
    {
        m_singleton = this;
        Ai_Manager.SetPlayerTransform(transform);
        GatherComponents();
        m_visual.parent = null;
        m_cameraPivotY.parent = null;
        m_camera.transform.parent = null;
        m_healthOverlay.enabled = true;
        m_fogObject.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_animatorMoveHash = Animator.StringToHash("move");
        m_animatorLanternHash = Animator.StringToHash("lantern");
        m_animatorFlipHash = Animator.StringToHash("flip");
        m_animatorDeadHash = Animator.StringToHash("dead");

        m_shaderPausedIntId = Shader.PropertyToID("GamePausedInt");
        m_shaderHealthIntId = Shader.PropertyToID("PlayerHealth");
    }

    private void GatherComponents()
    {
        if (m_visual == null) m_visual = transform.GetChild(0);
        m_controller = GetComponent<CharacterController>();
        m_footstepSource = GetComponent<AudioSource>();
        Health = GetComponent<Health>();
        Health.OnHealthZero.AddListener(() => OnDeath());
    }

    private void Update()
    {
        HandlePause();
        GatherInput();
        GroundCheck();
        UpdateCamera();
        Movement();
        HandleInteractionTriggers();
        HandleHealth();
        UpdateAnimations();
    }

    private void HandlePause()
    {
        Shader.SetGlobalInt(m_shaderPausedIntId, GameTime.IsPausedInt());
        if (GameTime.IsPaused() || m_gameOverGroup.alpha >= 1.0f)
        {

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void LateUpdate()
    {
        UpdatePlayerVisual();
        Footsteps();
    }

    private void UpdatePlayerVisual()
    {
        m_visual.position = Vector3.Lerp(m_visual.position, transform.position, m_visualLerp * GameTime.deltaTime);
        m_visual.localScale = Vector3.Lerp(m_visual.localScale, transform.localScale, m_visualLerp * GameTime.deltaTime);
        if (m_moveDirection.magnitude >= 0.1f)
        {
            m_storedMoveDirection = m_moveDirection;
        }
        Quaternion visualRotation = Quaternion.LookRotation(m_storedMoveDirection, Vector3.up);
        m_visual.rotation = Quaternion.Lerp(m_visual.rotation, visualRotation, m_visualRotationLerp * GameTime.deltaTime);

        Shader.SetGlobalVector("PlayerPosition", m_visual.position);
    }

    private void HandleInteractionTriggers()
    {
        m_closestInteraction = null;
        float closestDist = 1000;
        if (m_sceneInteractions.Count >= 1)
        {
            foreach (Interact_Trigger trigger in m_sceneInteractions)
            {
                if (trigger != null)
                {
                    if (trigger.enabled == true)
                    {
                        float dist = Vector3.Distance(transform.position, trigger.transform.position);
                        if (dist <= trigger.InteractionDistance())
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
            }
        }
        if (m_animator == null) return;
        m_closestInteraction?.SetAsClosest(true);
        if (PlayerInput.Interact && IsAlive()) { m_closestInteraction?.TriggerInteraction(); }
    }

    private void GroundCheck()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, m_groundedDist, LayerTools.Default());
        m_isGrounded = false;
        if (hit.collider)
        {
            m_isGrounded = true;
        }
    }

    private void GatherInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        PlayerInput.X = x;
        PlayerInput.Y = y;
        PlayerInput.XY = new Vector2(x, y);
        PlayerInput.XYNormalized = PlayerInput.XY.normalized;
        PlayerInput.XYZ = new Vector3(x, 0.0f, y);
        PlayerInput.XYZNormalized = new Vector3(PlayerInput.XYNormalized.x, 0.0f, PlayerInput.XYNormalized.y);
        PlayerInput.Jump = Input.GetButton("Jump");
        PlayerInput.Crouch = Input.GetKey(KeyCode.LeftShift);
        PlayerInput.Hide = Input.GetKey(KeyCode.LeftControl);
        PlayerInput.Walk = Input.GetKey(m_walkKey);
        PlayerInput.Interact = Input.GetKeyDown(KeyCode.E);
        PlayerInput.MouseVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        PlayerInput.PowderWheel = Input.GetKey(KeyCode.Q);

        if (m_inCutscene) return;

        KeyCode escKeyCode = KeyCode.Escape;
        if (Application.isEditor) escKeyCode = KeyCode.Tab;

        if (Input.GetKeyDown(escKeyCode))
        {
            if (m_pauseMenu.gameObject.activeInHierarchy)
            {
                ClosePauseMenu();
            }
            else if (Inventory_Controller.Singleton().IsOpen())
            {
                Inventory_Controller.Singleton().CloseInventory();
            }
            else
            {
                OpenPauseMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.I) && IsAlive())
        {
            if (Inventory_Controller.Singleton().IsOpen()) Inventory_Controller.Singleton().CloseInventory();
            else if (GameTime.IsPaused() == false) Inventory_Controller.Singleton().OpenInventory();
        }
    }

    private void Movement()
    {
        if (m_inCutscene == false && IsAlive())
        {
            float speed = m_runSpeed;
            if (PlayerInput.Walk) speed = m_walkSpeed;
            m_moveDirection = Quaternion.Euler(0, m_cameraPivotY.eulerAngles.y, 0) * PlayerInput.XYZNormalized * speed;
        }
        else if(m_moveToPosition != null)
        {
            Vector3 dir = m_moveToPosition.position - transform.position;
            dir.y = 0.0f;
            dir.Normalize();
            m_moveDirection = dir * m_runSpeed;
        }
        else
        {
            m_moveDirection = Vector3.zero;
        }
        Vector3 moveDirWithGravity = m_moveDirection;
        moveDirWithGravity.y -= m_gravity;
        m_controller.Move(moveDirWithGravity * GameTime.deltaTime);
    }

    private void UpdateAnimations()
    {
        if (m_animator == null) return;

        if (m_hasLamp == false)
        {
            m_hasLamp = Inventory_Controller.Singleton().HasItemInInventory("The Lamp");
        }
        m_lampObject.SetActive(m_hasLamp);

        m_animator.speed = GameTime.IsPausedInt();

        m_animator.SetFloat(m_animatorMoveHash, m_moveDirection.magnitude / m_runSpeed);
        m_animator.SetBool(m_animatorDeadHash, Health.IsDead());
        m_animator.SetBool(m_animatorFlipHash, PlayerInput.Jump);
        m_animator.SetBool(m_animatorLanternHash, m_hasLamp);
    }

    private void Footsteps()
    {
        if (m_moveDirection.magnitude >= 0.1f)
        {
            if (m_controller.velocity.magnitude > 0.1f)
            {
                float s = m_runningStepMultiplier;
                if (PlayerInput.Walk) s = m_walkingStepMultiplier;
                m_footstepTimer += GameTime.deltaTime * s * m_moveDirection.magnitude;
                if (m_footstepTimer >= 1.0f)
                {
                    GroundAudioType groundType = GetGroundType();
                    AudioClip clip = Audio_Manager.Singleton().GetFootstepAudio(groundType, -1);
                    m_footstepSource.PlayOneShot(clip);
                    m_footstepCounter++;
                    if (m_footstepCounter > Audio_Manager.Singleton().MaxFootstepId()) m_footstepCounter = 0;
                }
                else return;
            }
        }
        m_footstepTimer = 0.0f;
    }

    private GroundAudioType GetGroundType()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity, LayerTools.CreateLayerMask("Ground"));
        if (hit.collider)
        {
            return hit.collider.gameObject.GetComponent<Ground_Audio>().GetGroundType();
        }

        return GroundAudioType.dirt;
    }

    private void UpdateCamera()
    {
        m_camXAngle = Mathf.Clamp(m_camXAngle - PlayerInput.MouseVector.y, m_cameraMinAngle, m_cameraMaxAngle);
        float newDist = Mathf.Lerp(m_cameraCloseDist, m_cameraFarDist, (m_camXAngle / m_cameraMaxAngle));
        m_cameraPivotY.position = transform.position + new Vector3(0.0f, m_pivotHeight, 0.0f);
        float newYAngle = m_cameraPivotY.localEulerAngles.y + PlayerInput.MouseVector.x;
        if (newYAngle >= 360.0f) newYAngle -= 360.0f;
        if (newYAngle <= -360.0f) newYAngle += 360.0f;
        m_cameraPivotY.eulerAngles = new Vector3(0.0f, newYAngle, 0.0f);
        m_cameraPivotX.localEulerAngles = new Vector3(m_camXAngle, 0.0f, 0.0f);
        Vector3 rayStart = transform.position + new Vector3(0.0f, m_pivotHeight, 0.0f);
        Ray ray = new Ray(rayStart, m_cameraTarget.position - rayStart);
        RaycastHit hit;
        if (Physics.SphereCast(rayStart, 0.2f, m_cameraTarget.position - rayStart, out hit, newDist, LayerTools.Default().AddLayer("Ground").AddLayer("Terrain"), QueryTriggerInteraction.Ignore)) newDist = hit.distance;

        if (GameTime.IsPaused()) return;
        if (m_camMovementEnabled == false) return;

        newDist *= -1.0f;
        //m_cameraTarget.localPosition = new Vector3(0.0f, 0.0f, newDist);
        m_cameraTarget.localPosition = new Vector3(0.0f, 0.0f, Mathf.Lerp(m_cameraTarget.localPosition.z, newDist, m_camBlockerLerp * GameTime.deltaTime));
        m_camera.transform.position = new Vector3(m_cameraTarget.position.x, Mathf.Lerp(m_camera.transform.position.y, m_cameraTarget.transform.position.y, m_cameraPositionLerp * GameTime.deltaTime), m_cameraTarget.position.z);
        m_camera.transform.rotation = Quaternion.Lerp(m_camera.transform.rotation, m_cameraPivotX.rotation, m_cameraRotationLerp * GameTime.deltaTime);
    }

    private void HandleHealth()
    {
        Health.HealthUpdate(GameTime.deltaTime);
        Shader.SetGlobalFloat(m_shaderHealthIntId, Health.HealthFloat / Health.MaxHealth);
    }

    //  DIALOGUE STUFF
    #region

    public void DisplayDialogue(Dialogue _dialogue, bool _pauseGame)
    {
        if (_pauseGame) Time.timeScale = 0;
        m_dialoguePanel.gameObject.SetActive(true);

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
        if (_dialogue.GetOptions().Count >= 1)
        {
            for (int i = 0; i < _dialogue.GetOptions().Count; i++)
            {
                m_dialogueOptions.GetChild(i).gameObject.SetActive(true);
                m_dialogueOptions.GetChild(i).GetChild(0).GetComponent<Text>().text = _dialogue.GetOptions()[i].GetTitle();
            }
        }
        else
        {
            m_dialogueOptions.GetChild(0).gameObject.SetActive(true);
            m_dialogueOptions.GetChild(0).GetChild(0).GetComponent<Text>().text = "Close";
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void DisplayDialogFromChoice(Button _button)
    {
        if (m_currentDialogue != null)
        {
            if (m_currentDialogue.GetOptions().Count >= 1)
            {
                Dialogue newDialogue = m_currentDialogue.GetOptions()[_button.transform.GetSiblingIndex()];
                if (newDialogue != null) DisplayDialogueRecursive(newDialogue);
            }
            else CloseCurrentDialogue();

        }
    }

    public void CloseCurrentDialogue()
    {
        m_dialoguePanel.gameObject.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    #endregion

    public void ClosePauseMenu()
    {
        m_pauseMenu.gameObject.SetActive(false);
        GameTime.UnPause();
    }
    public void OpenPauseMenu()
    {
        m_pauseMenu.gameObject.SetActive(true);
        GameTime.Pause();
    }


    public void UsePowder(Powder _powder)
    {

    }

    public bool IsDead()
    {
        return Health.IsDead();
    }
    public bool IsAlive()
    {
        return !IsDead();
    }
    public void KillPlayer()
    {
        Health.DoDamage(Health.MaxHealth);
    }

    public void OnDeath() { StartCoroutine(OnDeathCoroutine()); }
    private IEnumerator OnDeathCoroutine()
    {
        yield return new WaitForSecondsRealtime(3.0f);
        if(m_gameOverGroup.alpha == 0.0f)
        {
            m_gameOverGroup.gameObject.SetActive(true);
            m_gameOverGroup.interactable = false;
            float t = 0.0f;
            while (t < 1.0f)
            {
                t += Time.deltaTime;
                m_gameOverGroup.alpha = t;
                yield return new WaitForEndOfFrame();
            }
            m_gameOverGroup.alpha = 1.0f;
            m_gameOverGroup.interactable = true;
        }        
        yield return null;
    }

    public void TeleportPlayer(Vector3 _newPosition, Vector3 _newForward, Vector2 _camRotation)
    {
        _newForward.y = 0;
        _newForward.Normalize();
        m_moveDirection = _newForward;
        transform.position = _newPosition;
        CamYAxis().eulerAngles = new Vector3(0, _camRotation.y, 0);
        m_camXAngle = _camRotation.x;
        Physics.SyncTransforms();
    }
}