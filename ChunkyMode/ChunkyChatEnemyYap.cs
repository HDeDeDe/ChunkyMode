using RoR2;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace HDeMods {
	public class ChunkyChatEnemyYap: ChatMessageBase {
		public string baseToken;
		public string enemyToken;
		private int m_eliteLength;
		public List<BuffIndex> eliteAffix = new List<BuffIndex>();

		public override string ConstructChatString() {
			string enemyName = Language.GetString(enemyToken);
			foreach (BuffIndex buffIndex in eliteAffix) {
				enemyName = Language.GetStringFormatted(BuffCatalog.GetBuffDef(buffIndex).eliteDef.modifierToken, enemyName);
			}
			return Language.GetStringFormatted("CHUNKYMODEDIFFMOD_YAP_FORMAT", Language.GetStringFormatted(enemyName), Language.GetString(baseToken));
		}
		
		public override void Serialize(NetworkWriter writer) {
			writer.Write(baseToken);
			writer.Write(enemyToken);
			writer.Write(eliteAffix.Count);
			foreach (BuffIndex buffIndex in eliteAffix) {
				writer.Write((int)buffIndex);
			}
		}

		public override void Deserialize(NetworkReader reader) {
			baseToken = reader.ReadString();
			enemyToken = reader.ReadString();
			m_eliteLength = reader.ReadInt32();
			if (m_eliteLength == 0) return;
			for (int i = 0; i < m_eliteLength; i++) {
				eliteAffix.Add((BuffIndex)reader.ReadInt32());
			}
		}
	}

	[SuppressMessage("ReSharper", "StringLiteralTypo")]
	internal static class ChunkyYap {
		public static void DoYapping([UnityEngine.Bindings.NotNull]string enemyToken, List<BuffIndex> eliteAffix) {
#if DEBUG
			CM.Log.Debug("Speaking now");
#endif
			string baseToken = (UnityEngine.Random.RandomRangeInt(0, 100000) % 16) switch {
				0 => "BROTHER_SPAWN_PHASE1_1",
				1 => "BROTHER_SPAWN_PHASE1_2",
				2 => "BROTHER_SPAWN_PHASE1_3",
				3 => "BROTHER_SPAWN_PHASE1_4",
				4 => "BROTHER_DAMAGEDEALT_7",
				5 => "BROTHER_DAMAGEDEALT_6",
				6 => "BROTHER_DAMAGEDEALT_2",
				7 => "BROTHER_KILL_1",
				8 => "BROTHER_DAMAGEDEALT_3",
				9 => "FALSESONBOSS_SPAWN_3",
				10 => "FALSESONBOSS_EARLYPHASE_HURT_2",
				11 => "FALSESONBOSS_EARLYPHASE_HURT_7",
				12 => "FALSESONBOSS_FINALPHASE_HURT_6",
				13 => "FALSESONBOSS_EARLYPHASE_PLAYERDEATH_4",
				14 => "FALSESONBOSS_EARLYPHASE_PLAYERDEATH_5",
				15 => "FALSESONBOSS_DRONEDEATH_2",
				_ => ""
			};
			Chat.SendBroadcastChat(new ChunkyChatEnemyYap() {
				baseToken = baseToken,
				enemyToken = enemyToken,
				eliteAffix = eliteAffix
			});
		}

		public static void DoWarning() {
#if DEBUG
			CM.Log.Debug("Warning now");
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