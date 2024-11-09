using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using RoR2;
using MaterialHud;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;

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
        private static bool mmhookSS2Missing;
        private const float chirrHealOverride = 2f;

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void TrySetHooks() {
            if (mmhookSS2Missing) return;
            try {
                SetHooks();
            }
            catch (Exception e) {
                CM.Log.Error(e);
                string errorMessage = "Failed to hook Soothing Venom Buff!";
                if (!File.Exists(Assembly.GetExecutingAssembly().Location
                        .Replace("ChunkyMode.dll", "MMHOOK_Starstorm2.dll"))) {
                    errorMessage += " MMHOOK_Starstorm2.dll is missing! Please reinstall ChunkyMode!";
                    mmhookSS2Missing = true;
                }
                CM.Log.Error(errorMessage);
            }
        }
        
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void TryRemoveHooks() {
            if (mmhookSS2Missing) return;
            try {
                RemoveHooks();
            }
            catch (Exception e) {
                CM.Log.Error(e);
                string errorMessage = "Failed to remove Soothing Venom Buff hook!";
                if (!File.Exists(Assembly.GetExecutingAssembly().Location
                        .Replace("ChunkyMode.dll", "MMHOOK_Starstorm2.dll"))) {
                    errorMessage += " MMHOOK_Starstorm2.dll is missing! Please reinstall ChunkyMode!";
                    mmhookSS2Missing = true;
                }
                CM.Log.Error(errorMessage);
            }
        }
        
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SetHooks() => IL.SS2.Survivors.Chirr.ModifyStats += BuffChirr;
        
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void RemoveHooks() => IL.SS2.Survivors.Chirr.ModifyStats -= BuffChirr;
        
        
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