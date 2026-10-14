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
        if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();
        if (flipper == null) flipper = GetComponent<CardFlipper>();

        if (showFace && (faces == null || cardIndex < 0 || cardIndex >= faces.Length))
        {
            Debug.LogError("Invalid card index: " + cardIndex, gameObject);
            return;
        }

        if (showFace)
        {
            if (flipper != null) flipper.FlipCard(_renderer.sprite, faces[cardIndex], cardIndex);
            else ToggleFaceNoAnimation(true);
        }
        else
        {
            if (cardBack == null)
            {
                Debug.LogError("Card back sprite is missing!", gameObject);
                return;
            }

            if (flipper != null) flipper.FlipCard(_renderer.sprite, cardBack, -1);
            else ToggleFaceNoAnimation(false);
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