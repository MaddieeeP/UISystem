using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]

public class Layout : MonoBehaviour
{
    [SerializeField] private Dimension alignBy = Dimension.y;
    [SerializeField] private bool invertX = false;
    [SerializeField] private bool invertY = true;
    [SerializeField] private XAlign alignX = XAlign.Center;
    [SerializeField] private YAlign alignY = YAlign.Center;
    [SerializeField] private bool wrap = true;
    [SerializeField] private Vector2 padding = new Vector2(50f, 50f);
    public List<GameObject> ignoreObjects = new List<GameObject>();

    private Vector2 invert 
    { 
        get
        {
            Vector2 invertVector = new Vector2(1, 1);
            if (invertX)
            {
                invertVector[0] = -1;
            }
            if (invertY)
            {
                invertVector[1] = -1;
            }
            return invertVector;
        } 
    }
    private Vector2 align
    {
        get
        {
            Vector2 alignVector = new Vector2(0, 0);
            if (alignX == XAlign.Left)
            {
                alignVector[0] = -1;
            } 
            else if (alignX == XAlign.Right)
            {
                alignVector[0] = 1;
            }
            if (alignY == YAlign.Bottom)
            {
                alignVector[1] = -1;
            } 
            else if (alignY == YAlign.Top)
            {
                alignVector[1] = 1;
            }
            return alignVector;
        }
    }

    private Vector2 _size = Vector2.zero;
    public Vector2 size { get { return _size; } }

    List<RectTransform> layoutElements = new List<RectTransform>();

    void OnValidate()
    {
        Apply();
    }

    void OnEnable()
    {
        Apply();
    }

    void UpdateLayoutElements()
    {
        layoutElements = new List<RectTransform>();
        foreach (Transform child in transform)
        {
            if (child.GetComponent<RectTransform>() != null)
            {
                layoutElements.Add(child.GetComponent<RectTransform>());
            }
        }
    }

    public void Apply()
    {
        UpdateLayoutElements();

        List<Vector3> positions = GetPositions();

        for (int i = 0; i < layoutElements.Count; i++)
        {
            layoutElements[i].transform.localPosition = positions[i];
        }
    }

    public List<Vector3> GetPositions()
    {
        RectTransform rect = GetComponent<RectTransform>();

        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.negativeInfinity;
        Vector3[] corners = new Vector3[4];

        rect.GetWorldCorners(corners);

        foreach (Vector3 corner in corners)
        {
            min = Vector3.Min(min, corner);
            max = Vector3.Max(max, corner);
        }

        min -= rect.position;
        max -= rect.position;

        Bounds bounds = new Bounds();
        bounds.SetMinMax(min, max);

        int dimension = (int)alignBy;

        float wrapAfter = float.MaxValue;
        if (wrap)
        {
            wrapAfter = bounds.size[dimension];
        }

        float runningOffset = 0f;
        List<List<RectTransform>> layoutGrouping = new List<List<RectTransform>>() { new List<RectTransform>() };
        List<Vector2> groupingMaxSize = new List<Vector2>() { Vector2.zero };
        List<Vector3> positions = new List<Vector3>();

        //Loop through elements to create wrap groups
        for (int i = 0; i < layoutElements.Count; i++)
        {
            Bounds elementBounds = layoutElements[i].GetBoundsWithChildren(ignoreObjects, transform.rotation);
            runningOffset += elementBounds.size[dimension] + padding[dimension];

            if (runningOffset > wrapAfter + padding[dimension]) //larger than wrap limit
            {
                if (runningOffset == elementBounds.size[dimension] + padding[dimension]) //Do not wrap because there is only one element in group
                {
                    layoutGrouping.Last().Add(layoutElements[i]);
                    groupingMaxSize[groupingMaxSize.Count - 1] = groupingMaxSize[groupingMaxSize.Count - 1].ChangeValue((1 - dimension), Math.Max(groupingMaxSize.Last()[1 - dimension], elementBounds.size[1 - dimension]));
                    groupingMaxSize[groupingMaxSize.Count - 1] = groupingMaxSize[groupingMaxSize.Count - 1].ChangeValue(dimension, runningOffset - padding[dimension]);
                    runningOffset = 0f;
                    layoutGrouping.Add(new List<RectTransform>());
                    groupingMaxSize.Add(Vector2.zero);
                    continue;
                }
                //Wrap and place element in new group
                runningOffset = elementBounds.size[dimension] + padding[dimension];
                layoutGrouping.Add(new List<RectTransform>() { layoutElements[i] });
                groupingMaxSize.Add(Vector2.zero);
                groupingMaxSize[groupingMaxSize.Count - 1] = groupingMaxSize[groupingMaxSize.Count - 1].ChangeValue((1 - dimension), Math.Max(groupingMaxSize.Last()[1 - dimension], elementBounds.size[1 - dimension]));
                groupingMaxSize[groupingMaxSize.Count - 1] = groupingMaxSize[groupingMaxSize.Count - 1].ChangeValue(dimension, runningOffset - padding[dimension]);
                continue;
            }
            //smaller than wrap limit
            layoutGrouping.Last().Add(layoutElements[i]);
            groupingMaxSize[groupingMaxSize.Count - 1] = groupingMaxSize[groupingMaxSize.Count - 1].ChangeValue((1 - dimension), Math.Max(groupingMaxSize.Last()[1 - dimension], elementBounds.size[1 - dimension]));
            groupingMaxSize[groupingMaxSize.Count - 1] = groupingMaxSize[groupingMaxSize.Count - 1].ChangeValue(dimension, runningOffset - padding[dimension]);
        }

        //Loop through layout groupings to calculate positions
        _size = Vector2.zero;
        foreach (Vector2 groupSize in groupingMaxSize)
        {
            _size = _size.ChangeValue((1 - dimension), _size[1 - dimension] + groupSize[1 - dimension]);
            _size = _size.ChangeValue(dimension, Math.Max(_size[dimension], groupSize[dimension]));
        }

        float groupOffset = 0f;
        if (align[1 - dimension] * invert[1 - dimension] == -1)
        {
            groupOffset = bounds.min[1 - dimension];
        }
        else if (align[1 - dimension] * invert[1 - dimension] == 1)
        {
            groupOffset = bounds.max[1 - dimension] - _size[1 - dimension];
        }
        else
        {
            groupOffset = bounds.center[1 - dimension] - (_size[1 - dimension] / 2);
        }

        for (int i = 0; i < layoutGrouping.Count; i++)
        {
            if (align[dimension] * invert[dimension] == -1)
            {
                runningOffset = bounds.min[dimension];
            }
            else if (align[dimension] * invert[dimension] == 1)
            {
                runningOffset = bounds.max[dimension] - groupingMaxSize[i][dimension];
            }
            else
            {
                runningOffset = bounds.center[dimension] - (groupingMaxSize[i][dimension] / 2);
            }

            foreach (RectTransform element in layoutGrouping[i]) 
            {
                Bounds elementBounds = element.GetBoundsWithChildren(ignoreObjects, transform.rotation);
                min = Vector2.zero;
                max = Vector2.zero;
                min = min.ChangeValue(dimension, Math.Min(invert[dimension] * runningOffset, invert[dimension] * (runningOffset + elementBounds.size[dimension])));
                min = min.ChangeValue((1 - dimension), Math.Min(invert[1 - dimension] * groupOffset, invert[1 - dimension] * (groupOffset + groupingMaxSize[i][1 - dimension])));
                max = max.ChangeValue(dimension, Math.Max(invert[dimension] * runningOffset, invert[dimension] * (runningOffset + elementBounds.size[dimension])));
                max = max.ChangeValue((1 - dimension), Math.Max(invert[1 - dimension] * groupOffset, invert[1 - dimension] * (groupOffset + groupingMaxSize[i][1 - dimension])));

                Vector3 position = CalculatePositionWithinLimits(elementBounds, dimension, min, max);
                positions.Add(position);
                runningOffset += elementBounds.size[dimension] + padding[dimension];
            }
            groupOffset += groupingMaxSize[i][1 - dimension] + padding[1 - dimension];
        }
        return positions;
    }

    Vector3 CalculatePositionWithinLimits(Bounds bounds, int dimension, Vector2 min, Vector2 max)
    {
        Vector3 position = Vector3.zero;

        if (align[dimension] == -1)
        {
            position[dimension] = min[dimension] - bounds.min[dimension];
        }
        else if (align[dimension] == 1)
        {
            position[dimension] = max[dimension] - bounds.max[dimension];
        }
        else
        {
            position[dimension] = (min[dimension] + max[dimension]) / 2 - bounds.center[dimension];
        }

        if (align[1 - dimension] == -1)
        {
            position[1 - dimension] = min[1 - dimension] - bounds.min[1 - dimension];
        }
        else if (align[1 - dimension] == 1)
        {
            position[1 - dimension] = max[1 - dimension] - bounds.max[1 - dimension];
        }
        else
        {
            position[1 - dimension] = (min[1 - dimension] + max[1 - dimension]) / 2 - bounds.center[1 - dimension];
        }

        return position;
    }
}