//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList.Processors
{
	public struct MouseDragFrame
	{
		public MouseDragFrame(int index, GripId grip, Rectangle bounds, MouseDragDirection direction, Rectangle constraint)
		{
			this.index      = index;
			this.grip       = grip;
			this.bounds     = bounds;
			this.direction  = direction;
			this.constraint = constraint;
		}

		public MouseDragFrame(MouseDragFrame original, Rectangle bounds)
		{
			this.index      = original.index;
			this.grip       = original.grip;
			this.bounds     = bounds;
			this.direction  = original.direction;
			this.constraint = original.constraint;
		}

		
		public int Index
		{
			get
			{
				return this.index;
			}
		}

		public GripId Grip
		{
			get
			{
				return this.grip;
			}
		}

		public Rectangle Bounds
		{
			get
			{
				return this.bounds;
			}
		}

		public MouseDragDirection Direction
		{
			get
			{
				return this.direction;
			}
		}

		public Rectangle Constraint
		{
			get
			{
				return this.constraint;
			}
		}


		private readonly int index;
		private readonly GripId grip;
		private readonly Rectangle bounds;
		private readonly MouseDragDirection	direction;
		private readonly Rectangle constraint;
	}
}
