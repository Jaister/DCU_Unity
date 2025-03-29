using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SelectorDePersonaje : MonoBehaviour
{
    [Header("Diálogo inicial")]
    public GameObject globoTextoInicial;
    public GameObject textoDialogoInicial;
    public GameObject marciano;

    [Header("Mensaje de selección")]
    public GameObject mensajeCentral;

    [Header("Textos debajo de los personajes")]
    public GameObject textoBajoGato;
    public GameObject textoBajoZorro;
    public GameObject textoBajoPerezoso;

    [Header("Texto de cuenta atrás")]
    public TextMeshProUGUI textoCuentaAtras;

    [Header("Personaje elegido")]
    public string personajeSeleccionado = "";

    private bool haMostradoMensajeCentral = false;
    private bool haElegido = false;

    void Start()
    {
        mensajeCentral?.SetActive(false);
        textoBajoGato?.SetActive(false);
        textoBajoZorro?.SetActive(false);
        textoBajoPerezoso?.SetActive(false);

        globoTextoInicial?.SetActive(true);
        textoDialogoInicial?.SetActive(true);

        if (textoCuentaAtras != null)
            textoCuentaAtras.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!haMostradoMensajeCentral && Input.GetMouseButtonDown(0))
        {
            MostrarMensajeCentral();
        }
    }

    void MostrarMensajeCentral()
    {
        globoTextoInicial?.SetActive(false);
        textoDialogoInicial?.SetActive(false);
        marciano?.SetActive(false);
        mensajeCentral?.SetActive(true);
        haMostradoMensajeCentral = true;
    }

    public void ElegirGato()
    {
        if (haMostradoMensajeCentral && !haElegido)
        {
            personajeSeleccionado = "Gato";
            textoBajoGato?.SetActive(true);
            FinalizarSeleccion();
        }
    }

    public void ElegirZorro()
    {
        if (haMostradoMensajeCentral && !haElegido)
        {
            personajeSeleccionado = "Zorro";
            textoBajoZorro?.SetActive(true);
            FinalizarSeleccion();
        }
    }

    public void ElegirPerezoso()
    {
        if (haMostradoMensajeCentral && !haElegido)
        {
            personajeSeleccionado = "Perezoso";
            textoBajoPerezoso?.SetActive(true);
            FinalizarSeleccion();
        }
    }

    void FinalizarSeleccion()
    {
        haElegido = true;
        mensajeCentral?.SetActive(false);
        Debug.Log("Personaje elegido: " + personajeSeleccionado);
        StartCoroutine(CuentaAtras());
    }

    IEnumerator CuentaAtras()
    {
        if (textoCuentaAtras == null)
            yield break;

        textoCuentaAtras.gameObject.SetActive(true);

        string[] cuenta = { "3", "2", "1", "¡YA!" };

        foreach (string paso in cuenta)
        {
            textoCuentaAtras.text = paso;
            yield return new WaitForSeconds(1f);
        }

        textoCuentaAtras.gameObject.SetActive(false);

        // Aquí podrías iniciar la carrera, mostrar otro panel, etc.
        Debug.Log("Comienza la carrera");
    }
}
