using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RiskOfOptions;
using RiskOfOptions.Options;

namespace ChunkyMode
{
    public static class ChunkyModeOptions {
        private static bool? _enabled;

        public static bool enabled {
            get {
                if (_enabled == null) {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
                }
                return (bool)_enabled;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void AddCheck(ConfigEntry<bool> option) {
            ModSettingsManager.AddOption(new CheckBoxOption(option));
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void SetSprite(Sprite sprite) {
            ModSettingsManager.SetModIcon(sprite);
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void SetDescription(string description) {
            ModSettingsManager.SetModDescription(description);
        }
    }
    
    [BepInDependency(DifficultyAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(DirectorAPI.PluginGUID)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ChunkyMode : BaseUnityPlugin
    {
        // Plugin details
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "HDeDeDe";
        public const string PluginName = "ChunkyMode";
        public const string PluginVersion = "0.1.6";

        // Difficulty related variables
        public AssetBundle ChunkyModeDifficultyModBundle;
        public static DifficultyDef ChunkyModeDifficultyDef;
        public static DifficultyIndex ChunkyModeDifficultyIndex;
        
        // Run start checks
        private static bool shouldRun;
        private static bool swarmsEnabled;
        private static int ogMonsterCap;
        
        // These are related to the loitering penalty
        private static bool getFuckedLMAO;
        private static bool teleporterExists;
        private static float stagePunishTimer;
        private static bool teleporterHit;
        
        // These are to prevent changing settings mid run
        private static bool doLoiterThisRun; 
        private static bool doGoldThisRun;
        private static bool doNerfsThisRun;
        
        // These are the override values
        private const float rexHealOverride = 1.5f;
        private const float acridHealOverride = 2f;
        private const float shieldRechargeOverride = 2f;
        
        // These values can be changed by the player through config options
        public static ConfigEntry<bool> doHealingBuffs { get; set; }
        public static ConfigEntry<bool> doLoiterPenalty { get; set; }
        public static ConfigEntry<bool> doEnemyLimitBoost { get; set; }
        public static ConfigEntry<bool> doGoldPenalty { get; set; }
        public static ConfigEntry<bool> doEnemyNerfs { get; set; }
        
        
        
        public void Awake()
        {
            Log.Init(Logger);
            ChunkyModeDifficultyModBundle = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("ChunkyMode.dll", "chunkydifficon"));
            AddDifficulty();
            BindSettings();
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

        public void BindSettings() {
            doHealingBuffs = Config.Bind<bool>(
                "Unlisted Difficulty Modifiers",
                "Do Healing Buffs",
                true,
                "Enables buffs to some survivor healing skills. Disable if you want a harder challenge.");
            doLoiterPenalty = Config.Bind<bool>(
                "Unlisted Difficulty Modifiers",
                "Do Loiter Penalty",
                true,
                "Enables a 5 minute loiter penalty on stages with a teleporter. Not recommended to disable.");
            doEnemyLimitBoost = Config.Bind<bool>(
                "Unlisted Difficulty Modifiers",
                "Do Enemy Limit Boost",
                true,
                "Enables enemy limit increase. If your computer is struggling to run on Chunky Mode, consider disabling this.");
            doGoldPenalty = Config.Bind<bool>(
                "Unlisted Difficulty Modifiers",
                "Do Gold Penalty",
                true,
                "Enables a -10% gold penalty. Disable to speed up gameplay.");
            doEnemyNerfs = Config.Bind<bool>(
                "Unlisted Difficulty Modifiers",
                "Do Enemy Nerfs",
                true,
                "Enables enemy nerfs. Disable if you like unreactable Wandering Vagrants");
            if (!ChunkyModeOptions.enabled) return;
            ChunkyModeOptions.AddCheck(doHealingBuffs);
            ChunkyModeOptions.AddCheck(doLoiterPenalty);
            ChunkyModeOptions.AddCheck(doEnemyLimitBoost);
            ChunkyModeOptions.AddCheck(doGoldPenalty);
            ChunkyModeOptions.AddCheck(doEnemyNerfs);
            ChunkyModeOptions.SetSprite(ChunkyModeDifficultyModBundle.LoadAsset<Sprite>("texChunkyModeDiffIcon"));
            ChunkyModeOptions.SetDescription("Options for Chunky Mode. These options can not be changed during a run.");
        }
        
        private static void Run_onRunSetRuleBookGlobal(Run arg1, RuleBook arg2)
        {
            if (arg1.selectedDifficulty != ChunkyModeDifficultyIndex) return;
            if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef))
                swarmsEnabled = true;
            else swarmsEnabled = false;
            Run.ambientLevelCap = 9999;
        }

        private void Run_onRunStartGlobal(Run run) {
            shouldRun = false;
            teleporterHit = false;
            teleporterExists = false;
            getFuckedLMAO = false;
            ogMonsterCap = TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit;
            
            if (run.selectedDifficulty != ChunkyModeDifficultyIndex) return;
            Log.Info("Chunky Mode Run started");
            shouldRun = true;
            Config.Reload();

            if (doEnemyLimitBoost.Value){ 
                //Thanks Starstorm 2 :)
                TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit = (int)(ogMonsterCap * 1.5);
                TeamCatalog.GetTeamDef(TeamIndex.Void).softCharacterLimit = (int)(ogMonsterCap * 1.5);
                TeamCatalog.GetTeamDef(TeamIndex.Lunar).softCharacterLimit = (int)(ogMonsterCap * 1.5);
            }

            IL.RoR2.HealthComponent.ServerFixedUpdate += ShieldRechargeRate;
            if (doHealingBuffs.Value){ 
                IL.EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile.HealPulse += REXHealPulse;
                IL.RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted.OnDamageInflictedServer += REXPrimaryAttack;
                IL.RoR2.CharacterBody.RecalculateStats += AcridRegenBuff; 
            }

            doGoldThisRun = doGoldPenalty.Value;
            doNerfsThisRun = doEnemyNerfs.Value;
            SceneDirector.onPrePopulateSceneServer += SceneDirector_onPrePopulateSceneServer;
            On.RoR2.CombatDirector.Awake += CombatDirector_Awake;
            On.RoR2.HealthComponent.Heal += OnHeal;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;

            doLoiterThisRun = doLoiterPenalty.Value;
            if (!doLoiterPenalty.Value) return;
            On.RoR2.Run.OnServerTeleporterPlaced += Run_OnServerTeleporterPlaced;
            On.RoR2.Run.BeginStage += Run_BeginStage;
            On.RoR2.TeleporterInteraction.IdleState.OnInteractionBegin += OnInteractTeleporter;
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
            IL.RoR2.CharacterBody.RecalculateStats -= AcridRegenBuff;
            
            RecalculateStatsAPI.GetStatCoefficients -= RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.HealthComponent.Heal -= OnHeal;
            On.RoR2.CombatDirector.Awake -= CombatDirector_Awake;
            SceneDirector.onPrePopulateSceneServer -= SceneDirector_onPrePopulateSceneServer;
            
            On.RoR2.Run.OnServerTeleporterPlaced -= Run_OnServerTeleporterPlaced;
            On.RoR2.Run.BeginStage -= Run_BeginStage;
            On.RoR2.TeleporterInteraction.IdleState.OnInteractionBegin -= OnInteractTeleporter;
        }
        
        // This handles the -50% Ally Healing stat
        private static float OnHeal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount,
            ProcChainMask procChainMask, bool nonRegen) {
            float newAmount = amount;
            if (self.body.teamComponent.teamIndex == TeamIndex.Player) newAmount /= 2f;
            return orig(self, newAmount, procChainMask, nonRegen);
        }
        
        //This handles the +40% Enemy Speed, -50% Enemy Cooldowns, and other stats
        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender,
            RecalculateStatsAPI.StatHookEventArgs args) {
            if (!sender) return;
            if (sender.teamComponent.teamIndex == TeamIndex.Player) return;
            
            if (getFuckedLMAO) args.healthMultAdd += 1.0f;

            if (!doNerfsThisRun) {
                args.attackSpeedMultAdd += 0.5f;
                args.moveSpeedMultAdd += 0.4f;
                args.cooldownReductionAdd += 0.5f;
                return;
            }
            
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
            if(doGoldThisRun) self.goldRewardCoefficient *= 0.9f;
            origAwake(self);
        }

        // This handles the +20% Loot Spawn Rate stat
        private void SceneDirector_onPrePopulateSceneServer(SceneDirector self) {
            self.interactableCredit = (int)(self.interactableCredit * 1.2);
            Log.Info("Updated Credits: " + self.interactableCredit);
        }

        // Set up Loitering Punishment
        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage beginStage, Run self) {
            teleporterHit = false;
            teleporterExists = false;
            getFuckedLMAO = false;
            Log.Info("Stage begin! Waiting for Teleporter to be created.");
            beginStage(self);
        }
        
        // If a teleporter does not exist on the stage the loitering penalty should not be applied
        private void Run_OnServerTeleporterPlaced(On.RoR2.Run.orig_OnServerTeleporterPlaced teleporterPlaced, Run self, SceneDirector sceneDirector, GameObject thing) {
            teleporterExists = true;
            stagePunishTimer = self.NetworkfixedTime + 300f;
            Log.Info("Teleporter created! Timer set to " + stagePunishTimer);
            teleporterPlaced(self, sceneDirector, thing);
        }
        
        // Enforcing loitering penalty
        private void FixedUpdate() {
            if (!shouldRun) return;
            if (!doLoiterThisRun) {
#if DEBUG
                ReportLoiterError("Loiter penalty disabled");
#endif
                return;
            }
            if (Run.instance.isGameOverServer) {
#if DEBUG
                ReportLoiterError("Game Over");
#endif
                return;
            }
            if (teleporterHit) {
#if DEBUG
                ReportLoiterError("Teleporter hit");
#endif
                return;
            }
            if (!teleporterExists) {
#if DEBUG
                ReportLoiterError("Teleporter does not exist");
#endif
                return;
            }
            if (getFuckedLMAO){
#if DEBUG
                ReportLoiterError("Time's up");
#endif
                return;
            }
            if (stagePunishTimer >= Run.instance.NetworkfixedTime) {
#if DEBUG
                ReportLoiterError("Not time yet");
#endif
                return;
            }
            Log.Info("Time's up! Loitering penalty has been applied. StagePunishTimer " + stagePunishTimer);
            getFuckedLMAO = true;
            if (!swarmsEnabled) RunArtifactManager.instance.SetArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef, true);
        }
        
#if DEBUG
        // Report why loitering hasn't been enabled every 5 seconds
        private static float reportErrorTime;
        private void ReportLoiterError(string err) {
            if (reportErrorTime >= Run.instance.NetworkfixedTime) return;
            Log.Info(err);
            reportErrorTime = Run.instance.NetworkfixedTime + 5f;
        }
#endif
        
        // Disable loitering penalty when the teleporter is interacted with
        private void OnInteractTeleporter(On.RoR2.TeleporterInteraction.IdleState.orig_OnInteractionBegin interact, EntityStates.BaseState teleporterState, Interactor interactor) {
            if (!swarmsEnabled && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef)) 
                RunArtifactManager.instance.SetArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef, false);
            getFuckedLMAO = false;
            teleporterHit = true;
            interact(teleporterState, interactor);
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
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, CharacterBody, float>>((toRecharge, cb) => {
                if (cb.teamComponent.teamIndex != TeamIndex.Player) return toRecharge;
                return toRecharge / shieldRechargeOverride;
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
        
        // This buffs Acrid's Vicious Wounds and Ravenous Bite
        private void AcridRegenBuff(ILContext il) {
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
