using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Init : MonoBehaviour
{
    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public GameObject myPrefab;
    private Rect world = new Rect(-50, -50, 100, 100);
    private int elementCount = 1000;

    // This script will simply instantiate the Prefab when the game starts.
    void Start()
    {
        for (int i = 0; i < elementCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(world.x, world.x + world.width), Random.Range(world.y, world.y + world.height), 0);
            GameObject element = Instantiate(myPrefab, position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
