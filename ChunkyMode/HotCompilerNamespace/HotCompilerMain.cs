#if DEBUG
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EntityStates;
using EntityStates.GolemMonster;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using RoR2;
using UnityEngine;

namespace HotCompilerNamespace
{
    [SuppressMessage("ReSharper", "SuggestVarOrType_SimpleTypes")]
    public class HotReloadMain
    {
        const BindingFlags allFlags = (BindingFlags)(-1);

        public static void HotReloadEntryPoint()
        {
            // This is just for being able to call self.OnEnter() inside hooks.
            {
                new ILHook(typeof(HotReloadMain).GetMethod(nameof(BaseStateOnEnterCaller), allFlags), BaseStateOnEnterCallerMethodModifier);
            }

            {
                //var methodToReload = typeof(HDeMods.ChunkySimulacrum).GetMethod(nameof(HDeMods.ChunkySimulacrum.InfiniteTowerWaveController_Initialize), allFlags);
                var newMethod = typeof(HotReloadMain).GetMethod(nameof(InfiniteTowerWaveController_Initialize_Overide), allFlags);
                //new Hook(methodToReload, newMethod);
            }
        }

        // This is just for being able to call self.OnEnter() inside hooks.
        private static void BaseStateOnEnterCaller(BaseState self)
        {

        }

        // This is just for being able to call self.OnEnter() inside hooks.
        private static void BaseStateOnEnterCallerMethodModifier(ILContext il)
        {
            var cursor = new ILCursor(il);
            cursor.Emit(OpCodes.Ldarg_0);
            //cursor.Emit(OpCodes.Call, typeof(HDeMods.ChunkySimulacrum).GetMethod(nameof(HDeMods.ChunkySimulacrum.InfiniteTowerWaveController_Initialize), allFlags));
        }

        private static void InfiniteTowerWaveController_Initialize_Overide(ILContext il) {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(moveType: MoveType.After,
                    x => x.MatchLdfld<Run>("difficultyCoefficient"))
               ) {
                HDeMods.CM.Log.Fatal("Failed to hook into InfiniteTowerWaveController.Initialize!");
                return;
            }

            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, float>>(b => b * 1.1f);
        }
    }
}
#endif