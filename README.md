# Motion_Capture

[Notion Link](https://paint-chinchilla-3cb.notion.site/Motion-Capture-63f430d4f2af41d283428167c30c5a70)

## Advantages of Using a Webcam for Motion Capture:

1. **Cost-Effective**:
    - **Webcams** are inexpensive, often costing a fraction of professional motion capture equipment.
    - This makes them accessible for hobbyists, indie developers, or small projects with limited budgets.
2. **Accessibility and Ease of Use**:
    - Setting up a webcam for motion capture is simple and doesn't require complex hardware or software integration.
    - You can start capturing motion with minimal technical knowledge, while professional mocap systems may require expertise and extensive calibration.
3. **Portability**:
    - Webcams are lightweight and portable, making them ideal for quick, mobile, or on-the-go motion capture setups.
    - Expensive mocap systems are usually large and fixed, requiring a dedicated space for proper usage.
4. **No Specialized Environment Required**:
    - Webcams can work in a variety of environments and lighting conditions, though results may vary.
    - High-end mocap systems often need controlled lighting, reflective markers, and a clear, large capture space to function optimally.
5. **Software Flexibility**:
    - Many modern AI-based software solutions (like OpenPose or MediaPipe) allow motion capture using basic RGB video inputs from webcams.
    - This eliminates the need for multiple infrared cameras, markers, or suits, offering simpler alternatives for basic motion tracking.

## Trade-offs:

- **Accuracy**: Webcams lack the precision and detailed tracking provided by expensive systems. They are generally limited in terms of capturing fine movements (e.g., finger or subtle facial motions).
- **Latency and Frame Rate**: Higher-end systems offer higher frame rates and lower latency, which is important for real-time applications.
- **Depth Perception**: Webcams lack depth sensors, unlike advanced mocap cameras that use infrared or other techniques to capture 3D motion data accurately.

## Development Environment

Pycharm (2024.2.1)

Unity (2022.3.24f1)

## Python Library

CVZone 1.6.1 (Using OpenCV, MediaPipe for pose detection, hand tracking, facial capture)
