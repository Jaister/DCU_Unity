using UnityEngine;
using TMPro;

public class PersistencyManager : MonoBehaviour
{
    public int stars;
    public string playerName;

    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_Text starsOutput;

    void Start()
    {
        //ResetData();
        LoadData();
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

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("Stars")){
        stars = PlayerPrefs.GetInt("Stars"); // Default value is 0
        }
        if (PlayerPrefs.HasKey("PlayerName")){
        playerName = PlayerPrefs.GetString("PlayerName", "Guest"); // Default value is "Guest"
        playerNameInput.text = playerName;
        }
        // Update TMP Input Fields with loaded data

        Debug.Log($"Loaded Data -> Stars: {stars}, PlayerName: {playerName}");
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
