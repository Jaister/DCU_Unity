using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System.Collections;
using System.Threading;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject Operation;
    [SerializeField] private PersistencyManager persistencyManager; // Reference to the persistency manager
    [SerializeField] private TMP_Text progress;
    [SerializeField] private float fadeSpeed = 2f; // Speed of tile fading

    //Tile dict
    private Dictionary<Vector2, Tile> tileDict = new Dictionary<Vector2, Tile>();
    private Transform firstTile;
    private int correctResult;
    private const int MAX_TILES = 8;
    private int winCondition = 10;
    private int winCount = 0;
    private Tile currentTile; // Reference to the current tile player is on

    void Start()
    {
        // Deactivate player initially
        PlayerPrefab.SetActive(false);
        GenerateGrid();

        // Position player after the grid is centered
        PositionPlayer();
        progress.text = $"{winCount}/{winCondition}";
        //Generate first operation
    }

    public void CheckResult(int result)
    {
        TMP_Text Text = Operation.GetComponent<TMP_Text>();
        
        if (result == correctResult)
        {
            // Find the tile with the correct result
            Tile correctTile = null;
            foreach (var tile in tileDict.Values)
            {
                if (tile.Value == result)
                {
                    correctTile = tile;
                    break;
                }
            }

            if (correctTile != null)
            {
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
            persistencyManager.AddStars();
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
            StartChangeOperationText("Incorrecto!", Text);
        }
    }
    //GESTION DE CAMBIO DE TEXTO AL FALLAR
    //---------------------------------------------------//
    private Coroutine changeTextCoroutine;
    private string trueOriginalText; // Always holds the real original text

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
        // Remove tile immediately from dictionary
        Vector2 tilePosition = new Vector2(tile.transform.position.x, tile.transform.position.y);
        if (tileDict.ContainsKey(tilePosition))
        {
            tileDict.Remove(tilePosition);
        }

        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
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
        int num1 = Random.Range(0, 10);
        int num2 = Random.Range(0, 10);
        correctResult = num1 + num2; // Store correct result

        string operation = $"{num1} + {num2}";
        TMP_Text text = Operation.GetComponent<TMP_Text>();

        // Cancel any ongoing text revert when we generate new operation
        if (changeTextCoroutine != null)
        {
            StopCoroutine(changeTextCoroutine);
            changeTextCoroutine = null;
        }
        trueOriginalText = operation; // Update true original text here

        text.text = operation; // Update UI

        return operation;
    }

    void GenerateGrid()
    {
        // Get tile size from SpriteRenderer
        Vector2 tileSize = _tilePrefab.GetComponent<SpriteRenderer>().bounds.size;

        // Adjust tile size if necessary (your hack was multiplying by 1.5f)
        tileSize *= 1.5f;

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
                //Add to the dict
                tileDict.Add(new Vector2(x, y), tile);
            }
        }
        currentTile = tileDict[new Vector2(0, 0)];

        TMP_Text Text = Operation.GetComponent<TMP_Text>();
        Text.text = GenerateOperation();
        NumberFiller(MAX_TILES);
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
                number = Random.Range(0, 20);
            }
            while (number == correctResult); // Avoid duplicate correct result

            tile.changeValue(number);
        }

        // Make sure all other tiles remain empty (-1)
        foreach (Tile tile in tileDict.Values.Except(selectedTiles))
        {
            tile.changeValue(-1);
        }
        currentTile.changeValue(-1);

    }

}