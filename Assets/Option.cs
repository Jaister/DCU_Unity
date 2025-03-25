using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    private Image image;
    private float HoverScale = 1.3f;
    [SerializeField]public int value;
    [SerializeField]private RaceManager raceManager;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponentInChildren<Image>();
        raceManager = GetComponentInParent<RaceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        raceManager.CheckResult(value);
    }
    private void OnMouseEnter()
    {
        image.transform.localScale = new Vector3(image.transform.localScale.x * HoverScale, image.transform.localScale.y * HoverScale, image.transform.localScale.z * HoverScale);
    }
    private void OnMouseExit()
    {
        image.transform.localScale = new Vector3(image.transform.localScale.x / HoverScale, image.transform.localScale.y / HoverScale, image.transform.localScale.z / HoverScale);
    }
}
