using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIAnimator : MonoBehaviour
{
    [SerializeField]
    RectTransform indicatorRectTrans;
    [SerializeField]
    Button forYou, categories, editorChoice, topChart;
    [SerializeField]
    float slideTime = 0.4f;
    float transformX;

    static IEnumerator slide;
    private void Start()
    {
        forYou.onClick.AddListener(delegate { ShiftIndicator(forYou.GetComponent<RectTransform>().localPosition.x); });
        categories.onClick.AddListener(delegate { ShiftIndicator(categories.GetComponent<RectTransform>().localPosition.x); });
        editorChoice.onClick.AddListener(delegate { ShiftIndicator(editorChoice.GetComponent<RectTransform>().localPosition.x); });
        topChart.onClick.AddListener(delegate { ShiftIndicator(topChart.GetComponent<RectTransform>().localPosition.x); });
    }
    public void ShiftIndicator(float transformX)
    {
        if (slide != null)
            StopCoroutine(slide);
        slide = SlideIndicator(transformX);
        StartCoroutine(slide);
    }

    
    IEnumerator SlideIndicator(float transformX)
    {
        float timer = 0;
        float startX = indicatorRectTrans.localPosition.x;
        float newPosX;
        while (timer<=slideTime)
        {
            newPosX = Mathf.SmoothStep(startX, transformX, timer / slideTime);
            indicatorRectTrans.localPosition = new Vector2(newPosX, indicatorRectTrans.localPosition.y);
            yield return null;
            timer += Time.deltaTime;
        }
    }
}
