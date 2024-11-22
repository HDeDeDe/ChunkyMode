using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using R2API;
using RoR2;
using UnityEngine;

namespace HDeMods {
    namespace HurricaneOptionalMods {
        internal static class InfernoDownpour {
            public static bool Enabled =>
                BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(Inferno.Main.PluginGUID)
                && DownpourMod.Enabled;
            
            public static bool DiffsEnabled() {
                if (!Enabled) return false;
                return GetBrimstoneStatus();
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            private static bool GetBrimstoneStatus() => Downpour.DownpourPlugin.EnableBrimstone.Value;

            private static Hook infernoHook;
            private static Hook downpourDescriptionHook;
            private static Hook downpourHook;

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void GenerateHooks() {
                MethodInfo isInfernoMethod = AccessTools.Method(typeof(Inferno.Main), "IsInferno",
                    [typeof(DifficultyIndex)]);
                infernoHook = new Hook(isInfernoMethod, I_IsInferno);
                
                MethodInfo getStageScaleMethod = AccessTools.Method(typeof(Downpour.Hooks), "GetStageScale",
                    [typeof(DifficultyDef), typeof(int), typeof(int), typeof(bool)]);
                downpourHook = new Hook(getStageScaleMethod, D_GetStageScale);
                
                MethodInfo getDescriptionMethod = AccessTools.Method(typeof(Downpour.Token), "GetDescription",
                    [typeof(string), typeof(DifficultyDef)]);
                downpourDescriptionHook = new Hook(getDescriptionMethod, D_GetDescription);
            }
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            private static float D_GetStageScale(Func<DifficultyDef, int, int, bool, float> orig, DifficultyDef diff, 
                int stage, int people, bool simulacrum = false) {
                if (diff == SadomasochismWish.diffDef) return 
                    Downpour.Hooks.GetStageScaleInternal(
                        SadomasochismWish.diffDef.scalingValue 
                        + (stage * Downpour.DownpourPlugin.StageScalingBrimstone.Value), people) 
                    * (simulacrum ? Downpour.DownpourPlugin.SimulacrumStageScalingBrimstone.Value : 1);

                return orig(diff, stage, people, simulacrum);
            }
            
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            private static string D_GetDescription(Func<string, DifficultyDef, string> orig, 
                string origString, DifficultyDef def) {
                if (def != SadomasochismWish.diffDef) return orig(origString, def);
                CM.Log.Info("Blocked attempt to change description of " + def.nameToken);
                return origString;
            }
            
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            private static bool I_IsInferno(Func<DifficultyIndex, bool> orig, DifficultyIndex difficultyIndex) {
                if (difficultyIndex == SadomasochismWish.diffIndex) return true;
                return orig(difficultyIndex);
            }
        }
    }
    public static class SadomasochismWish {
        public static bool Enabled { get; internal set; }
        
        internal static DifficultyDef diffDef;
        internal static DifficultyIndex diffIndex;
        
        internal static void AddSadomasochismWish() {
            diffDef = new DifficultyDef(5f,
                "SADOMASOCHISMWISH_DIFF_NAME",
                "SADOMASOCHISMWISH_ICON",
                "SADOMASOCHISMWISH_DIFF_DESCRIPTION",
                new Color32(255, 204, 0, 255),
                "smw",
                true
            ) {
                iconSprite = Hurricane.HurricaneBundle.LoadAsset<Sprite>("texSadomasochismWishDiffIcon"),
                foundIconSprite = true
            };
            diffIndex = DifficultyAPI.AddDifficulty(diffDef);
            if (HurricaneOptionalMods.InfernoDownpour.DiffsEnabled()) AppendDiff();
        }

        private static void AppendDiff() {
            Downpour.DownpourPlugin.DownpourList.Add(diffDef);
            Downpour.DownpourPlugin.BrimstoneList.Add(diffDef);
        }

        public static void RunStart() {
            InterlopingArtifact.StagesUntilWaveringBegins = 1;
        }
        
        public static void RunEnd() {
            InterlopingArtifact.StagesUntilWaveringBegins = 5;
        }
    }
}