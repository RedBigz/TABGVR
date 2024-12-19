using UnityEngine.XR;

namespace TABGVR.Input;

public static class Haptics
{
    /// <summary>
    /// Sends haptic feedback using Unity's XR API.
    /// </summary>
    /// <param name="device">XR device to send haptic feedback to</param>
    /// <param name="amplitude">The amplitude of the feedback</param>
    /// <param name="duration">The duration of the feedback</param>
    public static void Send(InputDevice device, float amplitude, float duration)
    {
        if (device.TryGetHapticCapabilities(out var capabilities) && !capabilities.supportsImpulse) return;
        
        device.SendHapticImpulse(0, amplitude, duration);
    }
}