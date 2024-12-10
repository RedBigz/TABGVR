/*
 * Hello!
 *
 * You may realise that if you uncomment the annotations in this file, your game will crash when you throw a grenade.
 * I have no idea why it does that.
 *
 * If you can fix it tysm <3
 */

using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using TABGVR.Player;

namespace TABGVR.Patches;

// [HarmonyPatch(typeof(InteractionHandler), nameof(InteractionHandler.Throwing), MethodType.Enumerator)]
// [HarmonyDebug]
public static class ThrowingPatch
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) =>
        new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Stloc_S),
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldfld,
                    AccessTools.Field(typeof(InteractionHandler), nameof(InteractionHandler.player))),
                new CodeMatch(OpCodes.Ldfld,
                    AccessTools.Field(typeof(global::Player), nameof(global::Player.m_playerCamera))))
            .Advance(4)
            .RemoveInstructions(3)
            .Insert(new CodeInstruction(OpCodes.Ldsfld,
                AccessTools.Field(typeof(Controllers), nameof(Controllers.RightHand))))
            
            // change Player.localPlayer.m_playerCamera.transform.forward to Controllers.RightHand.transform.forward
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Callvirt))
            .RemoveInstructions(3)
            .Insert(new CodeInstruction(OpCodes.Ldsfld,
                AccessTools.Field(typeof(Controllers), nameof(Controllers.RightHand))))
            .InstructionEnumeration();
}