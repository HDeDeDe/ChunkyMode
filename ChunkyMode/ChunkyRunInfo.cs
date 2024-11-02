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
		[SyncVar]
		public bool experimentCursePenaltyThisRun;
		[SyncVar]
		public float experimentCurseRateThisRun;
		[FormerlySerializedAs("experimentLimitPestsThisRun")] [SyncVar]
		public bool limitPestsThisRun;
		[FormerlySerializedAs("experimentLimitPestsAmountThisRun")] [SyncVar]
		public float limitPestsAmountThisRun;
		
		//These values is only synced not saved
		[SyncVar]
		public float allyCurse = 0f;
		[SyncVar]
		public float loiterTick = 0f;
		[SyncVar]
		// ReSharper disable once InconsistentNaming
		public bool getFuckedLMAO = false;

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
            experimentCursePenaltyThisRun = saveData.experimentCursePenaltyThisRun;
            experimentCurseRateThisRun = saveData.experimentCurseRateThisRun;
            limitPestsThisRun = saveData.limitPestsThisRun;
            limitPestsAmountThisRun = saveData.limitPestsAmountThisRun;
		}



		[ClientRpc]
		public void RpcDirtyAss() {
			DirtyAss();
		}

		public void DirtyAss() {
			foreach (TeamComponent teamComponent in TeamComponent.GetTeamMembers(TeamIndex.Player)) {
				teamComponent.body.MarkAllStatsDirty();
			}
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
		[DataMember(Name = "exCurse")]
		public bool experimentCursePenaltyThisRun;
		[DataMember(Name = "exCurseRate")]
		public float experimentCurseRateThisRun;
		[DataMember(Name = "exLimitPest")]
		public bool limitPestsThisRun;
		[DataMember(Name = "exPestCount")]
		public float limitPestsAmountThisRun;
	}
	
}