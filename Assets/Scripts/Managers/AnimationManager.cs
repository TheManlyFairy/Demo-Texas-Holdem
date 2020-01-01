using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    public float iconMoveTime = 0.5f;
    private void Start()
    {
        if(instance!=null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void MovePlayerIcons()
    {
        StartCoroutine(MoveIcons());
    }

    IEnumerator MoveIcons()
    {
        float timer=0, normalizedTime;
        List<Vector2> iconStartPositions = new List<Vector2>();
        List<Vector2> iconEndPositions = new List<Vector2>();
        List<RectTransform> iconRectTransforms = new List<RectTransform>();

        for(int i=0; i<UIManager.instance.pregamePlayerDisplay.Count; i++)
        {
            if(UIManager.instance.pregamePlayerDisplay[i].gameObject.activeSelf)
            {
                iconRectTransforms.Add(UIManager.instance.pregamePlayerDisplay[i].GetComponent<RectTransform>());
                iconRectTransforms[i].SetParent(UIManager.instance.playerSeats[i].GetComponent<RectTransform>());
                iconRectTransforms[i].anchorMin = iconRectTransforms[i].parent.GetComponent<RectTransform>().anchorMin;
                iconRectTransforms[i].anchorMax = iconRectTransforms[i].parent.GetComponent<RectTransform>().anchorMax;

                iconStartPositions.Add(UIManager.instance.pregamePlayerDisplay[i].GetComponent<RectTransform>().anchoredPosition);
                iconEndPositions.Add(UIManager.instance.playerSeats[i].IconAnchoredPosition);

                UIManager.instance.pregamePlayerDisplay[i].HideDisplayName();
            }
        }

        yield return null;

        while (timer<iconMoveTime)
        {
            timer += Time.deltaTime;
            normalizedTime = timer / iconMoveTime;

            for (int i = 0; i < iconRectTransforms.Count; i++)
            {
                iconRectTransforms[i].anchoredPosition = Vector2.Lerp(iconStartPositions[i], iconEndPositions[i], normalizedTime);
            }
            yield return null;
        }

        for (int i = 0; i < iconRectTransforms.Count; i++)
        {
            UIManager.instance.playerSeats[i].ShowPlayerGameDisplay();
            UIManager.instance.pregamePlayerDisplay[i].gameObject.SetActive(false);
        }
    }
}
