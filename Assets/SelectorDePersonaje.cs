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

    [Header("Elementos del juego de cuentas")]
    public TextMeshProUGUI textoCuenta;
    public Button boton1;
    public Button boton2;
    public Button boton3;

    [Header("Personaje elegido")]
    public string personajeSeleccionado = "";

    private JuegoMatematicas juegoMatematicas;
    private bool haMostradoMensajeCentral = false;
    private bool haElegido = false;

    [SerializeField] private PersistencyManager persistencyManager;

    void Start()
    {
        mensajeCentral?.SetActive(false);
        textoBajoGato?.SetActive(false);
        textoBajoZorro?.SetActive(false);
        textoBajoPerezoso?.SetActive(false);

        globoTextoInicial?.SetActive(true);
        textoDialogoInicial?.SetActive(true);
        marciano?.SetActive(true);

        if (textoCuentaAtras != null)
            textoCuentaAtras.gameObject.SetActive(false);

        // 🔒 Ocultar texto y botones del juego
        textoCuenta?.gameObject.SetActive(false);
        boton1?.gameObject.SetActive(false);
        boton2?.gameObject.SetActive(false);
        boton3?.gameObject.SetActive(false);

        juegoMatematicas = FindObjectOfType<JuegoMatematicas>();
        if (juegoMatematicas != null)
            juegoMatematicas.enabled = false;
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
            textoBajoGato.SetActive(true);
            textoBajoGato.GetComponent<TMP_Text>().text = persistencyManager.playerName;
            FinalizarSeleccion();
        }
    }

    public void ElegirZorro()
    {
        if (haMostradoMensajeCentral && !haElegido)
        {
            personajeSeleccionado = "Zorro";
            textoBajoZorro.SetActive(true);
            textoBajoZorro.GetComponent<TMP_Text>().text = persistencyManager.playerName;

            FinalizarSeleccion();
        }
    }

    public void ElegirPerezoso()
    {
        if (haMostradoMensajeCentral && !haElegido)
        {
            personajeSeleccionado = "Perezoso";
            textoBajoPerezoso.SetActive(true);
            textoBajoPerezoso.GetComponent<TMP_Text>().text = persistencyManager.playerName;

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
        Debug.Log("Comienza la carrera");

        // ✅ Mostrar los elementos del juego de cuentas
        textoCuenta?.gameObject.SetActive(true);
        boton1?.gameObject.SetActive(true);
        boton2?.gameObject.SetActive(true);
        boton3?.gameObject.SetActive(true);

        // ✅ Activar lógica de matemáticas
        if (juegoMatematicas != null)
            juegoMatematicas.enabled = true;

        FondoNubesScroll fondo = FindObjectOfType<FondoNubesScroll>();
        if (fondo != null)
            fondo.ActivarScroll();

        ImpulsoPersonajeJugador impulso = FindObjectOfType<ImpulsoPersonajeJugador>();
        if (impulso != null)
        {
            impulso.personajeJugador = personajeSeleccionado;
        }

    }
}
