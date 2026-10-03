using UnityEngine;

public class CardModel : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public Sprite[] faces;
    public Sprite cardBack;

    public int cardIndex;

    public void ToggleFace(bool showFace)
    {
        if (showFace)
        {
            spriteRenderer.sprite = faces[cardIndex];
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
