//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Bricks
{
	/// <summary>
	/// The <c>UIGroupAction</c> class is similar to <see cref="UIMultiAction"/>, but
	/// the individual widgets will all be created in a row, as children of a group.
	/// This is used when generating a horizontal group.
	/// </summary>
	internal class UIGroupAction : UIAction
	{
		public UIGroupAction(IEnumerable<UIAction> actions, string title)
			: base (null)
		{
			this.actions = new List<UIAction> (actions);
			this.title   = title;
		}


		protected override void InternalExecute(FrameBox frame, UIBuilder builder)
		{
			var tile  = frame as EditionTile;
			var group = builder.CreateGroup (tile, this.title);

			group.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			
			this.actions.ForEach (x => x.Execute (group, builder));
		}

		
		private readonly List<UIAction>			actions;
		private readonly string					title;
	}
}
