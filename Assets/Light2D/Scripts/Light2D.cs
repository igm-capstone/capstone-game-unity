using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

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

    struct Vertex
    {
        public float PseudoAngle;
        public Vector2 Position;
        public VertexLocation Location;
        public bool IsEndpoint;
        public bool IsSecondary;
    }
    
    public  int lightSegments = 8;
    public  float lightRadius = 100f;
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

            // get polygon raycast vertices
            foreach (var point in collider.points)
            {
                var vertex = new Vertex();

                var worldPoint = collider.transform.TransformPoint(point);

                var distance = (worldPoint - transform.position).magnitude;

                var hit = Physics2D.Raycast(transform.position, worldPoint - transform.position, distance, shadowMask);
                if (hit)
                {
                    vertex.Position = hit.point;

                    vertex.IsEndpoint = Mathf.Abs(worldPoint.sqrMagnitude - hit.point.sqrMagnitude) <= magRange;
                }
                else
                {
                    vertex.Position = worldPoint;
                    vertex.IsEndpoint = true;
                }

                //Debug.DrawLine(transform.position, vertex.Position, vertex.IsEndpoint ? Color.red : Color.white);

                // vertex position is saved in light local space.
                vertex.Position = transform.InverseTransformPoint(vertex.Position);
                vertex.PseudoAngle = FastMath.PseudoAtan2(vertex.Position.y, vertex.Position.x);

                if (vertex.PseudoAngle < 0f)
                {
                    touchDown = true;
                }
                else if (vertex.PseudoAngle > 2f)
                {
                    touchUp = true;
                }

                if (vertex.Position.sqrMagnitude <= lightRadius * lightRadius)
                {
                    colliderVertices.Add(vertex);
                }
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

                    var position = transform.TransformPoint(hiloVertex.Position);
                    var distance = position - transform.position;
                    var direction = distance.normalized;
                    position += direction * .0001f;

                    var hit = Physics2D.Raycast(position, direction, lightRadius, shadowMask);

                    Vector3 newVertexPosition;
                    newVertexPosition = hit ? (Vector3)hit.point : transform.TransformPoint(direction*lightRadius);

                    #if UNITY_EDITOR
                    //Debug.DrawLine(position, newVertexPosition, Color.green);
                    if (debug) Debug.DrawLine(transform.position, newVertexPosition, Color.white * .5f + Color.magenta * .5f);
                    #endif

                    vertex = new Vertex();
                    vertex.Position = transform.InverseTransformPoint(newVertexPosition);
                    vertex.PseudoAngle = FastMath.PseudoAtan2(vertex.Position.y, vertex.Position.x);
                    vertex.IsSecondary = true;

                    vertices.Add(vertex);
                }

            }
        }

        var theta = 0;
        //float amount = (Mathf.PI * 2) / lightSegments;
        var amount = 360 / lightSegments;

        // Generate vectors for light cast
        for (int i = 0; i < lightSegments; i++)
        {
            theta = (amount * i) % 360;

            var position2d = (Vector2) transform.position;

            var vertex = new Vertex();
            vertex.Position = new Vector3(FastMath.SinArray[theta], FastMath.CosArray[theta], 0);
            vertex.PseudoAngle = FastMath.PseudoAtan2(vertex.Position.y, vertex.Position.x);
            vertex.IsSecondary = true;

            vertex.Position *= lightRadius;
            vertex.Position += position2d;
            vertex.Position.Scale(transform.lossyScale);



            var hit = Physics2D.Raycast(transform.position, vertex.Position - position2d, lightRadius, shadowMask);
            //Debug.DrawRay(transform.position, v.pos - transform.position, Color.white);

            if (!hit)
            {
                //Debug.DrawLine(transform.position, v.pos, Color.white);
                vertex.Position = transform.InverseTransformPoint(vertex.Position);
                vertices.Add(vertex);
            }
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
        var meshTriangles = new int[(vertices.Count * 3)];

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
        for (int i = 0, j = 0, len = vertices.Count * 3; i < len; i += 3, j++)
        {

            meshTriangles[i] = 0;
            meshTriangles[i + 1] = j + 1;
            
            // last index is 1 if is the last vertex (one loop)
            meshTriangles[i + 2] = (i == len - 3) ? 1 : j + 2;
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
    private void ResetBounds()
    {
        var bounds = lightMesh.bounds;
        bounds.center = Vector3.zero;
        lightMesh.bounds = bounds;
    }

    public void Toggle()
    {
        meshRenderer.enabled = !meshRenderer.enabled;
    }

}
