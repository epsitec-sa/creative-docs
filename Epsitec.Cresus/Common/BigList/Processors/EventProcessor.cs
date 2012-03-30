//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList.Processors
{
	public abstract class EventProcessor : IEventProcessor
	{
		public bool ProcessMessage(Message message, Point pos)
		{
			return this.Process (message, pos);
		}

		public void PaintOverlay(Graphics graphics, Rectangle clipRect)
		{
			this.Paint (graphics, clipRect);
		}

		protected abstract bool Process(Message message, Point pos);
		
		protected virtual void Paint(Graphics graphics, Rectangle clipRect)
		{
		}
	}
}
