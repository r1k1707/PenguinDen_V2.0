using UnityEngine;
using System.Collections;
using TMPro;

public class GameController : MonoBehaviour
{
    private int dealersFirstCard = -1;
    private bool playerTurn;
    private bool gameOver;

    [Header("Money System")]
    public int startingMoney;
    public int betAmount;
    public TMP_Text moneyText;
    public TMP_Text betText;
    private int money;

    [Header("References")]
    public CardStack player;
    public CardStack dealer;
    public CardStack deck;
    public TMP_Text winnerText;
    public GameObject exitGameObject;

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
        gameOver = true;
        playerTurn = false;

        if (winnerText != null)
        {
            winnerText.text = "Choose your bet!";
            winnerText.enabled = true;
        }
    }

    private void Start()
    {
        money = startingMoney;
        UpdateMoneyDisplay();
        UpdateBetDisplay();
        gameOver = true;

        if (winnerText != null)
        {
            winnerText.text = "Choose your bet!";
            winnerText.enabled = true;
        }
        if (exitGameObject != null)
        {
            exitGameObject.SetActive(false);
        }
    }
    public void StartRound()
    {
        if (!gameOver)
            return;

        StartGame();
    }

    private void UpdateMoneyDisplay()
    {
        if (moneyText != null)
        {
            moneyText.text = "Money: $" + money;
        }
    }
    private void UpdateBetDisplay()
    {
        if (betText != null)
        {
            betText.text = "You bet: $" + betAmount;
        }
    }
    public void SetBet(int amount)
    {
        if (playerTurn || !gameOver)
            return;

        if (amount <= 0 || amount > money)
            return;

        betAmount = amount;
        UpdateBetDisplay();
        Debug.Log("Bet selected: $" + betAmount);
    }
    public void AllIn()
    {
        if (playerTurn || !gameOver)
            return;

        if (money <= 0)
            return;

        betAmount = money;
        UpdateBetDisplay();
        Debug.Log("ALL IN! Bet: $" + betAmount);
    }

    private void StartGame()
    {

        if (money <= 0)
        {
            gameOver = true;
            playerTurn = false;

            if (winnerText != null)
            {
                winnerText.text = "You're out of money!";
                winnerText.enabled = true;
            }

            if (exitGameObject != null)
            {
                exitGameObject.SetActive(true);
            }

            return;
        }

        if (betAmount <= 0 || betAmount > money)
        {
            if (winnerText != null)
            {
                winnerText.text = "Choose a valid bet!";
                winnerText.enabled = true;
            }

            return;
        }

        if (winnerText != null)
        {
            winnerText.enabled = false;
        }

        money -= betAmount;
        UpdateMoneyDisplay();
        playerTurn = true;
        gameOver = false;

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
            winnerText.text = "You bust! You lose $" + betAmount;
        }
        else if (dealerScore > 21)
        {
            money += betAmount * 2;
            winnerText.text = "You win $" + betAmount + "!";
        }
        else if (playerScore > dealerScore)
        {
            money += betAmount * 2;
            winnerText.text = "You win $" + betAmount + "!";
        }
        else if (playerScore < dealerScore)
        {
            winnerText.text = "You lose $" + betAmount + "!";
        }
        else
        {
            money += betAmount;
            winnerText.text = "It's a tie! Your bet is returned.";
        }
        UpdateMoneyDisplay();
        winnerText.enabled = true;
        gameOver = true;

        if (money <= 0 && exitGameObject != null)
        {
            exitGameObject.SetActive(true);
        }
        Debug.Log("Player: " + playerScore + " | Dealer: " + dealerScore);
    }
    public void ExitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        Application.Quit();
    }
}
