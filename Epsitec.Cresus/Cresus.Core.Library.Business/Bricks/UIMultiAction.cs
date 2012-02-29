//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Bricks
{
	internal class UIMultiAction : UIAction
	{
		public UIMultiAction(IEnumerable<UIAction> actions) :
			base (null)
		{
			this.actions = new List<UIAction> (actions);
		}

		
		protected override void InternalExecute(FrameBox frame, UIBuilder builder)
		{
			EditionTile tile = frame as EditionTile;

			if (tile != null)
			{
				tile.Padding = Common.Drawing.Margins.Zero;
			}

			foreach (var action in this.actions)
			{
				var subTile = builder.CreateEditionTile (frame as EditionTile);
				action.Execute (subTile, builder);
			}
		}

		
		private readonly List<UIAction>			actions;
	}
}
