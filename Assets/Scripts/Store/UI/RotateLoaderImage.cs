using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateLoaderImage : MonoBehaviour
{
    RectTransform rectTransform;
    [SerializeField]
    float rotateSpeed;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    void Update()
    {
        rectTransform.Rotate(new Vector3(0, 0, rotateSpeed), Space.Self);
    }
}
