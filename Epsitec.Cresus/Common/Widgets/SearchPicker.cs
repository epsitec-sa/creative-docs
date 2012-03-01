//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Behaviors;

namespace Epsitec.Common.Widgets
{
	public class SearchPicker : FrameBox
	{
		public SearchPicker()
		{
			this.searchBox = new SearchBox ()
			{
				Parent = this,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 3),
				TabIndex = 1,
			};

			this.spacer = new StaticText ()
			{
				Parent = this,
				Dock = DockStyle.Top,
			};

			this.searchResults = new ScrollList ()
			{
				Parent = this,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 3, 0),
				TabIndex = 2,
			};
		}




		private readonly SearchBox				searchBox;
		private readonly StaticText				spacer;
		private readonly ScrollList				searchResults;
	}
}
