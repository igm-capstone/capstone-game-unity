﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TargetFollower : MonoBehaviour
{
    private GridBehavior grid;

    public LayerMask wallMask;

    // repel
    public Vector3 repelOffset = new Vector3(.66f, -.6f, 0);
    public float repelFocus = 4f;
    public float repelCastDistance = 2f;
    public float repelIncrement = 0.2f;
    public float rapelDecay = 0.1f;
    public float maxRepel = 1f;

    public float turnRate = 50;
    public float moveSpeed = 20;

    float reverseTime = 0;
    float repel = 0;
    private IList<Vector2> path;
    private IList<Node> nodePath;
    private RpcNetworkAnimator animator;
    private Transform container;

    void Awake()
    {
        grid = FindObjectOfType<GridBehavior>();
        animator = GetComponent<RpcNetworkAnimator>();
        container = transform.Find("Rotation");
    }

    public void MoveTowards(Transform target)
    {
        path = GetPathTo(target);
        var node = grid.getNodeAtPos(transform.position);
        var speedFactor = 1 / (node.Weight);

        if (path == null)
        {
            return;
        }

        var colors = new[] { Color.magenta, Color.yellow };


        for (int i = 1; i < path.Count; i++)
        {
            Debug.DrawLine(path[i - 1], path[i], colors[i%colors.Length]);
        }

        Vector2 targetPosition = transform.position;
        Vector2 position = transform.position;
        foreach (var nodePosition in path.Take(3).Reverse())
        {
            var distance = nodePosition - position;
            var direction = distance.normalized;

            var hit = Physics2D.Raycast(position, direction, distance.magnitude, wallMask);
            if (!hit)
            {
                targetPosition = nodePosition;
                break;
            }
        }

        Debug.DrawLine(position, targetPosition, Color.cyan);

        var currentAngle = container.eulerAngles.z;
        
        var targetDistance = targetPosition - position;
        var targetAngle = FastMath.Atan2(-targetDistance.x, targetDistance.y) * Mathf.Rad2Deg;


        System.Func<float, float, float> mod = (a, n) => a - Mathf.Floor(a/n) * n;

        var da = Mathf.Abs(mod((targetAngle - currentAngle + 180), 360) - 180);

        //Debug.LogFormat("{0}, {1}, {2}", targetAngle, currentAngle, da);


        var targetRotation = Quaternion.Euler(0, 0, targetAngle);
        container.rotation = Quaternion.RotateTowards(container.rotation, targetRotation, turnRate  * 5 * Time.deltaTime);

        var frontOffset = container.TransformPoint(Vector3.up * repelFocus);
        var rightOffset = container.TransformPoint(repelOffset);
        var leftOffset =  container.TransformPoint(new Vector3(-repelOffset.x, repelOffset.y));

        Debug.DrawLine(rightOffset, rightOffset + (frontOffset - rightOffset).normalized * repelCastDistance);
        if (Physics2D.Raycast(rightOffset, frontOffset - rightOffset, repelCastDistance, wallMask))
        {
            repel = Mathf.Max(repel - repelIncrement, -maxRepel);
        }
        
        Debug.DrawLine(leftOffset, leftOffset + (frontOffset - leftOffset).normalized * repelCastDistance);
        if (Physics2D.Raycast(leftOffset, frontOffset - leftOffset, repelCastDistance, wallMask))
        {
            repel = Mathf.Min(repel + repelIncrement, maxRepel);
        }
        
        repel = repel * (1 - rapelDecay); // Mathf.Abs(repel) < .01f ? 0 : repel * .8f;


        if (da > 90)
        {
            reverseTime = .25f;
        }

        var reverse = false;
        if (reverseTime > 0)
        {
            reverse = true;
            reverseTime -= Time.deltaTime;
            animator.SetFloat("Speed", 0);
            return;
        }

        //var reverse = da > 60;
        //if (reverse)
        //    return;

        var moveDirection = container.rotation * new Vector3(repel, (1 - Mathf.Abs(repel)) * (reverse ? -1 : 1));
        
        // "forward"
        Debug.DrawLine(transform.position, transform.position + container.up * 3);
        Debug.DrawLine(transform.position, transform.position + moveDirection.normalized * 3, Color.yellow);

        if (animator)
        {
            animator.SetFloat("Slide", repel);
        }

        transform.Translate(moveDirection * moveSpeed * speedFactor * .1f * Time.deltaTime, Space.Self);
        animator.SetFloat("Speed", (1 + speedFactor) * .5f, .5f, .5f);
    }


    IList<Vector2> GetPathTo(Transform target)
    {
        nodePath = grid.GetFringePath(gameObject, target.gameObject).Cast<Node>().ToList();
        if (!nodePath.Any()) return null; 

        // compress path
        var path = new List<Vector2>();

        if (nodePath.Count() < 2)
        {
            return path;
        }

        var previousNode = nodePath.First();
        var previousAngle = float.MaxValue;

        foreach (var node in nodePath.Skip(1))
        {
            var distance = node.position - previousNode.position;
            var pseudoAngle = FastMath.PseudoAtan2(distance.y, distance.x);

            if (FastMath.Approximately(previousAngle, pseudoAngle))
            {
                previousNode = node;
                continue;
            }

            path.Add(previousNode.position);

            previousNode = node;
            previousAngle = pseudoAngle;
        }

        path.Add(nodePath.Last().position);

        return path;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (nodePath != null)
        {
            Handles.color = Color.red;
            foreach (Node node in nodePath)
            {
                Handles.DrawWireDisc(node.position, Vector3.back, grid.nodeRadius/2.0f);
            }
        }
    }
#endif
}
