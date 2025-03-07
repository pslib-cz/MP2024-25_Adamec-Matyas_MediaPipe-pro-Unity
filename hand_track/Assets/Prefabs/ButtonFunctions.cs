using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFunctions : MonoBehaviour 
{

    [SerializeField] public GameObject blockPrefab;
    [SerializeField] public GameObject pickablePrefab;


    public void OnRainPressed()
    {
    
        Debug.Log("Central Button Pressed");

        var numBlocks = 50;
        var y = 5f;
        var xRange = 2f;
        var zRange = 1f;
        var zOffset = 2.7f;
        // spawn the blockPrefab numblocks times with random x and z values within the range of xRange and zRange and a fixed y value
        for (int i = 0; i < numBlocks; i++)
        {
            var x = Random.Range(-xRange, xRange);
            var z = Random.Range(-zRange, zRange) + zOffset;
            var block = Instantiate(blockPrefab, new Vector3(x, y, z), Quaternion.identity);
        }
    }
    public void onNewPickablePressed()
    {
        Instantiate(pickablePrefab, new Vector3(0f, 1.5f, 2.2f), Quaternion.identity);
    }
}
