using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private float sensitivity = 120f;
    [SerializeField] private float distance = 6f;
    [SerializeField] private float minY = -30f;
    [SerializeField] private float maxY = 60f;

    private float _yaw;
    private float _pitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        transform.position = target.position;
        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0);
    }

    public void AddInput(Vector2 delta)
    {
        _yaw += delta.x * sensitivity * Time.deltaTime;
        _pitch -= delta.y * sensitivity * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, minY, maxY);
    }
}