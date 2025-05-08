using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Game3 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI OperationText;
    [SerializeField] private GameObject[] Options;
    [SerializeField] private PersistencyManager persistencyManager;
    [SerializeField] private GameObject progressText;
    private int GOAL = 10;

    private int correctAnswer, correctOptionIndex, progress;


    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnEnable()
    {
        progressText.GetComponent<TextMeshProUGUI>().text = $"{progress}/{GOAL}";
        GenerateOperation();
        GenerateOptions();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GenerateOperation()
    {
        int num1 = Random.Range(1, 10);
        int num2 = Random.Range(1, 10);
        int operation = 0; //CAMBIAR SI QUEREMOS DISTINTAS OPERACIONES
        //Habra que cambiar la cuenta ya que el numero que queremos no es el resultado si no uno de los operandos
        int result = 0;

        switch (operation)
        {
            case 0:
                result = num1 + num2;
                correctAnswer = num2;
                OperationText.text = $"{num1} + ? = {result}";
                break;
            case 1:
                result = num1 - num2;
                break;
            case 2:
                result = num1 * num2;
                break;
        }
    }
    void GenerateOptions()
    {
        // Generate a random index for the correct answer
        correctOptionIndex = Random.Range(0, Options.Length);
        //Fill buttons with random numbers
        for (int i = 0; i < Options.Length; i++)
        {
            if (i == correctOptionIndex)
            {
                Options[i].GetComponentInChildren<TextMeshProUGUI>().text = correctAnswer.ToString();
            }
            else
            {
                int randomNum = Random.Range(1, 10);
                while (randomNum == correctAnswer)
                {
                    randomNum = Random.Range(1, 10);
                }
                Options[i].GetComponentInChildren<TextMeshProUGUI>().text = randomNum.ToString();
            }
        }
    }
    public void CheckAnswer(int selectedOption)
    {

        // Comprobar si la respuesta es correcta
        if (selectedOption == correctOptionIndex)
        {
            if (progress + 1 == GOAL)
            {
                progressText.GetComponent<TextMeshProUGUI>().text = "¡Has ganado!";
                //GESTIONAR LA VICTORIA!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                return;
            }
            // Respuesta correcta
            Debug.Log("Correcto!");
            GenerateOperation();
            GenerateOptions();
            persistencyManager.AddStars();
            persistencyManager.UpdateStarsText();
            progress++;
            progressText.GetComponent<TextMeshProUGUI>().text = $"{progress}/{GOAL}";
        }
        else
        {
            // Respuesta incorrecta
            Debug.Log("Incorrecto!");
        }
    }
    private void OnDisable()
    {
        // Limpiar el texto de la operación al desactivar el objeto
        if (OperationText != null)
        {
            OperationText.text = "OPERACION";
        }
        progress = 0;
    }
}
