﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Client.Data;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;
using Microsoft.AspNetCore.Mvc;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PlayerController : ControllerBase
	{
		readonly ISessionLoader session;
		readonly IProvinceLoader province_load;
		readonly IPlayerLoader player_load;
		readonly IGameCreator game_creator;
		readonly IGameLoader game_load;
		public PlayerController(IProvinceLoader province_load, IPlayerLoader player_load, ISessionLoader session, IGameCreator game_creator, IGameLoader game_load)
		{
			this.province_load = province_load;
			this.session = session;
			this.player_load = player_load;
			this.game_creator = game_creator;
			this.game_load = game_load;
		}
		[HttpPost("Colors")] public IEnumerable<Color> Colors([FromBody] int gameId) => player_load[gameId].Select(p => p.Color);
		[HttpPost("Money")] public int Money([FromBody] Session ses) => player_load[ses.G, ses.P].Money;
		[HttpPost("Infos")] public IEnumerable<PlayerInfo> Infos([FromBody] Session ses)
		{
			if (!session.IsValid(player_load[ses.G, ses.P].Name, ses.Key))
			{
				return Enumerable.Empty<PlayerInfo>();
			}
			var players = player_load[ses.G];
			var provinces = province_load.Get(ses.G, players);
			return players.Select((p, i) => new PlayerInfo(i, p.Name, p.Color, p.Alive, p.Money, p.Debt, provinces.IncomeOf(p)));
		}
		[HttpPost("Correct")]
		public Game.State Correct([FromBody] Session ses) => session.IsValid(player_load[ses.G, ses.P].Name, ses.Key) ? game_load[ses.G]?.Current ?? Game.State.Invalid : Game.State.Invalid;
		[HttpPost("Login")]
		public Session Login([FromBody] Login trial)
		{
			game_creator.StartAll();
			return player_load[trial.N] is Player p && p.Password.IsCorrect(trial.P) ? new Session(p.Index, p.GameId, session.Add(p.Name, false)) : new Session();
		}
		[HttpPost("Color")]
		public Color GetColor([FromBody] PlayerId id) => Color.Parse(ctx.Players.AsNoTracking().SingleOrDefault(p => p.GameId == id.G && p.Index == id.P)?.Color ?? "#00000000");
		[HttpPost("Logout")] public void Logout([FromBody] string key) => session.Remove(key);
	}
}
