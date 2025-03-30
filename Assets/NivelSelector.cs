using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NivelSelector : MonoBehaviour
{
    public Button[] botonesNiveles;
    public Color colorBloqueado = Color.gray;
    public Color colorDesbloqueado = Color.white;

    [Header("Pantallas")]
    public GameObject pantallaActual;       // Pantalla del selector de niveles
    public GameObject pantallaJuego;        // Pantalla del nivel 1 (juego)
    [SerializeField]private List<GameObject> pantallasNiveles = new List<GameObject>(); 

    void Start()
    {
        
        for (int i = 0; i < botonesNiveles.Length; i++)
        {
            botonesNiveles[i].interactable = false;
            botonesNiveles[i].GetComponent<Image>().color = colorBloqueado;
        }
    }
    /// <summary>
    /// Desbloquea un nivel dado un entero del 1 al 3
    /// </summary>
    /// <param name="nivel"></param>
    public void DesbloquearNivel(int nivel)
    {
        for (int i = 0; i < nivel; i++)
        {
            botonesNiveles[i].interactable = true;
            botonesNiveles[i].GetComponent<Image>().color = Color.white;
        }
    }

    
 public void VolverDesdeCarrera()
{
    Debug.Log("Volviendo a selector de niveles");

    if (pantallaJuego != null)
    {
        pantallaJuego.SetActive(false);
        Debug.Log("Pantalla de juego ocultada");
    }

    if (pantallaActual != null)
    {
        pantallaActual.SetActive(true);
        Debug.Log("Pantalla de niveles activada");
    }
}


}
