using System.Runtime.Serialization;

namespace ChunkyMode {
	public class RunInfo {
		// This is the instance of RunInfo
		[IgnoreDataMember]
		public static RunInfo Instance;
		// This should only be true if ProperSave is present and added settings
		[IgnoreDataMember]
		public static bool preSet;
		// These are to prevent changing settings mid run
		[DataMember(Name = "loiter")]
		public bool doLoiterThisRun; 
		[DataMember(Name = "fold")]
		public bool doGoldThisRun;
		[DataMember(Name = "nerf")]
		public bool doNerfsThisRun;
		[DataMember(Name = "heal")]
		public bool doHealBuffThisRun;
		[DataMember(Name = "boost")]
		public bool doEnemyBoostThisRun;
	}
}