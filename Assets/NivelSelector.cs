using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NivelSelector : MonoBehaviour
{
    public Button[] botonesNiveles;
    public Color colorBloqueado = Color.gray;
    public Color colorDesbloqueado = Color.white;

    [Header("Pantallas")]
    public GameObject pantallaActual;       // Pantalla del selector de niveles
    public GameObject pantallaJuego;        // Pantalla del nivel 1 (juego)
    [SerializeField]private List<GameObject> pantallasNiveles = new List<GameObject>();
    [SerializeField]private PersistencyManager persistencyManager;
    [SerializeField] private Sprite buttonsDif1;
    [SerializeField] private Sprite buttonsDif2;
    [SerializeField] private Image[] buttonsimages;
    [SerializeField] private GameObject[] planets;
    [SerializeField] private Button ShipButton;
    [SerializeField] private GameObject DIF2TEXT;
    [SerializeField] private GameObject soundBank;
    [SerializeField] private DialogoInteractivo dialogoInteractivo;

    private bool puedeHacerClickNave = true;
    private bool postNaveDialogoMostrado = false;



    void Start()
    {
        
        for (int i = 0; i < botonesNiveles.Length; i++)
        {
            botonesNiveles[i].interactable = false;
            botonesNiveles[i].GetComponent<Image>().color = colorBloqueado;
        }

    }

    /// <summary>
    /// Desbloquea un nivel dado un entero del 1 al 3
    /// </summary>
    /// <param name="nivel"></param>
    public void DesbloquearNivel(int nivel)
    {

        for (int i = 0; i < nivel; i++)
        {
            if (i >= botonesNiveles.Length) break; // Evitar IndexOutOfRangeException
            botonesNiveles[i].interactable = true;
            botonesNiveles[i].GetComponent<Image>().color = Color.white;
        }
    }
    public void UnlockDifficulty2()
    {
        ShipButton.interactable = true;
        DIF2TEXT.SetActive(false);
    }
    public void ChangeButtonImages()
    {
        for (int i = 0; i < buttonsimages.Length; i++)
        {
            if (persistencyManager.dificultadActual == 1)
            {
                buttonsimages[i].sprite = buttonsDif2;
            }
            else if (persistencyManager.dificultadActual == 2)
            {
                buttonsimages[i].sprite = buttonsDif1;
            }
        }
    }
    public void ChangePlanetImages()
    {

        for (int i = 0; i < planets.Length; i++)
        {
            if (persistencyManager.dificultadActual == 1)
            {
                planets[0].SetActive(false);
                planets[1].SetActive(true);
            }
            else if (persistencyManager.dificultadActual == 2)
            {
                planets[0].SetActive(true);
                planets[1].SetActive(false);
            }
        }
    }
   public void ChangeDifficulty()
{
    ChangeButtonImages();
    ChangePlanetImages();

    persistencyManager.dificultadActual = persistencyManager.dificultadActual == 1 ? 2 : 1;

    // 💡 Solo mostrar el diálogo la primera vez
    if (!postNaveDialogoMostrado)
    {
        Debug.Log("Mostrando diálogo post-dificultad por primera vez.");
        dialogoInteractivo.MostrarDialogoPostNave();
        postNaveDialogoMostrado = true; // Ahora no se volverá a mostrar
        puedeHacerClickNave = false;
    }
}

public void HabilitarClickShipButton()
{
    puedeHacerClickNave = true;
}

    public void VolverDesdeCarrera()
    {
        Debug.Log("Volviendo a selector de niveles");

        if (pantallaJuego != null)
        {
            // Reiniciamos el script del juego
            JuegoMatematicas juego = pantallaJuego.GetComponent<JuegoMatematicas>();
            if (juego != null)
            {
                juego.enabled = false;
                juego.enabled = true; // Esto asegura que se llame a OnEnable()
            }

            pantallaJuego.SetActive(false);
            Debug.Log("Pantalla de juego ocultada");
        }

        if (pantallaActual != null)
        {
            pantallaActual.SetActive(true);
            Debug.Log("Pantalla de niveles activada");
        }
    }


}
