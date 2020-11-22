using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImperitWASM.Server
{
	public static class DbExtensions
	{
		public static void UpdateAt<T>(this DbSet<T> set, int id, Action<T> mod) where T : class, IEntity, new() => mod(set.Attach(new T { Id = id }).Entity);
		public static void UpdateAt<T>(this DbSet<T> set, Expression<Func<T, bool>> cond, Action<T> mod) where T : class => set.Where(cond).ToArray().Each(mod);
		public static void RemoveAt<T>(this DbSet<T> set, int id) where T : class, IEntity, new() => set.Remove(set.Attach(new T { Id = id }).Entity);
		public static void RemoveAt<T>(this DbSet<T> set, Expression<Func<T, bool>> cond) where T : class => set.RemoveRange(set.Where(cond).ToArray());
		public static T NotRequired<T>(this T value) where T : class => value;
		public static ReferenceReferenceBuilder<TD, TP> OneToOne<TD, TP>(this ModelBuilder mod, Expression<Func<TD, TP?>> get) where TP : class where TD : class => mod.Entity<TD>().HasOne((Expression<Func<TD, TP>>)get!).WithOne().OnDelete(DeleteBehavior.Cascade);
		public static ReferenceCollectionBuilder<TP, TD> OneToMany<TP, TD>(this ModelBuilder mod, Expression<Func<TD, TP?>> get, Expression<Func<TP, IEnumerable<TD>?>>? list = null) where TP : class where TD : class
		{
			var rel = mod.Entity<TD>().HasOne((Expression<Func<TD, TP>>)get!);
			return (list is null ? rel.WithMany() : rel.WithMany(list!)).OnDelete(DeleteBehavior.Cascade); 
		}
	}
}