import cv2
from cvzone.HandTrackingModule import HandDetector

# WebCam 을 사용할 경우
cap = cv2.VideoCapture(0)

# 손을 감지
detector = HandDetector(maxHands=1, detectionCon=0.8)

# 웹켐에서 프레임 가져오기
while True:
    success, img = cap.read()

# Hands
    hands, img = detector.findHands(img)

    cv2.imshow("Image", img)

    if cv2.waitKey(1) == ord("q"): # q 누를 시 웹켐 종료
        break