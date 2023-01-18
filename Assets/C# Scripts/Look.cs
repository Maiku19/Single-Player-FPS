using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look : MonoBehaviour
{
    public float mouseSensitivity = 1;

    public Transform playerBody;
    public Transform fpsCamera;
    float xRotation = 0f;

    public static Look Instance { get { return _Instance; } }
    static Look _Instance;

    public float MouseX { get; private set; }
    public float MouseY { get; private set; }

    private void Awake()
    {
        _Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        MouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100 * Time.deltaTime;
        MouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100 * Time.deltaTime;

        xRotation -= MouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        fpsCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * MouseX);
    }
}
