using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Game3 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI OperationText;
    [SerializeField] private GameObject[] Options;
    [SerializeField] private PersistencyManager persistencyManager;
    [SerializeField] private GameObject progressText;
    [SerializeField] private GameObject soundBank;
    [SerializeField] private GameObject selectorNivel;
    [SerializeField] private NivelSelector selectorScript; // manda cojones

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
        string OperationString = "";
        //Habra que cambiar la cuenta ya que el numero que queremos no es el resultado si no uno de los operandos
        int result = 0;
        if (persistencyManager.dificultadActual == 2)
        {
            // Generar operación resta O multiplicación
            int operacion = Random.Range(0, 2); // 0 para resta, 1 para multiplicación
            if (operacion == 0)
            {
                // Generar operación de resta
                int num1 = Random.Range(2, 20);
                int num2 = Random.Range(1, num1); // Asegurarse de que B es menor que A
                result = num1 - num2;
                correctAnswer = num2; // Correct answer is the second operand
                OperationString = $"{num1} - ? = {result}";
            }
            else
            {
                // Generar operación de multiplicación
                int num1 = Random.Range(1, 10);
                int num2 = Random.Range(1, 10);
                result = num1 * num2;
                correctAnswer = num2; // Correct answer is the second operand
                OperationString = $"{num1} x ? = {result}";
            }
        }
        else
        {
            // Generar operación de suma
            int num1 = Random.Range(1, 10);
            int num2 = Random.Range(1, 10);
            result = num1 + num2;
            correctAnswer = num2; // Correct answer is the second operand
            OperationString = $"{num1} + ? =  {result}";
        }
        if (changeTextCoroutine != null)
        {
            StopCoroutine(changeTextCoroutine);
            changeTextCoroutine = null;
        }
        trueOriginalText = OperationString; // Update true original text here

        OperationText.text = OperationString; // Update UI
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
            if (progress+1 == GOAL)
            {
                progressText.GetComponent<TextMeshProUGUI>().text = "¡Has ganado!";
                Win();
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
            soundBank.GetComponent<SoundBank>().PlaySound("CORRECT");
            foreach (var option in Options) 
            {
                option.GetComponentInChildren<Button>().interactable = true;
            }

        }
        else
        {
            // Respuesta incorrecta
            Debug.Log("Incorrecto!");
            Options[selectedOption].GetComponentInChildren<Button>().interactable = false;
            StartChangeOperationText("Incorrecto", OperationText);
            soundBank.GetComponent<SoundBank>().PlaySound("WRONG");
        }
    }
    void OnDisable()
    {
        // Limpiar el texto de la operación al desactivar el objeto
        if (OperationText != null)
        {
            OperationText.text = "OPERACION";
        }
        progress = 0;
    }
    void Win()
    {
        // CAMBIAR PARA NS QUE HAGA LO QUE QUEREMOS AL GANAR
        //SI HAY QUE PONER ALGUNA RETROALIMENTACION PONER CORRUTINA PARA ESTO:
        selectorNivel.SetActive(true);
        persistencyManager.UnlockDifficulty2();
        selectorScript.UnlockDifficulty2();
        transform.parent.gameObject.SetActive(false);

    }

    //GESTION DE CAMBIO DE TEXTO AL FALLAR ROBADO DE TILESXD
    //---------------------------------------------------//
    private Coroutine changeTextCoroutine;
    private string trueOriginalText; // Always holds the real original text

    private IEnumerator ChangeOperationTextCoroutine(string newText, TMP_Text texto)
    {
        texto.text = newText;
        yield return new WaitForSeconds(1f);

        texto.text = trueOriginalText; // Always restore the true original
        changeTextCoroutine = null;
    }

    public void StartChangeOperationText(string newText, TMP_Text texto)
    {
        if (changeTextCoroutine != null)
        {
            StopCoroutine(changeTextCoroutine);
            texto.text = trueOriginalText; // If interrupted, immediately restore
        }

        trueOriginalText = texto.text; // Save the real text only once, before changing
        changeTextCoroutine = StartCoroutine(ChangeOperationTextCoroutine(newText, texto));
    }
}
