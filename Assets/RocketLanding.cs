using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DisableOnAnimationEnd : MonoBehaviour
{
    [SerializeField] private GameObject Nextgame;
    [SerializeField] private List<GameObject> Modes;
    public void DisableGameObject()
    {
        Nextgame.SetActive(true);
        gameObject.SetActive(false);
    }
    public void SelectGameMode(int mode)
    {
        Nextgame = Modes[mode];
    }
}
