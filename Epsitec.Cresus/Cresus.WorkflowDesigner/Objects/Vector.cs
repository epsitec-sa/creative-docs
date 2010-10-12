//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner.Objects
{
	public struct Vector
	{
		public Vector(Point start, Point end)
		{
			this.start = start;
			this.end = end;
		}

		public Vector(Point start, Size direction)
		{
			this.start = start;
			this.end = start + direction;
		}


		public Point Start
		{
			get
			{
				return this.start;
			}
		}

		public Point End
		{
			get
			{
				return this.end;
			}
		}


		public Point GetPoint(double distance)
		{
			return Point.Move (this.start, this.end, distance);
		}


		public bool IsValid
		{
			get
			{
				return !this.IsZero;
			}
		}

		public bool IsZero
		{
			get
			{
				return start.IsZero || end.IsZero;
			}
		}

		public static readonly Vector Zero;

		private readonly Point start;
		private readonly Point end;
	}
}
