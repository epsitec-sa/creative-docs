//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Bricks
{
	internal class UIGroupAction : UIAction
	{
		public UIGroupAction(IEnumerable<UIAction> actions, string title)
			: base (null)
		{
			this.actions = new List<UIAction> (actions);
			this.title = title;
		}

		protected override void InternalExecute(FrameBox frame, UIBuilder builder)
		{
			var group = builder.CreateGroup (frame as EditionTile, this.title);
			group.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			this.actions.ForEach (x => x.Execute (group, builder));
		}

		private readonly List<UIAction>			actions;
		private readonly string					title;
	}
}
