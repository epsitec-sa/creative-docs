//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList.Processors
{
	public interface IEventProcessor
	{
		bool ProcessMessage(Message message, Point pos);
		void PaintOverlay(Graphics graphics, Rectangle clipRect);
	}
}
