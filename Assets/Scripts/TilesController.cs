using TMPro;
using UnityEngine;

public class TilesController : MonoBehaviour
{
    [SerializeField] private float TILE_SIZE = 1f;
    [SerializeField] private int TILE_COUNT_X = 10;
    [SerializeField] private int TILE_COUNT_Z = 10;
    [SerializeField] private Material tileMaterial;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private MeshRenderer groundMeshRenderer;

    private GameObject[,] tiles;
    private PathfindigTileTextBox[,] textBoxes;
    Transform tilesParent;
    Transform textBoxesParent;
    Transform linesParent;

    void Start()
    {
        GenerateTileMap();
    }

    // void Update(){
    //     if(GameManager.Instance.GetEnableDebugShowPathfindig()){
    //         textBoxesParent.gameObject.SetActive(true);
    //     }
    //     else{
    //         textBoxesParent.gameObject.SetActive(false);
    //     }
    // }

    private void GenerateTileMap()
    {
        tilesParent = new GameObject("TilesParent").transform;
        textBoxesParent = new GameObject("TextBoxesParent").transform;
        linesParent = new GameObject("linesParent").transform;

        tiles = new GameObject[TILE_COUNT_X, TILE_COUNT_Z];
        textBoxes = new PathfindigTileTextBox[TILE_COUNT_X, TILE_COUNT_Z];

        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int z = 0; z < TILE_COUNT_Z; z++)
            {
                tiles[x, z] = GenerateSingleTile(tilesParent, x, z);
                textBoxes[x, z] = GenerateSingleTextBox(textBoxesParent, x, z);
                DrawLine(linesParent, GetWorldPositionWithOffset(x, z), GetWorldPositionWithOffset(x, z + 1), Color.black);
                DrawLine(linesParent, GetWorldPositionWithOffset(x, z), GetWorldPositionWithOffset(x + 1, z), Color.black);
            }
        DrawLine(linesParent, GetWorldPositionWithOffset(0, TILE_COUNT_Z), GetWorldPositionWithOffset(TILE_COUNT_X, TILE_COUNT_Z), Color.black);
        DrawLine(linesParent, GetWorldPositionWithOffset(TILE_COUNT_X, 0), GetWorldPositionWithOffset(TILE_COUNT_X, TILE_COUNT_Z), Color.black);
    }

    private GameObject GenerateSingleTile(Transform parent, int x, int z)
    {
        GameObject tileObject = new GameObject($"X:{x},Z:{z}");
        tileObject.transform.SetParent(parent);
        
        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        MeshRenderer tileMeshRenderer = tileObject.AddComponent<MeshRenderer>();
        tileMeshRenderer.material = tileMaterial;
        tileMaterial.renderQueue  = groundMeshRenderer.material.renderQueue + 1;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x, 0, z) * TILE_SIZE + transform.position;
        vertices[1] = new Vector3(x, 0, z + 1) * TILE_SIZE + transform.position;
        vertices[2] = new Vector3(x + 1, 0, z) * TILE_SIZE + transform.position;
        vertices[3] = new Vector3(x + 1, 0, z + 1) * TILE_SIZE + transform.position;

        int[] triangles = new int[] { 0, 1, 2, 1, 3, 2 };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider2D>();

        return tileObject;
    }

    private PathfindigTileTextBox GenerateSingleTextBox(Transform parent, int x, int z)
    {
        Vector3 tileBoxPostion = GetWorldPositionWithOffset(x, z) + new Vector3(TILE_SIZE, 0, TILE_SIZE) * .5f;
        PathfindigTileTextBox pathfindigTileTextBox = new PathfindigTileTextBox(parent, $"X:{x},Z:{z}", tileBoxPostion);
        return pathfindigTileTextBox;
    }

    private void DrawLine(Transform parent, Vector3 start, Vector3 end, Color color)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.SetParent(parent);
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();

        lineRenderer.material = lineMaterial; 
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    public Vector3 GetWorldPositionWithOffset(int x, int z) {
        return new Vector3(x, 0, z) * TILE_SIZE + transform.position + new Vector3(0, 0.001f, 0);
    }

}

public class PathfindigTileTextBox
{
    private enum TextBoxType{
        G,  //Cost from the origin
        H,  //Heuristic cost to reach the target
        F   //The sum of G + H
    }

    private GameObject textBoxG; 
    private GameObject textBoxH; 
    private GameObject textBoxF; 

    public PathfindigTileTextBox(Transform parent, string name, Vector3 position)
    {
        textBoxG = GenerateSingleTextBox(parent, name, position, TextBoxType.G);
        textBoxH = GenerateSingleTextBox(parent, name, position, TextBoxType.H);
        textBoxF = GenerateSingleTextBox(parent, name, position, TextBoxType.F);
    }

    private GameObject GenerateSingleTextBox(Transform parent, string name, Vector3 position, TextBoxType type){
        GameObject textBoxObject = new GameObject(name + "_" + type.ToString(), typeof(TextMeshPro));
        textBoxObject.transform.SetParent(parent);
        textBoxObject.transform.localPosition = position;
        textBoxObject.transform.rotation = Quaternion.Euler(90, 0, 0);
        TextMeshPro textMeshPro = textBoxObject.GetComponent<TextMeshPro>();
        textMeshPro.color = Color.black;
        textMeshPro.fontStyle = FontStyles.Bold;

        RectTransform rectTransform = textMeshPro.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        switch (type)
        {
            case TextBoxType.G:
                textMeshPro.alignment = TextAlignmentOptions.TopLeft;
                textMeshPro.text = "0";
                textMeshPro.fontSize = 2.5f;
                break;
            case TextBoxType.H:
                textMeshPro.alignment = TextAlignmentOptions.Center;
                textMeshPro.text = "0";
                textMeshPro.fontSize = 4;
                break;
            case TextBoxType.F:
                textMeshPro.alignment = TextAlignmentOptions.BottomRight;
                textMeshPro.text = "0";
                textMeshPro.fontSize = 2.5f;
                break;
        }

        return textBoxObject;
    }

    public void UpdateTextBox(string g = null, string h = null, string f = null)
    {
        if(g != null)
            textBoxG.GetComponent<TextMeshPro>().text = g;
        if(h != null)
            textBoxH.GetComponent<TextMeshPro>().text = h;
        if(f != null)
            textBoxF.GetComponent<TextMeshPro>().text = f;
    }
}