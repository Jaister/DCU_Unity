using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotonSI : MonoBehaviour
{
    public GameObject nuevoDialogo;   // El texto que quieres mostrar
    public GameObject viejoDialogo;   // El texto original que quieres ocultar
    public GameObject botonSI;        // Botón verde
    public GameObject botonNO;        // Botón rojo

    public void MostrarNuevoDialogo()
    {
        if (nuevoDialogo != null)
            nuevoDialogo.SetActive(true);

        if (viejoDialogo != null)
            viejoDialogo.SetActive(false);

        if (botonSI != null)
            botonSI.SetActive(false);

        if (botonNO != null)
            botonNO.SetActive(false);
    }
}
