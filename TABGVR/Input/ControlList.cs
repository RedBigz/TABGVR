using UnityEngine;

namespace TABGVR.Input;

public class ControlList
{
    public VRInputBinary AButton = new();
    public VRInputBinary BButton = new();
    public VRInputBinary XButton = new();
    public VRInputBinary YButton = new();

    public VRInputBinary LeftTriggerBinary = new();
    public VRInputBinary RightTriggerBinary = new();

    public VRInput1D Turn = new();
    public VRInputBinary WeaponSwitchUp = new();
    public VRInputBinary WeaponSwitchDown = new();
    
    public VRInputBinary LeftStick = new();
    public VRInputBinary RightStick = new();
    
    public VRInputBinary MenuButton = new();
}