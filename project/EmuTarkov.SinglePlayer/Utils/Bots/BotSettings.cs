using System;
using System.Collections.Generic;
using EmuTarkov.Common.Utils.HTTP;
using UnityEngine;
using EFT;

namespace EmuTarkov.SinglePlayer.Utils.Bots
{
    public class BotSettings
    {
		private static string Session;
		private static string BackendUrl;

		public static Dictionary<WildSpawnType, int> Limits { get; private set; }
		public static List<Difficulty> Difficulties { get; private set; }
		public static string CoreDifficulty { get; private set; }

		public BotSettings(string session, string backendUrl)
		{
			Limits = new Dictionary<WildSpawnType, int>();
			Difficulties = new List<Difficulty>();
			Session = session;
			BackendUrl = backendUrl;

			// set core values
			CoreDifficulty = null;
			RequestCoreDifficulty();

			// set bot values
			var roles = Enum.GetValues(typeof(WildSpawnType));
			var difficulties = Enum.GetValues(typeof(BotDifficulty));

			foreach (WildSpawnType role in roles)
			{
				if (role == WildSpawnType.assaultGroup          // NOTE: never encounted this one, what is it?
				|| role == WildSpawnType.bossStormtrooper       // TODO: we need server-side dumps
				|| role == WildSpawnType.bossTest               // NOTE: interesting
				|| role == WildSpawnType.followerGluharSnipe    // TODO: we need server-side dumps
				|| role == WildSpawnType.followerStormtrooper	// TODO: we need server-side dumps
				|| role == WildSpawnType.followerTest           // NOTE: interesting
				|| role == WildSpawnType.test)                  // NOTE: interesting
				{
					continue;
				}

				// set default values
				Limits[role] = 30;
				RequestLimit(role);

				foreach (BotDifficulty botDifficulty in difficulties)
				{
					Difficulties.Add(RequestDifficulty(role, botDifficulty, new Difficulty(role, botDifficulty, null)));
				}
			}
		}

		private static void RequestLimit(WildSpawnType role)
		{
			string json = new Request(Session, BackendUrl).GetJson("/client/game/bot/limit/" + role.ToString());

			if (string.IsNullOrEmpty(json))
			{
				Debug.LogError("EmuTarkov.SinglePlayer: Received bot " + role.ToString() + " limit data is NULL, using fallback");
				return;
			}

			Debug.LogError("EmuTarkov.SinglePlayer: Sucessfully received bot " + role.ToString() + " limit data");
			Limits[role] = Convert.ToInt32(json);
		}

		private static Difficulty RequestDifficulty(WildSpawnType role, BotDifficulty botDifficulty, Difficulty difficulty)
		{
			string json = new Request(Session, BackendUrl).GetJson("/client/game/bot/difficulty/" + role.ToString() + "/" + botDifficulty.ToString());

			if (string.IsNullOrEmpty(json))
			{
				Debug.LogError("EmuTarkov.SinglePlayer: Received bot " + role.ToString() + " " + botDifficulty.ToString() + " difficulty data is NULL, using fallback");
				return null;
			}

			Debug.LogError("EmuTarkov.SinglePlayer: Sucessfully received bot " + role.ToString() + " " + botDifficulty.ToString() + " difficulty data");
			difficulty.Json = json;
			return difficulty;
		}

		private static void RequestCoreDifficulty()
		{
			string json = new Request(Session, BackendUrl).GetJson("/client/game/bot/difficulty/core/core");

			if (string.IsNullOrEmpty(json))
			{
				Debug.LogError("EmuTarkov.SinglePlayer: Received core bot difficulty data is NULL, using fallback");
				return;
			}

			Debug.LogError("EmuTarkov.SinglePlayer: Sucessfully core bot difficulty data");
			CoreDifficulty = json;
		}
	}
}
