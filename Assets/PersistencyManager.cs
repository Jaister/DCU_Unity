using UnityEngine;
using TMPro;

public class PersistencyManager : MonoBehaviour
{
    public int stars;
    public string playerName;
    public bool selectorDialogue;
    public bool acertoTodo = false;
    public int nivelActual = 1;
    public int dificultadActual = 1; // 1: Facil, 2: Normal, 3:YA VEREMOS....
    [SerializeField] private bool resetData = false; // Para resetear los datos al iniciar el juego
    [SerializeField] private bool fullGame;
    [SerializeField] private GameObject planetasLoading;


    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_Text starsOutput;

    public bool desbloqueoPendiente = false;

    public void SetDesbloqueoPendiente(bool valor)
    {
        desbloqueoPendiente = valor;
        PlayerPrefs.SetInt("DesbloqueoPendiente", valor ? 1 : 0);
        PlayerPrefs.Save();
    }

   void Start()
    {
        if (resetData) ResetData();
        LoadData(); // No resetear al arrancar
        if (fullGame)
        {
            // Desbloquear el nivel 2 al iniciar el juego
            dificultadActual = 2;
            nivelActual = 3;
        }
        starsOutput.text = $"Tienes {stars} estrellas {playerName}!!";
        if (dificultadActual == 1)
        {
            planetasLoading.transform.GetChild(0).gameObject.SetActive(true); //MERCURIO
        }
        else if (dificultadActual == 2)
        {
            planetasLoading.transform.GetChild(1).gameObject.SetActive(true); //VENUS
        }
    }


    public void SaveName()
    {
        // Get values from TMP Input Fields
        string name = playerNameInput.text;

        if (!string.IsNullOrEmpty(name))
        {
            playerName = name;
            PlayerPrefs.SetString("PlayerName", name);
        }

        PlayerPrefs.Save();
        UpdateStarsText(name);
    }

   public void SetAcertoTodo(bool valor)
    {
        acertoTodo = valor;
        PlayerPrefs.SetInt("AcertoTodo", valor ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("Stars"))
            stars = PlayerPrefs.GetInt("Stars");
            
        if (PlayerPrefs.HasKey("DesbloqueoPendiente"))
        desbloqueoPendiente = PlayerPrefs.GetInt("DesbloqueoPendiente") == 1;

        if (PlayerPrefs.HasKey("PlayerName"))
        {
            playerName = PlayerPrefs.GetString("PlayerName", "Guest");
            playerNameInput.text = playerName;
        }
        if (PlayerPrefs.HasKey("NivelActual")){
            nivelActual = PlayerPrefs.GetInt("NivelActual",1);
        }

        if (PlayerPrefs.HasKey("SelectorDialogue"))
            selectorDialogue = PlayerPrefs.GetInt("SelectorDialogue") == 1;

        if (PlayerPrefs.HasKey("AcertoTodo"))
            acertoTodo = PlayerPrefs.GetInt("AcertoTodo") == 1;
        if (PlayerPrefs.HasKey("DificultadActual"))
            dificultadActual = PlayerPrefs.GetInt("DificultadActual",1);
        Debug.Log($"Loaded Data -> Stars: {stars}, PlayerName: {playerName}, SelectorDialogue: {selectorDialogue}, AcertoTodo: {acertoTodo}");
    }


    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        playerNameInput.text = "";
        Debug.Log("All data reset!");
    }
    public void AddStars(int quantity )
    {
        stars += quantity;
        PlayerPrefs.SetInt("Stars", stars);
        PlayerPrefs.Save();
    }
    public void SelectorDialogueComplete()
    {
        selectorDialogue = true;
        PlayerPrefs.SetInt("SelectorDialogue", 1);
        PlayerPrefs.Save();
        Debug.Log("Selector dialogue complete!");

    }
    public void UnlockDifficulty2()
    {
        PlayerPrefs.SetInt("DificultadActual", 2);
        PlayerPrefs.Save();
    }
    public void SetNivelActual(int nivel)
    {
        nivelActual = nivel;
        PlayerPrefs.SetInt("NivelActual", nivel);
        PlayerPrefs.Save();
    }
    public void UpdateStarsText(string playerName2 = null )
    {
        if (playerName2 == null)
        {
            starsOutput.text = $"Tienes {stars} estrellas {playerName}!!";

        }
        else
        {
            starsOutput.text = $"Tienes {stars} estrellas {playerName2}!!";
        }
    }
}
