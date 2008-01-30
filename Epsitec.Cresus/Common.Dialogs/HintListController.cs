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

		private void ShowHintListWidget()
		{
			if (this.hintListEmbedder == null)
			{
				this.hintListWidget = new HintListWidget ();
				this.hintListWidget.PreferredWidth = 200;
				this.hintListWidget.CurrentItemChanged += this.HandleHintListWidgetCurrentItemChanged;

				this.hintListEmbedder = new HintListEmbedder (this.searchController.DialogWindow, this.hintListWidget);
			}

			this.hintListEmbedder.Show ();
		}

		private void HideHintListWidget()
		{
			if (this.hintListEmbedder != null)
			{
				this.hintListEmbedder.Hide ();
			}
		}

		private void HandleHintListWidgetCurrentItemChanged(object sender)
		{
			AbstractEntity item = this.hintListWidget.Items.CurrentItem as AbstractEntity;
			ISearchContext context = this.searchController.ActiveSearchContext;

			if (context != null)
			{
				context.SetSuggestion (item);
			}
		}


		private class HintListEmbedder
		{
			public HintListEmbedder(Window window, HintListWidget widget)
			{
				this.window = window;
				this.widget = widget;

				this.rootPadding = this.window.Root.Padding;
				
				this.widget.Visibility = false;
				this.widget.SetParent (this.window.Root);

				this.SetWidth (this.widget.PreferredWidth);
			}

			public void Show()
			{
				if (!this.widget.Visibility)
				{
					WindowRoot        root    = this.window.Root;
					Drawing.Rectangle bounds  = this.window.WindowBounds;
					Drawing.Margins   padding = this.rootPadding;

					padding.Left += this.width;
					root.Padding = padding;

					bounds.Left -= this.width;

					this.widget.Margins = new Drawing.Margins (-padding.Left, 0, 0, 0);
					this.widget.Visibility   = true;
					this.window.WindowBounds = bounds;
				}
			}

			public void Hide()
			{
				if (this.widget.Visibility)
				{
					WindowRoot        root    = this.window.Root;
					Drawing.Rectangle bounds  = this.window.WindowBounds;
					Drawing.Margins   padding = this.rootPadding;

					root.Padding = padding;

					bounds.Left += this.width;
					
					this.widget.Visibility   = false;
					this.window.WindowBounds = bounds;
				}
			}

			private void SetWidth(double width)
			{
				if (this.width != width)
				{
					bool show = false;

					if (this.widget.Visibility)
					{
						this.Hide ();
						show = true;
					}

					this.widget.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Left;
					this.width = width;

					if (show)
					{
						this.Show ();
					}
				}
			}


			private readonly Window window;
			private readonly HintListWidget widget;
			private readonly Drawing.Margins rootPadding;

			private double width;
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

			ICollectionView view = this.searchResult.CollectionView;

			view.Refresh ();

			this.hintListWidget.Items = view;
		}

		private readonly DialogSearchController searchController;
		private ISearchContext activeSearchContext;
		private EntityResolverResult searchResult;
		private HintListWidget hintListWidget;
		private HintListEmbedder hintListEmbedder;
	}
}
