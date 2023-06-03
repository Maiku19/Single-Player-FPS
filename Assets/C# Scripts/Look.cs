using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Look : MonoBehaviour
{
    public float mouseSensitivity = 1;

    public Transform playerBody;
    public Transform fpsCamera;
    float _xRotation = 0f;

    public static Look Instance { get { return _instance; } }
    static Look _instance;

    public float MouseX { get => Input.GetAxis("Mouse X") * mouseSensitivity * 100 * Time.deltaTime; }
    public float MouseY { get => Input.GetAxis("Mouse Y") * mouseSensitivity * 100 * Time.deltaTime; }

    public event UnityAction OnPreRotate;
    public event UnityAction OnPostRotate;

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        _xRotation -= MouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        OnPreRotate?.Invoke();

        fpsCamera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * MouseX);

        OnPostRotate?.Invoke();
    }
}
