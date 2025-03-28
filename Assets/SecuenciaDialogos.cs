using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecuenciaDialogos : MonoBehaviour
{
    public GameObject[] dialogos; // Arrastra aquí los diálogos en orden
    public GameObject pantallaActual;
    public GameObject pantallaSiguiente;
    private int indice = 0;

    void Start()
    {
        // Ocultar todos los diálogos
        foreach (GameObject dialogo in dialogos)
        {
            dialogo.SetActive(false);
        }

        // Mostrar el primer diálogo sin clic
        if (dialogos.Length > 0)
        {
            dialogos[0].SetActive(true);
            indice = 1; // El siguiente será el segundo
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Click izquierdo o toque
        {
            MostrarSiguienteDialogo();
        }
    }

    void MostrarSiguienteDialogo()
    {
        if (indice < dialogos.Length)
        {
            // Ocultar todos los anteriores
            foreach (GameObject d in dialogos)
            {
                d.SetActive(false);
            }

            // Mostrar el siguiente diálogo
            dialogos[indice].SetActive(true);
            indice++;
        }
        else
        {
            // Final de los diálogos: cambio de pantalla
            CambiarPantalla();
        }
    }

    void CambiarPantalla()
    {
        if (pantallaActual != null)
            pantallaActual.SetActive(false);

        if (pantallaSiguiente != null)
            pantallaSiguiente.SetActive(true);
    }
}
