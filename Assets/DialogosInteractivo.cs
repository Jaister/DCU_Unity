using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogoInteractivo : MonoBehaviour
{
    public GameObject[] dialogos;
    public GameObject globoTexto;

    private int indice = 0;

    [Header("Referencia al script de niveles")]
    public NivelSelector nivelSelector; // ¡ASÍ se vuelve a conectar!

    [Header("Diálogos post-carrera")]
    public GameObject[] dialogosFinales;
    private int indiceFinal = 0;
    private bool mostrandoFinal = false;

    [Header("Diálogos si NO acierta todo")]
    public GameObject[] dialogosFallo;
    private int indiceFallo = 0;
    private bool mostrandoFallo = false;

    [Header("Diálogos si ha completado todos los niveles")]
    public GameObject[] dialogosTodosCompletados;
    private int indiceTodos = 0;
    private bool mostrandoTodos = false;

    [SerializeField] private AudioSource BG_MUSIC;


    [SerializeField] private PersistencyManager persistencyManager;

    void OnEnable()
    {
        //Reanudamos Musica
        if (BG_MUSIC != null && !BG_MUSIC.isPlaying)
            BG_MUSIC.Play();
        // Ocultar todos los diálogos por si acaso
        foreach (GameObject d in dialogos) d.SetActive(false);
        foreach (GameObject d in dialogosFinales) d.SetActive(false);
        foreach (GameObject d in dialogosFallo) d.SetActive(false);

        // Si ACABA de jugar el nivel 1 y hay que mostrar el resultado
        if (persistencyManager.desbloqueoPendiente && persistencyManager.dificultadActual == 1)
        {
            persistencyManager.SetDesbloqueoPendiente(false);

            if (persistencyManager.acertoTodo)
            {
                // Aquí comprobamos si se completó el último nivel
                if (persistencyManager.nivelActual == 3)
                {
                    MostrarDialogoTodosCompletados();
                    return;
                }

                mostrandoFinal = true;
                MostrarDialogoFinal();
            }
            else
            {
                mostrandoFallo = true;
                MostrarDialogoFallo();
            }

            return;
        }


        // Diálogo normal inicial (si nunca se ha mostrado antes)
        if (!persistencyManager.selectorDialogue && dialogos.Length > 0 && persistencyManager.dificultadActual == 1)
        {
            dialogos[0].SetActive(true);
            globoTexto.SetActive(true);
            indice = 1;
        }
        else
        {
            nivelSelector.DesbloquearNivel(persistencyManager.nivelActual);
            Debug.Log("Desbloqueando nivel " + persistencyManager.nivelActual);
        }
        if (persistencyManager.dificultadActual == 2)
        {
            nivelSelector.UnlockDifficulty2();
            persistencyManager.dificultadActual = 1; // Cambia dificultad a 1 porque se cambia abajo xd

            nivelSelector.ChangeDifficulty();

        }
    }


    void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (mostrandoTodos)
                {
                    MostrarSiguienteTodosCompletados();
                }
                else if (mostrandoFinal)
                {
                    MostrarSiguienteFinal();
                }
                else if (mostrandoFallo)
                {
                    MostrarSiguienteFallo();
                }
                else if (!persistencyManager.selectorDialogue)
                {
                    MostrarSiguiente();
                }
            }

        }

    void MostrarSiguienteFinal()
    {
        if (indiceFinal < dialogosFinales.Length)
        {
            dialogosFinales[indiceFinal - 1].SetActive(false);
            dialogosFinales[indiceFinal].SetActive(true);
            indiceFinal++;
        }
        else
        {
            dialogosFinales[indiceFinal - 1].SetActive(false);
            globoTexto?.SetActive(false);
            mostrandoFinal = false;

            Debug.Log("Fin del diálogo final.");

            // Desbloquear nivel 2
            if (nivelSelector != null)
            {
                nivelSelector.DesbloquearNivel(persistencyManager.nivelActual + 1);
                persistencyManager.SetNivelActual(persistencyManager.nivelActual + 1);
            }

            // Limpiar el flag para que no se repita
            persistencyManager.acertoTodo = false;
        }

    }

    void MostrarSiguiente()
{
    if (indice < dialogos.Length)
    {
        // Oculta el diálogo anterior si no es el primero
        if (indice - 1 >= 0)
            dialogos[indice - 1].SetActive(false);

        dialogos[indice].SetActive(true);
        indice++;
    }
    else
    {
        if (indice - 1 < dialogos.Length)
            dialogos[indice - 1].SetActive(false);

        if (globoTexto != null)
            globoTexto.SetActive(false);
            persistencyManager.SelectorDialogueComplete();
            Debug.Log("Fin del diálogo");

        if (nivelSelector != null)
            nivelSelector.DesbloquearNivel(1);
    }
}

    public void MostrarDialogoFinal()
    {
        mostrandoFinal = true;
        indiceFinal = 0;

        foreach (GameObject d in dialogos)
            d.SetActive(false);

        globoTexto?.SetActive(true);

        if (dialogosFinales.Length > 0)
        {
            dialogosFinales[0].SetActive(true);
            indiceFinal = 1;
        }
    }

    void MostrarDialogoFallo()
    {
    mostrandoFallo = true;
    indiceFallo = 0;

    foreach (GameObject d in dialogos)
        d.SetActive(false);

    globoTexto?.SetActive(true);

    if (dialogosFallo.Length > 0)
    {
        dialogosFallo[0].SetActive(true);
        indiceFallo = 1;
    }
    }

    void MostrarSiguienteFallo()
{
    if (indiceFallo < dialogosFallo.Length)
    {
        dialogosFallo[indiceFallo - 1].SetActive(false);
        dialogosFallo[indiceFallo].SetActive(true);
        indiceFallo++;
    }
    else
    {
        dialogosFallo[indiceFallo - 1].SetActive(false);
        globoTexto?.SetActive(false);
        mostrandoFallo = false;

        Debug.Log("Fin del diálogo de fallo.");
    }
}

    public void MostrarDialogoTodosCompletados()
    {
        mostrandoTodos = true;
        indiceTodos = 0;

        foreach (GameObject d in dialogos)
            d.SetActive(false);

        globoTexto?.SetActive(true);

        if (dialogosTodosCompletados.Length > 0)
        {
            dialogosTodosCompletados[0].SetActive(true);
            indiceTodos = 1;
        }
    }

    void MostrarSiguienteTodosCompletados()
{
    if (indiceTodos < dialogosTodosCompletados.Length)
    {
        dialogosTodosCompletados[indiceTodos - 1].SetActive(false);
        dialogosTodosCompletados[indiceTodos].SetActive(true);
        indiceTodos++;
    }
    else
    {
        dialogosTodosCompletados[indiceTodos - 1].SetActive(false);
        globoTexto?.SetActive(false);
        mostrandoTodos = false;

        Debug.Log("Fin del diálogo de todos los niveles completados.");
        
        // Aquí podrías mostrar la nave desbloqueada o activar algo especial
        nivelSelector.UnlockDifficulty2();
    }
}


}
