using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCursor : MonoBehaviour
{
    public static WorldCursor Instance;
    static List<GameObject> hoveredObjs = new List<GameObject>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    void Update()
    {
        Vector3 mouse = Input.mousePosition;

        bool enabled = GetComponent<Collider>().enabled;
        GetComponent<Collider>().enabled = false;

        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
        {
            transform.position = hit.point;
        }

        GetComponent<Collider>().enabled = enabled;

        foreach (GameObject g in GetHoveredObjects())
        {
            Debug.Log(g.name);
        }

        foreach (Transform t in GetComponentsInHoveredObjects<Transform>())
        {
            Debug.Log(t.name);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        hoveredObjs.Add(col.gameObject);
    }

    void OnCollisionExit(Collision col)
    {
        hoveredObjs.Remove(col.gameObject);
    }

    public static List<GameObject> GetHoveredObjects()
    {
        return hoveredObjs;
    }

    public static List<T> GetComponentsInHoveredObjects<T>()
    {
        List<T> list = new List<T>();

        foreach (GameObject gameObject in GetHoveredObjects())
        {
            T component = gameObject.GetComponent<T>();

            if (component != null)
            {
                list.Add(component);
            }
        }
        return list;
    }
}