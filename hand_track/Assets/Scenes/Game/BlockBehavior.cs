using Intel.RealSense;
using PathCreation;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlockBehavior : MonoBehaviour
{
    public ParticleSystem destroyEffectR;
    public ParticleSystem destroyEffectL;
    public ParticleSystem destroyEffectLong;
    public AudioClip destroySound;     
    public AudioClip destroySoundLong;     
    public Material blue;
    private AudioSource audioSource;
    public bool isRight = false;
    public int score = 0;

    public float yOffset = 0f;

    public float moveSpeed = 1f;
    public Long longData = null;
    public float HoldPositionz = 1f;

    private string LeftCollider = "JointL";
    private string RightCollider = "JointR";




    Vector3[] points;
    private void Start()
    {
        if (longData != null && longData.stops.Length > 1)
        {
            points = new Vector3[longData.stops.Length];
            for (int i = 0; i < longData.stops.Length; i++)
            {
                points[i] = new Vector3(longData.stops[i].x, longData.stops[i].y, moveSpeed * longData.stops[i].time);
            }
            PathGenerator.instance.GeneratePath(points, yOffset);
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        if (isRight)
        {
            MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
            meshRenderer.material = blue; 
        }
    }

    bool isHolding = false;

    private void OnTriggerEnter(Collider other)
    {
        if (longData != null) return;

        if (other.CompareTag(RightCollider))
        {
            if (isRight)
            {
                LevelLoader.instance.addScore(+score);
            }
            else
            {
                LevelLoader.instance.addScore(-score);
            }
        }
        else if (other.CompareTag(LeftCollider))
        {
            if (isRight)
            {
                LevelLoader.instance.addScore(-score);
            }
            else
            {
                LevelLoader.instance.addScore(+score);
            }
        }


        DestroyBlock();
    }

    private void DestroyBlock()
    {
        var effectvariation = isRight ? destroyEffectR : destroyEffectL;
        var effect = Instantiate(effectvariation, transform.position, Quaternion.identity);
        Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
        LevelLoader.instance.playSound(destroySound);

        Destroy(gameObject);
    }

    private void ParishBlock()
    {
        var effect = Instantiate(destroyEffectLong, transform.position, Quaternion.identity);
        Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
        LevelLoader.instance.playSound(destroySoundLong);
        PathGenerator.instance.DeletePath();
        Destroy(gameObject);
    }



    public float checkRadius = 10f; 
    public LayerMask detectionLayer;

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, checkRadius, detectionLayer);
        Debug.DrawLine(transform.position, transform.position + Vector3.up * checkRadius, Color.red);

        isHolding = colliders.Length > 1;

        if (LevelLoader.instance.isPaused) return;
        if (longData != null)
        {
            if (HoldPositionz > transform.position.z && !(currentStopIndex >= longData.stops.Length - 1))
            {
                FollowPath();
                if (!isHolding)
                {
                    ParishBlock();
                }
            }
            else
            {
                transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
                var thisPositionz = transform.position.z;
                PathGenerator.instance.MovePath(thisPositionz);
                if (isHolding && currentStopIndex >= longData.stops.Length - 1)
                {
                    DestroyBlock();
                    PathGenerator.instance.DeletePath();
                }

            }
        }
        else
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }


        if (transform.position.z < -10)
        {
            Destroy(gameObject);
        }
    }

    private float elapsedTime = 0f;
    private float blockElapsedTime = 0f;
    private int currentStopIndex = 0;

    public void ActivateLong()
    {
        elapsedTime = 0f;
        currentStopIndex = 0;
    }
    
    private void FollowPath()
    {
        if (currentStopIndex >= longData.stops.Length - 1) return;

        PathGenerator.instance.AdjustPath(transform, currentStopIndex, elapsedTime);

        Stop currentStop = longData.stops[currentStopIndex];
        Stop nextStop = longData.stops[currentStopIndex + 1];

        float segmentTime = nextStop.time - currentStop.time;
        if (segmentTime <= 0f) return;

        elapsedTime += Time.deltaTime;
        blockElapsedTime += Time.deltaTime;

        float t = Mathf.Clamp01(blockElapsedTime / segmentTime);

        Vector3 newPosition = Vector3.Lerp(
            new Vector3(currentStop.x * 1.8f, currentStop.y + yOffset, transform.position.z),
            new Vector3(nextStop.x * 1.8f, nextStop.y + yOffset, transform.position.z),
            t
        );

        transform.position = newPosition;

        if (elapsedTime > nextStop.time)
        {
            currentStopIndex++;
            blockElapsedTime = 0f;
        }
    }
}