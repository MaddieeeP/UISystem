using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]

public class Layout : MonoBehaviour, IUIElement
{
    [SerializeField] protected Dimension _alignBy = Dimension.y;
    [SerializeField] protected bool _invertX = false;
    [SerializeField] protected bool _invertY = true;
    [SerializeField] protected XAlign _alignX = XAlign.Center;
    [SerializeField] protected YAlign _alignY = YAlign.Center;
    [SerializeField] protected bool _wrap = true;
    [SerializeField] protected Vector2 _padding = new Vector2(50f, 50f);
    [SerializeField] protected List<GameObject> _ignoreObjects = new List<GameObject>();
    protected List<RectTransform> _layoutElements = new List<RectTransform>();
    protected Vector2 _size = Vector2.zero;

    //getters and setters
    public Vector2 size { get { return _size; } }
    public Vector2 align { get { return new Vector2((int)_alignX, (int)_alignY); } }
    public Vector2 invert { get { return new Vector2(_invertX ? -1 : 1, _invertY ? -1 : 1); } }

    protected virtual void OnValidate()
    {
        Apply();
    }

    protected virtual void OnEnable()
    {
        Apply();
    }

    public void Initialize()
    {
        _layoutElements = new List<RectTransform>();
        foreach (Transform child in transform)
        {
            if (child.GetComponent<RectTransform>() != null)
            {
                _layoutElements.Add(child.GetComponent<RectTransform>());
            }
        }
    }

    public virtual void Apply()
    {
        Initialize();

        List<Vector3> positions = GetPositions();

        for (int i = 0; i < _layoutElements.Count; i++)
        {
            _layoutElements[i].transform.localPosition = positions[i];
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

        int dimension = (int)_alignBy;

        float _wrapAfter = float.MaxValue;
        if (_wrap)
        {
            _wrapAfter = bounds.size[dimension];
        }

        float runningOffset = 0f;
        List<List<RectTransform>> layoutGrouping = new List<List<RectTransform>>() { new List<RectTransform>() };
        List<Vector2> groupingMaxSize = new List<Vector2>() { Vector2.zero };
        List<Vector3> positions = new List<Vector3>();

        //Loop through elements to create _wrap groups
        for (int i = 0; i < _layoutElements.Count; i++)
        {
            Bounds elementBounds = _layoutElements[i].GetBoundsWithChildren(_ignoreObjects, transform.rotation);
            runningOffset += elementBounds.size[dimension] + _padding[dimension];

            if (runningOffset > _wrapAfter + _padding[dimension]) //larger than _wrap limit
            {
                if (runningOffset == elementBounds.size[dimension] + _padding[dimension]) //Do not _wrap because there is only one element in group
                {
                    layoutGrouping.Last().Add(_layoutElements[i]);
                    groupingMaxSize[groupingMaxSize.Count - 1] = groupingMaxSize[groupingMaxSize.Count - 1].ChangeValue((1 - dimension), Math.Max(groupingMaxSize.Last()[1 - dimension], elementBounds.size[1 - dimension]));
                    groupingMaxSize[groupingMaxSize.Count - 1] = groupingMaxSize[groupingMaxSize.Count - 1].ChangeValue(dimension, runningOffset - _padding[dimension]);
                    runningOffset = 0f;
                    layoutGrouping.Add(new List<RectTransform>());
                    groupingMaxSize.Add(Vector2.zero);
                    continue;
                }
                //Wrap and place element in new group
                runningOffset = elementBounds.size[dimension] + _padding[dimension];
                layoutGrouping.Add(new List<RectTransform>() { _layoutElements[i] });
                groupingMaxSize.Add(Vector2.zero);
                groupingMaxSize[groupingMaxSize.Count - 1] = groupingMaxSize[groupingMaxSize.Count - 1].ChangeValue((1 - dimension), Math.Max(groupingMaxSize.Last()[1 - dimension], elementBounds.size[1 - dimension]));
                groupingMaxSize[groupingMaxSize.Count - 1] = groupingMaxSize[groupingMaxSize.Count - 1].ChangeValue(dimension, runningOffset - _padding[dimension]);
                continue;
            }
            //smaller than _wrap limit
            layoutGrouping.Last().Add(_layoutElements[i]);
            groupingMaxSize[groupingMaxSize.Count - 1] = groupingMaxSize[groupingMaxSize.Count - 1].ChangeValue((1 - dimension), Math.Max(groupingMaxSize.Last()[1 - dimension], elementBounds.size[1 - dimension]));
            groupingMaxSize[groupingMaxSize.Count - 1] = groupingMaxSize[groupingMaxSize.Count - 1].ChangeValue(dimension, runningOffset - _padding[dimension]);
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
                Bounds elementBounds = element.GetBoundsWithChildren(_ignoreObjects, transform.rotation);
                min = Vector2.zero;
                max = Vector2.zero;
                min = min.ChangeValue(dimension, Math.Min(invert[dimension] * runningOffset, invert[dimension] * (runningOffset + elementBounds.size[dimension])));
                min = min.ChangeValue((1 - dimension), Math.Min(invert[1 - dimension] * groupOffset, invert[1 - dimension] * (groupOffset + groupingMaxSize[i][1 - dimension])));
                max = max.ChangeValue(dimension, Math.Max(invert[dimension] * runningOffset, invert[dimension] * (runningOffset + elementBounds.size[dimension])));
                max = max.ChangeValue((1 - dimension), Math.Max(invert[1 - dimension] * groupOffset, invert[1 - dimension] * (groupOffset + groupingMaxSize[i][1 - dimension])));

                Vector3 position = CalculatePositionWithinLimits(elementBounds, dimension, min, max);
                positions.Add(position);
                runningOffset += elementBounds.size[dimension] + _padding[dimension];
            }
            groupOffset += groupingMaxSize[i][1 - dimension] + _padding[1 - dimension];
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