using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
 
public class Tile : MonoBehaviour {
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    private GridManager gridManager;


    private TMP_Text _textMesh;

    [SerializeField] private int _value;
    public int Value
    {
        get { return _value; }
        set { _value = value; }
    }
    public void Init(bool isOffset) {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
        _textMesh = GetComponentInChildren<TMP_Text>();
        gridManager = GetComponentInParent<GridManager>();
    }
 
    void OnMouseEnter() {
        _highlight.SetActive(true);
    }
 
    void OnMouseExit()
    {
        _highlight.SetActive(false);
    }
    private void OnMouseDown()
    {
        gridManager.CheckResult(Value);
    }
    public void changeValue(int value)
    {
        Value = value;
    if (_textMesh != null)
    {
        _textMesh.text = value >= 0 ? value.ToString() : "";
    }        
    Debug.Log("Value changed to " + value);
    }
}