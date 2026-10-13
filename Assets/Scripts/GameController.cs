using UnityEngine;
using System.Collections;
using TMPro;

public class GameController : MonoBehaviour
{
    private int dealersFirstCard = -1;
    private bool playerTurn;
    private bool gameOver;

    public CardStack player;
    public CardStack dealer;
    public CardStack deck;

    public TextMeshPro winnerText;

    public void Hit()
    {
        if (!playerTurn || gameOver) return;
        if (!deck.HasCards)
        {
            Debug.LogWarning("The deck is empty!");
            return;
        }

        player.Push(deck.Pop());

        if (player.HandValue() > 21)
        {
            playerTurn = false;
            StartCoroutine(DealersTurn());
        }
    }

    public void Stand()
    {
        if (!playerTurn || gameOver) return;
        playerTurn = false;
        StartCoroutine(DealersTurn());
    }

    public void PlayAgain()
    {
        if (!gameOver) return;

        StopAllCoroutines();

        player.GetComponent<CardStackView>().Clear();
        dealer.GetComponent<CardStackView>().Clear();

        player.Reset();
        dealer.Reset();
        deck.CreateDeck();

        winnerText.text = "";
        dealersFirstCard = -1;
        gameOver = false;

        StartGame();
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        playerTurn = true;

        for (int i = 0; i < 2; i++)
        {
            player.Push(deck.Pop());
            HitDealer();
        }

        Debug.Log("Your hand: " + player.HandValue());
    }

    private void HitDealer()
    {
        if (!deck.HasCards) return;

        int card = deck.Pop();

        if (dealersFirstCard < 0) dealersFirstCard = card;

        dealer.Push(card);

        if (dealer.CardCount >= 2)
        {
            CardStackView view = dealer.GetComponent<CardStackView>();
            view.Toggle(card, true);
        }
    }

    private IEnumerator DealersTurn()
    {
        playerTurn = false;

        CardStackView view = dealer.GetComponent<CardStackView>();
        view.Toggle(dealersFirstCard, true);
        view.ShowCards();

        yield return new WaitForSeconds(1f);

        while (dealer.HandValue() < 17 && deck.HasCards)
        {
            HitDealer();
            yield return new WaitForSeconds(1f);
        }

        int playerScore = player.HandValue();
        int dealerScore = dealer.HandValue();

        if (playerScore > 21)
        {
            winnerText.text = "Sorry, you bust!";
        }
        else if (dealerScore > 21)
        {
            winnerText.text = "You win!";
        }
        else if (playerScore > dealerScore)
        {
            winnerText.text = "You win!";
        }
        else if (playerScore < dealerScore)
        {
            winnerText.text = "You lose!";
        }
        else
        {
            winnerText.text = "It's a tie!";
        }

        gameOver = true;
        Debug.Log("Player: " + playerScore + " | Dealer: " + dealerScore);
    }
}
