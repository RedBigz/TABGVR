using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Landfall.TABG.UI;
using TABGVR.Player;
using TABGVR.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TABGVR.Patches;

[HarmonyPatch(typeof(DraggedItem), nameof(DraggedItem.LateUpdate))]
[HarmonyDebug]
public class DraggedItemPatch
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Callvirt))
            .RemoveInstructions(11)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, typeof(DraggedItem).GetProperty(nameof(DraggedItem.transform))!.GetGetMethod()),
                Transpilers.EmitDelegate((Transform transform) =>
                    {
                        transform.position = Vector3.Lerp(transform.position, UIPorter.DragPosition,
                            Time.unscaledDeltaTime * 25);
                    }
                ))
            .InstructionEnumeration();
    }
}