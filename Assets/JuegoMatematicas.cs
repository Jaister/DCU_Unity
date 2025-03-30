using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JuegoMatematicas : MonoBehaviour
{
    [Header("Contenedor principal")]
    public GameObject contenedorCuentas; // Padre del texto y botones

    [Header("Texto de la cuenta")]
    public TextMeshProUGUI textoCuenta;

    [Header("Botones de respuesta")]
    public Button[] botonesRespuesta;

    [Header("Colores de feedback")]
    public Color colorCorrecto = Color.green;
    public Color colorIncorrecto = Color.red;
    public Color colorDefault = Color.white;

    [Header("Número de cuentas totales")]
    public int totalCuentas = 10;

    public int cuentasResueltas = 0;
    private int respuestaCorrecta;
    private int ultimoA, ultimoB;
    private bool falloEnEstaCuenta = false;
    private int aciertosSinFallo = 0;



    [SerializeField] private PersistencyManager persistencyManager;
    [SerializeField] private TMP_Text progress;
    [SerializeField] private SelectorDePersonaje selectorDePersonaje;
    [SerializeField] private GameObject juegoGO;
    [SerializeField] private GameObject NivelSelector;

    void Awake()
    {
        // Ocultar todo al principio
        if (contenedorCuentas != null)
            contenedorCuentas.SetActive(false);
    }

    void Start()
    {
        // Mostrar el contenido cuando empieza realmente
        if (contenedorCuentas != null)
            contenedorCuentas.SetActive(true);
        GenerarNuevaCuenta();
    }

    void GenerarNuevaCuenta()
    {
        // ✅ Reiniciamos el flag de fallo
        falloEnEstaCuenta = false;

        if (cuentasResueltas >= totalCuentas)
        {
            FinDelJuego();
            return;
        }

        // Restaurar estado inicial de botones
        foreach (Button btn in botonesRespuesta)
        {
            btn.interactable = true;
            btn.GetComponent<Image>().color = colorDefault;
        }

        // Generar operación de suma
        ultimoA = Random.Range(1, 10);
        ultimoB = Random.Range(1, 10);
        respuestaCorrecta = ultimoA + ultimoB;
        textoCuenta.text = $"{ultimoA} + {ultimoB} = ?";

        // Generar respuestas (1 correcta + 2 incorrectas únicas)
        List<int> opciones = new List<int> { respuestaCorrecta };
        while (opciones.Count < 3)
        {
            int distractor = respuestaCorrecta + Random.Range(-3, 4);
            if (distractor != respuestaCorrecta && distractor >= 0 && !opciones.Contains(distractor))
            {
                opciones.Add(distractor);
            }
        }

        // Mezclar y asignar a los botones
        opciones = Barajar(opciones);

        for (int i = 0; i < botonesRespuesta.Length; i++)
        {
            int valor = opciones[i];
            Button boton = botonesRespuesta[i];

            boton.GetComponentInChildren<TextMeshProUGUI>().text = valor.ToString();
            boton.onClick.RemoveAllListeners();
            boton.onClick.AddListener(() => ComprobarRespuesta(valor, boton));
        }
    }

   void ComprobarRespuesta(int seleccion, Button boton)
    {
        foreach (Button btn in botonesRespuesta)
            btn.interactable = false;

        if (seleccion == respuestaCorrecta)
        {
            boton.GetComponent<Image>().color = colorCorrecto;
            textoCuenta.text = "Correcto!";
            FindObjectOfType<ImpulsoPersonajeJugador>()?.DarImpulso();

            if (!falloEnEstaCuenta)
            {
                aciertosSinFallo++; // ✅ Contamos solo si acierta a la primera
                persistencyManager.AddStars();
            }

            cuentasResueltas++;
            persistencyManager.UpdateStarsText();
            progress.text = $"{cuentasResueltas}/{totalCuentas}";

            Invoke(nameof(GenerarNuevaCuenta), 1.2f);
        }

        else
        {
            falloEnEstaCuenta = true; // ❌ Marcamos que ha fallado en esta cuenta
            boton.GetComponent<Image>().color = colorIncorrecto;
            textoCuenta.text = "Incorrecto, inténtalo de nuevo";
            Invoke(nameof(RepetirMismaCuenta), 1.5f);
        }
    }


    void RepetirMismaCuenta()
    {
        textoCuenta.text = $"{ultimoA} + {ultimoB} = ?";

        foreach (Button btn in botonesRespuesta)
        {
            btn.interactable = true;
            btn.GetComponent<Image>().color = colorDefault;
        }
    }

    List<int> Barajar(List<int> lista)
    {
        for (int i = 0; i < lista.Count; i++)
        {
            int randomIndex = Random.Range(0, lista.Count);
            int temp = lista[i];
            lista[i] = lista[randomIndex];
            lista[randomIndex] = temp;
        }
        return lista;
    }

   void FinDelJuego()
    {
        Debug.Log("Llamado FinDelJuego");

        textoCuenta.text = "¡Fin de la carrera!";
        selectorDePersonaje.DisableAnimations();
        foreach (Button btn in botonesRespuesta)
            btn.gameObject.SetActive(false);
        NivelSelector.SetActive(true);
        juegoGO.SetActive(false);

        persistencyManager.selectorDialogue = true;

     bool superadoSinErrores = aciertosSinFallo == totalCuentas;
        persistencyManager.SetAcertoTodo(superadoSinErrores);
        persistencyManager.SetDesbloqueoPendiente(true);


    NivelSelector.SetActive(true);
    juegoGO.SetActive(false);

    // 🧠 Reiniciamos el diálogo interactivo
    DialogoInteractivo dialogo = FindObjectOfType<DialogoInteractivo>();
    if (dialogo != null)
    {
        dialogo.enabled = false;
        dialogo.enabled = true;
    }
    }

    

}