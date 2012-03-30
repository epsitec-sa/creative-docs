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
		
		
		public bool Filter(MouseDragFrame frame)
		{
			switch (this.ResizePolicy)
			{
				case ResizePolicy.None:
					return false;
				
				case ResizePolicy.Independent:
					return frame.Grip == GripId.EdgeRight || frame.Grip == GripId.EdgeBottom;
				
				case ResizePolicy.Coupled:
					return true;

				default:
					throw new System.NotSupportedException (string.Format ("{0} not supported", this.ResizePolicy.GetQualifiedName ()));
			}
		}

	}
}
