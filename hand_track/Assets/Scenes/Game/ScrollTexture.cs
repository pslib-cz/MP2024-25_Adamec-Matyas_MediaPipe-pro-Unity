using UnityEngine;


public class ScrollTexture : MonoBehaviour
{
    public float scrollSpeedY = -0.3f;

    private Renderer rend;
    private Vector2 offset;

    void Start()
    {
        rend = GetComponent<Renderer>();
        offset = rend.material.mainTextureOffset;
    }

    void Update()
    {
        if (LevelLoader.instance.isPaused)
        {
            return;
        }

        offset.y += scrollSpeedY * Time.deltaTime;
        rend.material.mainTextureOffset = offset;
    }
}
