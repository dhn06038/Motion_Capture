import cv2
from cvzone.FaceMeshModule import FaceMeshDetector
import socket

# Parameters
width, height = 1280, 720

# WebCam's IP
cap = cv2.VideoCapture(0)

cap.set(3, width)
cap.set(4, height)

# Detect pose
detector = FaceMeshDetector()

# Network
sock = socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5052)

posData = []
# Get Frame
while True:
    success, img = cap.read()

# bbox
    img, faces = detector.findFaceMesh(img)

    data = []

    # send landmark data
    if faces:
        for face in faces[0]:
            data.extend([face[0], height - face[1], face[2]])
        sock.sendto(str.encode(str(data)), serverAddressPort)

    img = cv2.resize(img, (0, 0), None, 0.5, 0.5)
    cv2.imshow("Image", img)

    if cv2.waitKey(1) == ord("q"): # q 누를 시 웹켐 종료
        break