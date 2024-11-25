using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using TABGVR.Player;

namespace TABGVR.Patches;

[HarmonyPatch(typeof(InteractionHandler), nameof(InteractionHandler.StartPickup), MethodType.Enumerator)]
public static class SwapWeaponPatch
{
    
    
    [HarmonyTranspiler]
    [HarmonyDebug]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions.Select((value, i) => new { i, value }))
        {
            Plugin.Logger.LogInfo($"{instruction.i}: {instruction.value}");
            switch (instruction.i)
            {
                case 34:
                    instruction.value.opcode = OpCodes.Call;
                    instruction.value.operand = typeof(VRControls).GetMethod(nameof(VRControls.GetSomethingTriggered));
                    
                    yield return Transpilers.EmitDelegate(() => Plugin.Logger.LogInfo(VRControls.GetSomethingTriggered()));
                    
                    break;
                case 33:
                case 35:
                case 36:
                    instruction.value.opcode = OpCodes.Nop;
                    break;
            }
            
            Plugin.Logger.LogInfo($"=> {instruction.value}");
    
            yield return instruction.value;
        }
    }
}