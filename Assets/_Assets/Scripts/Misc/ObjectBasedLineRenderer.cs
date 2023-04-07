using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectBasedLineRenderer : MonoBehaviour
{
    [SerializeField] private Transform[] transforms;

    private int numPoints;
    private LineRenderer lineRend;
    private Vector3[] worldPositions;

    void Awake()
    {
        lineRend = GetComponent<LineRenderer>();
        numPoints = transforms.Length;
        lineRend.positionCount = numPoints;
        worldPositions = new Vector3[numPoints];
    }


    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numPoints; i++)
        {
            worldPositions[i] = transforms[i].position;
        }

        lineRend.SetPositions(worldPositions);
    }
}
