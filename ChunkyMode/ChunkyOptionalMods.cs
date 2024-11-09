using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using RoR2;
using MaterialHud;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace HDeMods { namespace ChunkyOptionalMods {
    internal static class Hunk {
        public static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rob.Hunk");
    }
    
    internal static class Spikestrip {
        public static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("_com.prodzpod.ProdzpodSpikestripContent");
        
    }

    internal static class RiskUI {
        public static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("bubbet.riskui");

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void AddLegacyDifficulty() => RiskUIPlugin.DifficultyIconMap["CHUNKYMODEDIFFMOD_NAME"] = 
            ChunkyMode.HurricaneBundle.LoadAsset<Sprite>("texChunkyModeRiskUI");
    }

    internal static class Starstorm2 {
        public static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm");
        private const float chirrHealOverride = 2f;
        private static ILHook chirrHook;

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void GenerateHooks() {
            MethodInfo chirrHealMethod = AccessTools.Method(typeof(SS2.Survivors.Chirr), "ModifyStats");
            chirrHook = new ILHook(chirrHealMethod, BuffChirr);
            chirrHook.Undo();
        }
        
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void SetHooks() => chirrHook.Apply();
        
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void RemoveHooks() => chirrHook.Undo();
        
        
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void BuffChirr(ILContext il) {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(
                    moveType: MoveType.After,
                    x => x.MatchLdsfld("SS2.Survivors.Chirr", "_percentHealthRegen")
                )) {
                CM.Log.Error("Failed to hook Soothing Venom buff!");
                return;
            }
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, CharacterBody, float>>(
                (toHeal, cb) => {
                    if (cb.teamComponent.teamIndex != TeamIndex.Player) return toHeal;
                    if (ChunkyMode.isSimulacrumRun && !ChunkyMode.waveStarted) return toHeal;
                    return toHeal * chirrHealOverride;
                });
        }
    }
} }