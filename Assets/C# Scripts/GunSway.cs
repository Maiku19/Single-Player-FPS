using UnityEngine;

public class GunSway : MonoBehaviour
{
    [SerializeField] float _swayIntensity = 2.5f;
    [SerializeField] float _swaySpeed = 1f;

    void LateUpdate()
    {
        
        if(Mathf.Approximately(Look.Instance.MouseY, 0) && Mathf.Approximately(Look.Instance.MouseX, 0)) { return; }

        transform.localEulerAngles = Vector3.Slerp(new(-Look.Instance.MouseY, Look.Instance.MouseX), transform.localEulerAngles, Time.deltaTime * _swaySpeed);
        /*transform.localEulerAngles = sway * _swayIntensity;*/
    }
}
