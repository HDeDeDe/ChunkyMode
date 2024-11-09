using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using RoR2;
using MaterialHud;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace HDeMods { 
    namespace ChunkyOptionalMods {
        internal static class Hunk {
            public static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rob.Hunk");
        }

        internal static class Spikestrip {
            public static bool Enabled =>
                BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("_com.prodzpod.ProdzpodSpikestripContent");

            private static string unusedString = @"https://github.com/HDeDeDe/ChunkyMode/blob/main/Resources/922279cb37ba22c549cb24845246cab250b7a671b7e1997d10e742e81c945785_1.jpg?raw=true";
        }

        internal static class RiskUI {
            public static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("bubbet.riskui");

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void AddLegacyDifficulty() => RiskUIPlugin.DifficultyIconMap["CHUNKYMODEDIFFMOD_NAME"] =
                ChunkyMode.HurricaneBundle.LoadAsset<Sprite>("texChunkyModeRiskUI");
        }

        internal static class Starstorm2 {
            public static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm");
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
                        return toHeal * ChunkyRunInfo.instance.chirrHealOverride;
                    });
            }
        }

        internal static class AlienHominid {
            public static bool Enabled =>
                BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TheTimeSweeper.Aliem");

            private static ILHook aliemHook;

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void GenerateHooks() {
                MethodInfo aliemHealMethod = AccessTools.Method(typeof(ModdedEntityStates.Aliem.AliemRidingChomp),
                    "FuckinHealInMultiplayerPlease");
                aliemHook = new ILHook(aliemHealMethod, BuffAliem);
                aliemHook.Undo();
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void SetHooks() => aliemHook.Apply();

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void RemoveHooks() => aliemHook.Undo();

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            private static void BuffAliem(ILContext il) {
                ILCursor c = new ILCursor(il);
                if (!c.TryGotoNext(
                        moveType: MoveType.After,
                        x => x.MatchLdsfld("AliemMod.Content.AliemConfig", "M3_Chomp_Healing")
                    )) {
                    CM.Log.Error("Failed to hook Chomp!");
                    return;
                }
                c.Index++;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, EntityStates.EntityState, float>>(
                    (toHeal, es) => {
                        HealthComponent hc = es.healthComponent;
                        if (hc.body.teamComponent.teamIndex != TeamIndex.Player) return toHeal;
                        if (ChunkyMode.isSimulacrumRun && !ChunkyMode.waveStarted) return toHeal;
                        return toHeal * ChunkyRunInfo.instance.aliemHealOverride;
                    });
            }
        }

        internal static class Ravager {
            public static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rob.Ravager");
            private static ILHook ravagerHook;

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void GenerateHooks() {
                MethodInfo ravagerHealMethod = AccessTools.Method(typeof(RedGuyMod.Content.ConsumeOrb), "OnArrival");
                ravagerHook = new ILHook(ravagerHealMethod, BuffRavager);
                ravagerHook.Undo();
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void SetHooks() => ravagerHook.Apply();

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void RemoveHooks() => ravagerHook.Undo();

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            private static void BuffRavager(ILContext il) {
                ILCursor c = new ILCursor(il);
                if (!c.TryGotoNext(
                        moveType: MoveType.After,
                        x => x.MatchLdfld<HurtBox>("healthComponent"),
                        x => x.MatchLdarg(0),
                        x => x.MatchLdfld<RedGuyMod.Content.ConsumeOrb>("healOverride")
                    )) {
                    CM.Log.Error("Failed to hook ConsumeOrb!");
                    return;
                }
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float,RoR2.Orbs.Orb, float>>(
                    (toHeal, orb) => {
                        if (orb.target.teamIndex != TeamIndex.Player) return toHeal;
                        if (ChunkyMode.isSimulacrumRun && !ChunkyMode.waveStarted) return toHeal;
                        return toHeal * ChunkyRunInfo.instance.ravagerHealOverride;
                    });
            }
        }

        internal static class Submariner {
            public static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.kenko.Submariner");
            private static ILHook submarinerHook;

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void GenerateHooks() {
                MethodInfo submarinerHealMethod = AccessTools.Method(
                    typeof(SubmarinerMod.SubmarinerCharacter.SubmarinerSurvivor), "CharacterBody_RecalculateStats");
                submarinerHook = new ILHook(submarinerHealMethod, BuffSubmariner);
                submarinerHook.Undo();
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void SetHooks() => submarinerHook.Apply();

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void RemoveHooks() => submarinerHook.Undo();
            
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            private static void BuffSubmariner(ILContext il) {
                ILCursor c = new ILCursor(il);
                if (!c.TryGotoNext(
                        moveType: MoveType.After,
                        x => x.MatchLdsfld(
                            "SubmarinerMod.SubmarinerCharacter.Content.SubmarinerBuffs", "SubmarinerRegenBuff"),
                        x => x.MatchCallvirt<CharacterBody>("GetBuffCount"),
                        x => x.MatchConvR4()
                    )) {
                    CM.Log.Error("Failed to hook Submariner Recalculate Stats!");
                    return;
                }
                c.Emit(OpCodes.Ldarg_2);
                c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float,
                    SubmarinerMod.SubmarinerCharacter.Components.SubmarinerController, float>>(
                    (toHeal, sbc) => {
                        if (sbc.convictedVictimBody.teamComponent.teamIndex != TeamIndex.Player) return toHeal;
                        if (ChunkyMode.isSimulacrumRun && !ChunkyMode.waveStarted) return toHeal;
                        return toHeal * ChunkyRunInfo.instance.submarinerHealOverride;
                    });
            }
        }
    }
}