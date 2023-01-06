using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float SelectLineWidthUnit = 0.025f;

    public MapObjectList MapObjects { get; private set; }
    public Dictionary<string, IDictionary> Builds { get; set; }
    public Dictionary<string, IDictionary> People { get; set; }

    public (int, int) Population { get; set; }
    public (int, int) Food { get; set; }
    public (int, int) Ore { get; set; }

    public Vector3 SelectStart { get; private set; }
    public Vector3 SelectEnd { get; private set; }
    private LineRenderer line;

    public static GameManager Instance
    {
        get
        {
            return GameObject.Find("GameManager").GetComponent<GameManager>();
        }
    }

    private void Awake()
    {
        MapObjects = new MapObjectList();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            SelectStart = mouse;
        }
        else if (Input.GetMouseButton(0))
        {
            if (line == null) line = new GameObject("Select").AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            line.startColor = Color.red;
            line.endColor = Color.red;
            line.positionCount = 5;
            line.SetPositions(new Vector3[]
            {
                new Vector3(mouse.x, mouse.y),
                new Vector3(SelectStart.x, mouse.y),
                new Vector3(SelectStart.x, SelectStart.y),
                new Vector3(mouse.x, SelectStart.y),
                new Vector3(mouse.x, mouse.y),
            });
            line.startWidth = SelectLineWidthUnit * Camera.main.orthographicSize;
            line.endWidth = SelectLineWidthUnit * Camera.main.orthographicSize;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Destroy(line.gameObject);
            line = null;
            SelectEnd = mouse;
        }
    }
}
