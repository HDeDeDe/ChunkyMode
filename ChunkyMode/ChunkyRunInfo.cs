using System.Runtime.Serialization;
using UnityEngine.Networking;

namespace HDeMods {
	internal class ChunkyRunInfo : NetworkBehaviour {
		// This is the instance of RunInfo
		[IgnoreDataMember]
		public static ChunkyRunInfo Instance;
		// This should only be true if ProperSave is present and added settings
		[IgnoreDataMember]
		public static bool preSet;
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
			if (Instance == null) Instance = this;
			else Destroy(this);
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