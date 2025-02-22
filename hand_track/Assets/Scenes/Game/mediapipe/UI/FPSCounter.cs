using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{

    public static FPSCounter instance;

    [Tooltip("Reference to the TextMeshProUGUI component for displaying FPS.")]
    public TextMeshProUGUI fpsText;

    [Tooltip("Color when FPS is high (good performance).")]
    public Color goodColor = Color.green;

    [Tooltip("Color when FPS is moderate (average performance).")]
    public Color warningColor = Color.yellow;

    [Tooltip("Color when FPS is low (poor performance).")]
    public Color badColor = Color.red;

    private int _frameCount = 0;
    private float _dt = 0f;
    private int _fps = 0;

    private void Awake()
    {
        instance = this;
    }

    public void frameRun()
    {
        _frameCount++;
    }

    void Update()
    {
        _dt += Time.deltaTime;

        if (_dt >= 1.0f)
        {
            _fps = _frameCount;
            _frameCount = 0;
            _dt = 0f;

            fpsText.text = "FPS: " + _fps.ToString();

            if (_fps >= 30)
                fpsText.color = goodColor;
            else if (_fps >= 15)
                fpsText.color = warningColor;
            else
                fpsText.color = badColor;
        }
    }
}
