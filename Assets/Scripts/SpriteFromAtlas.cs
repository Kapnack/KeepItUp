using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SpriteFromAtlas : MonoBehaviour
{
    [SerializeField] private SpriteAtlas atlas;
    [SerializeField] private string spriteName;

    private void Awake()
    {
        if (TryGetComponent(out Image image))
            image.sprite = atlas.GetSprite(spriteName);
        else if (TryGetComponent(out SpriteRenderer spriteRenderer))
            spriteRenderer.sprite = atlas.GetSprite(spriteName);
    }
}