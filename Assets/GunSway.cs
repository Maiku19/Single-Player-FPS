using Unity.Mathematics;
using UnityEngine;

public class GunSway : MonoBehaviour
{
    [SerializeField] float _swayIntensity = 2.5f;
    [SerializeField] float _swaySpeed = 1f;

    Vector3 CamMove { get => new(-Look.Instance.MouseY, Look.Instance.MouseX); }
    Vector3 Sway
    {
        get
        {
            if (CamMove == Vector3.zero)
            {
                if (_count < CountThreshold) { _count++; }
                else if (_sway != Vector3.zero)
                {
                    _count = 0;
                    _sway = Vector3.zero;
                }
            }
            else
            {
                _count = 0;
                _sway = CamMove * _swayIntensity;
            }

            return _sway;
        }
    }
    Vector3 _sway = Vector3.zero;
    int _count = 0;
    const int CountThreshold = 2;

    void Update()
    {
        RotateToTarget();
    }

    void RotateToTarget()
    {
        transform.localEulerAngles = Vector3.Slerp(Sway, transform.localEulerAngles, Time.deltaTime * _swaySpeed);
    }
}
