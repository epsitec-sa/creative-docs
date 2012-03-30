//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList.Processors
{
	public class MouseDragSplitterProcessor : EventProcessor
	{
		public MouseDragSplitterProcessor(IEventProcessorHost host, Message message, Point pos, MouseDragDirection direction, int splitterIndex)
		{
			this.host = host;
			this.direction = direction;
			this.origin = pos;
			this.button = message.Button;
			this.splitterIndex = splitterIndex;
		}

		public MouseDragDirection Direction
		{
			get
			{
				return this.direction;
			}
		}

		public static bool Attach(IEventProcessorHost host, Message message, Point pos, MouseDragDirection direction, int splitterIndex)
		{
			if (host.EventProcessors.OfType<MouseDragSplitterProcessor> ().Any ())
			{
				return false;
			}

			var proc = new MouseDragSplitterProcessor (host, message, pos, direction, splitterIndex);

			proc.host.Register (proc);
			proc.Process (message, pos);

			return true;
		}

		
		protected override bool Process(Widgets.Message message, Point pos)
		{
			return false;
		}

		protected override void Paint(Graphics graphics, Rectangle clipRect)
		{
			base.Paint (graphics, clipRect);
		}

		private readonly IEventProcessorHost host;
		private readonly MouseDragDirection	direction;
		private readonly Point origin;
		private readonly MouseButtons button;
		private readonly int splitterIndex;
	}
}
