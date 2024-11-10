using System.Runtime.Serialization;
using UnityEngine.Networking;
using RoR2;
using UnityEngine.Serialization;

namespace HDeMods {
	internal class HurricaneRunInfo : NetworkBehaviour {
		// This is the instance of RunInfo
		public static HurricaneRunInfo instance;
		// This should only be true if ProperSave is present and added settings
		public static bool preSet;
		public static HurricaneSaveData saveData;
		
		// These are to prevent changing settings mid run
		[FormerlySerializedAs("doNerfsThisRun")] [SyncVar]
		public bool doEnemyNerfs;
		
		[FormerlySerializedAs("doGoldThisRun")] public bool doGoldPenalty;
		[FormerlySerializedAs("doHealBuffThisRun")] public bool doHealingBuffs;
		[FormerlySerializedAs("doEnemyBoostThisRun")] public bool doEnemyLimitBoost;
		[FormerlySerializedAs("enemyChanceToYapThisRun")] public float enemyChanceToYap;
		[FormerlySerializedAs("enemyYapCooldownThisRun")] public float enemyYapCooldown;
		[FormerlySerializedAs("limitPestsThisRun")] public bool limitPest;
		[FormerlySerializedAs("limitPestsAmountThisRun")] public float limitPestAmount;
		public float rexHealOverride;
		public float acridHealOverride;
		public float chirrHealOverride;
		public float aliemHealOverride;
		public float submarinerHealOverride;
		public float ravagerHealOverride;
		

		public void Awake() {
            instance = this;
            DontDestroyOnLoad(this);
            if (!preSet) return;
            doEnemyLimitBoost = saveData.doEnemyLimitBoost;
            doHealingBuffs = saveData.doHealingBuffs;
            doGoldPenalty = saveData.doGoldPenalty;
            doEnemyNerfs = saveData.doEnemyNerfs;
            enemyChanceToYap = saveData.enemyChanceToYap;
            enemyYapCooldown = saveData.enemyYapCooldown;
            limitPest = saveData.limitPest;
            limitPestAmount = saveData.limitPestAmount;
            rexHealOverride = saveData.rexHealOverride;
            acridHealOverride = saveData.acridHealOverride;
            chirrHealOverride = saveData.chirrHealOverride;
            aliemHealOverride = saveData.aliemHealOverride;
            submarinerHealOverride = saveData.submarinerHealOverride;
            ravagerHealOverride = saveData.ravagerHealOverride;
		}
	}

	public struct HurricaneSaveData {
		[DataMember(Name = "validCheck")]
		public bool isValidSave;
		[DataMember(Name = "gold")]
		public bool doGoldPenalty;
		[DataMember(Name = "nerf")]
		public bool doEnemyNerfs;
		[DataMember(Name = "heal")]
		public bool doHealingBuffs;
		[DataMember(Name = "boost")]
		public bool doEnemyLimitBoost;
		[DataMember(Name = "yapChance")]
		public float enemyChanceToYap;
		[DataMember(Name = "yapTime")]
		public float enemyYapCooldown;
		[DataMember(Name = "exLimitPest")]
		public bool limitPest;
		[DataMember(Name = "exPestCount")]
		public float limitPestAmount;
		[DataMember(Name = "rexHeal")]
		public float rexHealOverride;
		[DataMember(Name = "acridHeal")]
		public float acridHealOverride;
		[DataMember(Name = "chirrHeal")]
		public float chirrHealOverride;
		[DataMember(Name = "aliemHeal")]
		public float aliemHealOverride;
		[DataMember(Name = "submarinerHeal")]
		public float submarinerHealOverride;
		[DataMember(Name = "ravagerHeal")]
		public float ravagerHealOverride;
	}
	
}