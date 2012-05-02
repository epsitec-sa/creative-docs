//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta;

using System.Collections.Generic;

namespace Epsitec.Cresus.Compta.Graph
{
	public class GraphSurface
	{
		public GraphSurface(int x, int y, Rectangle rect)
		{
			this.X = x;
			this.Y = y;
			this.Rect = rect;
		}

		public GraphSurface(int x, int y, Path path)
		{
			this.X = x;
			this.Y = y;
			this.Path = path;
		}


		public int X
		{
			get;
			private set;
		}

		public int Y
		{
			get;
			private set;
		}

		public Rectangle Rect
		{
			get;
			private set;
		}

		public Path Path
		{
			get;
			private set;
		}


		public bool Contains(Point pos)
		{
			if (!this.Rect.IsSurfaceZero)
			{
				return this.Rect.Contains (pos);
			}

			if (this.Path != null)
			{
				return this.Path.SurfaceContainsPoint(pos.X, pos.Y, 1);
			}

			return false;
		}
	}
}
