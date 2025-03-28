using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotonNO : MonoBehaviour
{
    public GameObject textoHastaPronto;
    public GameObject textoActual;
    public GameObject botonSI;
    public GameObject botonNO;
    public GameObject pantalla3;
    public GameObject startMenu;

    public float tiempoEspera = 2f;

    public void MostrarMensajeYSalir()
    {
        if (textoHastaPronto != null)
            textoHastaPronto.SetActive(true);

        if (textoActual != null)
            textoActual.SetActive(false);

        if (botonSI != null)
            botonSI.SetActive(false);

        if (botonNO != null)
            botonNO.SetActive(false);

        Invoke("CambiarPantalla", tiempoEspera);
    }

    void CambiarPantalla()
    {
        if (pantalla3 != null)
            pantalla3.SetActive(false);

        if (startMenu != null)
            startMenu.SetActive(true);
    }
}
