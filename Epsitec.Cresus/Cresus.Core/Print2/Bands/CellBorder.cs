//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Print2.Bands
{
	public struct CellBorder
	{
		public CellBorder(double width)
		{
			this.leftWidth   = width;
			this.rightWidth  = width;
			this.bottomWidth = width;
			this.topWidth    = width;
			this.color       = Color.FromBrightness (0);
		}

		public CellBorder(double width, Color color)
		{
			this.leftWidth   = width;
			this.rightWidth  = width;
			this.bottomWidth = width;
			this.topWidth    = width;
			this.color       = color;
		}

		public CellBorder(double leftWidth, double rightWidth, double bottomWidth, double topWidth)
		{
			this.leftWidth   = leftWidth;
			this.rightWidth  = rightWidth;
			this.bottomWidth = bottomWidth;
			this.topWidth    = topWidth;
			this.color       = Color.FromBrightness (0);
		}

		public CellBorder(double leftWidth, double rightWidth, double bottomWidth, double topWidth, Color color)
		{
			this.leftWidth   = leftWidth;
			this.rightWidth  = rightWidth;
			this.bottomWidth = bottomWidth;
			this.topWidth    = topWidth;
			this.color       = color;
		}


		private static readonly double EmptyWidth  = -1;
		public  static readonly double NormalWidth = 0.1;
		public  static readonly double BoldWidth   = 0.5;

		public static readonly CellBorder Empty   = new CellBorder (CellBorder.EmptyWidth);
		public static readonly CellBorder Default = new CellBorder (CellBorder.NormalWidth);


		public bool IsEmpty
		{
			get
			{
				return this.leftWidth   == CellBorder.EmptyWidth &&
					   this.rightWidth  == CellBorder.EmptyWidth &&
					   this.bottomWidth == CellBorder.EmptyWidth &&
					   this.topWidth    == CellBorder.EmptyWidth;
			}
		}

		public bool IsValid
		{
			get
			{
				return !this.IsEmpty;
			}
		}

		public bool IsConstantWidth
		{
			get
			{
				return this.leftWidth   == this.topWidth &&
					   this.rightWidth  == this.topWidth &&
					   this.bottomWidth == this.topWidth;
			}
		}


		public double LeftWidth
		{
			get
			{
				return this.leftWidth;
			}
		}

		public double RightWidth
		{
			get
			{
				return this.rightWidth;
			}
		}

		public double BottomWidth
		{
			get
			{
				return this.bottomWidth;
			}
		}

		public double TopWidth
		{
			get
			{
				return this.topWidth;
			}
		}

		public Color Color
		{
			get
			{
				return this.color;
			}
		}


		private readonly double		leftWidth;
		private readonly double		rightWidth;
		private readonly double		bottomWidth;
		private readonly double		topWidth;
		private readonly Color		color;
	}
}
