using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FondoNubesScroll : MonoBehaviour
{
    public float velocidad = 0.02f;
    private RawImage rawImage;
    private Vector2 offset = Vector2.zero;
    public bool mover = false;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        if (mover && rawImage != null)
        {
            offset.x += velocidad * Time.deltaTime;
            rawImage.uvRect = new Rect(offset, rawImage.uvRect.size);
        }
    }

    public void ActivarScroll()
    {
        mover = true;
    }

    public void DetenerScroll()
    {
        mover = false;
    }
}
