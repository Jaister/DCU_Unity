using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private Tile _tilePrefab;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        Vector2 tileSize = _tilePrefab.GetComponent<Renderer>().bounds.size;

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
                var tile = Instantiate(_tilePrefab,
                    new Vector3(
                        parentPosition.x + offset.x + (x * tileSize.x),
                        parentPosition.y + offset.y + (y * tileSize.y),
                        parentPosition.z
                    ),
                    Quaternion.identity,
                    this.transform);

                tile.name = $"Tile {x} {y}";
                var isOffset = (x % 2 == 0 && y % 2 == 0) || (x % 2 != 0 && y % 2 != 0);
                tile.Init(isOffset);
            }
        }
    }
}