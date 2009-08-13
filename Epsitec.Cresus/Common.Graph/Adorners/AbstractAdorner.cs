﻿//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;

namespace Epsitec.Common.Graph.Adorners
{
	public abstract class AbstractAdorner
	{
		protected AbstractAdorner()
		{
		}


		public virtual void PaintBackground(IPaintPort port, Renderers.AbstractRenderer renderer)
		{
		}

		public virtual void PaintForeground(IPaintPort port, Renderers.AbstractRenderer renderer)
		{
		}
	}
}
