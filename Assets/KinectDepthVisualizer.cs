using UnityEngine;
using Windows.Kinect;

public class KinectDepthVisualizer : MonoBehaviour
{
    // Kinect
    private KinectSensor sensor;
    private DepthFrameReader reader;

    // Data
    private ushort[] depthData;
    private Color32[] colorBuffer;

    // Rendering
    private Texture2D depthTexture;
    private MaterialPropertyBlock mpb;

    private int width;
    private int height;

    [SerializeField] private Renderer targetRenderer;

    // Distance thresholds (mm)
    [SerializeField] private int roi1Min = 1000;
    [SerializeField] private int roi1Max = 1500;
    [SerializeField] private int roi2Min = 1500;
    [SerializeField] private int roi2Max = 2000;

    void Start()
    {
        InitializeKinect();
        InitializeTexture();
        mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (reader == null)
            return;

        ProcessDepthFrame();
        UpdateRenderer();
    }

    // ---------- Initialization ----------

    // Initializes Kinect sensor and opens the depth stream
    private void InitializeKinect()
    {
        sensor = KinectSensor.GetDefault();

        if (sensor == null)
        {
            Debug.LogError("No Kinect sensor found");
            return;
        }

        reader = sensor.DepthFrameSource.OpenReader();

        if (!sensor.IsOpen)
            sensor.Open();

        Debug.Log("Kinect sensor opened, depth reader ready");
    }

    private void InitializeTexture()
    {
        width = sensor.DepthFrameSource.FrameDescription.Width;
        height = sensor.DepthFrameSource.FrameDescription.Height;

        depthTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        colorBuffer = new Color32[width * height];
    }

    // ---------- Frame processing ----------

    // Processes a single depth frame from the sensor
    private void ProcessDepthFrame()
    {
        // Kinect SDK uses unmanaged memory; DepthFrame must be disposed explicitly
        using (var frame = reader.AcquireLatestFrame())
        {
            if (frame == null)
                return;

            EnsureDepthBuffer(frame);
            frame.CopyFrameDataToArray(depthData);

            ConvertDepthToColors();
            ApplyTexture();
        }
    }

    private void EnsureDepthBuffer(DepthFrame frame)
    {
        if (depthData != null)
            return;

        int size = (int)frame.FrameDescription.LengthInPixels;
        depthData = new ushort[size];

        Debug.Log("Depth buffer size: " + size);
    }

    // Converts raw depth values (mm) into color-coded regions
    private void ConvertDepthToColors()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                
                int src = y * width + x; // kinect: top-left origin
                int dst = (height - y - 1) * width + x; // unity: bottom-left origin

                ushort depth = depthData[src];
                colorBuffer[dst] = ClassifyDepth(depth);
            }
        }
    }

    private void ApplyTexture()
    {
        depthTexture.SetPixels32(colorBuffer);
        depthTexture.Apply();
    }

    private void UpdateRenderer()
    {
        mpb.SetTexture("_MainTex", depthTexture);
        targetRenderer.SetPropertyBlock(mpb);
    }

    // ---------- Helpers ----------

    // Depth values are in millimeters
    private Color32 ClassifyDepth(ushort depth)
    {
        if (depth == 0)
            return Color.black;

        if (depth >= roi1Min && depth < roi1Max)
            return Color.red;

        if (depth >= roi2Min && depth < roi2Max)
            return Color.green;

        return Color.black;
    }

    void OnDestroy()
    {
        if (reader != null)
        {
            reader.Dispose();
            reader = null;
        }

        if (sensor != null && sensor.IsOpen)
        {
            sensor.Close();
            sensor = null;
        }
    }
}
