using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CardDisplay : MonoBehaviour
{
    [SerializeField]
    Card card;
    

    public void InitializeCard(Card card)
    {
        this.card = card;
        GetComponent<Image>().sprite = card.sprite;
    }
    public void SelectCard()
    {
        card.markedForDiscard = !card.markedForDiscard;

        if(card.markedForDiscard)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y+10);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 10);
        }
    }
}
