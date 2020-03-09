using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PhotonGameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    //public static event DealingCardsEvent OnDealingCards;
    public static PhotonGameManager instance;
    public static List<Player> players;
    public static event GameStart onGameStart;
    //static Player currentPlayer;

    public static Player CurrentPlayer { get; set; }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            players = new List<Player>();

            // players = FindObjectsOfType<Player>().ToList();
            //  currentPlayer = players[0];
        }
    }

    public void StartGame()
    {
        CurrentPlayer = players[0];

        if (onGameStart != null)
            onGameStart();

        Dealer.StartGame();
        foreach (Player p in players)
            p.SetupHand();


        //if (OnDealingCards != null)
        //OnDealingCards();

        // StartCoroutine(BettingRound());
        Dealer.StartBettingRound();
    }

    public static void DeclareWinner(List<Player> playersLeft)
    {
        if (playersLeft.Count == 1)
        {
            Debug.Log("Everyone folded, " + playersLeft[0] + " wins be default!");
            UIManager.DeclareWinner(playersLeft);
            //Dealer.GiveWinnersEarnings(playersLeft.Select(players => players.photonView.ViewID).ToArray());
            playersLeft[0].AddWinningsToMoney(Dealer.Pot);
        }
        else
        {
            foreach (Player p in players)
                p.SetHandStrength();

            List<Player> winners = new List<Player>();
            Hand strongestHand = players[0].hand.strength;

            foreach (Player p in players)
            {
                if (p.hand.strength > strongestHand)
                    strongestHand = p.hand.strength;
            }

            foreach (Player p in players)
            {
                if (p.hand.strength == strongestHand)
                    winners.Add(p);
            }

            if (winners.Count == 1)
            {
                Debug.Log(winners[0].name + " won with " + winners[0].hand.strength + " of " + winners[0].hand.rankingCard.value);

            }
            else
            {
                Debug.Log("Multiple players have " + winners[0].hand.strength);
                winners = BreakTie(winners);
                if (winners.Count == 1)
                {
                    Debug.Log(winners[0].name + " won the tie breaker with " + winners[0].hand.strength + " of " + winners[0].hand.rankingCard.value);
                }
                else
                {
                    Debug.Log("The following players are tied with " + winners[0].hand.strength + " of " + winners[0].hand.rankingCard.value);

                    foreach (Player p in winners)
                        Debug.Log(p.name);
                }
            }
            UIManager.DeclareWinner(winners);
            //Dealer.GiveWinnersEarnings(winners.Select(players => players.photonView.ViewID).ToArray());
            foreach (Player winner in winners)
            {
                winner.AddWinningsToMoney(Dealer.Pot / winners.Count);
            }
        }
    }
    static List<Player> BreakTie(List<Player> tiedPlayers)
    {
        Debug.Log("Breaking tie with ranking cards");
        CardValue highestRank = tiedPlayers[0].hand.rankingCard.value;
        Debug.Log("Assuming highest rank is: " + highestRank);
        List<Player> winningPlayers;
        foreach (Player p in tiedPlayers)
        {
            if (p.hand.rankingCard.value > highestRank)
                highestRank = p.hand.rankingCard.value;
        }
        Debug.Log("Highest rank found: " + highestRank);
        winningPlayers = tiedPlayers.Where(player => player.hand.rankingCard.value == highestRank).ToList();
        if (winningPlayers.Count > 1)
        {
            Debug.Log("Multiple players have a ranking card of " + highestRank);
            winningPlayers = BreakTieByKickers(winningPlayers);

        }

        return winningPlayers;
    }
    static List<Player> BreakTieByKickers(List<Player> tiedPlayers)
    {
        Debug.Log("Breaking tie with kickers");
        CardValue highestKicker;
        int bounds = tiedPlayers[0].hand.tieBreakerCards.Count;
        Debug.Log("Checking up to " + bounds + " kickers");
        List<Player> winningPlayers = tiedPlayers;
        for (int i = 0; i < bounds - 1; i++)
        {
            highestKicker = tiedPlayers[i].hand.tieBreakerCards[i].value;
            Debug.Log("Highest kicker is assumed to be: " + highestKicker);
            foreach (Player p in tiedPlayers)
            {
                if (p.hand.tieBreakerCards[i].value > highestKicker)
                    highestKicker = p.hand.tieBreakerCards[i].value;
            }
            Debug.Log("Highest kicker found is " + highestKicker);
            winningPlayers = tiedPlayers.Where(player => player.hand.tieBreakerCards[i].value == highestKicker).ToList();
            if (winningPlayers.Count == 1)
                break;
            Debug.Log("Several players have equal kicker, attempting next possible kicker");
        }
        return winningPlayers;
    }

    IEnumerator BettingRound()
    {
        List<Player> bettingPlayers = GameManager.players;

        yield return null;


    }

    Sprite BuildSpriteFromByteArray(byte[] imageData)
    {
        Sprite playerIcon;
        Texture2D newTexture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        newTexture.LoadImage(imageData);
        newTexture.Apply();
        playerIcon = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(.5f, .5f));
        return playerIcon;
    }
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case (byte)EventCodes.PlayerViewId:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    Player temp = PhotonView.Find((int)data[0]).gameObject.GetComponent<Player>();
                    temp.name = (string)data[1];
                    Sprite playerIcon = BuildSpriteFromByteArray((byte[])data[2]);
                    players.Add(temp);
                    UIManager.instance.UpdatePregamePlayers(temp,playerIcon);
                    Debug.Log("Players found: " + players.Count);
                }
                break;
            case (byte)EventCodes.PlayerRaise:
                {
                    object[] data = (object[])photonEvent.CustomData;

                    int betToAdd = (int)data[0];
                    CurrentPlayer.Raise(betToAdd);
                    Dealer.AddBet(CurrentPlayer.AmountToBet);
                    Debug.Log(CurrentPlayer.name + " raised by " + betToAdd);
                }
                break;

            case (byte)EventCodes.PlayerCall:
                {
                    object[] data = (object[])photonEvent.CustomData;

                    //AddBet(betToAdd);
                    CurrentPlayer.Call();
                    Dealer.AddBet(CurrentPlayer.AmountToBet);
                    Debug.Log(CurrentPlayer.name + " has called");
                }
                break;

            case (byte)EventCodes.PlayerCheck:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    CurrentPlayer.Check();
                    Debug.Log(CurrentPlayer.name + " has checked");

                }
                break;

            case (byte)EventCodes.PlayerFold:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    CurrentPlayer.Fold();
                    Debug.Log(CurrentPlayer.name + " has folded");
                }
                break;

            case (byte)EventCodes.PlayerDisconnected:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    Player temp = PhotonView.Find((int)data[0]).gameObject.GetComponent<Player>();
                    players.Remove(temp);
                    Debug.Log($"Player {temp.name} has disconnected, playercount {players.Count}");
                    Destroy(temp.playerSeat.gameObject);
                    Destroy(temp.gameObject);                    
                }
                break;

        }
    }

    #region Disconnect Players
    void DisconnectPlayers() // disconnect event
    {
        object[] datas = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.Others,
            CachingOption = EventCaching.DoNotCache
        };
        SendOptions sendOptions = new SendOptions() { Reliability = false };

        PhotonNetwork.RaiseEvent((byte)EventCodes.ServerDisconnected, datas, raiseEventOptions, sendOptions);
        Debug.Log("PhotonManager: disconnect func");
    }

    public void Disconnect() // disconnect button on lobby menu
    {
        StartCoroutine("DisconnectCor");
    }

    IEnumerator DisconnectCor()
    {
        DisconnectPlayers();
        yield return new WaitForSeconds(2);
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel(0);
        Debug.Log("Disconnected");
    }

    public void Quit() // quit button on victory menu
    {
        StartCoroutine("QuitCor");
    }

    IEnumerator QuitCor()
    {
        DisconnectPlayers();
        yield return new WaitForSeconds(2);
        Application.Quit();
    }

    void OnApplicationPause()
    {
        DisconnectPlayers();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }

    #endregion


    #region Unused Methods
    /*IEnumerator DiscardRound()
    {
        foreach (Player player in players)
        {
            currentPlayer = player;
            if (OnDealingCards != null)
                OnDealingCards();
            while (!currentPlayer.hasDiscardedCards)
            {
                yield return null;
            }

        }
        yield return null;
        StartCoroutine(BettingRound());
    }
    public void Discard()
    {
        currentPlayer.Discard();
    }
     */
    #endregion
}
