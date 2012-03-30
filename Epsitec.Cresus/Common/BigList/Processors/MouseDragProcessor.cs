//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList.Processors
{
	public class MouseDragProcessor : EventProcessor
	{
		public MouseDragProcessor(IEventProcessorHost host, MouseDragDirection direction, Message message, Point pos)
		{
			this.host = host;
			this.direction = direction;
		}

		public MouseDragDirection Direction
		{
			get
			{
				return this.direction;
			}
		}

		protected override bool Process(Widgets.Message message, Point pos)
		{
			return false;
		}

		private readonly IEventProcessorHost host;
		private readonly MouseDragDirection	direction;
	}
}
