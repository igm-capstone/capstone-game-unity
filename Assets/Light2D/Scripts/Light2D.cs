using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[Rig3DAsset("lamps", Rig3DExports.Rotation)]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Light2D : MonoBehaviour {

    public GameObject exportPosObj;

    [Export]
    public Vector3 position;
       
    enum VertexLocation
    {
        Middle = 0,
        Left = -1, 
        Right = 1,
    }

    [System.Diagnostics.DebuggerDisplay("[{Position} {DegAngle}({Angle})] {Location} End={IsEndpoint} Sec={IsSecondary}")]
    struct Vertex
    {
        private float DegAngle { get { return Angle*Mathf.Rad2Deg; } }

        public bool Ignore;
        public float Angle;
        public Vector2 Position;
        public VertexLocation Location;
        public bool IsEndpoint;
        public bool IsSecondary;
    }

    struct ColliderVertexInfo
    {
        public Vector3 WorldPoint;
        public Vector3 LocalPoint;
        public float Distance;
    }
    
    [Export, Range(3, 100)]
    public  int lightSegments = 8;

    [Export]
    public float lightRadius = 100f;

    [Export, Range(0, 360)]
    public float angle = 90;

    public Material lightMaterial;
    public LayerMask shadowMask = Physics.DefaultRaycastLayers;

    private bool debug;

    private PolygonCollider2D[] colliders;
    private List<Vertex> vertices;

    private Mesh lightMesh;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private PolygonCollider2D lightMeshCollider;

    // tolerance range for endpoint recognition
    private const float magRange = .15f;

    private float baseAngle;
    private float coneAngle;
    private float highAngle;
    private float lowAngle;

    public void OnEnable()
    {
        position = exportPosObj.transform.position;
    }

    void Awake()
    {
        #if UNITY_EDITOR
        debug = true;
        #endif

        lightMesh = new Mesh();
        lightMesh.name = string.Format("Light Mesh ({0})", name);
        lightMesh.MarkDynamic();

        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = lightMesh;

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = lightMaterial;

        var container = transform.Find("LightMesh");
        if (!container)
        {
            container = new GameObject("LightMesh").transform;
            container.SetParent(transform, true);
            container.localPosition = Vector3.zero;
            container.gameObject.layer = gameObject.layer;
        }

        lightMeshCollider = container.GetComponent<PolygonCollider2D>();
        if (lightMeshCollider == null)
        {
            lightMeshCollider = container.gameObject.AddComponent<PolygonCollider2D>();
        }

        lightMeshCollider.isTrigger = true;

        UpdateLightFX();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying)
        {
            UpdateLightFX();
        }
    }
#endif

    public void UpdateLightFX()
    {

        if (vertices == null)
        {
            vertices = new List<Vertex>();
        }

        // TODO avoid getting all colliders every frame
        FindLightColliders();

        vertices.Clear();

        AddColliderPoints();
        AddSegmentPoints();

        // remove ignored
        for (int i = vertices.Count - 1; i >= 0 ; i--)
        {
            if (vertices[i].Ignore)
            {
                vertices.RemoveAt(i);
            }
        }

        vertices.Sort(CompareVertices);

        BuildLightMesh();
        BuildCollider();
        ResetBounds();
    }

    private void FindLightColliders()
    {
        // TODO filter colliders by LayerMask
        colliders = FindObjectsOfType<PolygonCollider2D>();
    }


    private void AddColliderPoints()
    {
        baseAngle = transform.eulerAngles.z * Mathf.Deg2Rad;
        coneAngle = (angle * Mathf.Deg2Rad) * .5f;
        highAngle = baseAngle + coneAngle;
        lowAngle  =  baseAngle - coneAngle;

        var colliderVertices = new List<Vertex>();

        // iterate meshes gathering vertices.
        foreach (var collider in colliders)
        {
            if (((1 << collider.gameObject.layer) & shadowMask) == 0)
            {
                continue;
            }

            colliderVertices.Clear();

            var basePosition = transform.position;
            basePosition.z = 0;

            var points = (
                from p in collider.points
                let worldPoint = (Vector2)collider.transform.TransformPoint(p)
                let distance = (worldPoint - (Vector2)basePosition).magnitude
                select new ColliderVertexInfo {
                    LocalPoint = p,
                    WorldPoint = worldPoint,
                    Distance = distance
                }).ToArray();

            // ignore colliders that doesn't have any point in range
            if (points.All(p => p.Distance > lightRadius))
            {
                continue;
            }

            AddPrimaryPoints(points, ref colliderVertices);
            AddSecondaryPoints(ref colliderVertices);
        }
    }

    private void AddPrimaryPoints(ColliderVertexInfo[] points, ref List<Vertex> colliderVertices)
    {
        // get polygon raycast vertices
        foreach (var point in points)
        {
            var vertex = new Vertex();

            var worldPoint = point.WorldPoint;
            var distance = point.Distance;

            var hit = Physics2D.Raycast(transform.position, (Vector3)worldPoint - transform.position, distance, shadowMask);
            if (hit)
            {
                // vertex position is saved in light local space.
                vertex.Position = transform.InverseTransformPoint(hit.point);
                vertex.IsEndpoint = Mathf.Abs(worldPoint.sqrMagnitude - hit.point.sqrMagnitude) <= magRange;
            }
            else
            {
                // vertex position is saved in light local space.
                vertex.Position = transform.InverseTransformPoint(worldPoint);
                vertex.IsEndpoint = true;
            }

            if (vertex.Position.sqrMagnitude > lightRadius * lightRadius)
            {
                vertex.Position = vertex.Position.normalized * lightRadius;
            }

            var localAngle = Mathf.Atan2(vertex.Position.y, vertex.Position.x);
            vertex.Angle = ClampAngle(baseAngle + localAngle, lowAngle, highAngle);

            // filter by angle
            if (vertex.Angle > highAngle || vertex.Angle < lowAngle)
            {
                vertex.Ignore = true;
            }

            colliderVertices.Add(vertex);
        }
    }

    private void AddSecondaryPoints(ref List<Vertex> colliderVertices)
    {
        // Identify endpoints
        if (colliderVertices.Count == 0)
        {
            return;
        } 
        
        // sort by angle in ASCENDING order
        colliderVertices.Sort((v1, v2) => v1.Angle.CompareTo(v2.Angle));
                
        var hiloVertices = new Vertex[2];
        var loIndex = 0;
        var hiIndex = colliderVertices.Count - 1;

        Vertex vertex;
        vertex = colliderVertices[loIndex];
        vertex.Location = VertexLocation.Right;
        hiloVertices[0] = vertex;
        colliderVertices[loIndex] = vertex;

        vertex = colliderVertices[hiIndex];
        vertex.Location = VertexLocation.Left;
        hiloVertices[1] = vertex;
        colliderVertices[hiIndex] = vertex;

        vertices.AddRange(colliderVertices);

        foreach (var hiloVertex in hiloVertices)
        {
            if (hiloVertex.Ignore || !hiloVertex.IsEndpoint)
            {
                continue;
            }


            // vertex calculate position
            var position = transform.TransformPoint(hiloVertex.Position);
            var distance = position - transform.position;
            var direction = distance.normalized;

            position += direction * .0001f;
            //Debug.DrawLine(position + Vector3.one, position - Vector3.one, Color.blue);
            //Debug.DrawLine(transform.position, transform.position + direction * 30, Color.blue);
            var secVertex = new Vertex { IsSecondary = true, Location = hiloVertex.Location };
            var hit = Physics2D.Raycast(position, direction, lightRadius - distance.magnitude, shadowMask);
            if (hit)
            {
                //Debug.DrawLine(position, hit.point, Color.blue);
                secVertex.Position = transform.InverseTransformPoint(hit.point);
            }
            else
            {
                // rotate by baseangle first 
                        
                secVertex.Position = Quaternion.Euler(0, 0, -baseAngle * Mathf.Rad2Deg) * direction * lightRadius;
                Debug.DrawLine(position, transform.TransformPoint(secVertex.Position), Color.blue);
            }
            
            float newVertexAngle = Mathf.Atan2(secVertex.Position.y, secVertex.Position.x);
            secVertex.Angle = ClampAngle(baseAngle + newVertexAngle, lowAngle, highAngle);

            vertices.Add(secVertex);

#if UNITY_EDITOR
            // extensions
            if (debug) Debug.DrawLine(transform.TransformPoint(hiloVertex.Position), 
                transform.TransformPoint(secVertex.Position), Color.white * .5f + Color.red * .5f);
#endif
        }
    }

    private void AddSegmentPoints()
    {
        var amount = coneAngle * 2.0f / lightSegments;
        var theta = lowAngle;

        // Generate vectors for light cast
        for (int i = 0; i <= lightSegments; i++)
        {
            var position = (Vector2)transform.position;

            var vertex = new Vertex();
            vertex.Angle = theta;
            vertex.IsSecondary = true;

            //vertex.Position += position;
            //vertex.Position.Scale(transform.lossyScale);

            var direction = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
            var hit = Physics2D.Raycast(position, direction, lightRadius, shadowMask);
            if (hit)
            {
                // first and last segments must be added to the mesh
                if (i == 0 || i == lightSegments)
                {
                    vertex.Position = transform.InverseTransformPoint(hit.point);
                    vertices.Add(vertex);
                }
            }
            else
            {
                vertex.Position = transform.InverseTransformPoint(position + (direction * lightRadius));
                vertices.Add(vertex);
            }
            
            theta += amount;

#if UNITY_EDITOR
            if (debug && i == 0 || i == lightSegments)
            {
                Debug.DrawLine(transform.position, transform.TransformPoint(vertex.Position),
                     Color.white * .5f + Color.blue * .5f);
            }
#endif
        }
    }
    
    
    private int CompareVertices(Vertex v1, Vertex v2)
    {
        var epsilon = 0.00001f;

        if (Mathf.Abs(v1.Angle - v2.Angle) > epsilon)
        {
            return v2.Angle.CompareTo(v1.Angle);
        }

        if (v1.Location == VertexLocation.Right || v2.Location == VertexLocation.Right)
        { // Right Ray
            return v1.Position.sqrMagnitude.CompareTo(v2.Position.sqrMagnitude);
        }

        if (v1.Location == VertexLocation.Left || v2.Location == VertexLocation.Left)
        { // Left Ray
            return v2.Position.sqrMagnitude.CompareTo(v1.Position.sqrMagnitude);
        }

        return 0;
    }

    private void BuildLightMesh()
    {
        // fill the mesh with vertices
        var meshVertices = new Vector3[vertices.Count + 1];
        var meshUvs = new Vector2[meshVertices.Length];
        var meshTriangles = new int[(vertices.Count - 1) * 3];

        meshVertices[0] = Vector3.zero;

        // vertices
        for (int i = 0; i < vertices.Count; i++)
        {
            meshVertices[i + 1] = vertices[i].Position;
        }

        // uvs
        for (int i = 0; i < meshVertices.Length; i++)
        {
            meshUvs[i] = meshVertices[i];
        }

        // triangles
        for (int i = 0, j = 0, len = (vertices.Count - 1) * 3; i < len; i += 3, j++)
        {

            meshTriangles[i] = 0;
            meshTriangles[i + 1] = j + 1;
            meshTriangles[i + 2] = j + 2;
        }

        lightMesh.Clear();
        lightMesh.vertices = meshVertices;
        lightMesh.uv = meshUvs;
        lightMesh.triangles = meshTriangles;

        // update light material (in case it has changed)
        meshRenderer.sharedMaterial = lightMaterial;
    }


    private void BuildCollider()
    {
        var points = new List<Vector2>();

        points.Add(Vector3.zero);
        foreach (var vertex in vertices)
        {
            if (vertex.Location == VertexLocation.Middle && !vertex.IsSecondary)
            {
                continue;
            }
            
            points.Add(vertex.Position);
        }

        lightMeshCollider.points = points.ToArray();
    }

    private float ClampAngle(float angle, float min, float max)
    {
        var val = angle < min ? angle + Mathf.PI * 2 : angle > max ? angle - Mathf.PI * 2 : angle;
        return val;
    }

    private void ResetBounds()
    {
        lightMesh.RecalculateBounds();
    }

    public void Toggle()
    {
        meshRenderer.enabled = !meshRenderer.enabled;
    }

}
