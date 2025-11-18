using UnityEngine;

public class StretchToCamera : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            Debug.LogWarning("StretchToCamera: No SpriteRenderer or Sprite found!");
            return;
        }
        
        Camera cam = Camera.main;
        
        if (cam == null)
        {
            Debug.LogWarning("StretchToCamera: No main camera found!");
            return;
        }
        
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;
        
        float worldHeight = cam.orthographicSize * 2f;
        float worldWidth = worldHeight * cam.aspect;
        
        Vector3 scale = transform.localScale;
        scale.x = worldWidth / spriteWidth;
        scale.y = worldHeight / spriteHeight;

        transform.localScale = scale;
    }
}