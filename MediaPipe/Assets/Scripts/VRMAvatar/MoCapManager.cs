using UnityEngine;
using VRMAvatar;

public class MoCapManager : MonoBehaviour
{
    public AvatarBody ab;
    public AvatarFace af;

    // Start is called before the first frame update
    void Start()
    {
        ab.InitFilter(5);
        ab.SetMoCapMode(MoCapMode.jump);
        ab.SetSensitivity(5);
    }

    public void CalibrateBody()
    {
        ab.Calibrate();
    }

    public void ResetBody()
    {
        ab.ResetCalibration();
    }

    public void CalibrateFace()
    {
        af.Calibrate();
    }

    public void ResetFace()
    {
        af.ResetCalibration();
    }

    public void SetStrength(float s)
    {
        af.SetStrength(s);
    }

    public void SetSensitivity(float s)
    {
        ab.SetSensitivity((int)s);
    }

    public void SetMoCapMode(int mode)
    {
        ab.SetMoCapMode((MoCapMode)mode);
    }

    public void SetDominantEye(int eye)
    {
        af.SetDominantEye((DominantEye)eye);
    }

}