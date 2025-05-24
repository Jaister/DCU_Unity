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
    [SerializeField] public TMP_Text progress;
    [SerializeField] private SelectorDePersonaje selectorDePersonaje;
    [SerializeField] private GameObject juegoGO;
    [SerializeField] private GameObject NivelSelector;
    [SerializeField] private DialogoConBotones dialogoConBotones;
    [SerializeField] private ImpulsoPersonajeJugador impulsoPersonajeJugador;
    [SerializeField] private SoundBank soundBank;

    void OnEnable()
{
        GenerarNuevaCuenta(); //Muy importante llamar esto aquí también
}



    void Awake()
    {
        // Ocultar todo al principio
        if (contenedorCuentas != null)
            contenedorCuentas.SetActive(false);
    }


    void GenerarNuevaCuenta()
    {
        //Reiniciamos el flag de fallo
        falloEnEstaCuenta = false;

        if (cuentasResueltas >= totalCuentas)
        {
            if (aciertosSinFallo == 10) { // Si ha acertado todo, se le da un bonus
                persistencyManager.AddStars(10);
            }
            else
            {
                persistencyManager.AddStars(1);

            }
            persistencyManager.UpdateStarsText();

            FinDelJuego();
            return;
        }

        // Restaurar estado inicial de botones
        foreach (Button btn in botonesRespuesta)
        {
            btn.interactable = true;
            btn.GetComponent<Image>().color = colorDefault;
        }
       if (persistencyManager.dificultadActual == 1)
        {
            // Solo sumas
            ultimoA = Random.Range(1, 10);
            ultimoB = Random.Range(1, 10);
            respuestaCorrecta = ultimoA + ultimoB;
            textoCuenta.text = $"{ultimoA} + {ultimoB} = ?";
        }
        else if (persistencyManager.dificultadActual == 2)
        {
            // Solo restas
            ultimoA = Random.Range(2, 20);
            ultimoB = Random.Range(1, ultimoA); // Asegurar que B < A
            respuestaCorrecta = ultimoA - ultimoB;
            textoCuenta.text = $"{ultimoA} - {ultimoB} = ?";
        }
        else if (persistencyManager.dificultadActual == 3)
        {
            // Solo multiplicaciones
            ultimoA = Random.Range(1, 10);
            ultimoB = Random.Range(1, 10);
            respuestaCorrecta = ultimoA * ultimoB;
            textoCuenta.text = $"{ultimoA} x {ultimoB} = ?";
        }

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
            soundBank.PlaySound("CORRECT");
            boton.GetComponent<Image>().color = colorCorrecto;
            textoCuenta.text = "Correcto!";
            FindObjectOfType<ImpulsoPersonajeJugador>()?.DarImpulso();

            if (!falloEnEstaCuenta)
            {
                aciertosSinFallo++; // Contamos solo si acierta a la primera
            }

            cuentasResueltas++;
            persistencyManager.UpdateStarsText();
            progress.text = $"{cuentasResueltas}/{totalCuentas}";

            Invoke(nameof(GenerarNuevaCuenta), 1.2f);
        }

        else
        {
            soundBank.PlaySound("WRONG");

            falloEnEstaCuenta = true; //Marcamos que ha fallado en esta cuenta
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
        dialogoConBotones.ResetDialogo();
        bool superadoSinErrores = aciertosSinFallo == totalCuentas;
        persistencyManager.SetAcertoTodo(superadoSinErrores);
        persistencyManager.SetDesbloqueoPendiente(true);


        NivelSelector.SetActive(true);
        juegoGO.SetActive(false);

        //Reiniciamos el diálogo interactivo
        DialogoInteractivo dialogo = FindObjectOfType<DialogoInteractivo>();
        if (dialogo != null)
        {
            dialogo.enabled = false;
            dialogo.enabled = true;
        }
            ReiniciarJuego();
    }

   public void ReiniciarJuego()
    {
        cuentasResueltas = 0;
        aciertosSinFallo = 0;
        falloEnEstaCuenta = false;

        impulsoPersonajeJugador.personajeJugador = "";
        impulsoPersonajeJugador.ResetPosition();
        selectorDePersonaje.haElegido = false;
        selectorDePersonaje.haMostradoMensajeCentral = false;
        // Restaurar progreso visual
        if (progress != null)
            progress.text = "0/" + totalCuentas;

        // Mostrar contenedor si estaba oculto
        if (contenedorCuentas != null)
            contenedorCuentas.SetActive(true);

        // Reactivar los botones por si acaso
        foreach (Button btn in botonesRespuesta)
        {
            btn.gameObject.SetActive(false); 
            btn.interactable = true;
            btn.GetComponent<Image>().color = colorDefault;
        }

        Debug.Log("Juego reiniciado");
    }



    

}