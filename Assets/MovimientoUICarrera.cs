using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulsoPersonajeJugador : MonoBehaviour
{
    [Header("Botones de los personajes")]
    public RectTransform botonGato;
    public RectTransform botonZorro;
    public RectTransform botonPerezoso;

    [Header("Configuración de impulso")]
    public float desplazamiento = 10f;
    public float duracionDeslizamiento = 0.3f; // Duración del impulso suave

    [Header("Personaje jugador")]
    public string personajeJugador = "";

    public void DarImpulso()
    {
        RectTransform objetivo = null;

        if (personajeJugador == "Gato") objetivo = botonGato;
        else if (personajeJugador == "Zorro") objetivo = botonZorro;
        else if (personajeJugador == "Perezoso") objetivo = botonPerezoso;

        if (objetivo != null)
        {
            StartCoroutine(DeslizarPersonaje(objetivo));
        }
    }

    private IEnumerator DeslizarPersonaje(RectTransform objetivo)
    {
        Vector2 inicio = objetivo.anchoredPosition;
        Vector2 destino = inicio + new Vector2(desplazamiento, 0f);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duracionDeslizamiento;
            objetivo.anchoredPosition = Vector2.Lerp(inicio, destino, t);
            yield return null;
        }

        objetivo.anchoredPosition = destino;
    }
}
