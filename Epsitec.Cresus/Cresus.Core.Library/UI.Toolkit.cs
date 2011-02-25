//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	public static partial class UI
	{
		public static class Toolkit
		{
			public static FrameBox CreateMiniToolbar(Widget parent, double height=0)
			{
				var toolbar = new FrameBox
				{
					Parent = parent,
					DrawFullFrame = true,
					BackColor = Color.FromHexa ("f4f9ff"), //ArrowedFrame.SurfaceColors.First (),
					Padding = new Margins (2),
					Dock = DockStyle.Top,
				};

				if (height != 0)
				{
					toolbar.PreferredHeight = height;
				}

				return toolbar;
			}

		}
	}
}
