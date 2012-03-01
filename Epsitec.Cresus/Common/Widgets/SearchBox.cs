//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Behaviors;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.SearchBox))]

namespace Epsitec.Common.Widgets
{
	public class SearchBox : AbstractTextField
	{
		public SearchBox()
		{
			this.searchBehavior = new SearchBehavior (this);
			this.searchBehavior.CreateButtons ();

			this.searchBehavior.SearchClicked += this.HandleSearchClicked;

			this.DefocusAction       = DefocusAction.None;
			this.ButtonShowCondition = ButtonShowCondition.Always;
		}

		public SearchBox(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		private void HandleSearchClicked(object sender)
		{
		}


		private readonly SearchBehavior			searchBehavior;
	}
}
