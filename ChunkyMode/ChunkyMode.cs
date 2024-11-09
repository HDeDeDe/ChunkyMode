// ReSharper disable once RedundantUsingDirective
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HDeMods
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static class ChunkyMode {
        // Difficulty related variables
        public static AssetBundle HurricaneBundle;
        public static DifficultyDef LegacyDifficultyDef;
        public static DifficultyIndex LegacyDifficultyIndex;
        public static GameObject HurricaneInfo;
        private static GameObject m_hurricaneInfo;

        // Run start checks
        private static bool shouldRun;
        private static int ogMonsterCap;
        private static int ogRunLevelCap;
        internal static bool isSimulacrumRun;

        // These are related to simulacrum
        internal static int totalBlindPest;
        internal static int totalLemurians;
        internal static bool waveStarted;

        // These are related to random enemy speaking
        private static float enemyYapTimer;

        // These values can be changed by the player through config options
        public static ConfigEntry<bool> doHealingBuffs { get; set; }
        public static ConfigEntry<bool> doEnemyLimitBoost { get; set; }
        public static ConfigEntry<bool> doGoldPenalty { get; set; }
        public static ConfigEntry<bool> doEnemyNerfs { get; set; }

        public static ConfigEntry<float> enemyChanceToYap { get; set; }
        public static ConfigEntry<float> enemyYapCooldown { get; set; }
        public static ConfigEntry<bool> limitPest { get; set; }
        public static ConfigEntry<float> limitPestAmount { get; set; }

        internal static void StartUp() {
            if (!File.Exists(Assembly.GetExecutingAssembly().Location
                    .Replace("ChunkyMode.dll", "chunkydifficon"))) {
                CM.Log.Fatal("Could not load asset bundle, aborting!");
                return;
            }
            HurricaneBundle = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location
                .Replace("ChunkyMode.dll", "chunkydifficon"));
            
            CreateNetworkObject();
            AddHooks();
        }

        private static void CreateNetworkObject() {
            GameObject temp = new GameObject("thing");
            temp.AddComponent<NetworkIdentity>();
            HurricaneInfo = temp.InstantiateClone("ChunkyRunInfo");
            GameObject.Destroy(temp);
            HurricaneInfo.AddComponent<ChunkyRunInfo>();
            
            ChatMessageBase.chatMessageTypeToIndex.Add(typeof(ChunkyChatEnemyYap),
                (byte)ChatMessageBase.chatMessageIndexToType.Count);
            ChatMessageBase.chatMessageIndexToType.Add(typeof(ChunkyChatEnemyYap));
        }

        private static void AddHooks() {
            On.RoR2.RewiredIntegrationManager.Init += VerifyInterloper;
            Run.onRunSetRuleBookGlobal += Run_onRunSetRuleBookGlobal;
            Run.onRunStartGlobal += Run_onRunStartGlobal;
            Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;
            RoR2Application.onLoad += ChunkyCachedIndexes.GenerateCache;
            if (ChunkyOptionalMods.Starstorm2.Enabled) ChunkyOptionalMods.Starstorm2.GenerateHooks();
            if (ChunkyOptionalMods.AlienHominid.Enabled) ChunkyOptionalMods.AlienHominid.GenerateHooks();
            if (ChunkyOptionalMods.Ravager.Enabled) ChunkyOptionalMods.Ravager.GenerateHooks();
            if (ChunkyOptionalMods.Submariner.Enabled) ChunkyOptionalMods.Submariner.GenerateHooks();
        }

        private static void RemoveHooks() {
            Run.onRunSetRuleBookGlobal -= Run_onRunSetRuleBookGlobal;
            Run.onRunStartGlobal -= Run_onRunStartGlobal;
            Run.onRunDestroyGlobal -= Run_onRunDestroyGlobal;
            RoR2Application.onLoad -= ChunkyCachedIndexes.GenerateCache;
        }

        private static void VerifyInterloper(On.RoR2.RewiredIntegrationManager.orig_Init init) {
            if (!InterlopingArtifactPlugin.startupSuccess) {
                CM.Log.Fatal("Artifact of Interloping did not start up successfully, aborting!");
                RemoveHooks();
                init();
                return;
            }
            
            BindSettings();
            AddHurricaneDifficulty();
            AddLegacyDifficulty();
            
            if (ChunkyOptionalMods.RoO.Enabled) AddOptions();
            if (ChunkyOptionalMods.Saving.Enabled) ChunkyOptionalMods.Saving.SetUp();
            if (ChunkyOptionalMods.RiskUI.Enabled) ChunkyOptionalMods.RiskUI.AddLegacyDifficulty();
            init();
        }

        private static void AddHurricaneDifficulty() {
            return;
        }

        private static void AddLegacyDifficulty() {
            LegacyDifficultyDef = new DifficultyDef(4f,
                "CHUNKYMODEDIFFMOD_NAME",
                "CHUNKYMODEDIFFMOD_ICON",
                "CHUNKYMODEDIFFMOD_DESCRIPTION",
                new Color32(61, 25, 255, 255),
                "cm",
                true
            ) {
                iconSprite = HurricaneBundle.LoadAsset<Sprite>("texChunkyModeDiffIcon"),
                foundIconSprite = true
            };
            LegacyDifficultyIndex = DifficultyAPI.AddDifficulty(LegacyDifficultyDef);
        }

        public static void BindSettings() {
            doHealingBuffs = ChunkyModePlugin.instance.Config.Bind<bool>(
                "Unlisted Difficulty Modifiers",
                "Do Healing Buffs",
                true,
                "Enables buffs to some survivor healing skills. Disable if you want a harder challenge.");
            doEnemyLimitBoost = ChunkyModePlugin.instance.Config.Bind<bool>(
                "Unlisted Difficulty Modifiers",
                "Do Enemy Limit Boost",
                true,
                "Enables enemy limit increase. If your computer is struggling to run on Chunky Mode, consider disabling this.");
            doGoldPenalty = ChunkyModePlugin.instance.Config.Bind<bool>(
                "Unlisted Difficulty Modifiers",
                "Do Gold Penalty",
                true,
                "Enables a -10% gold penalty. Disable to speed up gameplay.");
            doEnemyNerfs = ChunkyModePlugin.instance.Config.Bind<bool>(
                "Unlisted Difficulty Modifiers",
                "Do Enemy Nerfs",
                true,
                "Enables enemy nerfs. Disable if you like unreactable Wandering Vagrants.");
            enemyChanceToYap = ChunkyModePlugin.instance.Config.Bind<float>(
                "Yapping",
                "Enemy Yap Chance",
                0.0003f,
                "The probability of enemies to yap. Set to 0 to stop the yapping.");
            enemyYapCooldown = ChunkyModePlugin.instance.Config.Bind<float>(
                "Yapping",
                "Enemy Yap Cooldown",
                30f,
                "The amount of time before enemies are allowed to yap again. Set to 0 for turbo yapping.");
            limitPest = ChunkyModePlugin.instance.Config.Bind<bool>(
                "Simulacrum",
                "Limit Blind Pest",
                true,
                "Enable Blind Pest limit in Simulacrum. This also affects Lemurians.");
            limitPestAmount = ChunkyModePlugin.instance.Config.Bind<float>(
                "Simulacrum",
                "Blind Pest Amount",
                10f,
                "The percentage of enemies that are allowed to be blind pest. Only affects Simulacrum.");
            ChunkySurvivorBuffs.RegisterOptions();
        }

        private static void ClampConfigOptions() {
            enemyChanceToYap.Value = Math.Clamp(enemyChanceToYap.Value, 0f, 1f);
            enemyYapCooldown.Value = Math.Clamp(enemyYapCooldown.Value, 0f, 600f);
            limitPestAmount.Value = Math.Clamp(limitPestAmount.Value, 0f, 100f);
            ChunkySurvivorBuffs.ClampValues();
        }

        private static void AddOptions() {
            ChunkyOptionalMods.RoO.AddCheck(doHealingBuffs);
            ChunkyOptionalMods.RoO.AddCheck(doEnemyLimitBoost);
            ChunkyOptionalMods.RoO.AddCheck(doGoldPenalty);
            ChunkyOptionalMods.RoO.AddCheck(doEnemyNerfs);
            ChunkyOptionalMods.RoO.AddFloat(enemyChanceToYap, 0f, 1f, "{0}%");
            ChunkyOptionalMods.RoO.AddFloatStep(enemyYapCooldown, 0f, 600f, 1, "{0}");
            ChunkyOptionalMods.RoO.AddCheck(limitPest);
            ChunkyOptionalMods.RoO.AddFloatStep(limitPestAmount, 0f, 100f, 1f);
            ChunkySurvivorBuffs.RegisterRiskOfOptions();
            ChunkyOptionalMods.RoO.SetSprite(HurricaneBundle.LoadAsset<Sprite>("texChunkyModeDiffIcon"));
            ChunkyOptionalMods.RoO.SetDescriptionToken("CHUNKYMODEDIFFMOD_RISK_OF_OPTIONS_DESCRIPTION");
        }

        internal static void Run_onRunSetRuleBookGlobal(Run arg1, RuleBook arg2) {
            if (arg1.selectedDifficulty != LegacyDifficultyIndex) return;
            if (arg1.GetType() == typeof(InfiniteTowerRun)) isSimulacrumRun = true;
            InterlopingArtifact.HurricaneRun = true;
        }

        internal static void Run_onRunStartGlobal(Run run) {
            shouldRun = false;
            totalBlindPest = 0;
            totalLemurians = 0;
            ogRunLevelCap = Run.ambientLevelCap;
            ogMonsterCap = TeamCatalog.GetTeamDef(TeamIndex.Monster)!.softCharacterLimit;

            if (run.selectedDifficulty != LegacyDifficultyIndex) return;
            CM.Log.Info("Chunky Mode Run started");
            shouldRun = true;
            Run.ambientLevelCap += 9900;

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            if (!NetworkServer.active) return;

            m_hurricaneInfo = GameObject.Instantiate(HurricaneInfo);
            NetworkServer.Spawn(m_hurricaneInfo);

            if (!ChunkyRunInfo.preSet) {
                ChunkyModePlugin.instance.Config.Reload();
                ClampConfigOptions();
                ChunkyRunInfo.instance.doEnemyLimitBoost = doEnemyLimitBoost.Value;
                ChunkyRunInfo.instance.doHealingBuffs = doHealingBuffs.Value;
                ChunkyRunInfo.instance.doGoldPenalty = doGoldPenalty.Value;
                ChunkyRunInfo.instance.doEnemyNerfs = doEnemyNerfs.Value;
                ChunkyRunInfo.instance.enemyChanceToYap = enemyChanceToYap.Value;
                ChunkyRunInfo.instance.enemyYapCooldown = enemyYapCooldown.Value;
                ChunkyRunInfo.instance.limitPest = limitPest.Value;
                ChunkyRunInfo.instance.limitPestAmount = limitPestAmount.Value;
                ChunkyRunInfo.instance.rexHealOverride = ChunkySurvivorBuffs.RexHealOverride.Value;
                ChunkyRunInfo.instance.acridHealOverride = ChunkySurvivorBuffs.AcridHealOverride.Value;
                ChunkyRunInfo.instance.chirrHealOverride = ChunkySurvivorBuffs.ChirrHealOverride.Value;
                ChunkyRunInfo.instance.aliemHealOverride = ChunkySurvivorBuffs.AliemHealOverride.Value;
                ChunkyRunInfo.instance.submarinerHealOverride = ChunkySurvivorBuffs.SubmarinerHealOverride.Value;
                ChunkyRunInfo.instance.ravagerHealOverride = ChunkySurvivorBuffs.RavagerHealOverride.Value;
            }

            if (ChunkyRunInfo.instance.doEnemyLimitBoost) {
                //Thanks Starstorm 2 :)
                TeamCatalog.GetTeamDef(TeamIndex.Monster)!.softCharacterLimit = (int)(ogMonsterCap * 1.5);
                TeamCatalog.GetTeamDef(TeamIndex.Void)!.softCharacterLimit = (int)(ogMonsterCap * 1.5);
                TeamCatalog.GetTeamDef(TeamIndex.Lunar)!.softCharacterLimit = (int)(ogMonsterCap * 1.5);
            }

            HealthComponentAPI.GetHealthStats += ChunkyILHooks.ShieldRechargeAndBarrierDecayRate;
            if (ChunkyRunInfo.instance.doHealingBuffs) {
                IL.EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile.HealPulse += ChunkyILHooks.REXHealPulse;
                IL.RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted.OnDamageInflictedServer +=
                    ChunkyILHooks.REXPrimaryAttack;
                IL.RoR2.CharacterBody.RecalculateStats += ChunkyILHooks.AcridRegenBuff;
                if (ChunkyOptionalMods.Starstorm2.Enabled) ChunkyOptionalMods.Starstorm2.SetHooks();
                if (ChunkyOptionalMods.AlienHominid.Enabled) ChunkyOptionalMods.AlienHominid.SetHooks();
                if (ChunkyOptionalMods.Ravager.Enabled) ChunkyOptionalMods.Ravager.SetHooks();
                if (ChunkyOptionalMods.Submariner.Enabled) ChunkyOptionalMods.Submariner.SetHooks();
            }

            SceneDirector.onPrePopulateSceneServer += SceneDirector_onPrePopulateSceneServer;
            On.RoR2.CombatDirector.Awake += CombatDirector_Awake;
            HealthComponentAPI.GetHealStats += ChunkyILHooks.HealingOverride;
            On.RoR2.Run.BeginStage += Run_BeginStage;

            if (!isSimulacrumRun) return;
            waveStarted = false;

            if (ChunkyRunInfo.instance.limitPest) {
                CharacterBody.onBodyStartGlobal += TrackShittersAdd;
                CharacterBody.onBodyDestroyGlobal += TrackShittersRemove;
            }

            InfiniteTowerRun.onAllEnemiesDefeatedServer += ChunkySimulacrum.OnAllEnemiesDefeatedServer;
            On.RoR2.InfiniteTowerRun.BeginNextWave += ChunkySimulacrum.InfiniteTowerRun_BeginNextWave;
            IL.RoR2.InfiniteTowerWaveController.Initialize += ChunkySimulacrum.InfiniteTowerWaveController_Initialize;
            IL.RoR2.InfiniteTowerWaveController.FixedUpdate += ChunkySimulacrum.InfiniteTowerWaveController_FixedUpdate;
            IL.RoR2.CombatDirector.AttemptSpawnOnTarget += ChunkySimulacrum.ExtractRNGFromCombatDirector;
            On.RoR2.CombatDirector.PrepareNewMonsterWave += ChunkySimulacrum.CombatDirector_PrepareNewMonsterWave;
        }

        internal static void Run_onRunDestroyGlobal(Run run) {
            if (!shouldRun) return;
            CM.Log.Info("Chunky Mode Run ended");
            shouldRun = false;
            isSimulacrumRun = false;
            ChunkyRunInfo.preSet = false;
            Run.ambientLevelCap = ogRunLevelCap;
            GameObject.Destroy(m_hurricaneInfo);

            TeamCatalog.GetTeamDef(TeamIndex.Monster)!.softCharacterLimit = ogMonsterCap;
            TeamCatalog.GetTeamDef(TeamIndex.Void)!.softCharacterLimit = ogMonsterCap;
            TeamCatalog.GetTeamDef(TeamIndex.Lunar)!.softCharacterLimit = ogMonsterCap;

            HealthComponentAPI.GetHealthStats -= ChunkyILHooks.ShieldRechargeAndBarrierDecayRate;
            IL.EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile.HealPulse -= ChunkyILHooks.REXHealPulse;
            IL.RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted.OnDamageInflictedServer -=
                ChunkyILHooks.REXPrimaryAttack;
            IL.RoR2.CharacterBody.RecalculateStats -= ChunkyILHooks.AcridRegenBuff;
            if (ChunkyOptionalMods.Starstorm2.Enabled) ChunkyOptionalMods.Starstorm2.RemoveHooks();
            if (ChunkyOptionalMods.AlienHominid.Enabled) ChunkyOptionalMods.AlienHominid.RemoveHooks();
            if (ChunkyOptionalMods.Ravager.Enabled) ChunkyOptionalMods.Ravager.RemoveHooks();
            if (ChunkyOptionalMods.Submariner.Enabled) ChunkyOptionalMods.Submariner.RemoveHooks();

            RecalculateStatsAPI.GetStatCoefficients -= RecalculateStatsAPI_GetStatCoefficients;
            HealthComponentAPI.GetHealStats -= ChunkyILHooks.HealingOverride;
            On.RoR2.CombatDirector.Awake -= CombatDirector_Awake;
            SceneDirector.onPrePopulateSceneServer -= SceneDirector_onPrePopulateSceneServer;

            On.RoR2.Run.BeginStage -= Run_BeginStage;
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
                args.baseCurseAdd += InterRunInfo.instance.allyCurse;
                return;
            }

            if (!NetworkServer.active) goto ENEMYSTATS;
            
            float funko = UnityEngine.Random.Range(0f,1f);
            float yap = ChunkyRunInfo.instance.enemyChanceToYap;
            if (InterRunInfo.instance.loiterPenaltyActive) yap *= 2;

            if (funko < yap && ChunkyRunInfo.instance.enemyChanceToYap > 0 &&
                enemyYapTimer < Run.instance.NetworkfixedTime) {
                enemyYapTimer = Run.instance.NetworkfixedTime + ChunkyRunInfo.instance.enemyYapCooldown;
                List<BuffIndex> eliteAffix = new List<BuffIndex>();
                if (sender.isElite) eliteAffix.AddRange(BuffCatalog.eliteBuffIndices.Where(sender.HasBuff));
                ChunkyYap.DoYapping(sender.baseNameToken, eliteAffix);
            }

            ENEMYSTATS:
            if (!ChunkyRunInfo.instance.doEnemyNerfs) {
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
            if (ChunkyRunInfo.instance.doGoldPenalty && !isSimulacrumRun) self.goldRewardCoefficient *= 0.9f;
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
            beginStage(self);
        }
    }
}
