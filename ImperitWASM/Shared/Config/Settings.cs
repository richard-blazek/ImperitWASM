using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Config
{
	public sealed record Settings(int CountdownSeconds, Ratio DefaultInstability, int DebtLimit, int DefaultMoney, int FinalLandsCount, Graph Graph, Ratio Interest, Color LandColor, int PlayerCount, Color MountainsColor, int MountainsWidth, ImmutableArray<string> Names, ImmutableArray<Region> Regions, Color SeaColor, ImmutableArray<SoldierType> SoldierTypes)
	{
		public static Color ColorOf(int i) => Color.Generate(i, 120.0, 1.0, 1.0);
		public static int LandPrice(Soldiers soldiers, int earnings) => soldiers.Price + soldiers.DefensePower + earnings * 2;

		public Player CreatePlayer(int i, string name, int land, Password password, bool human) => new Player(ColorOf(i), name, StartMoney(land), true, ImmutableList<IAction>.Empty, this, human, password, i == 0);

		public TimeSpan CountdownTime => TimeSpan.FromSeconds(CountdownSeconds);
		public ImmutableDictionary<SoldierType, int> GetSoldierTypeIndices() => SoldierTypes.Lookup();
		public Provinces Provinces => new Provinces(Regions.Select(r => new Province(r, null, r.Soldiers, this)).ToImmutableArray(), Graph);

		public int StartMoney(int province) => DefaultMoney - (Regions[province].Income * 4);
		public int CalculateDebt(int amount) => amount + amount * Interest;
		public int Discount(int amount) => (int)((long)amount * int.MaxValue / (int.MaxValue + Interest.ToInt()));
		public Ratio Instability(Soldiers now, Soldiers start) => DefaultInstability.Adjust(Math.Max(start.DefensePower - now.DefensePower, 0), start.DefensePower);

		public IEnumerable<SoldierType> RecruitableTypes(Region where) => SoldierTypes.Where(t => t.IsRecruitable(where));
		IEnumerable<string> GetNames(Func<string, int, string> obf) => Enumerable.Range(0, PlayerCount).Select(i => obf(Names[i % Names.Length], i / Names.Length));
		public IEnumerable<(int, Player)> CreateRobots(int first, IEnumerable<int> starts, Func<string, int, string> obf)
		{
			foreach (var (i, (start, name)) in starts.Zip(GetNames(obf)).Index())
			{
				yield return (i, new Player(ColorOf(i + first), name, StartMoney(start), true, ImmutableList<IAction>.Empty, this, false, new Password(), false));
			}
		}

		public sealed override string ToString() => string.Empty;
	}
}