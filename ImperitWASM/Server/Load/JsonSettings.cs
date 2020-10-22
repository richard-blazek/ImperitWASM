using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.State;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImperitWASM.Server.Load
{
	public class JsonSettings : IEntity<Settings, bool>
	{
		public int DebtLimit { get; set; }
		public int DefaultInstability { get; set; }
		public int DefaultMoney { get; set; }
		public double Interest { get; set; }
		public string LandColor { get; set; } = "";
		public string MountainsColor { get; set; } = "";
		public int MountainsWidth { get; set; }
		public ImmutableArray<string> RobotNames { get; set; }
		public string SeaColor { get; set; } = "";
		public IEnumerable<JsonSoldierType>? SoldierTypes { get; set; }
		public Settings Convert(int _i, bool _b) => new Settings(DebtLimit, new Probability(DefaultInstability), DefaultMoney, Interest, Color.Parse(LandColor), Color.Parse(MountainsColor), MountainsWidth, RobotNames, Color.Parse(SeaColor), SoldierTypes.Select((t, i) => t.Convert(i, false)).ToImmutableArray());
		public static JsonSettings From(Settings s) => new JsonSettings { Interest = s.Interest, DefaultInstability = s.DefaultInstability.ToInt(), DefaultMoney = s.DefaultMoney, DebtLimit = s.DebtLimit, RobotNames = s.RobotNames, LandColor = s.LandColor.ToString(), SeaColor = s.SeaColor.ToString(), MountainsColor = s.MountainsColor.ToString(), MountainsWidth = s.MountainsWidth, SoldierTypes = s.SoldierTypes.Select(t => JsonSoldierType.From(t)) };
	}
}