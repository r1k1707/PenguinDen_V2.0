using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CardStack))]
public class CardStackView : MonoBehaviour
{
    CardStack deck;
    Dictionary<int, CardView> fetchedCards;

    public Vector3 start;
    public float cardOffset;
    public bool faceUp = false;
    public bool reverseLayerOrder = false;
    public GameObject cardPrefab;

    public void Toggle(int card, bool isFaceUp)
    {
        if (!fetchedCards.ContainsKey(card)) return;

        CardView view = fetchedCards[card];
        if (view.IsFaceUp == isFaceUp) return;

        view.IsFaceUp = isFaceUp;
        CardModel model = view.Card.GetComponent<CardModel>();
        if (model != null) model.ToggleFace(isFaceUp);
    }

    public void Clear()
    {
        deck.Reset();

        foreach (CardView view in fetchedCards.Values)
        {
            Destroy(view.Card);
        }

        fetchedCards.Clear();
    }

    void Awake()
    {
        fetchedCards = new Dictionary<int, CardView>();
        deck = GetComponent<CardStack>();
        ShowCards();

        deck.CardRemoved += deck_CardRemoved;
        deck.CardAdded += deck_CardAdded;
    }

    void deck_CardAdded(object sender, CardEventArgs e)
    {
        float co = cardOffset * deck.CardCount;
        Vector3 temp = transform.TransformPoint(start + new Vector3(0f, 0f, co));
        AddCard(temp, e.CardIndex, deck.CardCount, true);
    }

    void deck_CardRemoved(object sender, CardEventArgs e)
    {
        if (fetchedCards.ContainsKey(e.CardIndex))
        {
            Destroy(fetchedCards[e.CardIndex].Card);
            fetchedCards.Remove(e.CardIndex);
        }
    }

    void Update()
    {
        ShowCards();
    }

    public void ShowCards()
    {
        int cardCount = 0;

        if (deck.HasCards)
        {
            foreach (int i in deck.GetCards())
            {
                float co = cardOffset * cardCount;
                Vector3 temp = transform.TransformPoint(start + new Vector3(0f, 0f, co));
                AddCard(temp, i, cardCount);
                cardCount++;
            }
        }
    }

    void AddCard(Vector3 position,int cardIndex, int positionalIndex, bool doAnimation = false)
    {
        if (fetchedCards.ContainsKey(cardIndex))
        {
            fetchedCards[cardIndex].Card.transform.position = position;
            return;
        }

        GameObject cardCopy = (GameObject)Instantiate(cardPrefab);
        cardCopy.transform.position = position;

        CardModel cardModel = cardCopy.GetComponent<CardModel>();
        cardModel.cardIndex = cardIndex;
        if (doAnimation)
        {
            cardModel.ToggleFace(faceUp);
        }
        else
        {
            cardModel.ToggleFaceNoAnimation(faceUp);
        }

        SpriteRenderer spriteRenderer = cardCopy.GetComponent<SpriteRenderer>();
        if (reverseLayerOrder)
        {
            spriteRenderer.sortingOrder = 51 - positionalIndex;
        }
        else
        {
            spriteRenderer.sortingOrder = positionalIndex;
        }

        fetchedCards.Add(cardIndex, new CardView(cardCopy));
    }
}