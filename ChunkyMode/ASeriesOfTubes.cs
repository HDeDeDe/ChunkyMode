using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine.Networking;
using System.Threading;

namespace ChunkyMode {
	public static class ASeriesOfTubes {
		public static bool readyToRumble;
        
		public static void SetUpNetworking() {
			NetworkingAPI.RegisterRequestTypes<RequestRunInfo,ReplyRunInfo>();
		}
		
		public static void DoNetworkingStuff() {
			if (NetworkServer.active) {
				readyToRumble = true;
				return;
			}

			while (!readyToRumble && NetworkClient.active) {
				//TODO: Send message and then check to see if the code works (it probably won't lmao)
				new RequestRunInfo().Send<RequestRunInfo,ReplyRunInfo>(NetworkDestination.Server);
				if(!readyToRumble) Thread.Sleep(5);
			}
		}
	}

	internal class RequestRunInfo : INetRequest<RequestRunInfo, ReplyRunInfo> {
		public RequestRunInfo() {
			if(!NetworkServer.active) Log.Info("Requesting RunInfo from host.");
		}
		
		public ReplyRunInfo OnRequestReceived() {
			if (!NetworkServer.active) {
				Log.Warning("This is not the host, sending not ready flag.");
				return new ReplyRunInfo { _runInfo = null , _ready = false};
			}
			Log.Info("RunInfo requested by client.");
			if (!ASeriesOfTubes.readyToRumble) {
				Log.Warning("RunInfo is not ready yet! Sending not ready flag.");
				return new ReplyRunInfo { _runInfo = null , _ready = false};
			}
			Log.Info("Sending RunInfo now!");
			return new ReplyRunInfo { _runInfo = RunInfo.Instance , _ready = true};
		}

		public void Serialize(NetworkWriter writer) {
			return;
		}

		public void Deserialize(NetworkReader reader) {
			return;
		}
	}

	internal class ReplyRunInfo : INetRequestReply<RequestRunInfo, ReplyRunInfo> {
		internal RunInfo _runInfo;
		internal bool _ready;
		
		public void OnReplyReceived() {
			if (NetworkServer.active) {
				Log.Warning("This is the host, ignoring request");
				return;
			}
			if(!_ready) {
				Log.Warning("Host is not ready, trying again in 5 milliseconds.");
				return;
			}
			if (_runInfo == null) return;
			Log.Info("RunInfo received, applying now!");
            RunInfo.Instance = _runInfo;
            ASeriesOfTubes.readyToRumble = true;
		}

		public void Serialize(NetworkWriter writer) {
			writer.Write(_runInfo);
			writer.Write(_ready);
		}

		public void Deserialize(NetworkReader reader) {
			_runInfo = reader.Read<RunInfo>();
			_ready = reader.ReadBoolean();
		}
	}
}