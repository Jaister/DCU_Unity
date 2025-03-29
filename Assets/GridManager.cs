using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject Operation;
    [SerializeField] private PersistencyManager persistencyManager; // Reference to the persistency manager
    [SerializeField] private TMP_Text progress;

    //Tile dict
    private Dictionary<Vector2, Tile> tileDict = new Dictionary<Vector2, Tile>();
    private Transform firstTile;
    private int correctResult;
    private const int MAX_TILES = 8;
    private int winCondition = 10;
    private int winCount = 0;

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
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        CheckResult();
    //    }
    //}
    public void CheckResult(int result)
    {
        TMP_Text Text = Operation.GetComponent<TMP_Text>();
        if (result == correctResult)
        {
            Text.text = GenerateOperation();
            NumberFiller(MAX_TILES-1);
            persistencyManager.AddStars();
            persistencyManager.UpdateStarsText();
            winCount++;
            progress.text = $"{winCount}/{winCondition}";
        }
        else
        {
            Text.text = "Try again!";
        }
    }
    string GenerateOperation()
    {
        int num1 = Random.Range(0, 10);
        int num2 = Random.Range(0, 10);
        correctResult = num1 + num2; // Store correct result

        string operation = $"{num1} + {num2}";
        TMP_Text text = Operation.GetComponent<TMP_Text>();
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

            // Now activate the player
            PlayerPrefab.SetActive(true);
        }
    }

    void NumberFiller(int maxTiles)
    {
        List<Tile> availableTiles = new List<Tile>(tileDict.Values);

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
        foreach (Tile tile in availableTiles.Except(selectedTiles))
        {
            tile.changeValue(-1);
        }
    }




}
