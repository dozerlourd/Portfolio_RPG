using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;  // a character to follow
    [SerializeField] Vector3 offset = new Vector3(0f, 1.5f, -3f); // basic distance
    [SerializeField] float rotationSpeed = 2f; // rotation speed
    [SerializeField] float zoomSpeed = 2f; // zoom speed
    [SerializeField] float minZoom = -1.5f; // min zoom speed
    [SerializeField] float maxZoom = -5f; // max zoom speed

    private float currentX = 0f;
    private float currentY = 0f;
    private float distance;
    private Vector2 lookInput = Vector2.zero; // mouse input value
    private float zoomInput = 0f; // zoom input value

    private void Start()
    {
        distance = offset.z;
        Cursor.lockState = CursorLockMode.Locked;
    }

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
