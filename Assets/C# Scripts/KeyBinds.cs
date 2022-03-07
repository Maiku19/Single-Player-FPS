using UnityEngine;

public class KeyBinds
{
    public static Vector2 Movement
    {
        get
        {
            Vector2 vector2 = Vector2.zero;

            if (Input.GetKey(MoveForward)) { vector2.y += 1; }
            if (Input.GetKey(MoveBack)) { vector2.y -= 1; }

            if (Input.GetKey(MoveRight)) { vector2.x += 1; }
            if (Input.GetKey(MoveLeft)) { vector2.x -= 1; }

            return vector2;
        }
    }

    public static KeyCode MoveForward
    {
        get
        {
            return (KeyCode)PlayerPrefs.GetInt("Key MoveForward", (int)KeyCode.W);
        }
        set
        {
            PlayerPrefs.SetInt("Key MoveForward", (int)value);
        }
    }
    public static KeyCode MoveLeft
    {
        get
        {
            return (KeyCode)PlayerPrefs.GetInt("Key MoveLeft", (int)KeyCode.A);
        }
        set
        {
            PlayerPrefs.SetInt("Key MoveLeft", (int)value);
        }
    }
    public static KeyCode MoveRight
    {
        get
        {
            return (KeyCode)PlayerPrefs.GetInt("Key MoveRight", (int)KeyCode.D);
        }
        set
        {
            PlayerPrefs.SetInt("Key MoveRight", (int)value);
        }
    }
    public static KeyCode MoveBack
    {
        get
        {
            return (KeyCode)PlayerPrefs.GetInt("Key MoveBack", (int)KeyCode.S);
        }
        set
        {
            PlayerPrefs.SetInt("Key MoveBack", (int)value);
        }
    }
    public static KeyCode Jump
    {
        get
        {
            return (KeyCode)PlayerPrefs.GetInt("Key Jump", (int)KeyCode.Space);
        }
        set
        {
            PlayerPrefs.SetInt("Key Jump", (int)value);
        }
    }
    public static KeyCode Shoot
    {
        get
        {
            return (KeyCode)PlayerPrefs.GetInt("Key Shoot", (int)KeyCode.Mouse0);
        }
        set
        {
            PlayerPrefs.SetInt("Key Shoot", (int)value);
        }
    }
    public static KeyCode Reload
    {
        get
        {
            return (KeyCode)PlayerPrefs.GetInt("Key Reload", (int)KeyCode.R);
        }
        set
        {
            PlayerPrefs.SetInt("Key Reload", (int)value);
        }
    }
}
