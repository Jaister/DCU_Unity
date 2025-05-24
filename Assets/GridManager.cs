using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System.Collections;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject Operation;
    [SerializeField] private PersistencyManager persistencyManager;
    [SerializeField] private GameObject selectorNivel;
    [SerializeField] private TMP_Text progress;
    [SerializeField] private float fadeSpeed = 2f; // Speed of tile fading
    [SerializeField] private SoundBank soundBank; // Reference to the sound bank
    [SerializeField] private NivelSelector selectorScript; // Reference to the character selector

    // Add dialogue system references
    [SerializeField] private DialogoConBotones dialogoConBotones;

    //Tile dict
    private Dictionary<Vector2, Tile> tileDict = new Dictionary<Vector2, Tile>();
    private Transform firstTile;
    private int correctResult;
    private const int MAX_TILES = 8;
    private int winCondition = 10;
    private int winCount = 0;
    private Tile currentTile; // Reference to the current tile player is on
    private Coroutine changeTextCoroutine;
    private string trueOriginalText; // Always holds the real original text
    private bool haFallado = false; // Flag to check if the player has failed

    void Start()
    {
    }

    private void OnEnable()
    {
        // Ensure clean state
        CleanupGrid();

        // Deactivate player initially
        PlayerPrefab.SetActive(false);
        GenerateGrid();

        // Position player after the grid is centered
        PositionPlayer();
        progress.text = $"{winCount}/{winCondition}";
        //Generate first operation
    }

    private void OnDisable()
    {
        // Reset the grid and player when the game is disabled
        CleanupGrid();
    }

    private void CleanupGrid()
    {
        // Cancel any ongoing coroutines
        if (changeTextCoroutine != null)
        {
            StopCoroutine(changeTextCoroutine);
            changeTextCoroutine = null;
        }

        // Cleanup player state
        PlayerPrefab.SetActive(false);
        currentTile = null;
        winCount = 0;
        haFallado = false; // Reset failure flag
        progress.text = $"{winCount}/{winCondition}";
        Operation.GetComponent<TMP_Text>().text = "OPERACION";

        // Reset transform scale explicitly (soluciona el problema de crecimiento)
        this.transform.localScale = new Vector3(1f,1f,1f);

        // Safely destroy all tiles
        List<Tile> tilesToDestroy = new List<Tile>(tileDict.Values);
        foreach (var tile in tilesToDestroy)
        {
            if (tile != null)
            {
                Destroy(tile.gameObject);
            }
        }

        // Clear dictionary after destroying tiles
        tileDict.Clear();
    }

    public void CheckResult(int result)
    {
        TMP_Text Text = Operation.GetComponent<TMP_Text>();
        if (winCount + 1 == winCondition && result == correctResult)
        {
            if (haFallado)
            {
                persistencyManager.AddStars(1);
            }
            else
            {
                persistencyManager.AddStars(10);
            }
            persistencyManager.UpdateStarsText();

            progress.text = "¡Has ganado!";

            Win();
            return;
        }
        if (result == correctResult)
        {
            // Find the tile with the correct result
            Tile correctTile = null;
            foreach (var tile in tileDict.Values)
            {
                if (tile != null && tile.Value == result)
                {
                    correctTile = tile;
                    break;
                }
            }

            if (correctTile != null)
            {
                // Play sound for correct answer
                soundBank.PlaySound("CORRECT");
                // Start the movement animation
                StartCoroutine(MovePlayerToTile(correctTile));

                // Fade out previous tile if it exists
                if (currentTile != null)
                {
                    StartCoroutine(FadeTile(currentTile));
                }

                // Update current tile reference

                currentTile = correctTile;
            }

            Text.text = GenerateOperation();
            NumberFiller(MAX_TILES - 1);
            persistencyManager.UpdateStarsText();
            winCount++;
            progress.text = $"{winCount}/{winCondition}";
        }
        else if (result == -1)
        {
            //GEStion resultado vacio
            Debug.Log("Casilla Vacia!");
            StartChangeOperationText("Casilla Vacia!", Text);
        }
        else
        {
            //GEStion resultado incorrecto
            Debug.Log("Incorrecto!");
            soundBank.PlaySound("WRONG");
            haFallado = true; // Set the flag to indicate failure
            StartChangeOperationText("Incorrecto!", Text);
        }
    }

    void Win()
    {
        // Updated Win function with dialogue triggering
        selectorNivel.SetActive(true);
        persistencyManager.SetAcertoTodo(!haFallado);  // Pass the correct value (not haFallado)
        persistencyManager.SetDesbloqueoPendiente(true);

        // Set the dialogue selector flag like in JuegoMatematicas
        persistencyManager.selectorDialogue = true;

        // Reset the dialogue system if reference exists
        if (dialogoConBotones != null)
        {
            dialogoConBotones.ResetDialogo();
        }

        if (!haFallado)
        {
            Debug.Log("SIN FALLAR");
            persistencyManager.acertoTodo = true;
            if (persistencyManager.dificultadMaxima > 1 && persistencyManager.dificultadActual>1)
            {
                // Only unlock next level if current level is less than 3
                selectorScript.DesbloquearNivel(persistencyManager.nivelActual + 1);
                persistencyManager.SetNivelActual(persistencyManager.nivelActual + 1);
            }
        }

        // Find and reset DialogoInteractivo just like in JuegoMatematicas
        DialogoInteractivo dialogo = FindObjectOfType<DialogoInteractivo>();
        if (dialogo != null)
        {
            dialogo.enabled = false;
            dialogo.enabled = true;
        }

        // Reset flag for next game
        haFallado = false;

        // Deactivate game object
        transform.parent.gameObject.SetActive(false);
    }

    //GESTION DE CAMBIO DE TEXTO AL FALLAR
    //---------------------------------------------------//
    private IEnumerator ChangeOperationTextCoroutine(string newText, TMP_Text texto)
    {
        texto.text = newText;
        yield return new WaitForSeconds(2f);

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

    //---------------------------------------------------//

    // Coroutine to animate player movement
    private IEnumerator MovePlayerToTile(Tile targetTile)
    {
        Vector3 startPosition = PlayerPrefab.transform.position;
        Vector3 targetPosition = new Vector3(
            targetTile.transform.position.x,
            targetTile.transform.position.y,
            PlayerPrefab.transform.position.z
        );

        // Use a time-based approach instead of distance-based
        float duration = 0.3f; // Fixed duration for the animation - 0.3 seconds
        float elapsedTime = 0;

        // Move the player smoothly to the target position
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Use smoothstep for a more natural movement
            float smoothT = t * t * (3f - 2f * t);

            PlayerPrefab.transform.position = Vector3.Lerp(startPosition, targetPosition, smoothT);
            yield return null;
        }

        // Ensure player reaches exact position
        PlayerPrefab.transform.position = targetPosition;
    }

    // Coroutine to fade out a tile
    private IEnumerator FadeTile(Tile tile)
    {
        if (tile == null) yield break;

        // Remove tile immediately from dictionary
        Vector2? keyToRemove = null;
        foreach (var entry in tileDict)
        {
            if (entry.Value == tile)
            {
                keyToRemove = entry.Key;
                break;
            }
        }

        if (keyToRemove.HasValue && tileDict.ContainsKey(keyToRemove.Value))
        {
            tileDict.Remove(keyToRemove.Value);
        }

        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        float alpha = originalColor.a;

        // Fade out
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Set final transparency
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        // Also fade the number text if it exists
        TMP_Text tileText = tile.GetComponentInChildren<TMP_Text>();
        if (tileText != null)
        {
            tileText.alpha = 0;
        }

        // Destroy after fading
        Destroy(tile.gameObject);
    }

    string GenerateOperation()
    {
        string OperationString = "";
        //Habra que cambiar la cuenta ya que el numero que queremos no es el resultado si no uno de los operandos
        int result = 0;
        if (persistencyManager.dificultadActual == 2)
        {
                // Generar operación de resta
                int num1 = Random.Range(2, 20);
                int num2 = Random.Range(1, num1); // Asegurarse de que B es menor que A
                result = num1 - num2;
                correctResult = result;
                OperationString = $"{num1} - {num2} = ?"; // 
        }
        else if (persistencyManager.dificultadActual == 3)
        {
                // Generar operación de multiplicación
                int num1 = Random.Range(1, 10);
                int num2 = Random.Range(1, 10);
                result = num1 * num2;
                correctResult = result;
                OperationString = $"{num1} x {num2} = ?";
        }
        else
        {
            // Generar operación de suma
            int num1 = Random.Range(1, 10);
            int num2 = Random.Range(1, 10);
            result = num1 + num2;
            correctResult = result; // Correct answer is the second operand
            OperationString = $"{num1} + {num2} =  ?";
        }
        if (changeTextCoroutine != null)
        {
            StopCoroutine(changeTextCoroutine);
            changeTextCoroutine = null;
        }
        TMP_Text text = Operation.GetComponent<TMP_Text>();

        // Cancel any ongoing text revert when we generate new operation
        if (changeTextCoroutine != null)
        {
            StopCoroutine(changeTextCoroutine);
            changeTextCoroutine = null;
        }
        trueOriginalText = OperationString; // Update true original text here

        text.text = OperationString; // Update UI
        return OperationString;
    }

    void GenerateGrid()
    {
        // Get tile size from SpriteRenderer
        Vector2 tileSize = _tilePrefab.GetComponent<SpriteRenderer>().bounds.size;

        // Adjust tile size if necessary (your hack was multiplying by 1.5f)
        if (Screen.width < 1200)
            tileSize *= 2f; // Adjust tile size for smaller screens
        else if (Screen.width >= 1200)
            tileSize *= 1.75f; // Adjust tile size for larger screens

        // Calculate total grid size
        float gridWidth = width * tileSize.x;
        float gridHeight = height * tileSize.y;

        // Set the grid's origin based on its center
        Vector3 gridOrigin = transform.position - new Vector3(gridWidth / 2, gridHeight / 2, 0) + new Vector3(tileSize.x / 2, tileSize.y / 2, 0);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate the position for each tile
                Vector3 tilePosition = new Vector3(
                    gridOrigin.x + x * tileSize.x,
                    gridOrigin.y + y * tileSize.y,
                    0
                );

                // Instantiate the tile
                var tile = Instantiate(_tilePrefab, tilePosition, Quaternion.identity, this.transform);
                tile.name = $"Tile {x} {y}";

                // Determine if the tile should have an offset pattern
                bool isOffset = (x + y) % 2 == 0;
                tile.Init(isOffset);

                // Store reference to the first tile
                if (x == 0 && y == 0)
                {
                    firstTile = tile.transform;
                }
                //Add to the dict - use grid coordinates as key
                tileDict[new Vector2(x, y)] = tile;
            }
        }
        currentTile = tileDict[new Vector2(0, 0)];

        TMP_Text Text = Operation.GetComponent<TMP_Text>();
        Text.text = GenerateOperation();
        NumberFiller(MAX_TILES);
        this.transform.localScale = new Vector3(.8f, .8f, .8f); // Reset scale to normal
    }

    void PositionPlayer()
    {
        if (firstTile != null)
        {
            // Make the player a child of the grid
            PlayerPrefab.transform.SetParent(transform);

            // Position the player at the first tile's position with a z offset
            PlayerPrefab.transform.position = new Vector3(
                firstTile.position.x,
                firstTile.position.y,
                firstTile.position.z - 1 // Moved further forward to ensure it's visible
            );

            // Set the initial tile as current tile
            currentTile = tileDict[new Vector2(0, 0)];

            // Now activate the player
            PlayerPrefab.SetActive(true);
        }
    }

    void NumberFiller(int maxTiles)
    {
        // Create a list of available tiles excluding the current tile 
        List<Tile> availableTiles = tileDict.Values
            .Where(tile => tile != null && tile != currentTile)
            .ToList();

        // Ensure there are enough tiles to fill
        if (availableTiles.Count < maxTiles) return;

        // Shuffle the available tiles to pick random ones
        availableTiles = availableTiles.OrderBy(t => Random.value).ToList();

        // Select the tiles to be filled
        List<Tile> selectedTiles = availableTiles.Take(maxTiles).ToList();

        // Pick one random tile to hold the correct result
        Tile correctTile = selectedTiles[Random.Range(0, selectedTiles.Count)];
        correctTile.changeValue(correctResult);

        Debug.Log($"Correct result {correctResult} placed on {correctTile.name}");

        // Fill the rest of the tiles with random values
        foreach (Tile tile in selectedTiles)
        {
            if (tile == correctTile) continue; // Skip the correct tile

            int number;
            do
            {
                if (persistencyManager.dificultadActual == 2)
                    number = Random.Range(1, 20); // For subtraction, range is 1-20
                else if (persistencyManager.dificultadActual == 3)
                    number = Random.Range(1, 81); // For multiplication, range is 1-81
                else
                    number = Random.Range(1, 20);
            }
            while (number == correctResult); // Avoid duplicate correct result

            tile.changeValue(number);
        }

        // Make sure all other tiles remain empty (-1)
        foreach (Tile tile in tileDict.Values.Except(selectedTiles))
        {
            if (tile != null)
            {
                tile.changeValue(-1);
            }
        }

        // Ensure current tile is empty
        if (currentTile != null)
        {
            currentTile.changeValue(-1);
        }
    }
}