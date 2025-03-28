using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogoInteractivo : MonoBehaviour
{
    public GameObject[] dialogos;
    public GameObject globoTexto; // 🗨️ El contenedor del globo de diálogo
    private int indice = 0;

    void Start()
    {
        // Ocultar todos los diálogos
        foreach (GameObject d in dialogos)
        {
            d.SetActive(false);
        }

        // Mostrar el primero
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
            dialogos[indice - 1].SetActive(false); // Oculta el anterior
            dialogos[indice].SetActive(true);      // Muestra el nuevo
            indice++;
        }
        else
        {
            // Oculta el último diálogo visible si queda
            if (indice - 1 < dialogos.Length)
                dialogos[indice - 1].SetActive(false);

            // Oculta el globo completo
            if (globoTexto != null)
                globoTexto.SetActive(false);

            Debug.Log("Fin de la explicación.");
        }
    }
}
