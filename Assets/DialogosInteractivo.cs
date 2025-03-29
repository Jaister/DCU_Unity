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

    void Start()
    {
        if (globoTexto != null)
            globoTexto.SetActive(true);

        foreach (GameObject d in dialogos)
        {
            d.SetActive(false);
        }

        if (dialogos.Length > 0)
        {
            dialogos[0].SetActive(true);
            indice = 1;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MostrarSiguiente();
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

        Debug.Log("Fin del diálogo");

        if (nivelSelector != null)
            nivelSelector.DesbloquearNivel(1);
    }
}

}
