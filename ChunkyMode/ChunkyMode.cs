// ReSharper disable once RedundantUsingDirective
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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
    [BepInDependency("bubbet.riskui", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class ChunkyMode : BaseUnityPlugin
    {
        // Plugin details
        public const string PluginGUID = "com." + PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "HDeDeDe";
        public const string PluginName = "ChunkyMode";
        public const string PluginVersion = "0.3.1";
        public static ChunkyMode instance;
        public static ConfigFile experiments;

        // Difficulty related variables
        public static AssetBundle ChunkyModeDifficultyModBundle;
        public static DifficultyDef ChunkyModeDifficultyDef;
        public static DifficultyIndex ChunkyModeDifficultyIndex;
        public static GameObject ChunkyInfo;
        private static GameObject m_chunkyInfo;
        
        // Run start checks
        private static bool shouldRun;
        private static int ogMonsterCap;
        private static int ogRunLevelCap;
        internal static bool isSimulacrumRun;
        internal static bool waveStarted;
        
        // These are related to the loitering penalty
        private static bool teleporterExists;
        private static float stagePunishTimer;
        private static bool teleporterHit;
        internal static int totalBlindPest;
        internal static int totalLemurians;
        
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
        
        public static ConfigEntry<bool> experimentCursePenalty { get; set; }
        public static ConfigEntry<float> experimentCurseRate { get; set; }
        public static ConfigEntry<bool> limitPest { get; set; }
        public static ConfigEntry<float> limitPestAmount { get; set; }
        
        public void Awake()
        {
            if (instance != null) {
                CM.Log.Error("There can be only 1 instance of " + PluginName + "!");
                Destroy(this);
                return;
            }
            
            CM.Log.Init(Logger);
            instance = this;
#if DEBUG
            //HotCompilerNamespace.HotCompiler.CompileIt();
            On.RoR2.SteamworksClientManager.ctor += KillOnThreePercentBug;
#endif
            ChunkyModeDifficultyModBundle = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("ChunkyMode.dll", "chunkydifficon"));
            AddDifficulty();
            BindSettings();

            GameObject temp = new GameObject("thing");
            temp.AddComponent<NetworkIdentity>();
            ChunkyInfo = temp.InstantiateClone("ChunkyRunInfo");
            Destroy(temp);
            ChunkyInfo.AddComponent<ChunkyRunInfo>();
            
            if (ChunkyOptionalMods.Saving.Enabled) ChunkyOptionalMods.Saving.SetUp();

            
            Run.onRunSetRuleBookGlobal += Run_onRunSetRuleBookGlobal;
            Run.onRunStartGlobal += Run_onRunStartGlobal;
            Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;
            RoR2Application.onLoad += ChunkyCachedIndexes.GenerateCache;

            if (ChunkyOptionalMods.RiskUI.Enabled) ChunkyOptionalMods.RiskUI.AddChunkyMode();
            
            ChatMessageBase.chatMessageTypeToIndex.Add(typeof(ChunkyChatEnemyYap), (byte)ChatMessageBase.chatMessageIndexToType.Count);
            ChatMessageBase.chatMessageIndexToType.Add(typeof(ChunkyChatEnemyYap));
        }
        
        private void FixedUpdate() {
            EnforceLoiter();
        }
        
#if DEBUG
        public static void KillOnThreePercentBug(On.RoR2.SteamworksClientManager.orig_ctor ctor, SteamworksClientManager self) {
            try {
                ctor(self);
            }
            catch (Exception err) {
                CM.Log.Fatal(err);
                Application.Quit();
                throw;
            }
            On.RoR2.SteamworksClientManager.ctor -= KillOnThreePercentBug;
        }
        
        /*[ConCommand(commandName = "hotReloadChunky", flags = ConVarFlags.Engine, helpText = "Hot Reload ChunkyMode")]
        public static void Reload(ConCommandArgs args) => HotCompilerNamespace.HotCompiler.CompileIt();*/
#endif

        public static void AddDifficulty() {
            ChunkyModeDifficultyDef = new DifficultyDef(4f,
                "CHUNKYMODEDIFFMOD_NAME",
                "CHUNKYMODEDIFFMOD_ICON",
                "CHUNKYMODEDIFFMOD_DESCRIPTION",
                new Color32(61, 25, 255, 255),
                "cm",
                true
            ) {
                iconSprite = ChunkyModeDifficultyModBundle.LoadAsset<Sprite>("texChunkyModeDiffIcon"),
                foundIconSprite = true
            };
            ChunkyModeDifficultyIndex = DifficultyAPI.AddDifficulty(ChunkyModeDifficultyDef);
        }

        public static void BindSettings() {
            experiments = new ConfigFile(Paths.ConfigPath + @"\" + PluginGUID + ".experiments.cfg", false, instance.Info.Metadata);
            
            doHealingBuffs = instance.Config.Bind<bool>(
                "Unlisted Difficulty Modifiers",
                "Do Healing Buffs",
                true,
                "Enables buffs to some survivor healing skills. Disable if you want a harder challenge.");
            doLoiterPenalty = instance.Config.Bind<bool>(
                "Unlisted Difficulty Modifiers",
                "Do Loiter Penalty",
                true,
                "Enables a 5 minute loiter penalty on stages with a teleporter. Not recommended to disable.");
            doEnemyLimitBoost = instance.Config.Bind<bool>(
                "Unlisted Difficulty Modifiers",
                "Do Enemy Limit Boost",
                true,
                "Enables enemy limit increase. If your computer is struggling to run on Chunky Mode, consider disabling this.");
            doGoldPenalty = instance.Config.Bind<bool>(
                "Unlisted Difficulty Modifiers",
                "Do Gold Penalty",
                true,
                "Enables a -10% gold penalty. Disable to speed up gameplay.");
            doEnemyNerfs = instance.Config.Bind<bool>(
                "Unlisted Difficulty Modifiers",
                "Do Enemy Nerfs",
                true,
                "Enables enemy nerfs. Disable if you like unreactable Wandering Vagrants.");
            enemyChanceToYap = instance.Config.Bind<int>(
                "Yapping",
                "Enemy Yap Chance",
                30,
                "The probability of enemies to yap. Set to 0 to stop the yapping.");
            enemyYapCooldown = instance.Config.Bind<float>(
                "Yapping",
                "Enemy Yap Cooldown",
                30f,
                "The amount of time before enemies are allowed to yap again. Set to 0 for turbo yapping.");
            timeUntilLoiterPenalty = instance.Config.Bind<float>(
                "Loitering",
                "Time until loiter penalty",
                300f,
                "The amount of time from the start of the stage until the loiter penalty is enforced. Minimum of 15 seconds.");
            loiterPenaltyFrequency = instance.Config.Bind<float>(
                "Loitering",
                "Loiter penalty frequency",
                5f,
                "The amount of time between forced enemy spawns.");
            loiterPenaltySeverity = instance.Config.Bind<float>(
                "Loitering",
                "Loiter penalty severity",
                40f,
                "The strength of spawned enemies. 40 is equal to 1 combat shrine.");
            limitPest = instance.Config.Bind<bool>(
                "Loitering",
                "Limit Blind Pest",
                true,
                "Enable Blind Pest limit. This also affects Lemurians when playing Simulacrum.");
            limitPestAmount = instance.Config.Bind<float>(
                "Loitering",
                "Blind Pest Amount",
                10f,
                "The percentage of enemies that are allowed to be blind pest. " +
                "Only affects the Loitering penalty (unless playing Simulacrum).");
            
            experimentCursePenalty = experiments.Bind<bool>(
                "Experiments",
                "Curse Penalty",
                false,
                "Enable experimental curse penalty. This will not be a part of standard gameplay.");
            experimentCurseRate = experiments.Bind<float>(
                "Experiments",
                "Curse Rate",
                0.035f,
                "The amount of curse applied each loiter tick.");
            
            if (!ChunkyOptionalMods.RoO.Enabled) return;
            ChunkyOptionalMods.RoO.AddCheck(doHealingBuffs);
            ChunkyOptionalMods.RoO.AddCheck(doLoiterPenalty);
            ChunkyOptionalMods.RoO.AddCheck(doEnemyLimitBoost);
            ChunkyOptionalMods.RoO.AddCheck(doGoldPenalty);
            ChunkyOptionalMods.RoO.AddCheck(doEnemyNerfs);
            ChunkyOptionalMods.RoO.AddInt(enemyChanceToYap, 0, 100000);
            ChunkyOptionalMods.RoO.AddFloat(enemyYapCooldown, 0f, 600f, "{0}");
            ChunkyOptionalMods.RoO.AddFloat(timeUntilLoiterPenalty, 15f, 600f, "{0}");
            ChunkyOptionalMods.RoO.AddFloat(loiterPenaltyFrequency, 0f, 60f, "{0}");
            ChunkyOptionalMods.RoO.AddFloat(loiterPenaltySeverity, 10f, 100f);
            ChunkyOptionalMods.RoO.AddCheck(experimentCursePenalty);
            ChunkyOptionalMods.RoO.AddFloat(experimentCurseRate, 0f, 1f, "{0}");
            ChunkyOptionalMods.RoO.AddCheck(limitPest);
            ChunkyOptionalMods.RoO.AddFloat(limitPestAmount, 0f, 100f);
            ChunkyOptionalMods.RoO.SetDescriptionToken("CHUNKYMODEDIFFMOD_RISK_OF_OPTIONS_DESCRIPTION");
        }
        
        internal static void Run_onRunSetRuleBookGlobal(Run arg1, RuleBook arg2)
        {
            if (arg1.selectedDifficulty != ChunkyModeDifficultyIndex) return;
            if (arg1.GetType() == typeof(InfiniteTowerRun)) isSimulacrumRun = true;
            ogRunLevelCap = Run.ambientLevelCap;
            Run.ambientLevelCap += 9900;
        }

        internal static void Run_onRunStartGlobal(Run run) {
            shouldRun = false;
            teleporterHit = false;
            teleporterExists = false;
            totalBlindPest = 0;
            totalLemurians = 0;
            ogMonsterCap = TeamCatalog.GetTeamDef(TeamIndex.Monster)!.softCharacterLimit;
            
            if (run.selectedDifficulty != ChunkyModeDifficultyIndex) return;
            CM.Log.Info("Chunky Mode Run started");
            shouldRun = true;
            
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
#if DEBUG
            reportErrorAnyway = true;
#endif
            if (!NetworkServer.active) return;

            m_chunkyInfo = Instantiate(ChunkyInfo);
            NetworkServer.Spawn(m_chunkyInfo);
            
            if (!ChunkyRunInfo.preSet) {
                instance.Config.Reload();
                ChunkyRunInfo.instance.doEnemyBoostThisRun = doEnemyLimitBoost.Value;
                ChunkyRunInfo.instance.doHealBuffThisRun = doHealingBuffs.Value;
                ChunkyRunInfo.instance.doGoldThisRun = doGoldPenalty.Value;
                ChunkyRunInfo.instance.doNerfsThisRun = doEnemyNerfs.Value;
                ChunkyRunInfo.instance.doLoiterThisRun = doLoiterPenalty.Value;
                ChunkyRunInfo.instance.enemyChanceToYapThisRun = enemyChanceToYap.Value;
                ChunkyRunInfo.instance.enemyYapCooldownThisRun = enemyYapCooldown.Value;
                ChunkyRunInfo.instance.loiterPenaltyTimeThisRun = timeUntilLoiterPenalty.Value;
                ChunkyRunInfo.instance.loiterPenaltyFrequencyThisRun = loiterPenaltyFrequency.Value;
                ChunkyRunInfo.instance.loiterPenaltySeverityThisRun = loiterPenaltySeverity.Value;
                ChunkyRunInfo.instance.experimentCursePenaltyThisRun = experimentCursePenalty.Value;
                ChunkyRunInfo.instance.experimentCurseRateThisRun = experimentCurseRate.Value;
                ChunkyRunInfo.instance.limitPestsThisRun = limitPest.Value;
                ChunkyRunInfo.instance.limitPestsAmountThisRun = limitPestAmount.Value;
            }

            if (ChunkyRunInfo.instance.doEnemyBoostThisRun){ 
                //Thanks Starstorm 2 :)
                TeamCatalog.GetTeamDef(TeamIndex.Monster)!.softCharacterLimit = (int)(ogMonsterCap * 1.5);
                TeamCatalog.GetTeamDef(TeamIndex.Void)!.softCharacterLimit = (int)(ogMonsterCap * 1.5);
                TeamCatalog.GetTeamDef(TeamIndex.Lunar)!.softCharacterLimit = (int)(ogMonsterCap * 1.5);
            }

            HealthComponentAPI.GetHealthStats += ChunkyILHooks.ShieldRechargeAndBarrierDecayRate;
            if (ChunkyRunInfo.instance.doHealBuffThisRun){ 
                IL.EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile.HealPulse += ChunkyILHooks.REXHealPulse;
                IL.RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted.OnDamageInflictedServer += ChunkyILHooks.REXPrimaryAttack;
                IL.RoR2.CharacterBody.RecalculateStats += ChunkyILHooks.AcridRegenBuff; 
            }
            
            SceneDirector.onPrePopulateSceneServer += SceneDirector_onPrePopulateSceneServer;
            On.RoR2.CombatDirector.Awake += CombatDirector_Awake;
            HealthComponentAPI.GetHealStats += ChunkyILHooks.HealingOverride;
            On.RoR2.Run.BeginStage += Run_BeginStage;

            if (ChunkyRunInfo.instance.limitPestsThisRun) {
                CharacterBody.onBodyStartGlobal += TrackShittersAdd;
                CharacterBody.onBodyDestroyGlobal += TrackShittersRemove;
            }
                
            if (isSimulacrumRun) {
                waveStarted = false;
                
                InfiniteTowerRun.onAllEnemiesDefeatedServer += ChunkySimulacrum.OnAllEnemiesDefeatedServer;
                On.RoR2.InfiniteTowerRun.BeginNextWave += ChunkySimulacrum.InfiniteTowerRun_BeginNextWave;
                IL.RoR2.InfiniteTowerWaveController.Initialize +=
                    ChunkySimulacrum.InfiniteTowerWaveController_Initialize;
                IL.RoR2.InfiniteTowerWaveController.FixedUpdate +=
                    ChunkySimulacrum.InfiniteTowerWaveController_FixedUpdate;
                IL.RoR2.CombatDirector.AttemptSpawnOnTarget +=
                    ChunkySimulacrum.ExtractRNGFromCombatDirector;
                On.RoR2.CombatDirector.PrepareNewMonsterWave += ChunkySimulacrum.CombatDirector_PrepareNewMonsterWave;
                return;
            }
            
            if (!ChunkyRunInfo.instance.doLoiterThisRun) return;
            On.RoR2.Run.OnServerTeleporterPlaced += Run_OnServerTeleporterPlaced;
            On.RoR2.TeleporterInteraction.IdleState.OnInteractionBegin += OnInteractTeleporter;
            On.RoR2.CombatDirector.Simulate += CombatDirector_Simulate;
        }

        internal static void Run_onRunDestroyGlobal(Run run) {
            if (!shouldRun) return;
            CM.Log.Info("Chunky Mode Run ended");
            shouldRun = false;
            isSimulacrumRun = false;
            ChunkyRunInfo.preSet = false;
            Run.ambientLevelCap = ogRunLevelCap;
            Destroy(m_chunkyInfo);
            
            TeamCatalog.GetTeamDef(TeamIndex.Monster)!.softCharacterLimit = ogMonsterCap;
            TeamCatalog.GetTeamDef(TeamIndex.Void)!.softCharacterLimit = ogMonsterCap;
            TeamCatalog.GetTeamDef(TeamIndex.Lunar)!.softCharacterLimit = ogMonsterCap;
            
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
            CharacterBody.onBodyStartGlobal -= TrackShittersAdd;
            CharacterBody.onBodyDestroyGlobal -= TrackShittersRemove;
            
            InfiniteTowerRun.onAllEnemiesDefeatedServer -= ChunkySimulacrum.OnAllEnemiesDefeatedServer;
            On.RoR2.InfiniteTowerRun.BeginNextWave -= ChunkySimulacrum.InfiniteTowerRun_BeginNextWave;
            IL.RoR2.InfiniteTowerWaveController.Initialize -=
                ChunkySimulacrum.InfiniteTowerWaveController_Initialize;
            IL.RoR2.InfiniteTowerWaveController.FixedUpdate -=
                ChunkySimulacrum.InfiniteTowerWaveController_FixedUpdate;
            IL.RoR2.CombatDirector.AttemptSpawnOnTarget -=
                ChunkySimulacrum.ExtractRNGFromCombatDirector;
            On.RoR2.CombatDirector.PrepareNewMonsterWave -= ChunkySimulacrum.CombatDirector_PrepareNewMonsterWave;
        }
        
        internal static void TrackShittersAdd(CharacterBody body) {
            if (body.bodyIndex == ChunkyCachedIndexes.bodyCache[BodyCache.FlyingVermin]) totalBlindPest++;
            if (body.bodyIndex == ChunkyCachedIndexes.bodyCache[BodyCache.Lemurian]) totalLemurians++;
        }
        internal static void TrackShittersRemove(CharacterBody body) {
            if (body.bodyIndex == ChunkyCachedIndexes.bodyCache[BodyCache.FlyingVermin]) totalBlindPest--;
            if (body.bodyIndex == ChunkyCachedIndexes.bodyCache[BodyCache.Lemurian]) totalLemurians--;
        }
        
        //This handles the +40% Enemy Speed, -50% Enemy Cooldowns, and other stats
        public static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender,
            RecalculateStatsAPI.StatHookEventArgs args) {
            if (!sender) return;
            
            if (sender.teamComponent.teamIndex == TeamIndex.Player) {
                args.baseCurseAdd += ChunkyRunInfo.instance.allyCurse;
                return;
            }

            if (!NetworkServer.active) goto ENEMYSTATS;
            
            int funko = UnityEngine.Random.RandomRangeInt(0, 100000);
            int yap = ChunkyRunInfo.instance.enemyChanceToYapThisRun;
            if (ChunkyRunInfo.instance.getFuckedLMAO) yap *= 2;
            
            if (funko < yap && ChunkyRunInfo.instance.enemyChanceToYapThisRun > 0 && enemyYapTimer < Run.instance.NetworkfixedTime) {
                enemyYapTimer = Run.instance.NetworkfixedTime + ChunkyRunInfo.instance.enemyYapCooldownThisRun;
                List<BuffIndex> eliteAffix = new List<BuffIndex>();
                if(sender.isElite) eliteAffix.AddRange(BuffCatalog.eliteBuffIndices.Where(sender.HasBuff));
                ChunkyYap.DoYapping(sender.baseNameToken, eliteAffix);
            }
            
ENEMYSTATS:
            if (!ChunkyRunInfo.instance.doNerfsThisRun) {
                args.attackSpeedMultAdd += 0.5f;
                args.moveSpeedMultAdd += 0.4f;
                args.cooldownReductionAdd += 0.5f;
                return;
            }

            ChunkyCachedIndexes.bodyIndex.TryGetValue(sender.bodyIndex, out BodyCache bodyIndex);
#if DEBUG
            if (!isSimulacrumRun) {
               CM.Log.Debug(sender.name + ", " + sender.bodyIndex);
               CM.Log.Debug(bodyIndex); 
            }
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
        internal static void CombatDirector_Awake(On.RoR2.CombatDirector.orig_Awake origAwake, CombatDirector self) {
            //Got this from Starstorm 2 :)
            self.creditMultiplier *= 1.1f;
            if(ChunkyRunInfo.instance.doGoldThisRun && !isSimulacrumRun) self.goldRewardCoefficient *= 0.9f;
            origAwake(self);
        }

        // This handles the +20% Loot Spawn Rate stat
        internal static void SceneDirector_onPrePopulateSceneServer(SceneDirector self) {
            self.interactableCredit = (int)(self.interactableCredit * 1.2);
            CM.Log.Info("Updated Credits: " + self.interactableCredit);
        }

        // Set up Loitering Punishment
        internal static void Run_BeginStage(On.RoR2.Run.orig_BeginStage beginStage, Run self) {
            enemyYapTimer = self.NetworkfixedTime + 10f;
#if DEBUG
            CM.Log.Debug("Stage begin, setting allowedToSpeakTimer to " + enemyYapTimer);
#endif
            if (!ChunkyRunInfo.instance.doLoiterThisRun) {
                beginStage(self);
                return;
            }

            ChunkyRunInfo.instance.loiterTick = 0f;
            teleporterHit = false;
            teleporterExists = false;
            ChunkyRunInfo.instance.allyCurse = 0;
            ChunkyRunInfo.instance.getFuckedLMAO = false;
            CM.Log.Info("Stage begin! Waiting for Teleporter to be created.");
            beginStage(self);
        }
        
        // If a teleporter does not exist on the stage the loitering penalty should not be applied
        internal static void Run_OnServerTeleporterPlaced(On.RoR2.Run.orig_OnServerTeleporterPlaced teleporterPlaced, Run self, SceneDirector sceneDirector, GameObject thing) {
            teleporterExists = true;
            stagePunishTimer = self.NetworkfixedTime + ChunkyRunInfo.instance.loiterPenaltyTimeThisRun;
            CM.Log.Info("Teleporter created! Timer set to " + stagePunishTimer);
            teleporterPlaced(self, sceneDirector, thing);
        }
        
        // The loitering penalty
        internal static void CombatDirector_Simulate(On.RoR2.CombatDirector.orig_Simulate simulate, CombatDirector self, float deltaTime) {
            if (!ChunkyRunInfo.instance.getFuckedLMAO || teleporterHit || Run.instance.NetworkfixedTime < ChunkyRunInfo.instance.loiterTick) {
                simulate(self, deltaTime);
                return;
            }
#if DEBUG
            CM.Log.Warning("Attempting to spawn enemy wave");
#endif
            int gougeCount = 1;

            if (ChunkyRunInfo.instance.experimentCursePenaltyThisRun) {
                gougeCount += Util.GetItemCountForTeam(TeamIndex.Player, RoR2Content.Items.MonstersOnShrineUse.itemIndex, false);
            }

            float newCreditBalance = ChunkyRunInfo.instance.loiterPenaltySeverityThisRun * Stage.instance.entryDifficultyCoefficient * gougeCount;
            float oldTimer = self.monsterSpawnTimer - deltaTime;
            DirectorCard oldEnemy = self.currentMonsterCard;
            DirectorCard newEnemy = self.SelectMonsterCardForCombatShrine(newCreditBalance);

            if (newEnemy == null) {
#if DEBUG
                CM.Log.Error("Invalid enemy. Retrying next update.");
#endif
                simulate(self, deltaTime);
                return;
            }
#if DEBUG
            CM.Log.Warning("Checking if " + newEnemy.spawnCard.prefab.name + " is a Blind Pest.");
#endif
            if (newEnemy.spawnCard.prefab.GetComponent<CharacterMaster>().bodyPrefab == BodyCatalog.GetBodyPrefab(ChunkyCachedIndexes.bodyCache[BodyCache.FlyingVermin]) && ChunkyRunInfo.instance.limitPestsThisRun) {
#if DEBUG
                CM.Log.Warning("Bastards detected, checking if we have too many.");
#endif
                int totalEnemies = 0;
                
                totalEnemies += TeamComponent.GetTeamMembers(TeamIndex.Monster).Count;
                totalEnemies += TeamComponent.GetTeamMembers(TeamIndex.Void).Count;
                totalEnemies += TeamComponent.GetTeamMembers(TeamIndex.Lunar).Count;

#if DEBUG
                CM.Log.Warning("Total enemies: " + totalEnemies);
                CM.Log.Warning("Too many? " + (totalBlindPest >= totalEnemies * (ChunkyRunInfo.instance.limitPestsAmountThisRun / 100f)));
#endif
                if (totalBlindPest >= totalEnemies * (ChunkyRunInfo.instance.limitPestsAmountThisRun / 100f)) {
                    CM.Log.Warning("Too many bastards. Retrying in the next update.");
                    simulate(self, deltaTime);
                    return;
                }
            }
#if DEBUG
            CM.Log.Debug("Spawning enemy wave");
#endif
            ChunkyRunInfo.instance.loiterTick = Run.instance.NetworkfixedTime + ChunkyRunInfo.instance.loiterPenaltyFrequencyThisRun;
            if (ChunkyRunInfo.instance.experimentCursePenaltyThisRun) {
                ChunkyRunInfo.instance.allyCurse += ChunkyRunInfo.instance.experimentCurseRateThisRun;
                ChunkyRunInfo.instance.RpcDirtyAss();
            }
                
            
            //Thank you .score for pointing out CombatDirector.CombatShrineActivation
            self.monsterSpawnTimer = 0f;
            self.monsterCredit =+ newCreditBalance;
            self.OverrideCurrentMonsterCard(newEnemy);
            
            simulate(self, deltaTime);

            self.monsterSpawnTimer = oldTimer;
            if (oldEnemy != null) self.OverrideCurrentMonsterCard(oldEnemy);
        }
        
        // Disable loitering penalty when the teleporter is interacted with
        // ReSharper disable once IdentifierTypo
        internal static void OnInteractTeleporter(On.RoR2.TeleporterInteraction.IdleState.orig_OnInteractionBegin interact, EntityStates.BaseState teleporterState, Interactor interactor) {
            ChunkyRunInfo.instance.getFuckedLMAO = false;
            teleporterHit = true;
            ChunkyRunInfo.instance.allyCurse = 0f;
            ChunkyRunInfo.instance.RpcDirtyAss();
            interact(teleporterState, interactor);
        }
        
        // Enforcing loitering penalty

        internal static void EnforceLoiter() {
            if (!shouldRun) return;
            
            if (isSimulacrumRun) {
#if DEBUG
                ReportLoiterError("Can't do loiter penalty in Simulacrum");
#endif
                return;
            }
            if (!ChunkyRunInfo.instance.doLoiterThisRun) {
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
            if (!NetworkServer.active) {
#if DEBUG
                ReportLoiterError("Client can not enforce loiter penalty");
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
            if (ChunkyRunInfo.instance.getFuckedLMAO){
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
            CM.Log.Info("Time's up! Loitering penalty has been applied. StagePunishTimer " + stagePunishTimer);
            ChunkyRunInfo.instance.getFuckedLMAO = true;
            ChunkyYap.DoWarning();
        }
        
#if DEBUG
        // Report why loitering hasn't been enabled every 5 seconds
        private static float reportErrorTime;
        private static bool reportErrorAnyway;
        private static void ReportLoiterError(string err) {
            if ((reportErrorTime >= Run.instance.NetworkfixedTime || isSimulacrumRun) && !reportErrorAnyway) return;
            CM.Log.Debug(err);
            reportErrorTime = Run.instance.NetworkfixedTime + 5f;
            reportErrorAnyway = false;
        }
#endif
    }
}
