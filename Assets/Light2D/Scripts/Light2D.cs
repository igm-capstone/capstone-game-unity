using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

[Rig3DAsset("lamp", Rig3DExports.Position | Rig3DExports.Rotation)]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Light2D : MonoBehaviour {

    enum VertexLocation
    {
        Middle = 0,
        Left = -1, 
        Right = 1,
    }

    [System.Diagnostics.DebuggerDisplay("[{Position} {PseudoAngle}] {Location} End={IsEndpoint} Sec={IsSecondary}")]
    struct Vertex
    {
        public float PseudoAngle;
        public Vector2 Position;
        public VertexLocation Location;
        public bool IsEndpoint;
        public bool IsSecondary;
    }
    
    [Export]
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

        SetLight();

        BuildLightMesh();
        BuildCollider();
        ResetBounds();
    }

    private void FindLightColliders()
    {
        // TODO filter colliders by LayerMask
        colliders = FindObjectsOfType<PolygonCollider2D>();
    }



    private void SetLight()
    {
        // TODO consider using object pool
        vertices.Clear();

        // tolerance range for endpoint recognition
        const float magRange = .15f;

        var baseAngle = transform.eulerAngles.z * Mathf.Deg2Rad;
        var coneAngle = (angle * Mathf.Deg2Rad) * .5f;
        var highAngle = baseAngle + coneAngle;
        var lowAngle =  baseAngle - coneAngle;

        var colliderVertices = new List<Vertex>();

        // iterate meshes gathering vertices.
        foreach (var collider in colliders)
        {
            if (((1 << collider.gameObject.layer) & shadowMask) == 0)
            {
                continue;
            }

            colliderVertices.Clear();

            var touchDown = false;
            var touchUp = false;

            var basePosition = transform.position;
            basePosition.z = 0;

            var points = (from p in collider.points
                         let worldPoint = (Vector2)collider.transform.TransformPoint(p)
                         let distance = (worldPoint - (Vector2)basePosition).magnitude
            select new { Point = worldPoint, LocalPoint = p, Angle = angle, Distance = distance }).ToArray();

            // ignore colliders that doesn't have any point in range
            if (points.All(p => p.Distance > lightRadius))
            {
                continue;
            }

            // get polygon raycast vertices
            foreach (var point in points)
            {
                var vertex = new Vertex();

                var worldPoint = point.Point; // collider.transform.TransformPoint(point.LocalPoint);
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

                //Debug.DrawLine(transform.position, vertex.Position, vertex.IsEndpoint ? Color.red : Color.white);


                // filter by range
                if (vertex.Position.sqrMagnitude > lightRadius * lightRadius)
                {
                    continue;
                }

                var localAngle = FastMath.PseudoAtan2(vertex.Position.y, vertex.Position.x);
                vertex.PseudoAngle = ClampAngle(baseAngle + localAngle, lowAngle, highAngle);

                //if (vertex.PseudoAngle < 0f)
                //{
                //    touchDown = true;
                //}
                //else if (vertex.PseudoAngle > 2f)
                //{
                //    touchUp = true;
                //}


                // filter by angle
                if (vertex.PseudoAngle > highAngle || vertex.PseudoAngle < lowAngle)
                {
                    continue;
                }

                colliderVertices.Add(vertex);
            }

            // Identify endpoints
            if (colliderVertices.Count > 0)
            {
                // sort by angle in ASCENDING order
                colliderVertices.Sort((v1, v2) => v1.PseudoAngle.CompareTo(v2.PseudoAngle));
                
                var hiloVertices = new Vertex[2];
                var loIndex = 0;
                var hiIndex = colliderVertices.Count - 1;

                if (touchDown && touchUp)
                {
                    var lowest = float.MaxValue;
                    var highest = float.MinValue;

                    for (int i = 0; i < colliderVertices.Count; i++) 
                    {
                        var v = colliderVertices[i];

                        // from all angles that range from -inf to 1
                        // we need to find the highest angle (closest to 1)
                        // and from all angles rannging from 1 to inf
                        // we nned to find the lowest angle (closest to 1)
                        if (v.PseudoAngle < 1f && v.PseudoAngle > highest)
                        {
                            highest = v.PseudoAngle;
                            hiIndex = i;
                        }
                        else if (v.PseudoAngle > 1f && v.PseudoAngle < lowest)
                        {
                            lowest = v.PseudoAngle;
                            loIndex = i;
                        }
                    }
                }

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
                    if (!hiloVertex.IsEndpoint)
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
                    var secVertex = new Vertex { IsSecondary = true };
                    var hit = Physics2D.Raycast(position, direction, lightRadius - distance.magnitude, shadowMask);
                    if (hit)
                    {
                        Debug.DrawLine(position, hit.point, Color.blue);
                        secVertex.Position = transform.InverseTransformPoint(hit.point);
                    }
                    else
                    {
                        // rotate by baseangle first 
                        
                        secVertex.Position = Quaternion.Euler(0, 0, -baseAngle * Mathf.Rad2Deg) * direction * lightRadius;
                        Debug.DrawLine(position, transform.TransformPoint(secVertex.Position), Color.blue);
                    }
                    Debug.DrawLine(transform.TransformPoint(hiloVertex.Position), transform.TransformPoint(secVertex.Position), Color.white * .5f + Color.red * .5f);

                    float newVertexAngle = FastMath.PseudoAtan2(secVertex.Position.y, secVertex.Position.x);
                    secVertex.PseudoAngle = ClampAngle(baseAngle + newVertexAngle, lowAngle, highAngle);

                    vertices.Add(secVertex);

                    #if UNITY_EDITOR
                    if (debug) Debug.DrawLine(transform.position, transform.TransformPoint(secVertex.Position), Color.white * .5f + Color.magenta * .5f);
                    #endif
                }

            }
        }

        var amount = coneAngle * 2.0f / lightSegments;
        var theta = lowAngle;

        // Generate vectors for light cast
        for (int i = 0; i <= lightSegments; i++)
        {
            var position2d = (Vector2) transform.position;

            var vertex = new Vertex();
            vertex.Position = transform.TransformDirection(new Vector3(Mathf.Cos(theta - baseAngle), Mathf.Sin(theta - baseAngle), 0));
            vertex.PseudoAngle = theta;
            vertex.IsSecondary = true;

            vertex.Position *= lightRadius;
            vertex.Position += position2d;
            vertex.Position.Scale(transform.lossyScale);

            var hit = Physics2D.Raycast(transform.position, vertex.Position - position2d, lightRadius, shadowMask);
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
                vertex.Position = transform.InverseTransformPoint(vertex.Position);
                vertices.Add(vertex);
            }

            theta += amount;
        }

        // sort all vertices by angle in a descending order
        vertices.Sort((v1, v2) => v2.PseudoAngle.CompareTo(v1.PseudoAngle));

        var epsilon = 0.00001f;
        for (int i = 0; i < vertices.Count - 1; i++)
        {

            var fstVertex = vertices[i];
            var sndVertex = vertices[i + 1];

            if (Mathf.Abs(fstVertex.PseudoAngle - sndVertex.PseudoAngle) <= epsilon)
            {

                if (sndVertex.Location == VertexLocation.Right)
                { // Right Ray

                    if (fstVertex.Position.sqrMagnitude > sndVertex.Position.sqrMagnitude)
                    {
                        vertices[i] = sndVertex;
                        vertices[i + 1] = fstVertex;
                    }
                }


                // ALREADY DONE!!
                if (fstVertex.Location == VertexLocation.Left)
                { // Left Ray
                    if (fstVertex.Position.sqrMagnitude < sndVertex.Position.sqrMagnitude)
                    {

                        vertices[i] = sndVertex;
                        vertices[i + 1] = fstVertex;
                    }
                }
            }
        }
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

            // last index is 1 if is the last vertex (one loop)
            //meshTriangles[i + 2] = (i == len - 3) ? 1 : j + 2;
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
