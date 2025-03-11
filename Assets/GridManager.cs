using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private GameObject PlayerPrefab;
    //Tile dict
    private Dictionary<Vector2, Tile> tileDict = new Dictionary<Vector2, Tile>();
    private Transform firstTile;

    void Start()
    {
        // Deactivate player initially
        PlayerPrefab.SetActive(false);
        GenerateGrid();

        // Position player after the grid is centered
        PositionPlayer();
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
    NumberFiller();

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

    void NumberFiller(){
        //If random in range is 0, then fill the tile with a number
        //If random in range is 1, then deactivate number
        foreach (Tile tile in tileDict.Values)
        {
            int random = Random.Range(0, 2);
            if (random == 0)
            {
                tile.changeValue(Random.Range(0, 10));
            }
            else
            {
                tile.changeValue(-1);
            }
        }
    }
}
