using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
namespace HDeMods {
	public static class ChunkyILHooks {
        // These are the override values
        private const float rexHealOverride = 1.5f;
        private const float acridHealOverride = 2f;
        private const float shieldRechargeOverride = 2f;
        
        // This handles the -50% Ally Shield Recharge Rate stat
        public static void ShieldRechargeRate(ILContext il) {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchLdloc(4),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<HealthComponent>("body"),
                x => x.MatchCallvirt<CharacterBody>("get_maxShield"),
                x => x.MatchLdcR4(0.5f),
                // Inserting here
                x => x.MatchMul()
            );
            c.Index += 5;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldfld, typeof(HealthComponent).GetField("body"));
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, CharacterBody, float>>((toRecharge, cb) => {
                if (cb.teamComponent.teamIndex != TeamIndex.Player) return toRecharge;
                return toRecharge / shieldRechargeOverride;
            });
        }
        
        // This buffs REX's Tangling Growth
        public static void REXHealPulse(ILContext il) {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchConvR4(),
                x => x.MatchMul(),
                // Inserting here
                x => x.MatchStfld<RoR2.Orbs.HealOrb>("healValue"),
                x => x.MatchLdloc(2),
                x => x.MatchLdcR4(0.3f)
            );
            c.Index += 2;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile, float>>(
                (toHeal, tbf) => {
                    if (!tbf.owner) return toHeal;
                    if (tbf.owner.GetComponent<CharacterBody>().teamComponent.teamIndex != TeamIndex.Player) return toHeal;
                    return toHeal * rexHealOverride;
                });
        }
        
        // This buffs REX's DIRECTIVE: Inject
        public static void REXPrimaryAttack(ILContext il) {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchLdfld<RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted>("fractionOfDamage"),
                x => x.MatchMul(),
                // Inserting here
                x => x.MatchStfld<RoR2.Orbs.HealOrb>("healValue"),
                x => x.MatchLdloc(1),
                x => x.MatchLdcR4(0.3f)
            );
            c.Index += 2;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted,
                float>>(
                (toHeal,self) => {
                    if (self.projectileController.name != "SyringeProjectileHealing(Clone)") return toHeal;
                    if (self.projectileController.owner.GetComponent<CharacterBody>().teamComponent.teamIndex !=
                        TeamIndex.Player) return toHeal;
                    return toHeal * rexHealOverride;
                });
        }
        
        // This buffs Acrid's Vicious Wounds and Ravenous Bite
        public static void AcridRegenBuff(ILContext il) {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchCall<CharacterBody>("GetBuffCount"),
                x => x.MatchConvR4(),
                x => x.MatchLdarg(0),
                x => x.MatchCall<CharacterBody>("get_maxHealth"),
                x => x.MatchMul(),
                x => x.MatchLdcR4(0.1f),
                //Insert Here
                x => x.MatchMul()
            );
            c.Index += 6;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, CharacterBody, float>>(
                (toHeal, cb) => {
                    if (cb.teamComponent.teamIndex != TeamIndex.Player) return toHeal;
                    return toHeal * acridHealOverride;
                });
        }
	}
}