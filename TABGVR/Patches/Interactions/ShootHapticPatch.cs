using System;
using HarmonyLib;
using TABGVR.Input;
using TABGVR.Player;

namespace TABGVR.Patches.Interactions;

[HarmonyPatch(typeof(Weapon), nameof(Weapon.Awake))]
public class ShootHapticPatch
{
    private const float Amplitude = 0.8f;
    private const float Duration = 0.03f;
    
    /// <summary>
    /// Registers the <see cref="Shoot"/> listener after <see cref="Weapon.Awake"/>.
    /// </summary>
    /// <param name="__instance"></param>
    private static void Postfix(Weapon __instance)
    {
        if (__instance.gun != null) __instance.gun.shootEvent.AddListener(() => Shoot(__instance));
    }

    /// <summary>
    /// Calculates the haptic force and applies it to the controller.
    /// </summary>
    /// <param name="weapon">The weapon that was interacted with</param>
    private static void Shoot(Weapon weapon)
    {
        var handler = global::Player.localPlayer.m_weaponHandler;

        if (!weapon.gun) return;

        var force = Amplitude + (1 - Amplitude) * Math.Min(-weapon.gun.recoil.gunForce.z / 15, 1);
        var duration = Duration + -weapon.gun.recoil.gunForce.z * 0.02f;

        if (weapon == handler.leftWeapon || KinematicsPatch.Gripping)
        {
            Haptics.Send(Controllers.LeftHandXR, force, duration);
        }
        
        if (weapon == handler.rightWeapon)
        {
            Haptics.Send(Controllers.RightHandXR, force, duration);
        }
    }
}