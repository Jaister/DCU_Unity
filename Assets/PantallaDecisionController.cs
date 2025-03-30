using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PantallaDecisionController : MonoBehaviour
{
    [Header("Elementos de la pantalla")]
    public GameObject textoDialogoInicial;
    public GameObject textoHastaPronto;
    public GameObject botonSI;
    public GameObject botonNO;

    public GameObject pantallaActual;
    public GameObject pantallaDestino; // Si pulsa SÍ
    public GameObject pantallaStart;   // Si pulsa NO

    public float tiempoEspera = 2f;

    private void OnEnable()
    {
        ReiniciarPantalla();
    }

    public void ReiniciarPantalla()
    {
        if (textoDialogoInicial != null) textoDialogoInicial.SetActive(true);
        if (textoHastaPronto != null) textoHastaPronto.SetActive(false);
        if (botonSI != null) botonSI.SetActive(true);
        if (botonNO != null) botonNO.SetActive(true);
    }

    public void AlPulsarBotonSI()
    {
        textoDialogoInicial?.SetActive(false);
        botonSI?.SetActive(false);
        botonNO?.SetActive(false);
        Invoke(nameof(IrAPantallaDestino), tiempoEspera);
    }

    public void AlPulsarBotonNO()
    {
        textoDialogoInicial?.SetActive(false);
        textoHastaPronto?.SetActive(true);
        botonSI?.SetActive(false);
        botonNO?.SetActive(false);
        Invoke(nameof(IrAPantallaStart), tiempoEspera);
    }

    void IrAPantallaDestino()
    {
        pantallaActual?.SetActive(false);
        pantallaDestino?.SetActive(true);
    }

    void IrAPantallaStart()
    {
        pantallaActual?.SetActive(false);
        pantallaStart?.SetActive(true);
    }
}
