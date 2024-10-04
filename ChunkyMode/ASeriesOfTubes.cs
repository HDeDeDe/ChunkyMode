using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine.Networking;

namespace ChunkyMode {
	public static class ASeriesOfTubes {
		public static bool readyToRumble;
		public static void SetUpNetworking() {
			NetworkingAPI.RegisterMessageType<SyncRunInfo>();
		}
		//TODO: Finish this
		public static void DoNetworkingStuff() {
			if (NetworkServer.active) {
				new SyncRunInfo(RunInfo.Instance).Send(NetworkDestination.Clients);
				return;
			}
			
		}
	}

	public class SyncRunInfo : INetMessage {
		private RunInfo _runInfo;

		public SyncRunInfo() {
			_runInfo = new RunInfo();
		}

		public SyncRunInfo(RunInfo runInfo) {
			_runInfo = runInfo;
		}
		
		public void Serialize(NetworkWriter writer) {
			writer.Write(_runInfo.doLoiterThisRun);
			writer.Write(_runInfo.doGoldThisRun);
			writer.Write(_runInfo.doNerfsThisRun);
			writer.Write(_runInfo.doHealBuffThisRun);
			writer.Write(_runInfo.doEnemyBoostThisRun);
		}

		public void Deserialize(NetworkReader reader) {
			_runInfo.doLoiterThisRun = reader.ReadBoolean();
			_runInfo.doGoldThisRun = reader.ReadBoolean();
			_runInfo.doNerfsThisRun = reader.ReadBoolean();
			_runInfo.doHealBuffThisRun = reader.ReadBoolean();
			_runInfo.doEnemyBoostThisRun = reader.ReadBoolean();
		}

		public void OnReceived() {
			if (NetworkServer.active) {
				Log.Warning("We're the host, we don't need to sync :)");
				return;
			}
			Log.Info("Client received RunInfo, applying now.");
			RunInfo.Instance = _runInfo;
			ASeriesOfTubes.readyToRumble = true;
		}
	}
}