//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList.Processors
{
	public class MouseDragProcessorPolicy : EventProcessorPolicy
	{
		public MouseDragProcessorPolicy()
		{
			this.ResizePolicy = ResizePolicy.Independent;
		}


		public ResizePolicy ResizePolicy
		{
			get;
			set;
		}
		
		
		public IEnumerable<MouseDragFrame> Filter(IEnumerable<MouseDragFrame> frames)
		{
			MouseDragFrame previous = default (MouseDragFrame);

			foreach (var frame in frames)
			{
				if (this.Filter (frame, previous))
				{
					yield return frame;
				}

				previous = frame;
			}

		}
		
		private bool Filter(MouseDragFrame frame, MouseDragFrame previous)
		{
			switch (this.ResizePolicy)
			{
				case ResizePolicy.None:
					return false;
				
				case ResizePolicy.Independent:
					if (frame.Elasticity == MouseDragElasticity.None)
					{
						if (frame.Grip == GripId.EdgeRight || frame.Grip == GripId.EdgeBottom)
						{
							return true;
						}
						if (frame.Grip == GripId.EdgeLeft || frame.Grip == GripId.EdgeTop)
						{
							if ((previous.IsEmpty == false) &&
								(previous.Elasticity == MouseDragElasticity.Stretch))
							{
								return true;
							}
						}
					}
					return false;
				
				case ResizePolicy.Coupled:
					return true;

				default:
					throw new System.NotSupportedException (string.Format ("{0} not supported", this.ResizePolicy.GetQualifiedName ()));
			}
		}

	}
}
