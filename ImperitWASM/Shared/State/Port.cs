using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public class Port : Land
	{
		public Port(int id, string name, Shape shape, Army army, Army defaultArmy, bool isStart, int earnings, ImmutableList<IProvinceAction> actions, Settings settings, bool isFinal)
			: base(id, name, shape, army, defaultArmy, isStart, earnings, actions, settings, isFinal) { }
		public override Province GiveUpTo(Army army) => new Port(Id, Name, Shape, army, DefaultArmy, IsStart, Earnings, Actions, settings, IsFinal);
		protected override Province WithActions(ImmutableList<IProvinceAction> new_actions) => new Port(Id, Name, Shape, Army, DefaultArmy, IsStart, Earnings, new_actions, settings, IsFinal);
		public override string[] Text => new[] { Name + "\u2693", Earnings + "\uD83D\uDCB0", Army.ToString() };
	}
}