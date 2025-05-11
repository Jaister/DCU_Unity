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

    [Header("Configuración de carrera")]
    public float desplazamiento; // Desplazamiento en píxeles calculado al centro de la pantalla
    public float duracionDeslizamiento = 0.3f; // Duración del impulso suave
    [Range(0.5f, 0.95f)] public float factorRetrasoNPC = 0.8f; // Qué tan rápido avanzan los NPCs en relación al jugador (0.8 = 80% de la velocidad)
    [Range(0f, 0.2f)] public float variacionVelocidad = 0.1f; // Variación aleatoria en la velocidad de NPCs para que no sea predecible

    private Vector2 startPosGato, startPosZorro, startPosPerezoso;
    private float posXGato, posXZorro, posXPerezoso; // Posiciones horizontales actuales (progreso en la carrera)

    [Header("Personaje jugador")]
    public string personajeJugador = "";

    // Referencias para controlar las animaciones
    private Coroutine coroutineGato, coroutineZorro, coroutinePerezoso;

    private void Start()
    {
        // Guardar posiciones iniciales
        startPosGato = botonGato.anchoredPosition;
        startPosZorro = botonZorro.anchoredPosition;
        startPosPerezoso = botonPerezoso.anchoredPosition;

        // Inicializar posiciones de carrera
        posXGato = startPosGato.x;
        posXZorro = startPosZorro.x;
        posXPerezoso = startPosPerezoso.x;
    }

    private void OnEnable()
    {
        desplazamiento = 530f / juego.totalCuentas;
    }

    public void DarImpulso()
    {
        // Detener animaciones en curso
        DetenerAnimaciones();

        // Obtener el personaje del jugador y la posición de avance
        float avanceJugador = desplazamiento;

        // El personaje del jugador avanza completamente
        if (personajeJugador == "Gato")
        {
            posXGato += avanceJugador;
            coroutineGato = StartCoroutine(DeslizarPersonaje(botonGato, new Vector2(posXGato, startPosGato.y)));

            // Los NPCs avanzan un poco menos
            MoverNPC("Zorro", botonZorro, ref posXZorro, ref coroutineZorro);
            MoverNPC("Perezoso", botonPerezoso, ref posXPerezoso, ref coroutinePerezoso);
        }
        else if (personajeJugador == "Zorro")
        {
            posXZorro += avanceJugador;
            coroutineZorro = StartCoroutine(DeslizarPersonaje(botonZorro, new Vector2(posXZorro, startPosZorro.y)));

            // Los NPCs avanzan un poco menos
            MoverNPC("Gato", botonGato, ref posXGato, ref coroutineGato);
            MoverNPC("Perezoso", botonPerezoso, ref posXPerezoso, ref coroutinePerezoso);
        }
        else if (personajeJugador == "Perezoso")
        {
            posXPerezoso += avanceJugador;
            coroutinePerezoso = StartCoroutine(DeslizarPersonaje(botonPerezoso, new Vector2(posXPerezoso, startPosPerezoso.y)));

            // Los NPCs avanzan un poco menos
            MoverNPC("Gato", botonGato, ref posXGato, ref coroutineGato);
            MoverNPC("Zorro", botonZorro, ref posXZorro, ref coroutineZorro);
        }
    }

    private void MoverNPC(string nombrePersonaje, RectTransform personajeRT, ref float posXActual, ref Coroutine coroutine)
    {
        // Si este personaje NO es el jugador, entonces es un NPC
        if (nombrePersonaje != personajeJugador)
        {
            // Obtener la posición actual del jugador
            float posXJugador = GetPosXPersonaje(personajeJugador);

            // Calcular avance del NPC (un porcentaje del avance del jugador con variación aleatoria)
            float factorAvance = factorRetrasoNPC + Random.Range(-variacionVelocidad, variacionVelocidad);
            float avanceNPC = desplazamiento * factorAvance;

            // La nueva posición potencial del NPC
            float nuevaPosX = posXActual + avanceNPC;

            // Verificar que la nueva posición no adelante al jugador
            if (nuevaPosX >= posXJugador)
            {
                // Si lo adelanta, quedarse un poco atrás del jugador
                nuevaPosX = posXJugador - Random.Range(10f, 30f);
            }

            // Actualizar posición y animar
            posXActual = nuevaPosX;
            Vector2 posicionInicial = personajeRT.anchoredPosition;
            Vector2 posicionFinal = new Vector2(posXActual, posicionInicial.y);
            coroutine = StartCoroutine(DeslizarPersonaje(personajeRT, posicionFinal));
        }
    }

    private float GetPosXPersonaje(string nombre)
    {
        // Obtener la posición X actual del personaje indicado
        switch (nombre)
        {
            case "Gato": return posXGato;
            case "Zorro": return posXZorro;
            case "Perezoso": return posXPerezoso;
            default: return 0f;
        }
    }

    private void DetenerAnimaciones()
    {
        // Detener todas las animaciones en curso
        if (coroutineGato != null) { StopCoroutine(coroutineGato); coroutineGato = null; }
        if (coroutineZorro != null) { StopCoroutine(coroutineZorro); coroutineZorro = null; }
        if (coroutinePerezoso != null) { StopCoroutine(coroutinePerezoso); coroutinePerezoso = null; }
    }

    private IEnumerator DeslizarPersonaje(RectTransform personaje, Vector2 destino)
    {
        Vector2 inicio = personaje.anchoredPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duracionDeslizamiento;
            // Usar una curva de suavizado para el movimiento
            float curvaT = Mathf.SmoothStep(0, 1, Mathf.Clamp01(t));
            personaje.anchoredPosition = Vector2.Lerp(inicio, destino, curvaT);
            yield return null;
        }

        personaje.anchoredPosition = destino;
    }

    public void ResetPosition()
    {
        // Detener animaciones en curso
        DetenerAnimaciones();

        // Restaurar posiciones iniciales
        botonGato.anchoredPosition = startPosGato;
        botonZorro.anchoredPosition = startPosZorro;
        botonPerezoso.anchoredPosition = startPosPerezoso;

        // Reiniciar las posiciones de carrera
        posXGato = startPosGato.x;
        posXZorro = startPosZorro.x;
        posXPerezoso = startPosPerezoso.x;
    }
}