using UnityEngine;

public class CardModel : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private CardFlipper flipper;
    public Sprite[] faces;
    public Sprite cardBack;
    public int cardIndex;

    public void ToggleFace(bool showFace)
    {
        if (cardIndex < 0 || cardIndex >= faces.Length)
        {
            Debug.LogError($"Invalid cardIndex: {cardIndex}. " + $"The faces array contains {faces.Length} sprites.", gameObject);
            return;
        }

    }

    public void ToggleFaceNoAnimation(bool showFace)
    {
        if (showFace)
        {
            _renderer.sprite = faces[cardIndex];
        }
        else
        {
            _renderer.sprite = cardBack;
        }
    }

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        flipper = GetComponent<CardFlipper>();
    }
}