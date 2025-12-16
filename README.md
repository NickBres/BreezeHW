# Kinect Depth Segmentation – Unity

This project demonstrates real-time depth data processing using a Kinect v2 sensor (or Kinect Studio playback) integrated into Unity.

The application reads depth frames from the sensor, segments them into distance-based regions, and renders the result as a color-coded texture.

## Functionality

- Reads depth frames from Kinect in real time
- Processes raw depth values (millimeters)
- Segments the image into three regions:
  - **Red**: 1.0m – 1.5m
  - **Green**: 1.5m – 2.0m
  - **Black**: background (≤1.0m or >2.0m)
- Displays the processed depth image in Unity using a texture

Distance thresholds can be adjusted via the Unity Inspector.

## Requirements

- Windows
- Unity 6.x
- Kinect v2 SDK
- Kinect Unity package (provided with the assignment)
- Optional: Kinect Studio for depth playback (.xef files)

## How to Run

1. Open the project in Unity.
2. Make sure the Kinect SDK and Unity package are installed.
3. Assign a Renderer (e.g., Quad) to the `DepthBootstrap` script.
4. Press Play.
5. If using Kinect Studio, start playback of a recorded depth stream.


