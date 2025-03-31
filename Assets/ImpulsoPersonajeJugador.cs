using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulsoPersonajeJugador : MonoBehaviour
{
    [Header("Botones de los personajes")]
    public RectTransform botonGato;
    public RectTransform botonZorro;
    public RectTransform botonPerezoso;
    [SerializeField] private JuegoMatematicas juego;
    [Header("Configuración de impulso")]
    public float desplazamiento; // Desplazamiento en píxeles calculado al centro de la pantalla
    public float duracionDeslizamiento = 0.3f; // Duración del impulso suave
    private Vector2 startPosGato, startPosZorro, startPosPerezoso;
    [Header("Personaje jugador")]
    public string personajeJugador = "";
    private void Start()
    {
        startPosGato = botonGato.anchoredPosition;
        startPosZorro = botonZorro.anchoredPosition;
        startPosPerezoso = botonPerezoso.anchoredPosition;
    }
    private void OnEnable()
    {
        desplazamiento = 530f / juego.totalCuentas;
    }
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
    public void ResetPosition()
    {
        botonGato.anchoredPosition = startPosGato;
        botonZorro.anchoredPosition = startPosZorro;
        botonPerezoso.anchoredPosition = startPosPerezoso;
    }
}
