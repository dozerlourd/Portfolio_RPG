using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;  // 따라갈 캐릭터
    [SerializeField] Vector3 offset = new Vector3(0f, 1.5f, -3f); // 기본 거리
    [SerializeField] float rotationSpeed = 2f; // 회전 속도
    [SerializeField] float zoomSpeed = 2f; // 줌 속도
    [SerializeField] float minZoom = -1.5f; // 최소 줌 거리
    [SerializeField] float maxZoom = -5f; // 최대 줌 거리

    private float currentX = 0f;
    private float currentY = 0f;
    private float distance;
    private Vector2 lookInput = Vector2.zero; // 마우스 입력 값
    private float zoomInput = 0f; // 줌 입력 값

    private void Start()
    {
        distance = offset.z;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //public void OnMoveInput(InputAction.CallbackContext context)
    //{
    //    Vector2 input = context.ReadValue<Vector2>();
    //    print(input);
    //    //direction = new Vector3(input.x, 0f, input.y);
    //}

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        zoomInput = context.ReadValue<float>();
    }

    private void Update()
    {
        // 마우스 회전 처리
        currentX += lookInput.x * rotationSpeed * Time.deltaTime;
        currentY -= lookInput.y * rotationSpeed * Time.deltaTime;
        currentY = Mathf.Clamp(currentY, -20f, 60f);

        // 줌 처리
        distance = Mathf.Clamp(distance + zoomInput * zoomSpeed, maxZoom, minZoom);
    }

    private void LateUpdate()
    {
        // 카메라 회전 적용
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0, offset.y, distance);

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * offset.y);
    }
}
