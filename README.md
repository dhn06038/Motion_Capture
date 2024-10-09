# Motion_Capture

 This project demonstrates a cost-effective motion capture system using a webcam and Python libraries. It showcases the implementation of hand tracking, pose estimation, and facial landmark detection, with real-time visualization and avatar animation in Unity.

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

# Implementation Highlights

## 1. Hand Tracking

 **Hand tracking** is achieved using MediaPipe, which tracks 21 landmark points of the hand in real-time. In this project, the hand tracking system not only tracks hand movements but also allows interaction with objects in the Unity scene.

 <img src="https://github.com/user-attachments/assets/c2ca677f-bccf-45ad-b2e5-59ebe683b0f8" width="80%" height="45%"/>

## 2. Pose Estimation
 **Pose estimation** imports full-body pose data from MediaPipe and tracks 33 landmark points of a person's body. This system records a person’s path in Unity, which can be used for gameplay or performance analysis. The recorded data can visualize the user’s movement in 3D space, allowing applications like real-time navigation or fitness tracking.

 <img src="https://github.com/user-attachments/assets/354b627a-f5f6-4e3e-86e5-1262f4aecdfc" width="80%" height="45%"/>

## 3. Facial Landmark Detection
 **Facial landmark detection** reconstructs the 468 landmark points of the face in 3D space. In this project, facial data is used to interact with Unity objects based on facial movements. For instance, the distance between the mouth is calculated and used to control the rotation of a cube, making the system responsive to facial expressions. This approach opens up possibilities for interactive facial-driven interfaces in virtual environments.

 <img src="https://github.com/user-attachments/assets/9b12e653-613f-4d69-8244-dcedc29a2f5f" width="80%" height="45%"/>

## 4. Avatar Animation Using MediaPipe Pose Data
  The avatar animation system utilizes MediaPipe's pose data to animate a 3D character model in Unity. The pose data, which includes body, hand, and face landmarks, is mapped onto the avatar’s skeleton, allowing for real-time animation. This feature is ideal for creating interactive virtual characters or for use in virtual reality (VR) environments where users' movements are mirrored onto the avatar.

  <img src="https://github.com/user-attachments/assets/4b89fc99-aef1-41e1-9371-4123a305422b" width="80%" height="45%"/>

## Development Environment

Pycharm (2024.2.1)

Unity (2022.3.24f1)

## Python Library

OpenCV, MediaPipe for pose detection, hand tracking, facial capture

## Future Improvements

- It will be modified to receive motion capture data directly within Unity using the MediaPipe Plug-in.
  
- Enhance facial expression recognition for more realistic avatar emotions.
  
- Improve avatar animation smoothing and interpolation.
  
- **In Progress**: The project is ongoing with plans to integrate with **VirtualLiveStage**, creating a unified system for real-time virtual performance control.
