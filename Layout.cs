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
    [SerializeField] private bool alignByY = true;
    [SerializeField] private bool alignToTop = true;
    [SerializeField] private bool alignToLeft = false;
    [SerializeField] private bool wrap = true;
    [SerializeField] private int paddingY = 50;
    [SerializeField] private int paddingX = 50;

    List<RectTransform> layoutElements = new List<RectTransform>();

    public void UpdateLayoutElements()
    {
        layoutElements = GetUIChildren(transform);
    }

    void OnValidate()
    { 
        UpdateLayoutElements();
        Apply();
    }

    void Start()
    {
        Apply();
    }

    public void Apply()
    {
        RectTransform rect = GetComponent<RectTransform>();

        int dimension = 0;
        if (alignByY)
        {
            dimension = 1;
        }

        float wrapAfter = float.MaxValue;
        if (wrap)
        {
            wrapAfter = rect.sizeDelta[dimension];
        }
        Vector2 padding = new Vector2(paddingX, paddingY);

        List<Vector2> positions = GetPositions(dimension, wrapAfter, padding);

        for (int i = 0; i < layoutElements.Count; i++)
        {
            layoutElements[i].position = positions[i];
        }
    }

    public List<Vector2> GetPositions(int dimension, float wrapAfter, Vector2 padding)
    {
        float runningOffset = 0f;
        List<List<RectTransform>> layoutGrouping = new List<List<RectTransform>>() { new List<RectTransform>() };
        List<float> groupingMaxSize = new List<float>() { 0f };
        List<Vector2> positions = new List<Vector2>();

        for (int i = 0; i < layoutElements.Count; i++)
        {
            List<Vector2> bounds = GetBoundsOfElementAndUIChildren(layoutElements[i]);

            runningOffset += bounds[1][dimension] - bounds[0][dimension];

            if (i != 0 && runningOffset > wrapAfter)
            {
                runningOffset = 0f;
                layoutGrouping.Add(new List<RectTransform>());
                groupingMaxSize.Add(0f);
            } else
            {
                runningOffset += padding[dimension];
                groupingMaxSize[groupingMaxSize.Count - 1] = Math.Max(groupingMaxSize.Last(), bounds[1][1 - dimension] - bounds[0][1 - dimension]);
            }

            layoutGrouping.Last().Add(layoutElements[i]);
        }
        groupingMaxSize.Add(0f);

        float groupOffset = 0f;
        for (int i = 0; i < layoutGrouping.Count; i++) //FIX - ALIGN TO 1 - dimension BY CALCULATING GROUP SIZES
        {
            runningOffset = 0f;
            foreach (RectTransform element in layoutGrouping[i]) 
            {
                List<Vector2> bounds = GetBoundsOfElementAndUIChildren(element);
                Vector2 position = CalculatePosition(runningOffset, bounds, dimension);
                position[1 - dimension] += groupOffset;
                positions.Add(position);
                runningOffset += bounds[1][dimension] - bounds[0][dimension] + padding[dimension];
            }
            groupOffset += groupingMaxSize[i] + padding[1 - dimension];
        }

        return positions;
    }

    Vector2 CalculatePosition(float runningOffset, List<Vector2> bounds, int dimension) //FIX - REFACTOR
    {
        float width = bounds[1].x - bounds[0].x;
        float height = bounds[1].y - bounds[0].y;
        Vector2 position = Vector2.zero;

        if (dimension == 1)
        {
            position.y = -1f * runningOffset - bounds[1].y;
            if (alignToLeft)
            {
                position.x = -1f * bounds[0].x; //Align min x with 0
            } else
            {
                position.x = -width / 2f - bounds[0].x; //Align center with 0
            }
        } else
        {
            position.x = runningOffset - bounds[0].x;
            if (alignToTop)
            {
                position.y = -1f * bounds[1].y; //Align max y with 0
            } else
            {
                position.y = height / 2f - bounds[1].y; //Align center with 0
            }
        }

        return position;
    }

    List<RectTransform> GetUIChildren(Transform transform)
    {
        List<RectTransform> UIChildren = new List<RectTransform>();
        foreach (Transform child in transform)
        {
            if (child.GetComponent<RectTransform>() != null)
            {
                UIChildren.Add(child.GetComponent<RectTransform>());
            }
        }

        return UIChildren;

    }
    List<Vector2> GetBoundsOfElementAndUIChildren(RectTransform element) //FIX - pivot? - rotation, etc. Hide certain elements
    {
        float minX = -0.5f * element.sizeDelta.x * element.localScale.x;
        float minY = -0.5f * element.sizeDelta.y * element.localScale.y;
        float maxX = 0.5f * element.sizeDelta.x * element.localScale.x;
        float maxY = 0.5f * element.sizeDelta.y * element.localScale.y;

        foreach (RectTransform child in GetUIChildren(element))
        {
            Vector2 offset = child.position - element.position;
            minX = Math.Min(minX, offset.x - (1 - element.pivot.x) * child.sizeDelta.x * child.localScale.x);
            minY = Math.Min(minY, offset.y - (1 - element.pivot.y) * child.sizeDelta.y * child.localScale.y);
            maxX = Math.Max(maxX, offset.x + element.pivot.x * child.sizeDelta.x * child.localScale.x);
            maxY = Math.Max(maxY, offset.y + element.pivot.y * child.sizeDelta.y * child.localScale.y);
        }

        return new List<Vector2> { new Vector2(minX, minY), new Vector2(maxX, maxY) };
    }
}