using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Init : MonoBehaviour
{
    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public GameObject elementPrefab;
    public GameObject wallPrefab;
    private Rect world = new Rect(-5, -5, 10, 10);
    private int elementCount = 4;
    private float wallWidth = 0.2f;

    // This script will simply instantiate the Prefab when the game starts.
    void Start()
    {
        CollisionManager.Instance.Init();

        CollisionManager.Instance.AddRule(new Rule("R1", true, "E0", "E1", "E0", "E1"));
        CollisionManager.Instance.AddRule(new Rule("R2", true, "E1", "E2", "E1", "E2"));
        CollisionManager.Instance.AddRule(new Rule("R3", false, "E1", "E1", "E2", "E2"));
        CollisionManager.Instance.AddRule(new Rule("R4", true, "E2", "E2", "E2", "E2"));

        makeWalls();

        for (int i = 0; i < elementCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(world.x, world.x + world.width), Random.Range(world.y, world.y + world.height), 0);
            GameObject element = Instantiate(elementPrefab, position, Quaternion.identity);
            element.name = i.ToString();

            CollisionManager.Instance.AddElement(element.GetComponent<ElementScript>());
        }        
    }

    // Update is called once per frame
    void Update()
    {
        CollisionManager.Instance.UpdateAll();
    }

    void makeWalls()
    {
        Vector3 widthScale = new Vector3(world.width / wallWidth, 1, 1);
        Vector3 heightScale = new Vector3(1, world.height / wallWidth, 1);
        GameObject topWall = Instantiate(wallPrefab, new Vector3(0, world.y, 0), Quaternion.identity);
        topWall.transform.localScale = widthScale;
        GameObject bottomWall = Instantiate(wallPrefab, new Vector3(0, world.y + world.height, 0), Quaternion.identity);
        bottomWall.transform.localScale = widthScale;
        GameObject leftWall = Instantiate(wallPrefab, new Vector3(world.x, 0, 0), Quaternion.identity);
        leftWall.transform.localScale = heightScale;
        GameObject rightWall = Instantiate(wallPrefab, new Vector3(world.x + world.width, 0, 0), Quaternion.identity);
        rightWall.transform.localScale = heightScale;
    }
}
