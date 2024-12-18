using UnityEngine.XR;

namespace TABGVR.Input;

public static class Haptics
{
    public static void Send(InputDevice device, float amplitude, float duration)
    {
        if (device.TryGetHapticCapabilities(out var capabilities) && !capabilities.supportsImpulse) return;
        
        device.SendHapticImpulse(0, amplitude, duration);
    }
}