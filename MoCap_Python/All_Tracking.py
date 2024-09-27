import cv2
from cvzone.PoseModule import PoseDetector
from cvzone.FaceMeshModule import FaceMeshDetector
import socket
import mediapipe as mp

# Parameters
width, height = 1280, 720

# WebCam's IP
cap = cv2.VideoCapture(0)

cap.set(3, width)
cap.set(4, height)

# Detect pose
poseDetector = PoseDetector()
faceDetector = FaceMeshDetector()

# Network
sock = socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
serverAddressPort1 = ("127.0.0.1", 5052)
serverAddressPort2 = ("127.0.0.1", 5053)

staticMode=False
maxFaces=2
minDetectionCon=0.5
minTrackCon=0.5

modelComplexity=1
smoothLandmarks=True
enableSegmentation=False
smoothSegmentation=True
detectionCon=0.5
trackCon=0.5

mpDraw = mp.solutions.drawing_utils

mpFaceMesh = mp.solutions.face_mesh
faceMesh = mpFaceMesh.FaceMesh(static_image_mode=staticMode,
                                                 max_num_faces=maxFaces,
                                                 min_detection_confidence=minDetectionCon,
                                                 min_tracking_confidence=minTrackCon)
drawSpec = mpDraw.DrawingSpec(thickness=1, circle_radius=2)

mpPose = mp.solutions.pose

pose = mpPose.Pose(static_image_mode=staticMode,
                                     model_complexity=modelComplexity,
                                     smooth_landmarks=smoothLandmarks,
                                     enable_segmentation=enableSegmentation,
                                     smooth_segmentation=smoothSegmentation,
                                     min_detection_confidence=detectionCon,
                                     min_tracking_confidence=trackCon)
posData = []
# Get Frame
while True:
    success, img = cap.read()

# bbox
    imgRGB = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)

    face_results = faceMesh.process(imgRGB)
    faces = []

    draw = True

    if face_results.multi_face_landmarks:
        for faceLms in face_results.multi_face_landmarks:
            if draw:
                mpDraw.draw_landmarks(img, faceLms, mpFaceMesh.FACEMESH_CONTOURS,
                                           drawSpec, drawSpec)
            face = []
            for id, lm in enumerate(faceLms.landmark):
                ih, iw, ic = img.shape
                x, y, z = int(lm.x * iw), int(lm.y * ih), (lm.z * iw)
                face.append([x, y, z])
            faces.append(face)

    pose_results = pose.process(imgRGB)

    if pose_results.pose_landmarks:
        if draw:
            mpDraw.draw_landmarks(img, pose_results.pose_landmarks,
                                       mpPose.POSE_CONNECTIONS)

    lmList = []
    bboxInfo = {}
    bboxWithHands = False
    if pose_results.pose_landmarks:
        for id, lm in enumerate(pose_results.pose_landmarks.landmark):
            h, w, c = img.shape
            cx, cy, cz = int(lm.x * w), int(lm.y * h), int(lm.z * w)
            lmList.append([cx, cy, cz])

            # Bounding Box
        ad = abs(lmList[12][0] - lmList[11][0]) // 2
        if bboxWithHands:
            x1 = lmList[16][0] - ad
            x2 = lmList[15][0] + ad
        else:
            x1 = lmList[12][0] - ad
            x2 = lmList[11][0] + ad

        y2 = lmList[29][1] + ad
        y1 = lmList[1][1] - ad
        bbox = (x1, y1, x2 - x1, y2 - y1)
        cx, cy = bbox[0] + (bbox[2] // 2), \
                    bbox[1] + bbox[3] // 2

        bboxInfo = {"bbox": bbox, "center": (cx, cy)}

        if draw:
            cv2.rectangle(img, bbox, (255, 0, 255), 3)
            cv2.circle(img, (cx, cy), 5, (255, 0, 0), cv2.FILLED)

    pose_data = []
    face_data = []

    # send landmark data
    if bboxInfo:
        for lm in lmList:
            pose_data.extend([lm[0], height - lm[1], lm[2]])
        sock.sendto(str.encode(str(pose_data)), serverAddressPort1)

    if faces:
        for face in faces[0]:
            face_data.extend([face[0], height - face[1], face[2]])
        sock.sendto(str.encode(str(face_data)), serverAddressPort2)

    img = cv2.resize(img, (0, 0), None, 0.5, 0.5)
    cv2.imshow("Image", img)

    if cv2.waitKey(1) == ord("q"):
        break