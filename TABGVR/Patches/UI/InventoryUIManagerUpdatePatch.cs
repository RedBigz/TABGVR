using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Landfall.TABG.UI;
using TABGVR.Input;

namespace TABGVR.Patches.UI;

[HarmonyPatch(typeof(InventoryUIManager), nameof(InventoryUIManager.Update))]
public class InventoryUIManagerUpdatePatch
{
    /// <summary>
    /// Replaces <c>this.isDragging = Input.GetMouseButton(0);</c> with <c>this.isDragging = VRControls.GetSomethingTriggered()</c>.
    /// </summary>
    /// <param name="instructions"></param>
    /// <returns></returns>
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) =>
        new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Stfld),
                new CodeMatch(OpCodes.Ldarg_0))
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Call, typeof(VRControls).GetMethod(nameof(VRControls.GetSomethingTriggered))))
            .RemoveInstructions(2)
            .InstructionEnumeration();
    
    /// <summary>
    /// Starts dragging an item if either controller is triggered.
    /// </summary>
    /// <param name="__instance"></param>
    public static void Postfix(InventoryUIManager __instance)
    {
        if (__instance.isDragging || __instance.selectedSlot is null || __instance.selectedSlot.isEmpty) return;
        if (!VRControls.SomethingTriggered) return;

        __instance.StartDragging();
    }
}