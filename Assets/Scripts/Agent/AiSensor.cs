using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AiSensor : MonoBehaviour
{
    [SerializeField] private float distance = 10f;
    [SerializeField] private float angle = 30f;
    [SerializeField] private float height =1.0f;
    [SerializeField] private Color meshColor = Color.red;

    [SerializeField] private int scanFrequency = 30;
    [SerializeField] private LayerMask objectsLayer;
    [SerializeField] private LayerMask occlusionLayers;

    [SerializeField] private bool drawMesh;

    private Collider[] colliders = new Collider[50];
    private int count;
    private float scanInterval;
    private float scanTimer;
    public List<GameObject> Objects
    {
        get
        {
            objects.RemoveAll(obj => !obj);
            return objects;
        }
    }

    private List<GameObject> objects = new List<GameObject>();

    void Start()
    {
        scanInterval = 1.0f / scanFrequency; // Set scan interval
    }

    void Update()
    {
        // Scan not every frame, for optimization purposes
        scanTimer -= Time.deltaTime;
        if (scanTimer  < 0)
        {
            scanTimer += scanInterval;
            Scan();
        }
    }

    private void Scan() // Get object in sight and add it to the objects array
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, objectsLayer, QueryTriggerInteraction.Collide);
        objects.Clear();
        for (int i = 0; i < count; i++)
        {
            GameObject obj = colliders[i].gameObject;
            if (IsInSight(obj))
            {
                objects.Add(obj);
            }
        }
    }

    public bool IsInSight(GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;
        if (direction.y < 0 || direction.y > height) return false; // Ensure the target is within the sight height

        // Ensure the target is within the sight distance.
        if (direction.sqrMagnitude > distance * distance) return false;
        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if (deltaAngle > angle) return false;

        origin.y += height / 2;
        dest.y = origin.y;

        if (Physics.Linecast(origin, dest, occlusionLayers)) return false; // Ensure the target is not behind the obstacle

        return true;
    }

    public int Filter(GameObject[] buffer, string layerName) // Filter which type of object agent see
    {
        int layer = LayerMask.NameToLayer(layerName);   
        int count = 0;
        foreach (var obj in Objects)
        {
            if (obj.layer == layer) // If  object match the filter requirements add it to the buffer
            {
                buffer[count++] = obj;
            }

            if (buffer.Length == count)
            {
                break; // Buffer is full
            }
        }

        return count; // Return the number of found objects
    }

    #region SightMesh

    private Mesh mesh;
    Mesh CreateWedgeMesh() // Create a sight mesh in the editor
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;  // Each triangle have 3 verts, so we just miltiple triangle by 3 

        Vector3[] vertices = new Vector3[numVertices]; // Set lenght of vertices array
        int[] triangles = new int[numVertices];  // Set lenght of triangles array

        //To build a triangle shape needed 6 points:
        Vector3 bottomCenter = Vector3.zero; // Origin of the wedge

        //Calculate by taking the forward axis of the  agent and multiplying it by  distance, and then rotate it to the left and the right around Y axis
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        //Shift them up by multiplying up axis by height
        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
            
        int vert = 0; // int to track in vert array

        //Add vertices to the each side
        //left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;
        //right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;
        for (int i = 0; i < segments; i++)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

            topRight = bottomRight + Vector3.up * height;
            topLeft = bottomLeft + Vector3.up * height;

            //far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;
            //top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;
            //bottom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }

        for (int i = 0; i < numVertices; ++i)
        {
            triangles[i] = i;
        }

        //Assign verts and triangles in to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    // Change mesh in the inspector if we change same variables
    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
        scanInterval = 1.0f / scanFrequency;
    }

    //Draw sight mesh
    private void OnDrawGizmos()
    {
        if (drawMesh)
        {
            if (mesh)
            {
                Gizmos.color = meshColor;
                Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
            }

            // Draw sphere on objects that found in sight
            Gizmos.color = Color.green;
            foreach (var obj in objects)
            {
                Gizmos.DrawSphere(obj.transform.position, 1f);
            }
        }
    }

    #endregion
}
