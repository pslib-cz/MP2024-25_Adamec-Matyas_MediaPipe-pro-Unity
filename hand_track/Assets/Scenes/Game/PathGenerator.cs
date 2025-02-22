using UnityEngine;
using PathCreation;
using Intel.RealSense;
using PathCreation.Examples;

public class PathGenerator : MonoBehaviour
{
    public static PathGenerator instance;
    public PathCreator pathCreator;
    public RoadMeshCreator roadMeshCreator;
    private Vector3[] _points;
    private float _spawnBlockZ = 5f;
    private float[] _pointsZ;

    private void Awake()
    {
        instance = this;
    }

    public void GeneratePath(Vector3[] points, float yOffset)
    {

        _points = points;
        _pointsZ = new float[_points.Length];
        for (int i = 0; i < _points.Length; i++)
        {
            _points[i] = new Vector3(_points[i].x * 1.8f, _points[i].y + yOffset, _points[i].z);
            _pointsZ[i] = _points[i].z;
        }

        Debug.Log("Generating path");
        BezierPath bezierPath = new BezierPath(points, false, PathSpace.xyz);

        pathCreator.bezierPath = bezierPath;

        roadMeshCreator.TriggerUpdate();

    }

    public void MovePath(float blockZ)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, blockZ);
        roadMeshCreator.TriggerUpdate();

    }

    public void AdjustPath(Transform block, int currentIndex, float time)
    {
        for (int i = 0; i < _points.Length; i++)
        {
            if (i <= currentIndex)
            {
                _points[i] = new Vector3(block.position.x, block.position.y, 0.001f*i);
            }
            else
            {
                _points[i] = new Vector3(_points[i].x, _points[i].y, _pointsZ[i] - time*10);
            }
        }   
        
        BezierPath bezierPath = new BezierPath(_points, false, PathSpace.xyz);
        pathCreator.bezierPath = bezierPath;
        roadMeshCreator.TriggerUpdate();

    }

    public void DeletePath()
    {
        transform.position = new Vector3(0, 0, 50);
        roadMeshCreator.TriggerUpdate();
    }
}
