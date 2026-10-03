using UnityEngine;

public class DebugChangeCard : MonoBehaviour
{
    CardModel cardModel;
    int cardIndex = 0;
    public GameObject card;

    void Awake()
    {
        cardModel = card.GetComponent<CardModel>();
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 28), "Hit me!"))
        {
            if (cardIndex >= cardModel.faces.Length)
            {
                cardIndex = 0;
                cardModel.ToggleFace(false);
            }
            else
            {
                cardModel.cardIndex = cardIndex;
                cardModel.ToggleFace(true);
                cardIndex++;
            }
        }
    }
}
