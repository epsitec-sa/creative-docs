//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (HintListController))]

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>HintListController</c> class manages a hint list associated with
	/// a dialog or form.
	/// </summary>
	public sealed class HintListController : DependencyObject
	{
		public HintListController()
		{
			this.searchController = new DialogSearchController ();
			this.searchController.SuggestionChanged += this.HandleSearchControllerSuggestionChanged;
			this.searchController.Resolved += this.HandleSearchControllerResolved;

			DialogSearchController.GlobalSearchContextChanged += this.HandleGlobalSearchContextChanged;
		}


		public DialogSearchController SearchController
		{
			get
			{
				return this.searchController;
			}
		}

		public ISearchContext ActiveSearchContext
		{
			get
			{
				return this.activeSearchContext;
			}
			private set
			{
				if (this.activeSearchContext != value)
				{
					this.activeSearchContext = value;
					this.OnActiveSearchContextChanged ();
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.searchController.SuggestionChanged -= this.HandleSearchControllerSuggestionChanged;
				this.searchController.Resolved -= this.HandleSearchControllerResolved;
				DialogSearchController.GlobalSearchContextChanged -= this.HandleGlobalSearchContextChanged;
			}

			base.Dispose (disposing);
		}

		private void OnActiveSearchContextChanged()
		{
			//	TODO: the active search context changed, refresh the UI list

			if (this.activeSearchContext == null)
			{
				this.HideHintListWidget ();
			}
			else
			{
				this.ShowHintListWidget ();
			}
		}

		private void HideHintListWidget()
		{
			if ((this.hintListWidget != null) &&
				(this.hintListWidget.IsVisible))
			{
				Window     window = this.searchController.DialogWindow;
				WindowRoot root   = window.Root;

				root.Padding = new Drawing.Margins (0, 0, 0, 0);

				this.hintListWidget.Hide ();

				Drawing.Rectangle bounds = window.WindowBounds;
				bounds.Left += 200;
				window.WindowBounds = bounds;
			}
		}

		private void ShowHintListWidget()
		{
			Window     window = this.searchController.DialogWindow;
			WindowRoot root   = window.Root;
			
			root.Padding = new Drawing.Margins (200, 0, 0, 0);

			if (this.hintListWidget == null)
			{
				this.hintListWidget = new HintListWidget ();
				this.hintListWidget.SetManualBounds (new Drawing.Rectangle (0, 0, 200, 100));
				this.hintListWidget.Anchor = AnchorStyles.TopAndBottom;
				this.hintListWidget.SetParent (root);
			}
			else if (this.hintListWidget.IsVisible)
			{
				//	Do nothing, list already visible.

				return;
			}
			else
			{
				this.hintListWidget.Show ();
			}

			Drawing.Rectangle bounds = window.WindowBounds;
			bounds.Left -= 200;
			window.WindowBounds = bounds;
		}


		
		private void HandleGlobalSearchContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			System.Diagnostics.Debug.Write (e.ToString ());

			ISearchContext context = e.NewValue as ISearchContext;

			if ((context != null) &&
				(context.SearchController == this.searchController))
			{
				this.ActiveSearchContext = context;
			}
			else
			{
				this.ActiveSearchContext = null;
			}
		}

		private void HandleSearchControllerSuggestionChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			AbstractEntity suggestion = e.NewValue as AbstractEntity;
		}

		private void HandleSearchControllerResolved(object sender)
		{
			this.searchResult = this.searchController.GetResolverResult (this.activeSearchContext);

			System.Diagnostics.Debug.WriteLine (string.Format ("Found {0} results", this.searchResult.AllResults.Count));
		}

		private readonly DialogSearchController searchController;
		private ISearchContext activeSearchContext;
		private EntityResolverResult searchResult;
		private HintListWidget hintListWidget;
	}
}
