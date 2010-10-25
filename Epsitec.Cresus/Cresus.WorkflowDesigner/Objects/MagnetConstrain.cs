//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner.Objects
{
	public class MagnetConstrain
	{
		public MagnetConstrain(double position, bool isVertical)
		{
			this.position = position;
			this.isVertical = isVertical;
		}

		public double Position
		{
			get
			{
				return this.position;
			}
		}

		public bool IsVertical
		{
			get
			{
				return this.isVertical;
			}
		}

		public Point P1(Size areaSize)
		{
			if (this.isVertical)
			{
				return new Point (this.position, 0);
			}
			else
			{
				return new Point (0, this.position);
			}
		}

		public Point P2(Size areaSize)
		{
			if (this.isVertical)
			{
				return new Point (this.position, areaSize.Height);
			}
			else
			{
				return new Point (areaSize.Width, this.position);
			}
		}

		public bool Active
		{
			get;
			set;
		}


		private readonly double position;
		private readonly bool isVertical;
	}
}
