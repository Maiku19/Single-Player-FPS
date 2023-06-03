using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalTimeFade : MonoBehaviour
{
    [SerializeField] DecalProjector _decalProjector;
    [SerializeField] float _fadeTime = 30;
    [SerializeField] bool _destroyOnFadeOut = true;

    void Update()
    {
        _decalProjector.fadeFactor = math.clamp(_decalProjector.fadeFactor - Time.deltaTime / _fadeTime, 0, 1);

        if(_destroyOnFadeOut && _decalProjector.fadeFactor <= 0) { Destroy(gameObject); }
    }
}
