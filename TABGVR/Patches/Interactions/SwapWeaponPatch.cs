using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using TABGInput;
using TABGVR.Input;
using TABGVR.PatchAttributes;
using TABGVR.Player;

namespace TABGVR.Patches.Interactions;

[HarmonyPatch(typeof(InteractionHandler), nameof(InteractionHandler.StartPickup), MethodType.Enumerator)]
[VRPatch]
public static class SwapWeaponPatch
{
    [HarmonyTranspiler]
#if DEBUG
    [HarmonyDebug]
#endif
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) =>
        new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldfld,
                    AccessTools.Field(typeof(InteractionHandler), nameof(InteractionHandler.m_PlayerActions))),
                new CodeMatch(OpCodes.Ldfld,
                    AccessTools.Field(typeof(PlayerActions), nameof(PlayerActions.Interact))),
                new CodeMatch(OpCodes.Ldfld,
                    AccessTools.Field(typeof(PlayerActions.HaxAction), nameof(PlayerActions.HaxAction.IsPressed))))
            .SetOpcodeAndAdvance(OpCodes.Nop)
            .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(VRControls), nameof(VRControls.GetSomethingTriggered))))
            .SetOpcodeAndAdvance(OpCodes.Nop)
            .SetOpcodeAndAdvance(OpCodes.Nop)
            .InstructionEnumeration();
}