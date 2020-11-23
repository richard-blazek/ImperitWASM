﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Client.Data
{
	public class DisplayableShape
	{
		public ImmutableArray<Point> B { get; set; }
		public Point C { get; set; }
		public Color F { get; set; }
		public Color S { get; set; }
		public int W { get; set; }
		public bool R { get; set; }
		public IEnumerable<string> T { get; set; } = ImmutableArray<string>.Empty;
		public DisplayableShape() { }
		public DisplayableShape(ImmutableArray<Point> border, Point center, Color fill, Color stroke, int strokeWidth, bool isStart, IEnumerable<string> text)
		{
			B = border;
			C = center;
			F = fill;
			S = stroke;
			W = strokeWidth;
			R = isStart;
			T = text;
		}
		public DisplayableShape UpdateText()
		{
			return new DisplayableShape(B, C, F, S, W, R, T.Skip(R ? 0 : 1));
		}
	}
}
