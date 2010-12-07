//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner.Objects
{
	public class MagnetConstrain
	{
		public MagnetConstrain(bool isVertical)
		{
			this.isVertical = isVertical;
		}

		public bool IsVertical
		{
			get
			{
				return this.isVertical;
			}
		}

		public double Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.position = value;
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


		private readonly bool			isVertical;
		private double					position;
	}
}
