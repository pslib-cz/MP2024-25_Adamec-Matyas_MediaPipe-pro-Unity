using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public static BlockSpawner instance;

    public GameObject blockPrefab;
    public GameObject blocBluekPrefab;
    public float spawnDistance = 10f;
    public float xRange = .5f;
    public float yRange = .5f;
    public float yOffset = 1f;

    private void Awake()
    {
        instance = this;
    }

    public void spawnBlock(float x, float y, bool isRight, int score)
    {
        float spawnX = x * xRange;
        float spawnY = (y + yOffset) * yRange;
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnDistance);

        GameObject block = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
        block.GetComponent<BlockBehavior>().isRight = isRight;
        block.GetComponent<BlockBehavior>().score = score;
        block.GetComponent<BlockBehavior>().longData = null;
        block.GetComponent<BlockBehavior>().yOffset = yOffset;

    }
    public void spawnLongBlock(float x, float y, bool isRight, int score, Long longData)
    {
        if (longData == null)
        {
            spawnBlock(x, y, isRight, score);
            return;
        }

        Debug.Log("Spawning long block");

        float spawnX = x * xRange;
        float spawnY = (y + yOffset) * yRange;
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnDistance);

        GameObject block = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
        block.GetComponent<BlockBehavior>().isRight = isRight;
        block.GetComponent<BlockBehavior>().score = score;
        block.GetComponent<BlockBehavior>().longData = longData;
        block.GetComponent<BlockBehavior>().yOffset = yOffset;
    }

    public void deleteAllBlocks()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject block in blocks)
        {
            Destroy(block);
        }
    }

}
