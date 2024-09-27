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
serverAddressPort3 = ("127.0.0.1", 5054)

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

maxHands=2

mpDraw = mp.solutions.drawing_utils
drawSpec = mpDraw.DrawingSpec(thickness=1, circle_radius=2)

mpFaceMesh = mp.solutions.face_mesh
faceMesh = mpFaceMesh.FaceMesh(static_image_mode=staticMode,
                                                 max_num_faces=maxFaces,
                                                 min_detection_confidence=minDetectionCon,
                                                 min_tracking_confidence=minTrackCon)

mpPose = mp.solutions.pose
pose = mpPose.Pose(static_image_mode=staticMode,
                                     model_complexity=modelComplexity,
                                     smooth_landmarks=smoothLandmarks,
                                     enable_segmentation=enableSegmentation,
                                     smooth_segmentation=smoothSegmentation,
                                     min_detection_confidence=detectionCon,
                                     min_tracking_confidence=trackCon)

mpHands = mp.solutions.hands
hands = mpHands.Hands(static_image_mode=staticMode,
                                        max_num_hands=maxHands,
                                        model_complexity=modelComplexity,
                                        min_detection_confidence=detectionCon,
                                        min_tracking_confidence=minTrackCon)

# Get Frame
while True:
    success, img = cap.read()

# bbox
    imgRGB = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
    h, w, c = img.shape

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
                x, y, z = int(lm.x * w), int(lm.y * h), (lm.z * w)
                face.append([x, y, z])
            faces.append(face)

    pose_results = pose.process(imgRGB)

    if pose_results.pose_landmarks:
        if draw:
            mpDraw.draw_landmarks(img, pose_results.pose_landmarks,
                                       mpPose.POSE_CONNECTIONS)

    pose_lmList = []
    bboxInfo = {}
    bboxWithHands = False
    if pose_results.pose_landmarks:
        for id, lm in enumerate(pose_results.pose_landmarks.landmark):
            cx, cy, cz = int(lm.x * w), int(lm.y * h), int(lm.z * w)
            pose_lmList.append([cx, cy, cz])

            # Bounding Box
        ad = abs(pose_lmList[12][0] - pose_lmList[11][0]) // 2
        if bboxWithHands:
            x1 = pose_lmList[16][0] - ad
            x2 = pose_lmList[15][0] + ad
        else:
            x1 = pose_lmList[12][0] - ad
            x2 = pose_lmList[11][0] + ad

        y2 = pose_lmList[29][1] + ad
        y1 = pose_lmList[1][1] - ad
        bbox = (x1, y1, x2 - x1, y2 - y1)
        cx, cy = bbox[0] + (bbox[2] // 2), \
                    bbox[1] + bbox[3] // 2

        bboxInfo = {"bbox": bbox, "center": (cx, cy)}

        if draw:
            cv2.rectangle(img, bbox, (255, 0, 255), 3)
            cv2.circle(img, (cx, cy), 5, (255, 0, 0), cv2.FILLED)

    hands_results = hands.process(imgRGB)
    flipType = True
    allHands = []

    if hands_results.multi_hand_landmarks:
        for handType, handLms in zip(hands_results.multi_handedness, hands_results.multi_hand_landmarks):
            myHand = {}
            ## lmList
            mylmList = []
            xList = []
            yList = []
            for id, lm in enumerate(handLms.landmark):
                px, py, pz = int(lm.x * w), int(lm.y * h), int(lm.z * w)
                mylmList.append([px, py, pz])
                xList.append(px)
                yList.append(py)

            ## bbox
            xmin, xmax = min(xList), max(xList)
            ymin, ymax = min(yList), max(yList)
            boxW, boxH = xmax - xmin, ymax - ymin
            hands_bbox = xmin, ymin, boxW, boxH
            cx, cy = hands_bbox[0] + (hands_bbox[2] // 2), \
                     hands_bbox[1] + (hands_bbox[3] // 2)

            myHand["lmList"] = mylmList
            myHand["bbox"] = hands_bbox
            myHand["center"] = (cx, cy)

            if flipType:
                if handType.classification[0].label == "Right":
                    myHand["type"] = "Left"
                else:
                    myHand["type"] = "Right"
            else:
                myHand["type"] = handType.classification[0].label
            allHands.append(myHand)

            ## draw
            if draw:
                mpDraw.draw_landmarks(img, handLms,
                                           mpHands.HAND_CONNECTIONS)
                cv2.rectangle(img, (hands_bbox[0] - 20, hands_bbox[1] - 20),
                              (hands_bbox[0] + hands_bbox[2] + 20, hands_bbox[1] + hands_bbox[3] + 20),
                              (255, 0, 255), 2)
                cv2.putText(img, myHand["type"], (hands_bbox[0] - 30, hands_bbox[1] - 30), cv2.FONT_HERSHEY_PLAIN,
                            2, (255, 0, 255), 2)

    pose_data = []
    face_data = []
    hands_data = []

    # send landmark data
    if bboxInfo:
        for lm in pose_lmList:
            pose_data.extend([lm[0], height - lm[1], lm[2]])
        sock.sendto(str.encode(str(pose_data)), serverAddressPort1)

    if faces:
        for face in faces[0]:
            face_data.extend([face[0], height - face[1], face[2]])
        sock.sendto(str.encode(str(face_data)), serverAddressPort2)

    if allHands:
        hand = allHands[0]
        hands_lmList = hand['lmList']

        for lm in hands_lmList:
            hands_data.extend([lm[0], height - lm[1], lm[2]])
        sock.sendto(str.encode(str(hands_data)), serverAddressPort3)

    img = cv2.resize(img, (0, 0), None, 0.5, 0.5)
    cv2.imshow("Image", img)

    if cv2.waitKey(1) == ord("q"):
        break