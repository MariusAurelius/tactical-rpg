using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject spawnPoint;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 100; i++)
            {
                Vector3 position = spawnPoint.transform.position;
                Instantiate(cubePrefab, position, Quaternion.identity);
            }
        }
    }
}