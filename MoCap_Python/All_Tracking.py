import cv2
from cvzone.PoseModule import PoseDetector
from cvzone.FaceMeshModule import FaceMeshDetector
import socket

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

posData = []
# Get Frame
while True:
    success, img = cap.read()

# bbox
    poseImg = poseDetector.findPose(img)
    lmList, bboxInfo = poseDetector.findPosition(poseImg)

    faceImg, faces = faceDetector.findFaceMesh(img)


    data = []

    # send landmark data
    if bboxInfo:
        for lm in lmList:
            data.extend([lm[0], height - lm[1], lm[2]])
        sock.sendto(str.encode(str(data)), serverAddressPort1)

    if faces:
        for face in faces[0]:
            data.extend([face[0], height - face[1], face[2]])
        sock.sendto(str.encode(str(data)), serverAddressPort2)

    img = cv2.resize(poseImg + faceImg, (0, 0), None, 0.5, 0.5)
    cv2.imshow("Image", img)

    if cv2.waitKey(1) == ord("q"):
        break