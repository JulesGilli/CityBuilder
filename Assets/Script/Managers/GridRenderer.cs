using UnityEngine;

[RequireComponent(typeof(GridManager))]
public class GridRenderer : MonoBehaviour
{
    public Color gridColor = Color.gray;
    private Material lineMaterial;
    private GridManager gm;

    void Awake()
    {
        gm = GetComponent<GridManager>();
        CreateLineMaterial();
    }

    void OnRenderObject()
    {
        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(gridColor);

        // Verticales
        for (int x = 0; x <= gm.width; x++)
        {
            GL.Vertex(new Vector3(x * gm.cellSize, 0.01f, 0));
            GL.Vertex(new Vector3(x * gm.cellSize, 0.01f, gm.height * gm.cellSize));
        }
        // Horizontales
        for (int y = 0; y <= gm.height; y++)
        {
            GL.Vertex(new Vector3(0, 0.01f, y * gm.cellSize));
            GL.Vertex(new Vector3(gm.width * gm.cellSize, 0.01f, y * gm.cellSize));
        }

        GL.End();
    }

    void CreateLineMaterial()
    {
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        lineMaterial = new Material(shader)
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        lineMaterial.SetInt("_ZWrite", 0);
    }
}
