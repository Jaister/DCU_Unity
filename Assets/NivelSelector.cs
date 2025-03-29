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

    void Start()
    {
        // Todos bloqueados al principio
        for (int i = 0; i < botonesNiveles.Length; i++)
        {
            botonesNiveles[i].interactable = false;
            botonesNiveles[i].GetComponent<Image>().color = colorBloqueado;
        }
    }

    public void DesbloquearNivel(int nivel)
    {
        if (nivel - 1 >= 0 && nivel - 1 < botonesNiveles.Length)
        {
            Button boton = botonesNiveles[nivel - 1];
            boton.interactable = true;
            boton.GetComponent<Image>().color = colorDesbloqueado;

            if (nivel == 1)
                boton.onClick.AddListener(() => MostrarPantallaJuego());
        }
    }

    public void MostrarPantallaJuego()
    {
        if (pantallaActual != null)
            pantallaActual.SetActive(false);

        if (pantallaJuego != null)
            pantallaJuego.SetActive(true);
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
