using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace TABGVR.Patches.CameraPatches;

[HarmonyPatch(typeof(CameraMovement))]
public class CameraMovementPatch
{
    [HarmonyPatch(nameof(CameraMovement.LateUpdate))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> LateUpdate(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions.Select((value, i) => new { i, value }))
        {
            // instruction.value.opcode == OpCodes.Ldarg_0 &&
            //     instructionsList[instruction.i + 1].operand is FieldInfo fieldInfo &&
            //     fieldInfo == typeof(CameraMovement).GetField(nameof(CameraMovement.sitting))

            if (instruction.i is >= 144 and <= 196)
            {
                instruction.value.opcode = OpCodes.Nop;
                instruction.value.operand = null;
            }
            
            yield return instruction.value;
        }
    }

    [HarmonyPatch(nameof(CameraMovement.ResetVehicleCamera))]
    [HarmonyPrefix]
    public static bool ResetVehicleCameraPatch() => false;
}