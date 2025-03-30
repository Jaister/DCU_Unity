using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DisableOnAnimationEnd : MonoBehaviour
{
    [SerializeField] private GameObject Nextgame;
    public void DisableGameObject()
    {
        Nextgame.SetActive(true);
        gameObject.SetActive(false);
    }
}
