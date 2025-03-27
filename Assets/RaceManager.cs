using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class RaceManager : MonoBehaviour
{
    [SerializeField] private GameObject optionPrefab; // Reference to the option prefab
    [SerializeField] private GameObject Operation; // Reference to the operation text
    [SerializeField] private PersistencyManager persistencyManager; // Reference to the persistency manager
    private List<GameObject> instantiatedOptions = new List<GameObject>(); // Store instantiated options
    private int correctResult;

    void Start()
    {
        GenerateRaceOptions();
    }

    void GenerateRaceOptions()
    {
        // Clear previous options if any
        foreach (GameObject option in instantiatedOptions)
        {
            Destroy(option);
        }
        instantiatedOptions.Clear();

        // Generate a random math operation
        int num1 = Random.Range(0, 10);
        int num2 = Random.Range(0, 10);
        correctResult = num1 + num2;

        // Update operation text
        TMP_Text operationText = Operation.GetComponent<TMP_Text>();
        operationText.text = $"{num1} + {num2}";

        // Determine the index for the correct result
        int correctIndex = Random.Range(0, 3);

        // Instantiate options
        for (int i = 0; i < 3; i++)
        {
            Vector3 position = new Vector3(1100, i * 300 + 200, 0);
            GameObject newOption = Instantiate(optionPrefab, position, Quaternion.identity, transform);

            // Set the value for the option
            int optionValue;
            if (i == correctIndex) // Randomly place correct result
            {
                optionValue = correctResult;
            }
            else
            {
                do
                {
                    optionValue = Random.Range(0, 20);
                } while (optionValue == correctResult);
            }

            // Set the value of the option
            newOption.GetComponent<Option>().value = optionValue;

            // Set the text of the option
            TMP_Text optionText = newOption.GetComponentInChildren<TMP_Text>();
            if (optionText != null)
            {
                optionText.text = optionValue.ToString();
            }

            instantiatedOptions.Add(newOption);
        }
    }

    public void CheckResult(int selectedValue)
    {
        TMP_Text operationText = Operation.GetComponent<TMP_Text>();
        if (selectedValue == correctResult)
        {
            // Correct answer, generate new operation
            persistencyManager.AddStars();
            persistencyManager.UpdateStarsText();
            GenerateRaceOptions();
        }
        else
        {
            // Incorrect answer
            operationText.text = "Try again!";
        }
    }
}