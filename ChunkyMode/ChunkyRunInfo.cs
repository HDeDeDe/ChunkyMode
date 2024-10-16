using System.Runtime.Serialization;
using UnityEngine.Networking;

namespace HDeMods {
	internal class ChunkyRunInfo : NetworkBehaviour {
		// This is the instance of RunInfo
		public static ChunkyRunInfo instance;
		// This should only be true if ProperSave is present and added settings
		public static bool preSet;
		public static ChunkySaveData saveData;
		
		// These are to prevent changing settings mid run
		[SyncVar]
		public bool doLoiterThisRun; 
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
		[SyncVar]
		public float loiterPenaltyTimeThisRun;
		[SyncVar]
		public float loiterPenaltyFrequencyThisRun;
		[SyncVar]
		public float loiterPenaltySeverityThisRun;

		public void Awake() {
            instance = this;
            DontDestroyOnLoad(this);
            if (!preSet) return;
            doEnemyBoostThisRun = saveData.doEnemyBoostThisRun;
            doHealBuffThisRun = saveData.doHealBuffThisRun;
            doGoldThisRun = saveData.doGoldThisRun;
            doNerfsThisRun = saveData.doNerfsThisRun;
            doLoiterThisRun = saveData.doLoiterThisRun;
            enemyChanceToYapThisRun = saveData.enemyChanceToYapThisRun;
            enemyYapCooldownThisRun = saveData.enemyYapCooldownThisRun;
            loiterPenaltyTimeThisRun = saveData.loiterPenaltyTimeThisRun;
            loiterPenaltyFrequencyThisRun = saveData.loiterPenaltyFrequencyThisRun;
            loiterPenaltySeverityThisRun = saveData.loiterPenaltySeverityThisRun;
		}
	}

	public struct ChunkySaveData {
		[DataMember(Name = "validCheck")]
		public bool isValidSave;
		[DataMember(Name = "loiter")]
		public bool doLoiterThisRun; 
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
		[DataMember(Name = "loiterTime")]
		public float loiterPenaltyTimeThisRun;
		[DataMember(Name = "loiterFrequency")]
		public float loiterPenaltyFrequencyThisRun;
		[DataMember(Name = "loiterSeverity")]
		public float loiterPenaltySeverityThisRun;
	}
	
}