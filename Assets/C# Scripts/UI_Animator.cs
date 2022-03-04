using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Animator : MonoBehaviour
{
    public bool fadeIn = false;
    public bool fadeOut = false;
    public float alphaPerSec = 1f;
    public bool scale;
    public Vector2 scalePerSec;
    public bool clampSize;
    public Vector2 sizeClampPos;
    public Vector2 sizeClampNeg;

    public TextMeshProUGUI text;
    public Image image;
    public SpriteRenderer sprite;





    void Update()
    {
        if (fadeOut)
        {
            if (text != null)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - alphaPerSec * Time.deltaTime);
            }

            if (sprite != null)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - alphaPerSec * Time.deltaTime);
            }

            if (image != null)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - alphaPerSec * Time.deltaTime);
            }
        }

        if (fadeIn)
        {
            if (text != null && text.enabled)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + alphaPerSec * Time.deltaTime);
            }

            if (sprite != null && sprite.enabled)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a + alphaPerSec * Time.deltaTime);
            }

            if (image != null && image.enabled)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + alphaPerSec * Time.deltaTime);
            }
        }

        if (scale)
        {
            transform.localScale += new Vector3(scalePerSec.x, scalePerSec.y, 1) * Time.deltaTime;

            if(clampSize) transform.localScale = new Vector3(Mathf.Clamp(transform.localScale.x, sizeClampNeg.x, sizeClampPos.x), Mathf.Clamp(transform.localScale.y, sizeClampNeg.y, sizeClampPos.y), 1);
        }
    }
}
