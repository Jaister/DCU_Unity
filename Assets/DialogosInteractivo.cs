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

    [SerializeField] private PersistencyManager persistencyManager;

    void Start()
    {
        if (globoTexto != null && !persistencyManager.selectorDialogue)
            globoTexto.SetActive(true);

        foreach (GameObject d in dialogos)
        {
            d.SetActive(false);
        }

        if (dialogos.Length > 0 && !persistencyManager.selectorDialogue)
        {
            dialogos[0].SetActive(true);
            indice = 1;
        }
    }

   void Update()
    {
        if (!persistencyManager.selectorDialogue)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (mostrandoFinal)
                    MostrarSiguienteFinal();
                else
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
            // Aquí podrías activar botones, permitir repetir nivel, etc.
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

/*public void MostrarDialogoFinal()
{
    mostrandoFinal = true;
    indiceFinal = 0;
    // Oculta los diálogos iniciales
    foreach (GameObject d in dialogos)
        d.SetActive(false);

    globoTexto?.SetActive(true);

    if (dialogosFinales.Length > 0)
    {
        dialogosFinales[0].SetActive(true);
        indiceFinal = 1;
    }
}*/


}
