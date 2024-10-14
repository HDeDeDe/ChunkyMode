using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HDeMods
{
    [BepInDependency(DifficultyAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(DirectorAPI.PluginGUID)]
    [BepInDependency(R2API.Networking.NetworkingAPI.PluginGUID)]
    [BepInDependency(HealthComponentAPI.PluginGUID)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ProperSave.ProperSavePlugin.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("_com.prodzpod.ProdzpodSpikestripContent", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rob.Hunk", BepInDependency.DependencyFlags.SoftDependency)]
    //[BepInDependency(EnrageArtifact.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ChunkyMode : BaseUnityPlugin
    {
        // Plugin details
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "HDeDeDe";
        public const string PluginName = "ChunkyMode";
        public const string PluginVersion = "0.2.4";

        // Difficulty related variables
        public AssetBundle ChunkyModeDifficultyModBundle;
        public static DifficultyDef ChunkyModeDifficultyDef;
        public static DifficultyIndex ChunkyModeDifficultyIndex;
        
        // Run start checks
        private static bool shouldRun;
        private static int ogMonsterCap;
        private static int ogRunLevelCap;
        
        // These are related to the loitering penalty
        private static bool getFuckedLMAO;
        private static bool teleporterExists;
        private static float stagePunishTimer;
        private static bool teleporterHit;
        private static float enemyWaveTimerRefresh;
        
        // These are related to random enemy speaking
        private static float enemyYapTimer;
        
        // These values can be changed by the player through config options
        public static ConfigEntry<bool> doHealingBuffs { get; set; }
        public static ConfigEntry<bool> doLoiterPenalty { get; set; }
        public static ConfigEntry<bool> doEnemyLimitBoost { get; set; }
        public static ConfigEntry<bool> doGoldPenalty { get; set; }
        public static ConfigEntry<bool> doEnemyNerfs { get; set; }
        
        public static ConfigEntry<int> enemyChanceToYap { get; set; }
        public static ConfigEntry<float> enemyYapCooldown { get; set; }
        public static ConfigEntry<float> timeUntilLoiterPenalty { get; set; }
        public static ConfigEntry<float> loiterPenaltyFrequency { get; set; }
        public static ConfigEntry<float> loiterPenaltySeverity { get; set; }
        
        public void Awake()
        {
            Log.Init(Logger);
#if DEBUG
            On.RoR2.SteamworksClientManager.ctor += KillOnThreePercentBug;
#endif
            ChunkyModeDifficultyModBundle = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("ChunkyMode.dll", "chunkydifficon"));
            AddDifficulty();
            BindSettings();
            ChunkyRunInfo.Instance = new ChunkyRunInfo();
            if (ChunkyOptionalMods.Saving.enabled) ChunkyOptionalMods.Saving.SetUp();
            //if (ChunkyEnrage.enabled) ChunkyEnrage.PerformCrime();
            ChunkyASeriesOfTubes.SetUpNetworking();
            
            Run.onRunSetRuleBookGlobal += Run_onRunSetRuleBookGlobal;
            Run.onRunStartGlobal += Run_onRunStartGlobal;
            Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;
            RoR2Application.onLoad += ChunkyCachedIndexes.GenerateCache;
            
            ChatMessageBase.chatMessageTypeToIndex.Add(typeof(ChunkyChatEnemyYap), (byte)ChatMessageBase.chatMessageIndexToType.Count);
            ChatMessageBase.chatMessageIndexToType.Add(typeof(ChunkyChatEnemyYap));
        }
        
#if DEBUG
        public void KillOnThreePercentBug(On.RoR2.SteamworksClientManager.orig_ctor ctor, SteamworksClientManager self) {
            try {
                ctor(self);
            }
            catch (Exception err) {
                Log.Fatal(err);
                Application.Quit();
                throw;
            }
            On.RoR2.SteamworksClientManager.ctor -= KillOnThreePercentBug;
        }
#endif

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
                "Enables enemy nerfs. Disable if you like unreactable Wandering Vagrants.");
            enemyChanceToYap = Config.Bind<int>(
                "Yapping",
                "Enemy Yap Chance",
                30,
                "The probability of enemies to yap. Set to 0 to stop the yapping.");
            enemyYapCooldown = Config.Bind<float>(
                "Yapping",
                "Enemy Yap Cooldown",
                30f,
                "The amount of time before enemies are allowed to yap again. Set to 0 for turbo yapping.");
            timeUntilLoiterPenalty = Config.Bind<float>(
                "Loitering",
                "Time until loiter penalty",
                300f,
                "The amount of time from the start of the stage until the loiter penalty is enforced. Minimum of 60 seconds.");
            loiterPenaltyFrequency = Config.Bind<float>(
                "Loitering",
                "Loiter penalty frequency",
                5f,
                "The amount of time between forced enemy spawns.");
            loiterPenaltySeverity = Config.Bind<float>(
                "Loitering",
                "Loiter penalty severity",
                40f,
                "The strength of spawned enemies. 40 is equal to 1 combat shrine.");
            if (!ChunkyOptionalMods.RoO.enabled) return;
            ChunkyOptionalMods.RoO.AddCheck(doHealingBuffs);
            ChunkyOptionalMods.RoO.AddCheck(doLoiterPenalty);
            ChunkyOptionalMods.RoO.AddCheck(doEnemyLimitBoost);
            ChunkyOptionalMods.RoO.AddCheck(doGoldPenalty);
            ChunkyOptionalMods.RoO.AddCheck(doEnemyNerfs);
            ChunkyOptionalMods.RoO.AddInt(enemyChanceToYap, 0, 100000);
            ChunkyOptionalMods.RoO.AddFloat(enemyYapCooldown, 0f, 600f);
            ChunkyOptionalMods.RoO.AddFloat(timeUntilLoiterPenalty, 60f, 600f);
            ChunkyOptionalMods.RoO.AddFloat(loiterPenaltyFrequency, 0f, 60f);
            ChunkyOptionalMods.RoO.AddFloat(loiterPenaltySeverity, 10f, 100f);
            ChunkyOptionalMods.RoO.SetSprite(ChunkyModeDifficultyModBundle.LoadAsset<Sprite>("texChunkyModeDiffIcon"));
            ChunkyOptionalMods.RoO.SetDescriptionToken("CHUNKYMODEDIFFMOD_RISK_OF_OPTIONS_DESCRIPTION");
        }
        
        private void Run_onRunSetRuleBookGlobal(Run arg1, RuleBook arg2)
        {
            if (arg1.selectedDifficulty != ChunkyModeDifficultyIndex) return;
            ogRunLevelCap = Run.ambientLevelCap;
            Run.ambientLevelCap += 9900;
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
            
            if (!ChunkyRunInfo.preSet) {
                Config.Reload();
                ChunkyRunInfo.Instance.doEnemyBoostThisRun = doEnemyLimitBoost.Value;
                ChunkyRunInfo.Instance.doHealBuffThisRun = doHealingBuffs.Value;
                ChunkyRunInfo.Instance.doGoldThisRun = doGoldPenalty.Value;
                ChunkyRunInfo.Instance.doNerfsThisRun = doEnemyNerfs.Value;
                ChunkyRunInfo.Instance.doLoiterThisRun = doLoiterPenalty.Value;
                ChunkyRunInfo.Instance.enemyChanceToYapThisRun = enemyChanceToYap.Value;
                ChunkyRunInfo.Instance.enemyYapCooldownThisRun = enemyYapCooldown.Value;
                ChunkyRunInfo.Instance.loiterPenaltyTimeThisRun = timeUntilLoiterPenalty.Value;
                ChunkyRunInfo.Instance.loiterPenaltyFrequencyThisRun = loiterPenaltyFrequency.Value;
                ChunkyRunInfo.Instance.loiterPenaltySeverityThisRun = loiterPenaltySeverity.Value;
            }
            
            ChunkyASeriesOfTubes.DoNetworkingStuff();
            
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
#if DEBUG
            reportErrorAnyway = true;
#endif
            if (!NetworkServer.active) return;

            if (ChunkyRunInfo.Instance.doEnemyBoostThisRun){ 
                //Thanks Starstorm 2 :)
                TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit = (int)(ogMonsterCap * 1.5);
                TeamCatalog.GetTeamDef(TeamIndex.Void).softCharacterLimit = (int)(ogMonsterCap * 1.5);
                TeamCatalog.GetTeamDef(TeamIndex.Lunar).softCharacterLimit = (int)(ogMonsterCap * 1.5);
            }

            HealthComponentAPI.GetHealthStats += ChunkyILHooks.ShieldRechargeAndBarrierDecayRate;
            if (ChunkyRunInfo.Instance.doHealBuffThisRun){ 
                IL.EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile.HealPulse += ChunkyILHooks.REXHealPulse;
                IL.RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted.OnDamageInflictedServer += ChunkyILHooks.REXPrimaryAttack;
                IL.RoR2.CharacterBody.RecalculateStats += ChunkyILHooks.AcridRegenBuff; 
            }
            
            SceneDirector.onPrePopulateSceneServer += SceneDirector_onPrePopulateSceneServer;
            On.RoR2.CombatDirector.Awake += CombatDirector_Awake;
            HealthComponentAPI.GetHealStats += ChunkyILHooks.HealingOverride;
            On.RoR2.Run.BeginStage += Run_BeginStage;
            
            if (!ChunkyRunInfo.Instance.doLoiterThisRun) return;
            On.RoR2.Run.OnServerTeleporterPlaced += Run_OnServerTeleporterPlaced;
            On.RoR2.TeleporterInteraction.IdleState.OnInteractionBegin += OnInteractTeleporter;
            On.RoR2.CombatDirector.Simulate += CombatDirector_Simulate;
        }

        private void Run_onRunDestroyGlobal(Run run) {
            if (!shouldRun) return;
            Log.Info("Chunky Mode Run ended");
            shouldRun = false;
            ChunkyRunInfo.preSet = false;
            Run.ambientLevelCap = ogRunLevelCap;
            
            TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit = ogMonsterCap;
            TeamCatalog.GetTeamDef(TeamIndex.Void).softCharacterLimit = ogMonsterCap;
            TeamCatalog.GetTeamDef(TeamIndex.Lunar).softCharacterLimit = ogMonsterCap;
            
            HealthComponentAPI.GetHealthStats -= ChunkyILHooks.ShieldRechargeAndBarrierDecayRate;
            IL.EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile.HealPulse -= ChunkyILHooks.REXHealPulse;
            IL.RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted.OnDamageInflictedServer -= ChunkyILHooks.REXPrimaryAttack;
            IL.RoR2.CharacterBody.RecalculateStats -= ChunkyILHooks.AcridRegenBuff;
            
            RecalculateStatsAPI.GetStatCoefficients -= RecalculateStatsAPI_GetStatCoefficients;
            HealthComponentAPI.GetHealStats -= ChunkyILHooks.HealingOverride;
            On.RoR2.CombatDirector.Awake -= CombatDirector_Awake;
            SceneDirector.onPrePopulateSceneServer -= SceneDirector_onPrePopulateSceneServer;
            
            On.RoR2.Run.OnServerTeleporterPlaced -= Run_OnServerTeleporterPlaced;
            On.RoR2.Run.BeginStage -= Run_BeginStage;
            On.RoR2.TeleporterInteraction.IdleState.OnInteractionBegin -= OnInteractTeleporter;
            On.RoR2.CombatDirector.Simulate -= CombatDirector_Simulate;
        }
        
        //This handles the +40% Enemy Speed, -50% Enemy Cooldowns, and other stats
        public static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender,
            RecalculateStatsAPI.StatHookEventArgs args) {
            if (!sender) return;
            if (sender.teamComponent.teamIndex == TeamIndex.Player) return;
            
            int funko = UnityEngine.Random.RandomRangeInt(0, 100000);
            int yap = ChunkyRunInfo.Instance.enemyChanceToYapThisRun;
            if (getFuckedLMAO) yap *= 2;
            
            if (NetworkServer.active && funko < yap && ChunkyRunInfo.Instance.enemyChanceToYapThisRun > 0 && enemyYapTimer < Run.instance.NetworkfixedTime) {
                enemyYapTimer = Run.instance.NetworkfixedTime + ChunkyRunInfo.Instance.enemyYapCooldownThisRun;
                ChunkyYap.DoYapping(sender.baseNameToken);
            }

            if (!ChunkyRunInfo.Instance.doNerfsThisRun) {
                args.attackSpeedMultAdd += 0.5f;
                args.moveSpeedMultAdd += 0.4f;
                args.cooldownReductionAdd += 0.5f;
                return;
            }

            ChunkyCachedIndexes.Body.TryGetValue(sender.bodyIndex, out BodyCache bodyIndex);
#if DEBUG
            Log.Debug(sender.name + ", " + sender.bodyIndex);
            Log.Debug(bodyIndex);
#endif
            switch (bodyIndex) {
                case BodyCache.BeetleGuard:
                    args.moveSpeedMultAdd += 0.4f;
                    args.cooldownReductionAdd += 0.5f;
                    break;
                case BodyCache.Vagrant:
                    args.attackSpeedMultAdd += 0.25f;
                    args.moveSpeedMultAdd += 0.4f;
                    args.cooldownReductionAdd += 0.5f;
                    break;
                case BodyCache.Bell:
                    args.attackSpeedMultAdd += 2f;
                    args.moveSpeedMultAdd += 0.4f;
                    args.cooldownMultAdd += 0.25f;
                    break;
                case BodyCache.RobNemesis:
                    args.attackSpeedMultAdd += 0.25f;
                    args.moveSpeedMultAdd += 0.15f;
                    args.cooldownReductionAdd += 0.5f;
                    break;
                case BodyCache.SigmaConstruct:
                    args.attackSpeedMultAdd += 0.25f;
                    args.cooldownReductionAdd += 0.5f;
                    break;
                case BodyCache.FlyingVermin:
                    args.attackSpeedMultAdd += 0.15f;
                    args.moveSpeedMultAdd += 0.4f;
                    break;
                case BodyCache.Bison:
                    args.attackSpeedMultAdd += 2f;
                    args.moveSpeedMultAdd += 0.4f;
                    args.cooldownReductionAdd += 0.5f;
                    break;
                case BodyCache.None:
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
            if(ChunkyRunInfo.Instance.doGoldThisRun) self.goldRewardCoefficient *= 0.9f;
            origAwake(self);
        }

        // This handles the +20% Loot Spawn Rate stat
        private void SceneDirector_onPrePopulateSceneServer(SceneDirector self) {
            self.interactableCredit = (int)(self.interactableCredit * 1.2);
            Log.Info("Updated Credits: " + self.interactableCredit);
        }

        // Set up Loitering Punishment
        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage beginStage, Run self) {
            enemyYapTimer = self.NetworkfixedTime + 10f;
#if DEBUG
            Log.Debug("Stage begin, setting allowedToSpeakTimer to " + enemyYapTimer);
#endif
            if (!ChunkyRunInfo.Instance.doLoiterThisRun) {
                beginStage(self);
                return;
            }

            enemyWaveTimerRefresh = 0f;
            teleporterHit = false;
            teleporterExists = false;
            getFuckedLMAO = false;
            Log.Info("Stage begin! Waiting for Teleporter to be created.");
            beginStage(self);
        }
        
        // If a teleporter does not exist on the stage the loitering penalty should not be applied
        private void Run_OnServerTeleporterPlaced(On.RoR2.Run.orig_OnServerTeleporterPlaced teleporterPlaced, Run self, SceneDirector sceneDirector, GameObject thing) {
            teleporterExists = true;
            stagePunishTimer = self.NetworkfixedTime + ChunkyRunInfo.Instance.loiterPenaltyTimeThisRun;
            Log.Info("Teleporter created! Timer set to " + stagePunishTimer);
            teleporterPlaced(self, sceneDirector, thing);
        }
        
        // The loitering penalty
        private void CombatDirector_Simulate(On.RoR2.CombatDirector.orig_Simulate simulate, CombatDirector self, float deltaTime) {
            if (!getFuckedLMAO || teleporterHit || Run.instance.NetworkfixedTime < enemyWaveTimerRefresh) {
                simulate(self, deltaTime);
                return;
            }
#if DEBUG
            Log.Debug("Attempting to spawn enemy wave");
#endif
            float newCreditBalance = ChunkyRunInfo.Instance.loiterPenaltySeverityThisRun * Stage.instance.entryDifficultyCoefficient;
            float oldTimer = self.monsterSpawnTimer - deltaTime;
            DirectorCard oldEnemy = self.currentMonsterCard;
            DirectorCard newEnemy = self.SelectMonsterCardForCombatShrine(newCreditBalance);

            if (newEnemy == null) {
#if DEBUG
                Log.Error("Invalid enemy. Retrying next update.");
#endif
                simulate(self, deltaTime);
                return;
            }
#if DEBUG
            Log.Debug("Spawning enemy wave");
#endif
            enemyWaveTimerRefresh = Run.instance.NetworkfixedTime + ChunkyRunInfo.Instance.loiterPenaltyFrequencyThisRun;
            
            //Thank you .score for pointing out CombatDirector.CombatShrineActivation
            self.monsterSpawnTimer = 0f;
            self.monsterCredit =+ newCreditBalance;
            self.OverrideCurrentMonsterCard(newEnemy);
            
            simulate(self, deltaTime);

            self.monsterSpawnTimer = oldTimer;
            if (oldEnemy != null) self.OverrideCurrentMonsterCard(oldEnemy);
        }
        
        // Disable loitering penalty when the teleporter is interacted with
        private void OnInteractTeleporter(On.RoR2.TeleporterInteraction.IdleState.orig_OnInteractionBegin interact, EntityStates.BaseState teleporterState, Interactor interactor) {
            getFuckedLMAO = false;
            teleporterHit = true;
            interact(teleporterState, interactor);
        }
        
        // Enforcing loitering penalty
        private void FixedUpdate() {
            if (!shouldRun) return;
            if (!NetworkServer.active) {
#if DEBUG
                ReportLoiterError("Client can not enforce loiter penalty.");
#endif
                return;
            }
            if (!ChunkyRunInfo.Instance.doLoiterThisRun) {
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
            ChunkyYap.DoWarning();
        }
        
#if DEBUG
        // Report why loitering hasn't been enabled every 5 seconds
        private static float reportErrorTime;
        private static bool reportErrorAnyway;
        private void ReportLoiterError(string err) {
            if (reportErrorTime >= Run.instance.NetworkfixedTime && !reportErrorAnyway) return;
            Log.Debug(err);
            reportErrorTime = Run.instance.NetworkfixedTime + 5f;
            reportErrorAnyway = false;
        }
#endif
    }
}
