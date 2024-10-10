using RoR2;
using UnityEngine.Bindings;
using UnityEngine.Networking;

namespace HDeMods {
	public class ChunkyChatEnemyYap: ChatMessageBase {
		public string baseToken;
		public string enemyToken;

		public override string ConstructChatString() {
			return Language.GetStringFormatted("CHUNKYMODEDIFFMOD_YAP_FORMAT", Language.GetString(enemyToken), Language.GetString(baseToken));
		}
		
		public override void Serialize(NetworkWriter writer) {
			writer.Write(baseToken);
			writer.Write(enemyToken);
		}

		public override void Deserialize(NetworkReader reader) {
			baseToken = reader.ReadString();
			enemyToken = reader.ReadString();
		}
	}

	public static class ChunkyYap {
		public static void DoYapping(int randomNumber, [NotNull]string enemyToken) {
#if DEBUG
			Log.Debug("Speaking now");
#endif
			string baseToken = "";
			randomNumber %= 10;
			switch (randomNumber) {
				case 0:
					baseToken = "BROTHER_SPAWN_PHASE1_1";
					break;
				case 1:
					baseToken = "BROTHER_SPAWN_PHASE1_2";
					break;
				case 2:
					baseToken = "BROTHER_SPAWN_PHASE1_3";
					break;
				case 3:
					baseToken = "BROTHER_SPAWN_PHASE1_4";
					break;
				case 4:
					baseToken = "BROTHER_DAMAGEDEALT_7";
					break;
				case 5:
					baseToken = "BROTHER_DAMAGEDEALT_6";
					break;
				case 6:
					baseToken = "BROTHER_DAMAGEDEALT_2";
					break;
				case 7:
					baseToken = "BROTHER_KILL_1";
					break;
				case 8:
					baseToken = "BROTHER_DAMAGEDEALT_3";
					break;
				case 9:
					baseToken = "BROTHERHURT_DAMAGEDEALT_10";
					break;
			}
			Chat.SendBroadcastChat(new ChunkyChatEnemyYap() {
				baseToken = baseToken,
				enemyToken = enemyToken
			});
		}

		public static void DoWarning() {
#if DEBUG
			Log.Debug("Warning now");
#endif
			Chat.SendBroadcastChat(new Chat.NpcChatMessage() {
				baseToken = "CHUNKYMODEDIFFMOD_WARNING",
				formatStringToken = "CHUNKYMODEDIFFMOD_WARNING_FORMAT",
				sender = null,
				sound = null
			});
		}
	}
}