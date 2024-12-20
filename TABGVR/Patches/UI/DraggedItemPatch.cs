using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Landfall.TABG.UI;
using TABGVR.PatchAttributes;
using TABGVR.Util;
using UnityEngine;

namespace TABGVR.Patches.UI;

[HarmonyPatch(typeof(DraggedItem), nameof(DraggedItem.LateUpdate))]
#if DEBUG
[HarmonyDebug]
#endif
[VRPatch]
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
                new CodeInstruction(OpCodes.Callvirt,
                    typeof(DraggedItem).GetProperty(nameof(DraggedItem.transform))!.GetGetMethod()),
                Transpilers.EmitDelegate((Transform transform) =>
                    {
                        transform.position = Vector3.Lerp(transform.position, UIPorter.DragPosition,
                            Time.unscaledDeltaTime * 25);
                    }
                ))
            .InstructionEnumeration();
    }
}