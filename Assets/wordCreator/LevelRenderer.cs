using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelRenderer : MonoBehaviour
{
    public GameObject tilePrefab; // Assign your Letter Tile prefab
    public Transform boardParent; // Empty GameObject with GridLayoutGroup
    public Vector2 tileSize = new Vector2(100, 100); // Size of tile buttons
    public int layerCount = 4; // Number of layers
    //public Ge
    // Layered grid data (same structure as your JSON)
    private string[][][] layers = new string[][][]
    {
        new string[][] {
            new[] {"", "", "", "T", "", "", ""},
            new[] {"", "", "E", "A", "R", "", ""},
            new[] {"", "S", "T", "O", "N", "E", ""},
            new[] {"D", "E", "A", "L", "S", "T", "O"},
            new[] {"", "S", "O", "R", "E", "N", ""},
            new[] {"", "", "D", "U", "E", "", ""},
            new[] {"", "", "", "S", "", "", ""}
        }
    };

    void Start()
    {
        RenderLevel();
    }

    void RenderLevel()
    {
        int rows = layers[0].Length;
        int cols = layers[0][0].Length;

        for (int z = 0; z < layers.Length; z++) // Layer
        {
            for (int y = 0; y < rows; y++) // Row
            {
                for (int x = 0; x < cols; x++) // Column
                {
                    string letter = layers[z][y][x];
                   

                    GameObject tile = Instantiate(tilePrefab, boardParent);
                    tile.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(x * tileSize.x, -y * tileSize.y, -z * 10); // Stack in 3D z-depth
                    if (string.IsNullOrEmpty(letter))
                    {
                        tile.gameObject.GetComponent<Image>().enabled = false;
                        var card = tile.GetComponent<Card>();
                        if (card != null)
                        {
                           var letterObj = card.transform.Find("Letter");
                            var facedown = card.transform.Find("Value");
                            var value = card.transform.Find("FaceDown");
                            var wildCard = card.transform.Find("WildCard");
                            letterObj.gameObject.SetActive(false);
                            facedown.gameObject.SetActive(false);
                            value.gameObject.SetActive(false);
                            wildCard.gameObject.SetActive(false);
                        }

                    }


                    //// TextMeshProUGUI text = tile.GetComponentInChildren<TextMeshProUGUI>();
                    //text.text = letter;

                    // Only top layer is face up
                    // tile.SetActive(z == 0);
                    tile.name = $"Tile_{letter}_{x}_{y}_L{z}";
                }
            }
        }
    }
}
