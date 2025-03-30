using UnityEngine;
using TMPro;

public class PersistencyManager : MonoBehaviour
{
    public int stars;
    public string playerName;
    public bool selectorDialogue;
    public bool acertoTodo = false;
    public int nivelActual = 1;


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
        //ResetData();
        LoadData(); // No resetear al arrancar
        starsOutput.text = $"Tienes {stars} estrellas {playerName}!!";
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

        Debug.Log($"Loaded Data -> Stars: {stars}, PlayerName: {playerName}, SelectorDialogue: {selectorDialogue}, AcertoTodo: {acertoTodo}");
    }


    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        playerNameInput.text = "";
        Debug.Log("All data reset!");
    }
    public void AddStars()
    {
        stars += 1;
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
