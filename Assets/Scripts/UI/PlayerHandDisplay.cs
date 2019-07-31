using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
public class PlayerHandDisplay : MonoBehaviour
{
    //public CardDisplay prefab;
    [SerializeField]
    CardDisplay[] displayCards;

    private void Start()
    {
        //CreateButtons();
        displayCards = GetComponentsInChildren<CardDisplay>();
    }

   /* void CreateButtons()
    {
        displayCards = new CardDisplay[2];
        for (int i = 0; i < 2; i++)
        {
            displayCards[i] = Instantiate(prefab);
            displayCards[i].transform.SetParent(transform);
        }
        prefab.gameObject.SetActive(false);
    }*/
    public void SetupPlayerHand(Player player)
    {
        List<Card> playerCards = player.cards;
        for (int i = 0; i < 2; i++)
        {
            displayCards[i].InitializeCard(playerCards[i]);
        }
    }
}
