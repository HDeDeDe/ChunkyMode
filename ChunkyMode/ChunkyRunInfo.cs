using System.Runtime.Serialization;
using R2API.Networking.Interfaces;
using UnityEngine.Networking;

namespace HDeMods {
	public class ChunkyRunInfo : ISerializableObject {
		// This is the instance of RunInfo
		[IgnoreDataMember]
		public static ChunkyRunInfo Instance;
		// This should only be true if ProperSave is present and added settings
		[IgnoreDataMember]
		public static bool preSet;
		// These are to prevent changing settings mid run
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

		public void Serialize(NetworkWriter writer) {
			writer.Write(doLoiterThisRun);
			writer.Write(doGoldThisRun);
			writer.Write(doNerfsThisRun);
			writer.Write(doHealBuffThisRun);
			writer.Write(doEnemyBoostThisRun);
			writer.Write(enemyChanceToYapThisRun);
			writer.Write(enemyYapCooldownThisRun);
			writer.Write(loiterPenaltyTimeThisRun);
			writer.Write(loiterPenaltyFrequencyThisRun);
			writer.Write(loiterPenaltySeverityThisRun);
		}

		public void Deserialize(NetworkReader reader) {
			doLoiterThisRun = reader.ReadBoolean();
			doGoldThisRun = reader.ReadBoolean();
			doNerfsThisRun = reader.ReadBoolean();
			doHealBuffThisRun = reader.ReadBoolean();
			doEnemyBoostThisRun = reader.ReadBoolean();
			enemyChanceToYapThisRun = reader.ReadInt32();
			enemyYapCooldownThisRun = reader.ReadSingle();
			loiterPenaltyTimeThisRun = reader.ReadSingle();
			loiterPenaltyFrequencyThisRun = reader.ReadSingle();
			loiterPenaltySeverityThisRun = reader.ReadSingle();
		}
	}
}