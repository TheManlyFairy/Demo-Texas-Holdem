using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommunityHandDisplay : MonoBehaviour
{
    [SerializeField] float cardXSpacing;
    [SerializeField] float animationTimePerCard;
    [SerializeField] Image deckSprite;
    [SerializeField] Image[] communityCards;

    Vector2 deckPosition;
    int communityCardIndex = 0;

    static bool animationInProgress;
    public static bool AnimationInProgress { get { return animationInProgress; } }
    private void Start()
    {
        deckPosition = deckSprite.GetComponent<RectTransform>().anchoredPosition;
        InitializeDisplay();
        Dealer.OnCommunityUpdate += UpdateCommunityDisplay;
        PhotonGameManager.onGameStart += ShowDeckSprite;
    }
    public void ShowDeckSprite()
    {
        deckSprite.enabled = true;
    }
    void InitializeDisplay()
    {
        animationInProgress = false;
        communityCardIndex = 0;
        foreach (Image spRend in communityCards)
        {
            spRend.transform.position = deckPosition;
            if (spRend.enabled)
                spRend.enabled = false;
        }
    }
    public void UpdateCommunityDisplay(int cardsToPull)
    {
        Debug.LogWarning("Updating community display");
        if (cardsToPull < 0)
        {
            InitializeDisplay();
        }
        else
        {
            StartCoroutine(PullCommunityCard(cardsToPull));
        }
    }
    IEnumerator PullCommunityCard(int cardsToPull)
    {
        Debug.LogWarning("Animating community cards");
        float timer;
        float newXPosition;
        float cardScaleX;
        float cardScaleLerpX;
        Vector2 positionToLerpCardTo;
        Image cardToAnimate;
        RectTransform cardTransform;
        animationInProgress = true;

        for (int i = 0; i < cardsToPull; i++)
        {
            cardToAnimate = communityCards[communityCardIndex];
            cardTransform = cardToAnimate.GetComponent<RectTransform>();
            cardToAnimate.enabled = true;
            newXPosition = cardXSpacing * communityCardIndex;
            positionToLerpCardTo = new Vector2(newXPosition, 0);
            cardScaleX = cardTransform.localScale.x;
            timer = 0;

            while (timer < animationTimePerCard / 2)
            {
                timer += Time.deltaTime;
                cardTransform.anchoredPosition = Vector2.Lerp(deckPosition, positionToLerpCardTo, timer / animationTimePerCard);
                cardScaleLerpX = Mathf.Lerp(cardScaleX, -cardScaleX, timer / animationTimePerCard);
                cardTransform.localScale = new Vector2(cardScaleLerpX, cardToAnimate.transform.localScale.y);
                yield return null;
            }


            cardToAnimate.sprite = Dealer.CommunityCards[communityCardIndex].sprite;

            while (timer < animationTimePerCard)
            {
                timer += Time.deltaTime;
                cardTransform.anchoredPosition = Vector2.Lerp(deckPosition, positionToLerpCardTo, timer / animationTimePerCard);
                cardScaleLerpX = Mathf.Lerp(cardScaleX, -cardScaleX, timer / animationTimePerCard);
                cardTransform.localScale = new Vector2(cardScaleLerpX, cardToAnimate.transform.localScale.y);
                yield return null;
            }
            communityCardIndex++;
            yield return null;
        }
        animationInProgress = false;
    }
}
