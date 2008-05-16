﻿//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			this.searchController.DialogDataChanged += this.HandleSearchControllerDialogDataChanged;
			this.searchController.DialogFocusChanged += this.HandleSearchControllerDialogFocusChanged;
			this.searchController.PlaceholderPostProcessing += this.HandleSearchControllerPlaceholderPostProcessing;
			this.searchController.Resolved += this.HandleSearchControllerResolved;

			this.emptyCollectionView = new CollectionView (new System.Collections.ArrayList ());

			DialogSearchController.GlobalSearchContextChanged += this.HandleGlobalSearchContextChanged;
		}


		/// <summary>
		/// Gets or sets the hint list visibility mode.
		/// </summary>
		/// <value>The visibility.</value>
		public HintListVisibilityMode Visibility
		{
			get
			{
				return this.visiblityMode;
			}
			set
			{
				if (this.visiblityMode != value)
				{
					this.visiblityMode = value;
					this.OnVisibilityModeChanged ();
				}
			}
		}

		/// <summary>
		/// Gets or sets the content type, which will define what header the
		/// hint list will display.
		/// </summary>
		/// <value>The content type.</value>
		public HintListContentType ContentType
		{
			get
			{
				return this.contentType;
			}
			set
			{
				if (this.contentType != value)
				{
					this.contentType = value;
					this.OnContentTypeChanged ();
				}
			}
		}

		/// <summary>
		/// Gets the search controller associated with this hint list.
		/// </summary>
		/// <value>The search controller.</value>
		public DialogSearchController SearchController
		{
			get
			{
				return this.searchController;
			}
		}

		/// <summary>
		/// Gets the active search context, if any.
		/// </summary>
		/// <value>The active search context, or <c>null</c>.</value>
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

		/// <summary>
		/// Gets the hint list widget used to represent the list in the user
		/// interface.
		/// </summary>
		/// <value>The hint list widget.</value>
		public HintListWidget HintListWidget
		{
			get
			{
				this.CreateUserInterface ();

				return this.hintListWidget;
			}
		}


		/// <summary>
		/// Gets the path of the focused field.
		/// </summary>
		/// <value>The path of the focused field or <c>null</c>.</value>
		public EntityFieldPath FocusFieldPath
		{
			get
			{
				return this.focusFieldPath;
			}
			private set
			{
				if (this.focusFieldPath != value)
				{
					this.focusFieldPath = value;

					//	TODO: generate event...
				}
			}
		}


		public void DefineContainer(Widget widget)
		{
			this.CreateUserInterface ();
			
			this.hintListWidget.SetParent (widget);
			this.hintListWidget.Anchor = AnchorStyles.None;
			this.hintListWidget.Dock = DockStyle.Left;
		}
		

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.searchController.SuggestionChanged -= this.HandleSearchControllerSuggestionChanged;
				this.searchController.DialogDataChanged -= this.HandleSearchControllerDialogDataChanged;
				this.searchController.DialogFocusChanged -= this.HandleSearchControllerDialogFocusChanged;
				this.searchController.PlaceholderPostProcessing -= this.HandleSearchControllerPlaceholderPostProcessing;
				this.searchController.Resolved -= this.HandleSearchControllerResolved;
				DialogSearchController.GlobalSearchContextChanged -= this.HandleGlobalSearchContextChanged;
			}

			base.Dispose (disposing);
		}

		
		private void OnVisibilityModeChanged()
		{
			this.UpdateVisibility ();
		}

		private void OnContentTypeChanged()
		{
			this.UpdateContentHeader ();
		}

		private void OnActiveSearchContextChanged()
		{
			this.UpdateVisibility ();
			
			if (this.activeSearchContext == null)
			{
				this.hintListWidget.Items = this.emptyCollectionView;
			}
		}
		
		
		private void UpdateContentHeader()
		{
			if (this.hintListWidget != null)
			{
				this.hintListWidget.Header.ContentType = this.contentType;
			}
		}

		private void UpdateVisibility()
		{
			switch (this.visiblityMode)
			{
				case HintListVisibilityMode.AutoHide:
					if (this.activeSearchContext == null)
					{
						this.HideHintListWidget ();
					}
					else
					{
						this.ShowHintListWidget ();
					}
					break;

				case HintListVisibilityMode.Invisible:
					this.HideHintListWidget ();
					break;

				case HintListVisibilityMode.Visible:
					this.ShowHintListWidget ();
					break;

				default:
					throw new System.InvalidOperationException ();
			}
		}


		private void ShowHintListWidget()
		{
			this.CreateUserInterface ();

			if (this.hintListEmbedder != null)
			{
				this.hintListEmbedder.Show ();
			}
		}

		private void CreateUserInterface()
		{
			if (this.hintListWidget == null)
			{
				this.hintListWidget = new HintListWidget ();
				this.hintListWidget.PreferredWidth = 200;
				this.hintListWidget.Header.ContentType = this.contentType;
				this.hintListWidget.CurrentItemChanged += this.HandleHintListWidgetCurrentItemChanged;
			}

			if ((this.hintListEmbedder == null) &&
				(this.Visibility == HintListVisibilityMode.AutoHide))
			{
				this.hintListEmbedder = new HintListEmbedder (this.searchController.DialogWindow, this.hintListWidget);
			}
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

		
		#region HintListEmbedder

		/// <summary>
		/// The <c>HintListEmbedder</c> class manages the embedding of the hint
		/// list widget in the window root; it is responsible for the resize of
		/// the window when the list visibility changes.
		/// </summary>
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

		#endregion


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

		private void HandleSearchControllerSuggestionChanged(object sender, DialogDataEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Suggestion changed : " + e.ToString ());

			AbstractEntity suggestion = e.NewValue as AbstractEntity;

#if false
			if ((this.hintListWidget != null) &&
				(this.hintListWidget.Items != null))
			{
				if (suggestion == null)
				{
					this.hintListWidget.Items.MoveCurrentToFirst ();
				}
				else
				{
					this.hintListWidget.Items.MoveCurrentTo (suggestion);
				}
			}
#endif
		}

		private void HandleSearchControllerDialogDataChanged(object sender, DialogDataEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Data changed : " + e.ToString ());
		}

		private void HandleSearchControllerDialogFocusChanged(object sender, DialogFocusEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Focus changed : " + e.ToString ());

			this.FocusFieldPath = e.NewPath;
		}

		private void HandleSearchControllerPlaceholderPostProcessing(object sender, MessageEventArgs e)
		{
			switch (e.Message.MessageType)
			{
				case MessageType.KeyDown:
					if (this.hintListWidget.Navigate (e.Message))
					{
						e.Message.Handled = true;
					}
					break;
			}
		}

		private void HandleSearchControllerResolved(object sender)
		{
			this.searchResult = this.searchController.GetResolverResult (this.activeSearchContext);

			System.Diagnostics.Debug.WriteLine (string.Format ("Found {0} results", this.searchResult.AllResults.Count));

			ICollectionView view = this.searchResult.CollectionView;
			AbstractEntity suggestion = this.searchController.ActiveSearchContext.ActiveSuggestion;

			view.Refresh ();
			view.MoveCurrentTo (suggestion);

			this.hintListWidget.Items = view;
		}

		
		private readonly DialogSearchController searchController;
		private readonly CollectionView			emptyCollectionView;
		private ISearchContext					activeSearchContext;
		private EntityResolverResult			searchResult;
		private HintListWidget					hintListWidget;
		private HintListEmbedder				hintListEmbedder;
		private HintListVisibilityMode			visiblityMode;
		private HintListContentType				contentType;
		private EntityFieldPath					focusFieldPath;
	}
}
