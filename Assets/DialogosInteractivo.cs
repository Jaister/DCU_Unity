using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private PersistencyManager persistencyManager;

        void OnEnable()
    {
        // 🔁 Ocultar todos los diálogos por si acaso
        foreach (GameObject d in dialogos) d.SetActive(false);
        foreach (GameObject d in dialogosFinales) d.SetActive(false);
        foreach (GameObject d in dialogosFallo) d.SetActive(false);

        // ✅ Si ACABA de jugar el nivel 1 y hay que mostrar el resultado
        if (persistencyManager.desbloqueoPendiente)
        {
            persistencyManager.SetDesbloqueoPendiente(false); // lo consumes

            if (persistencyManager.acertoTodo)
            {
                mostrandoFinal = true;
                MostrarDialogoFinal();
            }
            else
            {
                mostrandoFallo = true;
                MostrarDialogoFallo();
            }

            return; // 🔁 Muy importante: no continuar mostrando más cosas
        }

        // ✅ Diálogo normal inicial (si nunca se ha mostrado antes)
        if (!persistencyManager.selectorDialogue && dialogos.Length > 0)
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
    }




        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (mostrandoFinal)
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

            // ✅ Desbloquear nivel 2
            if (nivelSelector != null)
            {
                nivelSelector.DesbloquearNivel(2);
                persistencyManager.SetNivelActual(2);
            }

            // ✅ Limpiar el flag para que no se repita
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


}
