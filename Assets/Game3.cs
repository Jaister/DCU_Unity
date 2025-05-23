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
    [SerializeField] private NivelSelector selectorScript;
    [SerializeField] private GameObject Personaje;

    // Nueva configuración para el movimiento curvo
    [Header("Configuración de Movimiento")]
    [SerializeField] private float movementDuration = 0.5f; // Duración del movimiento
    [SerializeField] private float curveHeight = 0.5f; // Alto de la curva
    [SerializeField] private AnimationCurve movementCurve; // Curva para suavizar el movimiento

    // Add dialogue system references
    [SerializeField] private DialogoConBotones dialogoConBotones;

    private int GOAL = 10;
    private int correctAnswer, correctOptionIndex, progress;
    private bool haFallado = false; // Track if player made mistakes
    private Coroutine characterMovementCoroutine; // Para controlar el movimiento del personaje
    private Vector3 characterInitialPosition; // Añadir esta variable para almacenar la posición inicial

    // Start is called before the first frame update
    void Start()
    {
        // Inicializar la curva de movimiento si está vacía
        if (movementCurve.keys.Length == 0)
        {
            movementCurve = new AnimationCurve(
                new Keyframe(0, 0, 0, 2),
                new Keyframe(0.5f, 1, 0, 0),
                new Keyframe(1, 0, -2, 0)
            );
        }
        
        // Guardar la posición inicial del personaje
        if (Personaje != null)
        {
            characterInitialPosition = Personaje.transform.position;
        }
    }

    private void OnEnable()
    {
        // Reset mistake tracking when game starts
        haFallado = false;
        progress = 0;
        progressText.GetComponent<TextMeshProUGUI>().text = $"{progress}/{GOAL}";
        GenerateOperation();
        GenerateOptions();
        
        // Asegurarse de que el personaje esté en su posición inicial
        if (Personaje != null)
        {
            characterInitialPosition = Personaje.transform.position;
            
            // Configurar imagen por defecto del personaje (primera imagen)
            if (Personaje.transform.childCount >= 2)
            {
                // Desactivar todos los hijos primero
                for (int i = 0; i < Personaje.transform.childCount; i++)
                {
                    Personaje.transform.GetChild(i).gameObject.SetActive(false);
                }
                
                // Activar la primera imagen por defecto
                Personaje.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
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
        // Cancelar cualquier movimiento en curso
        if (characterMovementCoroutine != null)
        {
            StopCoroutine(characterMovementCoroutine);
        }

        // Mover el personaje hacia el botón seleccionado con una trayectoria curva
        characterMovementCoroutine = StartCoroutine(MoveCharacterToButton(selectedOption));
    }

    private IEnumerator MoveCharacterToButton(int selectedOption)
    {
        // Deshabilitar temporalmente los botones durante la animación
        foreach (var option in Options)
        {
            option.GetComponentInChildren<Button>().interactable = false;
        }

        // Cambiar la imagen del personaje según el botón seleccionado
        if (Personaje != null && Personaje.transform.childCount >= 2)
        {
            // Desactivar todos los hijos primero
            for (int i = 0; i < Personaje.transform.childCount; i++)
            {
                Personaje.transform.GetChild(i).gameObject.SetActive(false);
            }
            
            // Activar el hijo correspondiente según el botón seleccionado
            // Botones 1 y 2 (índices 0 y 1) -> mostrar el segundo hijo (índice 1)
            // Botones 3 y 4 (índices 2 y 3) -> mostrar el primer hijo (índice 0)
            if (selectedOption < 2) // Botones 1 o 2
            {
                Personaje.transform.GetChild(1).gameObject.SetActive(true);
            }
            else // Botones 3 o 4
            {
                Personaje.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        // Posición inicial del personaje
        Vector3 startPosition = Personaje.transform.position;
        
        // Posición final (botón seleccionado)
        Vector3 targetPosition = Options[selectedOption].transform.position;
        
        // Punto de control para la curva (punto más alto)
        Vector3 controlPoint = (startPosition + targetPosition) / 2;
        controlPoint.y += curveHeight; // Ajustar altura de la curva

        float elapsedTime = 0;
        
        while (elapsedTime < movementDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / movementDuration;
            float curveValue = movementCurve.Evaluate(normalizedTime);
            
            // Interpolación cuadrática para crear una parábola
            Vector3 m1 = Vector3.Lerp(startPosition, controlPoint, normalizedTime);
            Vector3 m2 = Vector3.Lerp(controlPoint, targetPosition, normalizedTime);
            Personaje.transform.position = Vector3.Lerp(m1, m2, normalizedTime);
            
            yield return null;
        }
        
        // Asegurar que el personaje llegue exactamente a la posición final
        Personaje.transform.position = targetPosition;
        
        // Procesar el resultado después de que el personaje llegue al botón
        ProcessResult(selectedOption);
    }

    private void ProcessResult(int selectedOption)
    {
        // Comprobar si la respuesta es correcta
        if (selectedOption == correctOptionIndex)
        {
            if (progress + 1 == GOAL)
            {
                Win();
                return;
            }
            // Respuesta correcta
            Debug.Log("Correcto!");
            GenerateOperation();
            GenerateOptions();
            progress++;
            progressText.GetComponent<TextMeshProUGUI>().text = $"{progress}/{GOAL}";
            soundBank.GetComponent<SoundBank>().PlaySound("CORRECT");
            foreach (var option in Options)
            {
                option.GetComponentInChildren<Button>().interactable = true;
            }

            // Devolver el personaje a su posición original después de un breve retraso
            StartCoroutine(ReturnCharacterAfterDelay(0.5f));
        }
        else
        {
            // Respuesta incorrecta
            Debug.Log("Incorrecto!");
            haFallado = true; // Mark that player made a mistake
            Options[selectedOption].GetComponentInChildren<Button>().interactable = false;
            StartChangeOperationText("Incorrecto", OperationText);
            soundBank.GetComponent<SoundBank>().PlaySound("WRONG");
            
            // Devolver personaje a posición original
            StartCoroutine(ReturnCharacterAfterDelay(1.0f));
            
            // Reactivar botones excepto el incorrecto
            for (int i = 0; i < Options.Length; i++)
            {
                if (i != selectedOption)
                {
                    Options[i].GetComponentInChildren<Button>().interactable = true;
                }
            }
        }
    }
    
    private IEnumerator ReturnCharacterAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Guardar la posición actual
        Vector3 currentPos = Personaje.transform.position;
        
        // Usar la posición inicial guardada para retornar
        Vector3 originalPosition = characterInitialPosition;
        
        // Movimiento de regreso con una curva más suave
        float returnTime = 0;
        float returnDuration = 0.3f;
        
        // Punto de control para la curva de regreso (punto más alto)
        Vector3 controlPoint = (currentPos + originalPosition) / 2;
        controlPoint.y += curveHeight * 0.7f; // Un poco menos de altura en el regreso
        
        while (returnTime < returnDuration)
        {
            returnTime += Time.deltaTime;
            float t = returnTime / returnDuration;
            
            // Uso de easing para un movimiento más suave
            t = Mathf.SmoothStep(0, 1, t);
            
            // Interpolación cuadrática para crear una parábola en el regreso
            Vector3 m1 = Vector3.Lerp(currentPos, controlPoint, t);
            Vector3 m2 = Vector3.Lerp(controlPoint, originalPosition, t);
            Personaje.transform.position = Vector3.Lerp(m1, m2, t);
            
            yield return null;
        }
        
        // Asegurar que el personaje llegue exactamente a la posición inicial
        Personaje.transform.position = originalPosition;
        
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
        if (haFallado) {
            persistencyManager.AddStars(1);

        }
        else
        {
            persistencyManager.AddStars(10);
        }
        persistencyManager.UpdateStarsText();

        // Updated Win function with dialogue triggering
        selectorNivel.SetActive(true);

        // Set acertoTodo based on whether player made mistakes
        persistencyManager.SetAcertoTodo(!haFallado);
        persistencyManager.SetDesbloqueoPendiente(true);

        // Set the dialogue selector flag like in JuegoMatematicas
        persistencyManager.selectorDialogue = true;

        // Reset the dialogue system if reference exists
        if (dialogoConBotones != null)
        {
            dialogoConBotones.ResetDialogo();
        }

        // Find and reset DialogoInteractivo just like in JuegoMatematicas
        DialogoInteractivo dialogo = FindObjectOfType<DialogoInteractivo>();
        if (dialogo != null)
        {
            dialogo.enabled = false;
            dialogo.enabled = true;
        }
        if (haFallado)
        {
            Debug.Log("Has fallado en la cuenta, mostrando diálogo de error.");
        }
        else
        {
            persistencyManager.UnlockDifficulty2();
            selectorScript.UnlockDifficulty2();
        }
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