using System.Reflection;
using BepInEx;
using R2API;
using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace ChunkyMode
{
    [BepInDependency(DifficultyAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(DirectorAPI.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    public class ChunkyMode : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "HDeDeDe";
        public const string PluginName = "ChunkyMode";
        public const string PluginVersion = "0.1.1";

        public AssetBundle ChunkyModeDifficultyModBundle;
        public static DifficultyDef ChunkyModeDifficultyDef;
        public static DifficultyIndex ChunkyModeDifficultyIndex;
        private const float rexHealOverride = 1.5f;
        private static bool shouldRun = false;
        private static int ogMonsterCap;
        
        public void Awake()
        {
            Log.Init(Logger);
            ChunkyModeDifficultyModBundle = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("ChunkyMode.dll", "chunkydifficon"));
            AddDifficulty();
            FillTokens();
            Run.onRunSetRuleBookGlobal += Run_onRunSetRuleBookGlobal;
            Run.onRunStartGlobal += Run_onRunStartGlobal;
            Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;
        }

        public void AddDifficulty() {
            ChunkyModeDifficultyDef = new DifficultyDef(4f,
                "CHUNKYMODEDIFFMOD_NAME",
                "CHUNKYMODEDIFFMOD_ICON",
                "CHUNKYMODEDIFFMOD_DESCRIPTION",
                new Color32(61, 25, 255, 255),
                "cm",
                true
            );
            ChunkyModeDifficultyDef.iconSprite = ChunkyModeDifficultyModBundle.LoadAsset<Sprite>("texChunkyModeDiffIcon");
            ChunkyModeDifficultyDef.foundIconSprite = true;
            ChunkyModeDifficultyIndex = DifficultyAPI.AddDifficulty(ChunkyModeDifficultyDef);
        }

        public void FillTokens() {
            LanguageAPI.Add("CHUNKYMODEDIFFMOD_NAME", "Chunky Mode");
            LanguageAPI.Add("CHUNKYMODEDIFFMOD_DESCRIPTION", "<color=#C80000>For fools</color>. The nightmares this planet holds will devour you faster than ever before. You will be overwhelmed. You will suffer.\n\n" +
                                                             "<style=cStack>>Player Health Regeneration: <style=cIsHealth>-40%</style>\n" +
                                                             ">Difficulty Scaling: <style=cIsHealth>+100%</style>\n" +
                                                             ">Ally Healing: <style=cIsHealth>-50%</style>\n" +
                                                             ">Ally Shield Recharge Rate: <style=cIsHealth>-50%</style>\n" +
                                                             ">Enemy Speed: <style=cIsHealth>+40%</style>\n" +
                                                             ">Enemy Cooldowns: <style=cIsHealth>-50%</style>\n" +
                                                             ">Enemy Spawn Rate: <style=cIsHealth>+10%</style>\n" +
                                                             ">Loot Spawn Rate: <style=cIsHealing>+20%</style>\n" +
                                                             "and <style=cIsHealth>more</style>...</style>");
        }
        
        private static void Run_onRunSetRuleBookGlobal(Run arg1, RuleBook arg2)
        {
            if (arg1.selectedDifficulty != ChunkyModeDifficultyIndex) return;
            Run.ambientLevelCap = 9999;
        }

        private void Run_onRunStartGlobal(Run run) {
            shouldRun = false;
            ogMonsterCap = TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit;
            
            if (run.selectedDifficulty != ChunkyModeDifficultyIndex) return;
            Log.Info("Chunky Mode Run started");
            shouldRun = true;
            
            //Thanks Starstorm 2 :)
            TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit = (int)(ogMonsterCap * 1.5);
            TeamCatalog.GetTeamDef(TeamIndex.Void).softCharacterLimit = (int)(ogMonsterCap * 1.5);
            TeamCatalog.GetTeamDef(TeamIndex.Lunar).softCharacterLimit = (int)(ogMonsterCap * 1.5);

            IL.RoR2.HealthComponent.ServerFixedUpdate += ShieldRechargeRate;
            IL.EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile.HealPulse += REXHealPulse;
            IL.RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted.OnDamageInflictedServer += REXPrimaryAttack;
            
            SceneDirector.onPrePopulateSceneServer += SceneDirector_onPrePopulateSceneServer;
            On.RoR2.CombatDirector.Awake += CombatDirector_Awake;
            On.RoR2.HealthComponent.Heal += OnHeal;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            
            //On.RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted.OnDamageInflictedServer +=
            //    ProjectileHealOwnerOnDamageInflicted_OnDamageInflictedServer;
            //On.EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile.HealPulse += DumbAssRexFlower;
        }

        private void Run_onRunDestroyGlobal(Run run) {
            if (!shouldRun) return;
            Log.Info("Chunky Mode Run ended");
            shouldRun = false;
            
            TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit = ogMonsterCap;
            TeamCatalog.GetTeamDef(TeamIndex.Void).softCharacterLimit = ogMonsterCap;
            TeamCatalog.GetTeamDef(TeamIndex.Lunar).softCharacterLimit = ogMonsterCap;
            
            IL.RoR2.HealthComponent.ServerFixedUpdate -= ShieldRechargeRate;
            IL.EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile.HealPulse -= REXHealPulse;
            IL.RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted.OnDamageInflictedServer -= REXPrimaryAttack;
            
            RecalculateStatsAPI.GetStatCoefficients -= RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.HealthComponent.Heal -= OnHeal;
            On.RoR2.CombatDirector.Awake -= CombatDirector_Awake;
            SceneDirector.onPrePopulateSceneServer -= SceneDirector_onPrePopulateSceneServer;
            
            //On.RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted.OnDamageInflictedServer -=
            //    ProjectileHealOwnerOnDamageInflicted_OnDamageInflictedServer;
            //On.EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile.HealPulse -= DumbAssRexFlower;
        }
        
        // This handles the -50% Ally Healing stat
        private static float OnHeal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount,
            ProcChainMask procChainMask, bool nonRegen) {
            float newAmount = amount;
            if (self.body.teamComponent.teamIndex == TeamIndex.Player) newAmount /= 2f;
            return orig(self, newAmount, procChainMask, nonRegen);
        }
        
        // Buff Rex healing because holy fuck
        // Probably shouldn't just reimplement it with a check but I'm lazy
        //TODO: Replace with IL hook
        private void ProjectileHealOwnerOnDamageInflicted_OnDamageInflictedServer(
            On.RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted.orig_OnDamageInflictedServer onDamageInflictedServer, 
            RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted self, DamageReport report) {
            if (!self.projectileController.owner) return;
            if (self.projectileController.name == "SyringeProjectileHealing(Clone)") {
                if (self.projectileController.owner.GetComponent<CharacterBody>().teamComponent.teamIndex ==
                    TeamIndex.Player) {
                    HealthComponent component = self.projectileController.owner.GetComponent<HealthComponent>();
                    if (component) {
                        RoR2.Orbs.HealOrb healOrb = new RoR2.Orbs.HealOrb();
                        healOrb.origin = self.transform.position;
                        healOrb.target = component.body.mainHurtBox;
                        healOrb.healValue = report.damageDealt * self.fractionOfDamage * rexHealOverride;
                        healOrb.overrideDuration = 0.3f;
                        RoR2.Orbs.OrbManager.instance.AddOrb(healOrb);
                    }
                    return;
                }
            }
            onDamageInflictedServer(self, report);
        }
        //hatehatehatehatehatehatehatehatehatehatehatehatehatehatehatehatehatehatehatehatehatehate
        //TODO: Replace this shit with an IL hook
        private void DumbAssRexFlower(
            On.EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile.orig_HealPulse whatTheFuck,
            EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile self) {
            if (self.projectileController.owner.GetComponent<CharacterBody>().teamComponent.teamIndex !=
                TeamIndex.Player) whatTheFuck(self);
            
            HealthComponent healthComponent = self.owner ? self.owner.GetComponent<HealthComponent>() : null;
            
            if (healthComponent && self.rootedBodies.Count > 0)
            {
                float num = 1f / EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile.healPulseCount;
                RoR2.Orbs.HealOrb healOrb = new RoR2.Orbs.HealOrb();
                healOrb.origin = self.transform.position;
                healOrb.target = healthComponent.body.mainHurtBox;
                healOrb.healValue = num * EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile.healthFractionYieldPerHit 
                                        * healthComponent.fullHealth * (float)self.rootedBodies.Count * rexHealOverride;
                healOrb.overrideDuration = 0.3f;
                RoR2.Orbs.OrbManager.instance.AddOrb(healOrb);
            }
        }
        
        //This handles the +40% Enemy Speed, -50% Enemy Cooldowns, and other stats
        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender,
            RecalculateStatsAPI.StatHookEventArgs args) {
            if (!sender) return;
            if (sender.teamComponent.teamIndex == TeamIndex.Player) return;
            
            switch (sender.name) {
                case "BeetleGuardBody(Clone)":
                    args.moveSpeedMultAdd += 0.4f;
                    args.cooldownReductionAdd += 0.5f;
                    break;
                case "VagrantBody(Clone)":
                    args.attackSpeedMultAdd += 0.25f;
                    args.moveSpeedMultAdd += 0.4f;
                    args.cooldownReductionAdd += 0.5f;
                    break;
                case "BellBody(Clone)":
                    args.attackSpeedMultAdd += 2f;
                    args.moveSpeedMultAdd += 0.4f;
                    args.cooldownMultAdd += 0.25f;
                    break;
                default:
                    args.attackSpeedMultAdd += 0.5f;
                    args.moveSpeedMultAdd += 0.4f;
                    args.cooldownReductionAdd += 0.5f;
                    break;
            }
        }

        // This handles the +10% Enemy Spawn Rate stat and the hidden -10% Gold gain stat
        private void CombatDirector_Awake(On.RoR2.CombatDirector.orig_Awake origAwake, CombatDirector self) {
            //Got this from Starstorm 2 :)
            self.creditMultiplier *= 1.1f;
            self.goldRewardCoefficient *= 0.9f;
            origAwake(self);
        }

        // This handles the +20% Loot Spawn Rate stat
        private void SceneDirector_onPrePopulateSceneServer(SceneDirector self) {
            self.interactableCredit = (int)(self.interactableCredit * 1.2);
            Log.Info(self.interactableCredit);
        }

        // This handles the -50% Ally Shield Recharge Rate stat
        private void ShieldRechargeRate(ILContext il) {
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
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, CharacterBody, float>>((consumed, cb) => {
                if (cb.teamComponent.teamIndex != TeamIndex.Player) return 0.5f;
                return 0.25f;
            });
        }
        
        // This buffs REX's Tangling Growth
        private void REXHealPulse(ILContext il) {
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
        private void REXPrimaryAttack(ILContext il) {
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
        
    }
}
