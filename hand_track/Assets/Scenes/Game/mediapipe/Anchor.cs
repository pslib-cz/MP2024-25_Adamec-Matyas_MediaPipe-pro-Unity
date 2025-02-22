using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour
{

    [SerializeField] public GameObject CharucoBoardPrefab;
    [SerializeField] public int SquaresX = 5;
    [SerializeField] public int SquaresY = 7;

    [SerializeField] public float SquareLength = 0.037f;
    [SerializeField] public float MarkerLength = 0.025f;
    [SerializeField] public int DictionaryId = 10;
    
    
    [SerializeField] public float AnchorX = 0;
    [SerializeField] public float AnchorY = 0;
    [SerializeField] public float AnchorZ = 0;

    [SerializeField] public float RotationX = 0;
    [SerializeField] public float RotationY = 0;
    [SerializeField] public float RotationZ = 0;


    [SerializeField] public float ProjectorOffsetX = 0;
    [SerializeField] public float ProjectorOffsetY = 0;
    [SerializeField] public float ProjectorOffsetZ = 0;

    [SerializeField] public float ProjectorRotationX = 0;
    [SerializeField] public float ProjectorRotationY = 0;
    [SerializeField] public float ProjectorRotationZ = 0;


    private (float x, float y, float z) _position = (0, 0, 0);
    private (float x, float y, float z) _rotation = (0, 0, 0);

    void Start()
    {
        _position.x = _position.x - ProjectorOffsetX;
        _position.y = _position.y - ProjectorOffsetY;
        _position.z = _position.z - ProjectorOffsetZ;

        _rotation.x = _rotation.x - ProjectorRotationX;
        _rotation.y = _rotation.y - ProjectorRotationY;
        _rotation.z = _rotation.z - ProjectorRotationZ;

        transform.position = new Vector3(_position.x, _position.y, _position.z);
        transform.rotation = Quaternion.Euler(_rotation.x, _rotation.y, _rotation.z);

    }

    void Update()
    {
        
    }
}
