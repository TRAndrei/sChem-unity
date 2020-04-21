using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Init : MonoBehaviour
{
    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public GameObject elementPrefab;
    public GameObject wallPrefab;
    private float wallWidth = 0.2f;

    // This script will simply instantiate the Prefab when the game starts.
    void Start()
    {
        Level level = getLevel("level1.json");

        LoadLevel(level);
    }

    // Update is called once per frame
    void Update()
    {
        WorldManager.Instance.UpdateAll();
    }

    void makeWalls(int halfSize)
    {
        Vector3 widthScale = new Vector3(2 * halfSize / wallWidth, 1, 1);
        Vector3 heightScale = new Vector3(1, 2 * halfSize / wallWidth, 1);
        GameObject topWall = Instantiate(wallPrefab, new Vector3(0, -halfSize, 0), Quaternion.identity);
        topWall.transform.localScale = widthScale;
        GameObject bottomWall = Instantiate(wallPrefab, new Vector3(0, halfSize, 0), Quaternion.identity);
        bottomWall.transform.localScale = widthScale;
        GameObject leftWall = Instantiate(wallPrefab, new Vector3(-halfSize, 0, 0), Quaternion.identity);
        leftWall.transform.localScale = heightScale;
        GameObject rightWall = Instantiate(wallPrefab, new Vector3(halfSize, 0, 0), Quaternion.identity);
        rightWall.transform.localScale = heightScale;
    }

    public Level getLevel(string name)
    {
        string fileName = Path.Combine(Application.dataPath, "Levels", name);

        using (StreamReader r = new StreamReader(fileName))
        {
            string json = r.ReadToEnd();
            return JsonUtility.FromJson<Level>(json);
        }
    }

    public void LoadLevel(Level level)
    {
        WorldManager.Instance.Init();

        makeWalls(level.size);

        foreach (var item in level.randomElements)
        {
            for (int i = 0; i < item.count; i++)
            {
                Vector3 position = new Vector3(Random.Range(-level.size, level.size), Random.Range(-level.size, level.size), 0);
                GameObject element = Instantiate(elementPrefab, position, Quaternion.identity);
                element.name = item.type + " " + i.ToString();
                WorldManager.Instance.AddElement(element.GetComponent<ElementScript>());
            }            
        }

        foreach (Level.Elements e in level.elements)
        {
            Vector3 position = new Vector3(e.x, e.y, 0);
            GameObject element = Instantiate(elementPrefab, position, Quaternion.identity);
            element.name = e.id;
            ElementScript elementScript = element.GetComponent<ElementScript>();
            elementScript.Type = e.type;

            WorldManager.Instance.AddElement(elementScript);
        }

        foreach (Rule r in level.rules)
        {
            WorldManager.Instance.AddRule(r);
        }

        foreach (var l in level.links)
        {
            WorldManager.Instance.AddLink(l.id, l.first, l.second);
        }
    }
}
