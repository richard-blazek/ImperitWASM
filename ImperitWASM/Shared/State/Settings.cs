using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.State
{
	[JsonConverter(typeof(Conversion.SettingsConverter))]
	public class Settings
	{
		public readonly Ratio DefaultInstability;
		public readonly double Interest;
		public readonly int DebtLimit, DefaultMoney;
		public readonly int MountainsWidth;
		public readonly Color LandColor, MountainsColor, SeaColor;
		public readonly ImmutableArray<SoldierType> SoldierTypes;
		public readonly int FinalLandsCount;
		public Settings(int debtLimit, Ratio defaultInstability, int defaultMoney, double interest, Color landColor, Color mountainsColor, int mountainsWidth, Color seaColor, ImmutableArray<SoldierType> soldierTypes, int finalLandsCount)
		{
			DebtLimit = debtLimit;
			DefaultInstability = defaultInstability;
			DefaultMoney = defaultMoney;
			Interest = interest;
			LandColor = landColor;
			MountainsColor = mountainsColor;
			MountainsWidth = mountainsWidth;
			SeaColor = seaColor;
			SoldierTypes = soldierTypes;
			FinalLandsCount = finalLandsCount;
		}
		public Ratio Instability(Soldiers now, Soldiers start) => DefaultInstability.Adjust(Math.Max(start.DefensePower - now.DefensePower, 0), start.DefensePower);
		public IEnumerable<SoldierType> RecruitableTypes(Province where) => SoldierTypes.Where(t => t.IsRecruitable(where));
	}
}