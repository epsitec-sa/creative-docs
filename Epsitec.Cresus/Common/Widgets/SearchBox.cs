//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Behaviors;

[assembly: DependencyClass (typeof (SearchBox))]

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


		protected override void UpdateButtonGeometry()
		{
			if (this.searchBehavior != null)
			{
				this.margins.Right = this.searchBehavior.DefaultWidth;
				this.searchBehavior.UpdateButtonGeometry ();
			}

			base.UpdateButtonGeometry ();
		}

		protected override void SetButtonVisibility(bool show)
		{
			if (this.searchBehavior == null)
			{
				return;
			}

			if (this.searchBehavior.IsVisible != show)
			{
				this.searchBehavior.SetVisible (show);

				Window window = this.Window;

				if (window != null)
				{
					window.ForceLayout ();
				}

				this.UpdateButtonGeometry ();
				this.UpdateButtonEnable ();
				this.UpdateTextLayout ();
				this.UpdateMouseCursor (this.MapRootToClient (Message.CurrentState.LastPosition));
			}
		}

		protected void UpdateButtonEnable()
		{
			if (this.searchBehavior != null)
			{
				this.searchBehavior.SetSearchEnabled (this.IsValid);
			}
		}

		protected void OnSearchClicked()
		{
			this.SearchClicked.Raise (this);
		}

		private void HandleSearchClicked(object sender)
		{
			this.OnSearchClicked ();
		}

		public event EventHandler				SearchClicked;

		private readonly SearchBehavior			searchBehavior;
	}
}
