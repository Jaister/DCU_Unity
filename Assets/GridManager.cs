using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private GameObject PlayerPrefab;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        // Get tile size from SpriteRenderer instead of Renderer
        Vector2 tileSize = _tilePrefab.GetComponent<SpriteRenderer>().bounds.size;

        float totalWidth = width * tileSize.x;
        float totalHeight = height * tileSize.y;

        Vector2 offset = new Vector2(
            -totalWidth / 2 + (tileSize.x / 2),
            -totalHeight / 2 + (tileSize.y / 2)
        );

        Vector3 parentPosition = transform.position;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pos = new Vector3(
                    parentPosition.x + offset.x + (x * tileSize.x),
                    parentPosition.y + offset.y + (y * tileSize.y),
                    parentPosition.z
                );

                var tile = Instantiate(_tilePrefab, pos, Quaternion.identity, this.transform);
                tile.name = $"Tile {x} {y}";

                var isOffset = (x % 2 == 0 && y % 2 == 0) || (x % 2 != 0 && y % 2 != 0);
                tile.Init(isOffset);

                // Place the Rocket (PlayerPrefab) on the first tile
                if (x == 0 && y == 0)
                {
                    PlayerPrefab.SetActive(true);
                    PlayerPrefab.transform.position = new Vector3(pos.x, pos.y, -1); // Move it slightly forward
                }
            }
        }

        // Center the grid in the camera view
        var screenHeight = Camera.main.orthographicSize * 2;
        var screenWidth = screenHeight * Camera.main.aspect;
        transform.position = new Vector3(screenWidth / 2, screenHeight / 2, 0);
    }
}
