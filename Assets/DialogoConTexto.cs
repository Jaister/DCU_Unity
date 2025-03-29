using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogoConBotones : MonoBehaviour
{
    [Header("Diálogos iniciales")]
    public GameObject[] dialogos;

    [Header("Botones")]
    public GameObject botonSI;
    public GameObject botonNO;

    [Header("Pantallas")]
    public GameObject pantallaActual;    // La pantalla donde están los diálogos
    public GameObject pantallaDestino;   // La pantalla a la que vamos al pulsar "Sí"

    private int indice = 0;
    private bool terminado = false;

    void Start()
    {
        foreach (GameObject d in dialogos)
            d.SetActive(false);

        if (botonSI != null) botonSI.SetActive(false);
        if (botonNO != null) botonNO.SetActive(false);

        if (dialogos.Length > 0)
        {
            dialogos[0].SetActive(true);
            indice = 1;
        }
    }

    void Update()
    {
        if (!terminado && Input.GetMouseButtonDown(0))
        {
            MostrarSiguiente();
        }
    }

    void MostrarSiguiente()
    {
        if (indice < dialogos.Length)
        {
            dialogos[indice - 1].SetActive(false);
            dialogos[indice].SetActive(true);
            indice++;

            if (indice == dialogos.Length)
            {
                if (botonSI != null) botonSI.SetActive(true);
                if (botonNO != null) botonNO.SetActive(true);
                terminado = true;
            }
        }
    }

    public void OnBotonSi()
    {
        // Ocultar botones y pantalla actual
        if (botonSI != null) botonSI.SetActive(false);
        if (botonNO != null) botonNO.SetActive(false);

        if (pantallaActual != null) pantallaActual.SetActive(false);
        if (pantallaDestino != null) pantallaDestino.SetActive(true);
    }
}
