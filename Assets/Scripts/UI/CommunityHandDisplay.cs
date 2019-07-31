using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunityHandDisplay : MonoBehaviour
{
    int communityIndex;
    [SerializeField]
    CardDisplay[] communityCardsDisplay;

    public int CommunityIndex { get { return communityIndex; } }

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
            if (Dealer.dealerRef.communityCards[i] != null)
            {
                communityCardsDisplay[i].InitializeCard(Dealer.dealerRef.communityCards[i]);
                communityCardsDisplay[i].gameObject.SetActive(true);
            }
            else
                communityCardsDisplay[i].gameObject.SetActive(false);
        }
    }
   
}
