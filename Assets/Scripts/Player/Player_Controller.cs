using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[RequireComponent(typeof(Rigidbody))]
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

    private void Awake()
    {
        Ai_Manager.SetPlayerTransform(transform);
        GatherComponents();
        m_visual.parent = null;
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
        UpdateAnimations();
        Movement();
    }

    private void LateUpdate()
    {
        m_visual.position = Vector3.Lerp(m_visual.position, transform.position, m_visualLerp * Time.deltaTime);
        m_visual.localScale = Vector3.Lerp(m_visual.localScale, transform.localScale, m_visualLerp * Time.deltaTime);
        m_visual.rotation = Quaternion.Lerp(m_visual.rotation, transform.rotation, m_visualLerp * Time.deltaTime);
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
}