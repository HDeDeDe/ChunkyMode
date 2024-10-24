using System.Diagnostics.CodeAnalysis;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
namespace HDeMods {
	internal static class ChunkyILHooks {
        // These are the override values
        private const float rexHealOverride = 1.5f;
        private const float acridHealOverride = 2f;
        private const float shieldRechargeOverride = -0.5f;
        private const float barrierDecayOverride = -0.5f;

        // This handles the -50% Ally Healing stat
        public static void HealingOverride(HealthComponent sender, HealthComponentAPI.HealEventArgs args) {
            if (sender.body.teamComponent.teamIndex != TeamIndex.Player) return;
            if (ChunkyMode.isSimulacrumRun && !ChunkyMode.waveStarted) return;
            args.enableEclipseHealReduction = true;
        }

        // This handles the -50% Ally Shield Recharge Rate and +50% Ally Barrier Decay Rate stats
        public static void ShieldRechargeAndBarrierDecayRate(HealthComponent sender,
            HealthComponentAPI.UpdateHealthEventArgs args) {
            if (sender.body.teamComponent.teamIndex != TeamIndex.Player) return;
            args.barrierDecayRateMultAdd += barrierDecayOverride;
            if (ChunkyMode.isSimulacrumRun && !ChunkyMode.waveStarted) return;
            args.shieldRechargeRateMultAdd += shieldRechargeOverride;
        }
        
        // This buffs REX's Tangling Growth
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static void REXHealPulse(ILContext il) {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(
                    x => x.MatchConvR4(),
                    x => x.MatchMul(),
                    // Inserting here
                    x => x.MatchStfld<RoR2.Orbs.HealOrb>("healValue"),
                    x => x.MatchLdloc(2),
                    x => x.MatchLdcR4(0.3f)
                )) {
                CM.Log.Error("Failed to hook Tangling Growth!");
                return;
            }
            c.Index += 2;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile, float>>(
                (toHeal, tbf) => {
                    if (!tbf.owner) return toHeal;
                    if (ChunkyMode.isSimulacrumRun && !ChunkyMode.waveStarted) return toHeal;
                    if (tbf.owner.GetComponent<CharacterBody>().teamComponent.teamIndex != TeamIndex.Player) return toHeal;
                    return toHeal * rexHealOverride;
                });
        }
        
        // This buffs REX's DIRECTIVE: Inject
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static void REXPrimaryAttack(ILContext il) {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(
                    x => x.MatchLdfld<RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted>("fractionOfDamage"),
                    x => x.MatchMul(),
                    // Inserting here
                    x => x.MatchStfld<RoR2.Orbs.HealOrb>("healValue"),
                    x => x.MatchLdloc(1),
                    x => x.MatchLdcR4(0.3f)
                )) {
                CM.Log.Error("Failed to hook DIRECTIVE: Inject!");
                return;
            }
            c.Index += 2;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted,
                float>>(
                (toHeal,self) => {
                    if (ChunkyMode.isSimulacrumRun && !ChunkyMode.waveStarted) return toHeal;
                    if (self.projectileController.catalogIndex != ChunkyCachedIndexes.injector) return toHeal;
                    if (self.projectileController.owner.GetComponent<CharacterBody>().teamComponent.teamIndex !=
                        TeamIndex.Player) return toHeal;
                    return toHeal * rexHealOverride;
                });
        }
        
        // This buffs Acrid's Vicious Wounds and Ravenous Bite
        public static void AcridRegenBuff(ILContext il) {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(
                    x => x.MatchCall<CharacterBody>("GetBuffCount"),
                    x => x.MatchConvR4(),
                    x => x.MatchLdarg(0),
                    x => x.MatchCall<CharacterBody>("get_maxHealth"),
                    x => x.MatchMul(),
                    x => x.MatchLdcR4(0.1f),
                    //Insert Here
                    x => x.MatchMul()
                )) {
                CM.Log.Error("Failed to hook CrocoRegen!");
                return;
            }
            c.Index += 6;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, CharacterBody, float>>(
                (toHeal, cb) => {
                    if (ChunkyMode.isSimulacrumRun && !ChunkyMode.waveStarted) return toHeal;
                    if (cb.teamComponent.teamIndex != TeamIndex.Player) return toHeal;
                    return toHeal * acridHealOverride;
                });
        }
	}
}