using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunityHandDisplay : MonoBehaviour
{
    [SerializeField]
    CardDisplay[] communityCardsDisplay;

    private void Start()
    {
        communityCardsDisplay = GetComponentsInChildren<CardDisplay>();
        Dealer.OnCommunityUpdate += UpdateCommunityDisplay;
        foreach (CardDisplay display in communityCardsDisplay)
            display.gameObject.SetActive(false);
    }

    public void UpdateCommunityDisplay()
    {
        for(int i=0; i<5; i++)
        {
            if (Dealer.CommunityCards[i] != null)
            {
                communityCardsDisplay[i].InitializeCard(Dealer.CommunityCards[i]);
                communityCardsDisplay[i].gameObject.SetActive(true);
            }
            else
                communityCardsDisplay[i].gameObject.SetActive(false);
        }
    }
   
}
