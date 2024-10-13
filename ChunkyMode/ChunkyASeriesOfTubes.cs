using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine.Networking;

namespace HDeMods {
	internal static class ChunkyASeriesOfTubes {
		public static void SetUpNetworking() {
			NetworkingAPI.RegisterRequestTypes<ChunkyRequestRunInfo,ChunkyReplyRunInfo>();
		}
		
		public static void DoNetworkingStuff() {
			if (NetworkServer.active) return;
			
			new ChunkyRequestRunInfo().Send<ChunkyRequestRunInfo,ChunkyReplyRunInfo>(NetworkDestination.Server);
		}
	}

	internal class ChunkyRequestRunInfo : INetRequest<ChunkyRequestRunInfo, ChunkyReplyRunInfo> {
		public ChunkyRequestRunInfo() {
			if(!NetworkServer.active) Log.Info("Requesting RunInfo from host.");
		}
		
		public ChunkyReplyRunInfo OnRequestReceived() {
			if (!NetworkServer.active) {
				Log.Warning("This is not the host, sending not ready flag.");
				return new ChunkyReplyRunInfo { _runInfo = null , _ready = false};
			}
			Log.Info("ChunkyRunInfo requested by client.");
			Log.Info("Sending ChunkyRunInfo now!");
			return new ChunkyReplyRunInfo { _runInfo = ChunkyRunInfo.Instance , _ready = true};
		}

		public void Serialize(NetworkWriter writer) {
			return;
		}

		public void Deserialize(NetworkReader reader) {
			return;
		}
	}

	internal class ChunkyReplyRunInfo : INetRequestReply<ChunkyRequestRunInfo, ChunkyReplyRunInfo> {
		internal ChunkyRunInfo _runInfo;
		internal bool _ready;
		
		public void OnReplyReceived() {
			if (NetworkServer.active) {
				Log.Warning("This is the host, ignoring request");
				return;
			}
			if(!_ready) {
				Log.Warning("Host is not ready!");
				return;
			}
			if (_runInfo == null) return;
			Log.Info("ChunkyRunInfo received, applying now!");
            ChunkyRunInfo.Instance = _runInfo;
		}

		public void Serialize(NetworkWriter writer) {
			writer.Write(_runInfo);
			writer.Write(_ready);
		}

		public void Deserialize(NetworkReader reader) {
			_runInfo = reader.Read<ChunkyRunInfo>();
			_ready = reader.ReadBoolean();
		}
	}
}