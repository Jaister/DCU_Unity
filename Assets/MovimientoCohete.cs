using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayectoriaCurva : MonoBehaviour
{
    public Transform destino;
    public float duracionVuelo = 2f;
    public float alturaCurva = 2f;

    public GameObject pantallaActual;
    public GameObject pantallaSiguiente;

    private Vector3 puntoInicio;
    private Vector3 puntoControl;
    private Vector3 puntoFinal;
    private float tiempoTranscurrido = 0f;
    private bool volando = false;

    void Start()
    {
        if (destino != null)
        {
            puntoInicio = transform.position;
            puntoFinal = destino.position;
            puntoControl = (puntoInicio + puntoFinal) / 2 + Vector3.up * alturaCurva;
            volando = true;
        }
    }

    void Update()
    {
        if (!volando) return;

        tiempoTranscurrido += Time.deltaTime;
        float t = tiempoTranscurrido / duracionVuelo;

        if (t > 1f)
        {
            t = 1f;
            volando = false;

            // Al terminar el vuelo, cambiamos de pantalla
            CambiarPantalla();
        }

        // Interpolación en curva (Bézier)
        Vector3 m1 = Vector3.Lerp(puntoInicio, puntoControl, t);
        Vector3 m2 = Vector3.Lerp(puntoControl, puntoFinal, t);
        transform.position = Vector3.Lerp(m1, m2, t);

        // Rotar para apuntar hacia el destino
        Vector3 direccion = m2 - transform.position;
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
    }

    void CambiarPantalla()
    {
        if (pantallaActual != null)
            pantallaActual.SetActive(false);

        if (pantallaSiguiente != null)
            pantallaSiguiente.SetActive(true);
    }
}
