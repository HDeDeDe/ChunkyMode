using System.Diagnostics.CodeAnalysis;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
namespace HDeMods {
	internal static class HurricaneILHooks {
        // These are the override values
        private static readonly float shieldRechargeOverride = -0.5f;
        private static readonly float barrierDecayOverride = 1f;

        // This handles the -50% Ally Healing stat
        public static void HealingOverride(HealthComponent sender, HealthComponentAPI.HealEventArgs args) {
            if (sender.body.teamComponent.teamIndex != TeamIndex.Player) return;
            if (Hurricane.isSimulacrumRun && !Hurricane.waveStarted) return;
            args.enableEclipseHealReduction = true;
        }

        // This handles the -50% Ally Shield Recharge Rate and +50% Ally Barrier Decay Rate stats
        public static void ShieldRechargeAndBarrierDecayRate(HealthComponent sender,
            HealthComponentAPI.UpdateHealthEventArgs args) {
            if (sender.body.teamComponent.teamIndex != TeamIndex.Player) return;
            float barrierToOverride = barrierDecayOverride;
            if (sender.body.bodyIndex == HurricaneCachedIndexes.bodyCache[BodyCache.RobPaladin]) 
                barrierToOverride -= HurricaneRunInfo.instance.paladinBarrierOverride;
            args.barrierDecayRateMultAdd += barrierToOverride;
            if (Hurricane.isSimulacrumRun && !Hurricane.waveStarted) return;
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
                    if (Hurricane.isSimulacrumRun && !Hurricane.waveStarted) return toHeal;
                    CharacterBody cb = tbf.owner.GetComponent<CharacterBody>();
                    if (!cb) return toHeal;
                    if (cb.teamComponent.teamIndex != TeamIndex.Player) return toHeal;
                    return toHeal * HurricaneRunInfo.instance.rexHealOverride;
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
                    if (Hurricane.isSimulacrumRun && !Hurricane.waveStarted) return toHeal;
                    if (self.projectileController.catalogIndex != HurricaneCachedIndexes.injector) return toHeal;
                    CharacterBody cb = self.projectileController.owner.GetComponent<CharacterBody>();
                    if (!cb) return toHeal;
                    if (cb.teamComponent.teamIndex != TeamIndex.Player) return toHeal;
                    return toHeal * HurricaneRunInfo.instance.rexHealOverride;
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
                    if (Hurricane.isSimulacrumRun && !Hurricane.waveStarted) return toHeal;
                    if (cb.teamComponent.teamIndex != TeamIndex.Player) return toHeal;
                    return toHeal * HurricaneRunInfo.instance.acridHealOverride;
                });
        }

        public static void CaptainWardBuff(ILContext il) {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(
                    moveType: MoveType.After,
                    x => x.MatchLdfld<HealingWard>("healFraction")
                )) {
                CM.Log.Error("Failed to hook HealingWard!");
                return;
            }
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, HealingWard, float>>(
                (toHeal, hw) => {
                    if (Hurricane.isSimulacrumRun && !Hurricane.waveStarted) return toHeal;
                    if (hw.teamFilter.teamIndex != TeamIndex.Player) return toHeal;
                    if (hw.name != "CaptainHealingWard(Clone)") return toHeal;
                    return toHeal * HurricaneRunInfo.instance.captainHealOverride;
                });
        }
        
        public static void VoidFiendSuppressBuff(ILContext il) {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(
                    moveType: MoveType.Before,
                    x => x.MatchLdloc(0),
                    x => x.MatchCallvirt<HealthComponent>("HealFraction")
                )) {
                CM.Log.Error("Failed to hook Suppress!");
                return;
            }
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, EntityStates.EntityState, float>>(
                (toHeal, es) => {
                    if (Hurricane.isSimulacrumRun && !Hurricane.waveStarted) return toHeal;
                    if (es.teamComponent.teamIndex != TeamIndex.Player) return toHeal;
                    return toHeal * HurricaneRunInfo.instance.voidFiendHealOverride;
                });
        }
        
        public static void SeekerUnseenHandBuff(ILContext il) {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(
                    moveType: MoveType.After,
                    x => x.MatchLdfld<DamageReport>("damageDealt")
                )) {
                CM.Log.Error("Failed to hook Unseen Hand Healing!");
                return;
            }
            c.Emit(OpCodes.Ldloc_1);
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, CharacterBody, float>>(
                (toHeal, cb) => {
                    if (Hurricane.isSimulacrumRun && !Hurricane.waveStarted) return toHeal;
                    if (cb.teamComponent.teamIndex != TeamIndex.Player) return toHeal;
                    return toHeal * HurricaneRunInfo.instance.seekerHealOverride;
                });
        }
        
        /*public static void SeekerMeditateBuff(ILContext il) {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(
                    moveType: MoveType.After,
                    x => x.MatchLdfld<EntityStates.Seeker.MeditationUI>("healingExplosionAmount")
                )) {
                CM.Log.Error("Failed to hook Meditate!");
                return;
            }
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, EntityStates.EntityState, float>>(
                (toHeal, es) => {
                    if (Hurricane.isSimulacrumRun && !Hurricane.waveStarted) return toHeal;
                    if (es.teamComponent.teamIndex != TeamIndex.Player) return toHeal;
                    return toHeal * HurricaneRunInfo.instance.seekerHealOverride;
                });
        }*/
        
        public static void FalseSonLunarTamperingBuff(ILContext il) {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(
                    moveType: MoveType.After,
                    x => x.MatchLdfld<CharacterBody>("tamperedHeartRegenBonus")
                )) {
                CM.Log.Error("Failed to hook Tampered Heart Regen!");
                return;
            }
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, CharacterBody, float>>(
                (toHeal, cb) => {
                    if (Hurricane.isSimulacrumRun && !Hurricane.waveStarted) return toHeal;
                    if (cb.teamComponent.teamIndex != TeamIndex.Player) return toHeal;
                    return toHeal * HurricaneRunInfo.instance.falseSonHealOverride;
                });
        }
        
        public static void ChefSotSChefsKissBuff(ILContext il) {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(
                    moveType: MoveType.After,
                    x => x.MatchLdloc(68),
                    x => x.MatchLdloc(44)
                )) {
                CM.Log.Error("Failed to hook Chef's Kiss! (Flat Healing)");
                return;
            }
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, DamageReport, float>>(
                (toHeal, dr) => {
                    if (Hurricane.isSimulacrumRun && !Hurricane.waveStarted) return toHeal;
                    if (dr.attackerBody.teamComponent.teamIndex != TeamIndex.Player) return toHeal;
                    return toHeal * HurricaneRunInfo.instance.chefSotSHealOverride;
                });
            
            if (!c.TryGotoNext(
                    moveType: MoveType.After,
                    x => x.MatchLdloc(68),
                    x => x.MatchLdloc(45)
                )) {
                CM.Log.Error("Failed to hook Chef's Kiss! (Fractional Healing)");
                IL.RoR2.GlobalEventManager.OnCharacterDeath -= ChefSotSChefsKissBuff;
                return;
            }
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, DamageReport, float>>(
                (toHeal, dr) => {
                    if (Hurricane.isSimulacrumRun && !Hurricane.waveStarted) return toHeal;
                    if (dr.attackerBody.teamComponent.teamIndex != TeamIndex.Player) return toHeal;
                    return toHeal * HurricaneRunInfo.instance.chefSotSHealOverride;
                });
        }
    }
}