using UnityEngine;
using TMPro;

public class PersistencyManager : MonoBehaviour
{
    public int stars;
    public string playerName;

    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_InputField starsInput;

    void Start()
    {
        //ResetData();
        LoadData();
    }

    public void SaveData()
    {
        // Get values from TMP Input Fields
        //int.TryParse(starsInput.text, out int score);
        string name = playerNameInput.text;
        int score = 0;

        if (score >= 0)
        {
            stars = score;
            PlayerPrefs.SetInt("Stars", score);
        }

        if (!string.IsNullOrEmpty(name))
        {
            playerName = name;
            PlayerPrefs.SetString("PlayerName", name);
        }

        PlayerPrefs.Save();
        Debug.Log("Data Saved!");
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("Stars")){
        stars = PlayerPrefs.GetInt("Stars", 0); // Default value is 0
        //starsInput.text = stars.ToString();
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
        //starsInput.text = "";
        playerNameInput.text = "";
        Debug.Log("All data reset!");
    }
}
