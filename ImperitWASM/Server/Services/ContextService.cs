﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Func;
using System.Threading.Tasks;
using ImperitWASM.Server.Load;
using Microsoft.EntityFrameworkCore;
using ImperitWASM.Shared.Entities;

namespace ImperitWASM.Server.Services
{
	public interface IContextService
	{
		Task SaveAsync();
		
		ImmutableArray<Player> GetPlayers(int gameId);
		IContextService SetPlayers(int gameId, IEnumerable<Player> players);
		IContextService AddPlayers(int gameId, IEnumerable<Player> players);
		
		ImmutableArray<Province> GetProvinces(int gameId, IReadOnlyList<Player> players);
		IContextService SetProvinces(int gameId, IEnumerable<Province> provinces);
		IContextService AddProvinces(int gameId, IEnumerable<Province> provinces);

		List<PlayersPower> GetPlayersPowers(int gameId);
		IContextService AddPlayersPower(int gameId, PlayersPower psp);
		int CountPlayersPowers(int gameId);

		DbSet<Game> Games { get; }
		DbSet<Player> Players { get; }
		DbSet<PlayerPower> PlayerPowers { get; }
		DbSet<Province> Provinces { get; }
		DbSet<Session> Sessions { get; }
	}
	public class ContextService : IContextService
	{
		readonly IConfig cfg;
		readonly Context ctx;
		public DbSet<Game> Games => ctx.Games ?? throw new NullReferenceException();
		public DbSet<Player> Players => ctx.Players ?? throw new NullReferenceException();
		public DbSet<PlayerPower> PlayerPowers => ctx.PlayerPowers ?? throw new NullReferenceException();
		public DbSet<Province> Provinces => ctx.Provinces ?? throw new NullReferenceException();
		public DbSet<Session> Sessions => ctx.Sessions ?? throw new NullReferenceException();

		public ContextService(Context ctx, IConfig cfg)
		{
			this.ctx = ctx;
			this.cfg = cfg;
		}
		public Task SaveAsync() => ctx.SaveChangesAsync();
		//Players--------------------------------------------------------------
		IOrderedQueryable<EntityPlayer> GetEPlayers(int gameId) => Players.Where(p => p.GameId == gameId).OrderBy(p => p.Index);
		public ImmutableArray<Player> GetPlayers(int gameId) => GetEPlayers(gameId).Select(p => p.Convert(cfg.Settings)).ToImmutableArray();
		public IContextService SetPlayers(int gameId, IEnumerable<Player> players)
		{
			GetEPlayers(gameId).ToArray().Zip(players).Each(it => it.First.Assign(it.Second));
			return this;
		}
		public IContextService AddPlayers(int gameId, IEnumerable<Player> players)
		{
			players.Each((player, i) => Players.Add(EntityPlayer.From(player, i, gameId)));
			return this;
		}
		//Provinces------------------------------------------------------------
		IOrderedQueryable<EntityProvince> GetEProvinces(int gameId) => Provinces.Where(p => p.GameId == gameId).OrderBy(p => p.Index);
		public ImmutableArray<Province> GetProvinces(int gameId, IReadOnlyList<Player> players)
		{
			Province Convert(EntityProvince p) => p.Convert((cfg.Settings, cfg.Shapes, players));
			return GetEProvinces(gameId).Select(Convert).ToImmutableArray();
		}
		public IContextService SetProvinces(int gameId, IEnumerable<Province> provinces)
		{
			var map = GetEPlayers(gameId!)!.ToImmutableDictionary(p => p!.Convert(cfg!.Settings!)!, p => p!.Index!)!;
			GetEProvinces(gameId).ToArray().Zip(provinces).Each(it => it.First.Assign(it.Second, map, cfg.Settings.SoldierTypeIndices));
			return this;
		}
		public IContextService AddProvinces(int gameId, IEnumerable<Province> provinces)
		{
			var map = GetEPlayers(gameId).ToImmutableDictionary(p => p.Convert(cfg.Settings), p => p.Index);
			provinces.Each((province, i) => Provinces.Add(EntityProvince.From(province, map, cfg.Settings.SoldierTypeIndices, gameId, i)));
			return this;
		}
		//PlayerPower----------------------------------------------------------
		public List<PlayersPower> GetPlayersPowers(int gameId)
		{
			return PlayerPowers.Where(pp => pp.GameId == gameId).GroupBy(pp => pp.TurnIndex).OrderBy(ppg => ppg.Key).Select(pps => new PlayersPower(pps.OrderBy(pp => pp.PlayerIndex).Select(pp => pp.Convert(false)).ToImmutableArray())).ToList();
		}
		public int CountPlayersPowers(int gameId) => PlayerPowers.Count(pp => pp.GameId == gameId) / Players.Count(pp => pp.GameId == gameId);
		public IContextService AddPlayersPower(int gameId, PlayersPower psp)
		{
			int turn = CountPlayersPowers(gameId);
			PlayerPowers.AddRange(psp.Select((pp, i) => EntityPlayerPower.From(pp, gameId, turn, i)));
			return this;
		}
	}
}
