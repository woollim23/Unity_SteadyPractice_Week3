using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpPower;
    private Vector2 curMovementInput;
    public LayerMask groundLayerMask;
    public LayerMask wallLayer;
    private bool attatchWall = false;
    private float wallRayDistance = 0.5f;  // Raycast 거리
    private float climbSpeed = 3f;

    private float bottomOffset;


    [Header("Look")]
    public Transform cameraContainer; // 카메라 방향을 지정할 트랜스폼
    public float minXLook; // 회전 범위 최저값
    public float maxXLook; // 회전 범위 최대값
    private float camCurXRot; // 최종 출력 마우스 델타값
    public float lookSensitivity; // 회전 민감도
    private Vector2 mouseDelta; // inputsystem으로 입력 받는 마우스 델타값
    public bool canLook = true;

    public Action inventory;
    public event Action onSettingScreen;

    private Rigidbody _rigidbody;
    CapsuleCollider _capsuleCollider;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 커서 안보이게 숨기기
        bottomOffset = _capsuleCollider.bounds.extents.y; //  트랜스폼 높이의 절반
    }

    private void FixedUpdate()
    {
        Move();
        CheckWall();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    private void CheckWall()
    {
        RaycastHit hit;
        Vector3 rayDirection = transform.forward;  // 캐릭터가 바라보는 방향으로 레이 쏘기
        Vector3 rayOrigin = transform.position + Vector3.down * (GetComponent<Collider>().bounds.extents.y); // 캐릭터의 가장 아래쪽 위치

        // 레이캐스트가 벽을 감지할 때
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, wallRayDistance, wallLayer))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                AttachToWall(hit);
            }
        }
        else
        {
            DetachFromWall();
        }
    }

    private void AttachToWall(RaycastHit hit)
    {
        _rigidbody.useGravity = false; // 중력 비활성화
        attatchWall = true; // 벽에 붙었음

        // 벽의 방향으로 캐릭터 회전
        Quaternion targetRotation = Quaternion.LookRotation(-hit.normal);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5);
    }

    private void DetachFromWall()
    {
        _rigidbody.useGravity = true;  // 중력 다시 켜기
        attatchWall = false;
    }

    private void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;

        if (attatchWall)
        {
            // 벽에 붙어 있을 때 오르내릴수 있도록 설정
            if (curMovementInput.y > 0) // W 키를 누른 경우
            {
                _rigidbody.AddForce(Vector2.up * climbSpeed, ForceMode.Impulse);
            }
            else if (curMovementInput.y < 0) // S 키를 누른 경우
            {
                _rigidbody.AddForce(Vector2.down * climbSpeed, ForceMode.Impulse);
            }
            else
            {
                _rigidbody.velocity = Vector3.zero; // 키를 누르지 않은 경우 움직임 멈춤
            }
        }
        else
        {
            // 일반 이동
            dir *= moveSpeed;
            dir.y = _rigidbody.velocity.y; // 현재 y 속도 유지

            _rigidbody.velocity = dir;
        }
    }

    void CameraLook()
    {
        // 상하 회전 - 카메라
        camCurXRot += mouseDelta.y * lookSensitivity;
        // 상하 각도
        // y 델타값에 민감도를 곱하는 이유 : 마우스 포인트 y위치 변화 -> 상하 각도 변화
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        // 상하 각도 최솟값 최댓값 범주를 지정(덤블링 할게 아닌 이상 상하엔 제한이 필요)
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        // Rotation x에 마우스 델타 y 값을 넣어줘야함
        // 카메라 로컬 좌표를 돌려줌
        // 마우스 동작 방향과 값을 맞추기 위해 -를 붙임

        // 좌우 회전 - 캐릭터
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
        // 캐릭터 오일러각 회전
        // Rotation y에 마우스 델타 x값을 넣어줘야함 * 마우스 민감도
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            // 버튼 클릭하는 동안 수행
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if ((context.phase == InputActionPhase.Started))
        {
            if(IsGrounded())
            {
                _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
                CharacterManager.Instance.Player.condition.UseStamina(jumpPower / 10);
            }
        }
    }

    public void OnSetting(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            onSettingScreen?.Invoke();
            ToggleCursor();
        }
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke(); // UIInventory.Toggle()
            ToggleCursor();
        }
    }

    public void OnSwitchCamera(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            CameraManager.Instance.SwitchMainCamera();
        }
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
       {
            new Ray(transform.position - new Vector3(0, bottomOffset, 0) + (transform.forward * 0.2f), Vector3.down),
            new Ray(transform.position - new Vector3(0, bottomOffset, 0) + (-transform.forward * 0.2f), Vector3.down),
            new Ray(transform.position - new Vector3(0, bottomOffset, 0) + (transform.right * 0.2f), Vector3.down),
            new Ray(transform.position - new Vector3(0, bottomOffset, 0) + (-transform.right * 0.2f), Vector3.down)
       };

        for (int i = 0; i < rays.Length; i++)
        {
            Debug.DrawRay(rays[i].origin, rays[i].direction * 0.2f, Color.red);
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
