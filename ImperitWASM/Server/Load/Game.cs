﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImperitWASM.Server.Load
{
	public class Game : IEntity
	{
		public enum State { Created, Countdown, Started, Finished }
		[Key] public int Id { get; set; }
		public int Active { get; set; }
		public State Current { get; set; }
		public DateTime LastChange { get; set; }
		[ForeignKey("GameId")] public ICollection<EntityPlayer>? EntityPlayers { get; set; }
		[ForeignKey("GameId")] public ICollection<EntityProvince>? EntityProvinces { get; set; }
		[ForeignKey("GameId")] public ICollection<EntitySession>? EntitySessions { get; set; }
		[ForeignKey("GameId")] public ICollection<EntityPlayerPower>? EntityPlayerPowers { get; set; }
		public bool Created => Current == State.Created;
		public bool Countdown => Current == State.Countdown;
		public bool Started => Current == State.Started;
		public bool Finished => Current == State.Finished;
		public static Game Create => new Game { Current = State.Created, LastChange = DateTime.UtcNow };
		public Game StartCountdown()
		{
			LastChange = Countdown ? LastChange : DateTime.UtcNow;
			Current = State.Countdown;
			return this;
		}
		public Game Start()
		{
			LastChange = Started ? LastChange : DateTime.UtcNow;
			Current = State.Started;
			return this;
		}
		public Game Finish()
		{
			LastChange = Finished ? LastChange : DateTime.UtcNow;
			Current = State.Started;
			return this;
		}
		public Game SetActive(int i)
		{
			Active = i;
			return this;
		}
	}
}
