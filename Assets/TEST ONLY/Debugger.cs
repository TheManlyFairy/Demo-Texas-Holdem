
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class Debugger : MonoBehaviour
{

    [SerializeField] int cardsToSpawn;
    [SerializeField] float cardXSpacing;
    [SerializeField] float animationTimePerCard;
    [SerializeField] Transform deckTransform;
    [SerializeField] SpriteRenderer[] communityCards;

    Vector2 deckPosition;
    int communityCardIndex = 0;

    private void Start()
    {
        StartCardsAtDeckPosition();
        StartCoroutine(PullCommunityCard());
    }

    void StartCardsAtDeckPosition()
    {
        deckPosition = deckTransform.position;
        foreach (SpriteRenderer spRend in communityCards)
        {
            spRend.transform.position = deckPosition;
        }
    }

    IEnumerator PullCommunityCard()
    {
        float timer;
        float newXPosition;
        Vector2 positionToLerpCardTo;
        SpriteRenderer cardToAnimate;
        float cardScaleX;
        float cardScaleLerpX;

        for (int i = 0; i < cardsToSpawn; i++)
        {
            cardToAnimate = communityCards[communityCardIndex];
            cardToAnimate.gameObject.SetActive(true);
            newXPosition = transform.position.x + cardXSpacing * communityCardIndex;
            positionToLerpCardTo = new Vector2(newXPosition, transform.position.y);
            cardScaleX = cardToAnimate.transform.localScale.x;
            timer = 0;

            while (timer < animationTimePerCard / 2)
            {
                timer += Time.deltaTime;
                cardToAnimate.transform.position = Vector2.Lerp(deckPosition, positionToLerpCardTo, timer / animationTimePerCard);
                cardScaleLerpX = Mathf.Lerp(cardScaleX, -cardScaleX, timer / animationTimePerCard);
                cardToAnimate.transform.localScale = new Vector2(cardScaleLerpX, cardToAnimate.transform.localScale.y);
                yield return null;
            }


            cardToAnimate.sprite = communityCards[communityCardIndex].sprite;

            while (timer < animationTimePerCard)
            {
                timer += Time.deltaTime;
                cardToAnimate.transform.position = Vector2.Lerp(deckPosition, positionToLerpCardTo, timer / animationTimePerCard);
                cardScaleLerpX = Mathf.Lerp(cardScaleX, -cardScaleX, timer / animationTimePerCard);
                cardToAnimate.transform.localScale = new Vector2(cardScaleLerpX, cardToAnimate.transform.localScale.y);
                yield return null;
            }
            communityCardIndex++;

        }
    }
}
