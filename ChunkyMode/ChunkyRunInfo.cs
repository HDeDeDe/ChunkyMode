using System.Runtime.Serialization;
using UnityEngine.Networking;
using RoR2;
using UnityEngine.Serialization;

namespace HDeMods {
	internal class ChunkyRunInfo : NetworkBehaviour {
		// This is the instance of RunInfo
		public static ChunkyRunInfo instance;
		// This should only be true if ProperSave is present and added settings
		public static bool preSet;
		public static ChunkySaveData saveData;
		
		// These are to prevent changing settings mid run
		[SyncVar]
		public bool doGoldThisRun;
		[SyncVar]
		public bool doNerfsThisRun;
		[SyncVar]
		public bool doHealBuffThisRun;
		[SyncVar]
		public bool doEnemyBoostThisRun;
		[SyncVar]
		public int enemyChanceToYapThisRun;
		[SyncVar]
		public float enemyYapCooldownThisRun;
		[FormerlySerializedAs("experimentLimitPestsThisRun")] [SyncVar]
		public bool limitPestsThisRun;
		[FormerlySerializedAs("experimentLimitPestsAmountThisRun")] [SyncVar]
		public float limitPestsAmountThisRun;

		public void Awake() {
            instance = this;
            DontDestroyOnLoad(this);
            if (!preSet) return;
            doEnemyBoostThisRun = saveData.doEnemyBoostThisRun;
            doHealBuffThisRun = saveData.doHealBuffThisRun;
            doGoldThisRun = saveData.doGoldThisRun;
            doNerfsThisRun = saveData.doNerfsThisRun;
            enemyChanceToYapThisRun = saveData.enemyChanceToYapThisRun;
            enemyYapCooldownThisRun = saveData.enemyYapCooldownThisRun;
            limitPestsThisRun = saveData.limitPestsThisRun;
            limitPestsAmountThisRun = saveData.limitPestsAmountThisRun;
		}
	}

	public struct ChunkySaveData {
		[DataMember(Name = "validCheck")]
		public bool isValidSave;
		[DataMember(Name = "gold")]
		public bool doGoldThisRun;
		[DataMember(Name = "nerf")]
		public bool doNerfsThisRun;
		[DataMember(Name = "heal")]
		public bool doHealBuffThisRun;
		[DataMember(Name = "boost")]
		public bool doEnemyBoostThisRun;
		[DataMember(Name = "yapChance")]
		public int enemyChanceToYapThisRun;
		[DataMember(Name = "yapTime")]
		public float enemyYapCooldownThisRun;
		[DataMember(Name = "exLimitPest")]
		public bool limitPestsThisRun;
		[DataMember(Name = "exPestCount")]
		public float limitPestsAmountThisRun;
	}
	
}