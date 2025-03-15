using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;  // ���� ĳ����
    [SerializeField] Vector3 offset = new Vector3(0f, 1.5f, -3f); // �⺻ �Ÿ�
    [SerializeField] float rotationSpeed = 2f; // ȸ�� �ӵ�
    [SerializeField] float zoomSpeed = 2f; // �� �ӵ�
    [SerializeField] float minZoom = -1.5f; // �ּ� �� �Ÿ�
    [SerializeField] float maxZoom = -5f; // �ִ� �� �Ÿ�

    private float currentX = 0f;
    private float currentY = 0f;
    private float distance;
    private Vector2 lookInput = Vector2.zero; // ���콺 �Է� ��
    private float zoomInput = 0f; // �� �Է� ��

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
        // ���콺 ȸ�� ó��
        currentX += lookInput.x * rotationSpeed * Time.deltaTime;
        currentY -= lookInput.y * rotationSpeed * Time.deltaTime;
        currentY = Mathf.Clamp(currentY, -20f, 60f);

        // �� ó��
        distance = Mathf.Clamp(distance + zoomInput * zoomSpeed, maxZoom, minZoom);
    }

    private void LateUpdate()
    {
        // ī�޶� ȸ�� ����
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0, offset.y, distance);

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * offset.y);
    }
}
