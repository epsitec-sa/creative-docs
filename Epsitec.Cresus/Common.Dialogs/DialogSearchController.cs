//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>DialogSearchController</c> class manages searches based on
	/// <see cref="DialogData"/> and a set of <see cref="Placeholder"/>
	/// instances.
	/// </summary>
	public sealed class DialogSearchController : System.IDisposable, IPaintFilter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DialogSearchController"/> class.
		/// </summary>
		public DialogSearchController()
		{
			this.searchContexts = new List<SearchContext> ();
		}

		/// <summary>
		/// Gets or sets the dialog data attached to this search controller.
		/// </summary>
		/// <value>The dialog data.</value>
		public DialogData						DialogData
		{
			get
			{
				return this.dialogData;
			}
			set
			{
				if (this.dialogData != value)
				{
					this.Attach (value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the dialog window attached to this search controller.
		/// </summary>
		/// <value>The dialog window.</value>
		public Widgets.Window					DialogWindow
		{
			get
			{
				return this.dialogWindow;
			}
			set
			{
				if (this.dialogWindow != value)
				{
					Widgets.Window oldWindow = this.dialogWindow;
					Widgets.Window newWindow = value;
					
					this.dialogWindow = value;

					this.OnDialogWindowChanged (oldWindow, newWindow);
				}
			}
		}

		/// <summary>
		/// Gets or sets the dialog panel attached to this search controller.
		/// </summary>
		/// <value>The dialog panel.</value>
		public UI.Panel							DialogPanel
		{
			get
			{
				return this.dialogPanel;
			}
			set
			{
				if (this.dialogPanel != value)
				{
					this.dialogPanel = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the entity resolver for this search controller.
		/// </summary>
		/// <value>The entity resolver.</value>
		public IEntityResolver					Resolver
		{
			get
			{
				return this.entityResolver;
			}
			set
			{
				this.entityResolver = value;
			}
		}

		/// <summary>
		/// Gets the active search context.
		/// </summary>
		/// <value>The active search context.</value>
		public ISearchContext					ActiveSearchContext
		{
			get
			{
				return this.activeSearchContext;
			}
		}

		/// <summary>
		/// Gets the active placeholder.
		/// </summary>
		/// <value>The active placeholder.</value>
		public AbstractPlaceholder				ActivePlaceholder
		{
			get
			{
				return this.activePlaceholder;
			}
		}

		/// <summary>
		/// Gets or sets the default suggestion for the initial search. This
		/// is used by the <c>Cresus.Core</c> project to make sure that after
		/// a deserialization, the proper suggestion will be hilited in the
		/// list.
		/// </summary>
		/// <value>The default suggestion.</value>
		public AbstractEntity					DefaultSuggestion
		{
			get
			{
				return this.defaultSuggestion;
			}
			set
			{
				if (this.defaultSuggestion != value)
				{
					this.defaultSuggestion = value;

					if (this.activeSearchContext != null)
					{
						this.activeSearchContext.SetSuggestion (value);
					}
				}
			}
		}

		/// <summary>
		/// Resets the suggestions and the text typed in by the user in the
		/// associated <see cref="Placeholder"/> widgets.
		/// </summary>
		public void ResetSuggestions()
		{
			using (this.SuspendSearchHandler ())
			{
				this.searchCriteria = null;
				this.searchEntityId = Druid.Empty;

				foreach (SearchContext context in this.searchContexts)
				{
					context.Clear ();
					context.Resolve (this.entityResolver, Druid.Empty, null);
					context.Dispose ();
				}

				this.searchContexts.Clear ();
				this.ActivateSearchContext (null);
			}
		}

		public void SetSearchCriteria(Druid searchEntityId, string searchCriteria)
		{
			if ((this.searchEntityId == searchEntityId) &&
				(this.searchCriteria == searchCriteria))
			{
				return;
			}

			this.searchEntityId = searchEntityId;
			this.searchCriteria = searchCriteria;
			this.ClearActiveSuggestion ();
		}

		public void ClearActiveSuggestion()
		{
			using (this.SuspendSearchHandler ())
			{
				SearchContext context = this.activeSearchContext;

				if (context != null)
				{
					context.Clear ();
					this.AsyncResolveSearch ();
				}
			}
		}

		/// <summary>
		/// Gets the resolver result for the specified search context.
		/// </summary>
		/// <param name="searchContext">The search context.</param>
		/// <returns>The resolver results, never <c>null</c>.</returns>
		public EntityResolverResult GetResolverResult(ISearchContext searchContext)
		{
			SearchContext context = searchContext as SearchContext;

			if (context == null)
			{
				return EntityResolverResult.Empty;
			}
			else
			{
				return context.ResolverResult;
			}
		}

		/// <summary>
		/// Asserts that the search controller is ready for use.
		/// </summary>
		public void AssertReady()
		{
			System.Diagnostics.Debug.Assert (this.DialogData != null);
//			System.Diagnostics.Debug.Assert (this.DialogWindow != null);
			System.Diagnostics.Debug.Assert (this.DialogPanel != null);
			System.Diagnostics.Debug.Assert (this.Resolved != null);
		}

		/// <summary>
		/// Sets the focus on the placeholder associated with the specified
		/// field, if it can be found.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns><c>true</c> if the focus was successfully set.</returns>
		public bool SetFocus(EntityFieldPath path)
		{
			//	Make sure that the placeholders had the opportunity to create their
			//	user interface, or else we won't set the focus on the proper widget :

			Application.ExecuteAsyncCallbacks ();

			if ((path == null) ||
				(path.IsEmpty))
			{
				this.dialogPanel.SetFocusOnTabWidget ();
				return this.dialogPanel.ContainsKeyboardFocus;
			}
			else
			{
				AbstractPlaceholder placeholder = DialogSearchController.FindPlaceholder (this.dialogData.Panel, path);

				if (placeholder != null)
				{
					placeholder.SetFocusOnTabWidget ();
					return placeholder.ContainsKeyboardFocus;
				}
				else
				{
					return false;
				}
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
			foreach (SearchContext context in this.searchContexts)
			{
				context.Dispose ();
			}

			this.searchContexts.Clear ();
			this.DialogData = null;
			this.Resolver = null;
		}

		#endregion

		/// <summary>
		/// Attaches the search controller to the specified dialog data.
		/// </summary>
		/// <param name="data">The dialog data.</param>
		private void Attach(DialogData data)
		{
			if (this.dialogData != null)
			{
				this.Detach (this.dialogData);
			}

			this.dialogData = data;

			if (this.dialogData != null)
			{
				PlaceholderContext.ContextActivated += this.HandlePlaceholderContextActivated;
				PlaceholderContext.ContextDeactivated += this.HandlePlaceholderContextDeactivated;
			}
		}

		/// <summary>
		/// Detaches the search controller from the specified dialog data.
		/// </summary>
		/// <param name="data">The dialog data.</param>
		private void Detach(DialogData data)
		{
			if (this.dialogData != data)
			{
				throw new System.ArgumentException ("Invalid dialog data", "data");
			}

			PlaceholderContext.ContextActivated -= this.HandlePlaceholderContextActivated;
			PlaceholderContext.ContextDeactivated -= this.HandlePlaceholderContextDeactivated;
		}

		/// <summary>
		/// Finds the parent placeholder for the specified visual.
		/// </summary>
		/// <param name="visual">The visual.</param>
		/// <returns>The parent placeholder or <c>null</c> if no parent is a
		/// placeholder.</returns>
		private static AbstractPlaceholder FindParentPlaceholder(Widgets.Visual visual)
		{
			while (visual != null)
			{
				AbstractPlaceholder placeholder = visual as AbstractPlaceholder;

				if (placeholder != null)
				{
					return placeholder;
				}

				visual = visual.Parent;
			}

			return null;
		}

		/// <summary>
		/// Handles the activation of a placeholder context. If this is the first
		/// activation on the placeholder context stack, then it originates from
		/// a direct user interaction.
		/// </summary>
		/// <param name="sender">The controller which gets activated.</param>
		private void HandlePlaceholderContextActivated(object sender)
		{
			UI.Controllers.AbstractController controller = sender as UI.Controllers.AbstractController;

			if ((PlaceholderContext.Depth == 1) &&
				(this.suspendSearchHandler == 0) &&
				(this.DialogWindow != null) &&
				(controller.Placeholder.Window == this.DialogWindow))
			{
				object      value       = controller.GetConvertedUserInterfaceValue ();
				Placeholder placeholder = PlaceholderContext.GetInteractivePlaceholder (this.DialogWindow);

				this.OnUserInteraction ();
				this.UpdateSearchTemplate (placeholder, value);
			}
		}

		private void HandlePlaceholderContextDeactivated(object sender)
		{
			if ((PlaceholderContext.Depth == 0) &&
				(this.dialogDataEventArgsCache != null))
			{
				this.OnDialogDataChanged (this.dialogDataEventArgsCache);
			}
		}

		/// <summary>
		/// Handles the focused widget changes. This is used to track the active
		/// search context.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="Epsitec.Common.Types.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private void HandleWindowFocusedWidgetChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			AbstractPlaceholder oldPlaceholder = DialogSearchController.FindParentPlaceholder (e.OldValue as Widgets.Visual);
			AbstractPlaceholder newPlaceholder = DialogSearchController.FindParentPlaceholder (e.NewValue as Widgets.Visual);

			System.Diagnostics.Debug.WriteLine (e.ToString ());

			EntityFieldPath oldPath = DialogSearchController.GetPlaceholderPath (oldPlaceholder);
			EntityFieldPath newPath = DialogSearchController.GetPlaceholderPath (newPlaceholder);

			if ((this.activeSearchContext != null) &&
				(this.activeSearchContext.ContainsNode (newPlaceholder)))
			{
				//	Nothing to do : the currently active search context still has
				//	the focus.

				this.ActivatePlaceholder (newPlaceholder);
			}
			else
			{
				this.UpdateSearchTemplate (newPlaceholder, UndefinedValue.Value);
			}

			this.OnDialogFocusChanged (new DialogFocusEventArgs (oldPath, newPath));
		}

		/// <summary>
		/// Handles the focused widget changes, before they are applied, in order
		/// to avoid setting the focus on the hint list.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="Epsitec.Common.Widgets.FocusChangingEventArgs"/> instance containing the event data.</param>
		private void HandleWindowFocusedWidgetChanging(object sender, Widgets.FocusChangingEventArgs e)
		{
			Widgets.Widget focus = e.NewFocus;

			//	Prevent the focus from being set into the dialog search controller,
			//	when the user clicks in the left pane.

			while (focus != null)
			{
				if (focus is HintListWidget)
				{
					e.Cancel = true;
					break;
				}
				if (focus is HintListSearchWidget)
				{
					break;
				}

				focus = focus.Parent;
			}
		}

		/// <summary>
		/// Updates the search template based on the contents of the specified
		/// <see cref="Placeholder"/> widget.
		/// </summary>
		/// <param name="placeholder">The placeholder.</param>
		/// <param name="value">The actual value of the placeholder.</param>
		private void UpdateSearchTemplate(AbstractPlaceholder placeholder, object value)
		{
			if ((this.suspendSearchHandler > 0) ||
				(this.entityResolver == null))
			{
				return;
			}
			
			if (placeholder == null)
			{
				//	Non-interactive update of the template; we won't kick in here.

				return;
			}

			if ((this.activeSearchContext != null) &&
				(this.activeSearchContext.ContainsNode (placeholder)))
			{
				//	The placeholder already belongs to the active search context.

				this.activeSearchContext.SetTemplateValue (placeholder, value);
			}
			else
			{
				//	The placeholder does not belong to the active search context:
				//	activate the matching search context or create a new one if
				//	required.

				SearchContext newContext = null;

				if (placeholder.SuggestionMode != PlaceholderSuggestionMode.None)
				{
					foreach (SearchContext context in this.searchContexts)
					{
						if (context.ContainsNode (placeholder))
						{
							newContext = context;
							break;
						}
					}
				}

				if (newContext == null)
				{
					if (this.dialogData.Mode == DialogDataMode.Search)
					{
						newContext = new SearchContext (this, this.dialogData.Data, EntityFieldPath.CreateRelativePath ());
						newContext.AnalyzePlaceholderGraph (Panel.GetParentPanel (placeholder), true);

						this.searchContexts.Add (newContext);
					}
					else if (placeholder is ReferencePlaceholder)
					{
						ReferencePlaceholder reference = placeholder as ReferencePlaceholder;

						EntityFieldPath rootPath   = reference.EntityFieldPath;
						AbstractEntity  rootData   = this.DialogData.Data;
						Widgets.Widget  rootWidget = Panel.GetParentPanel (placeholder);

						System.Diagnostics.Debug.Assert (rootPath != null);
						System.Diagnostics.Debug.Assert (rootPath.Count == 1);
						System.Diagnostics.Debug.Assert (rootData != null);
						System.Diagnostics.Debug.Assert (rootWidget != null);

						newContext = new SearchContext (this, rootData, rootPath);
						newContext.AnalyzePlaceholderGraph (rootWidget, false);

						this.defaultSuggestion = reference.Value as AbstractEntity;
						this.searchContexts.Add (newContext);
					}
					else
					{
						IEntityProxyProvider  proxyProvider = DialogSearchController.GetEntityDataAndField (placeholder).Entity;
						DialogData.FieldProxy proxy = proxyProvider.GetEntityProxy () as DialogData.FieldProxy;

						if (proxy == null)
						{
							//	There is no proxy backing the current placeholder, assume that
							//	it does not belong to a search field...

							this.HandleDialogDataChanged (placeholder, value);
						}
						else
						{
							EntityFieldPath rootPath   = proxy.GetFieldPath ().GetRootPath ();
							AbstractEntity  rootData   = proxy.DialogData.Data;
							Widgets.Widget  rootWidget = Panel.GetParentPanel (placeholder);

							System.Diagnostics.Debug.Assert (rootPath != null);
							System.Diagnostics.Debug.Assert (rootPath.Count == 1);
							System.Diagnostics.Debug.Assert (rootData != null);
							System.Diagnostics.Debug.Assert (rootWidget != null);

							newContext = new SearchContext (this, rootData, rootPath);
							newContext.AnalyzePlaceholderGraph (rootWidget, false);

							this.searchContexts.Add (newContext);
						}
					}
				}

				this.ActivatePlaceholder (placeholder);
				this.ActivateSearchContext (newContext);
				
				if (this.activeSearchContext != null)
				{
					this.activeSearchContext.SetTemplateValue (placeholder, value);
				}
			}

			if (this.activeSearchContext != null)
			{
				Widgets.Application.QueueAsyncCallback (this.AsyncResolveSearch);
			}
		}

		/// <summary>
		/// Handle changes of the dialog data based on the placeholder and the
		/// associated value. This will call <see cref="M:OnDialogDataChanged"/>
		/// if the data did indeed change.
		/// </summary>
		/// <param name="placeholder">The placeholder.</param>
		/// <param name="value">The value.</param>
		private void HandleDialogDataChanged(AbstractPlaceholder placeholder, object value)
		{
			if (value == UndefinedValue.Value)
			{
				//	Do nothing; this is not a real dialog data change event, but
				//	a fake event provoked through a focus change, for instance.
			}
			else
			{
				EntityFieldPath fieldPath = DialogSearchController.GetPlaceholderPath (placeholder);
				
				if (fieldPath != null)
				{
					object oldValue = fieldPath.NavigateRead (this.dialogData.Data);
					object newValue = value;

					if (DependencyObject.EqualObjectValues (oldValue, newValue))
					{
						//	Nothing to do -- exactly the same value before and after
						//	the change (!)
					}
					else
					{
						this.OnDialogDataChanging (new DialogDataEventArgs (fieldPath, placeholder, oldValue, newValue));
					}
				}
			}
		}

		private static AbstractPlaceholder FindPlaceholder(Widgets.Widget root, EntityFieldPath path)
		{
			foreach (AbstractPlaceholder placeholder in root.FindAllChildren (child => child is AbstractPlaceholder))
			{
				if (DialogSearchController.GetPlaceholderPath (placeholder) == path)
				{
					return placeholder;
				}
			}

			return null;
		}

		private static EntityFieldPath GetPlaceholderPath(AbstractPlaceholder placeholder)
		{
			if (placeholder == null)
			{
				return null;
			}

			BindingExpression bindingExpression = placeholder.ValueBindingExpression;
			Binding           binding           = bindingExpression.ParentBinding;
			DataSourceType    sourceType        = bindingExpression.GetSourceType ();

			string path       = binding.Path;
			string pathPrefix = string.Concat (DataSource.DataName, ".");

			if ((sourceType == DataSourceType.StructuredData) &&
				(path.StartsWith (pathPrefix)))
			{
				return EntityFieldPath.Parse (path.Substring (pathPrefix.Length));
			}
			else
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Unexpected binding: source type={0}, path={1}", sourceType, path));
				return null;
			}
		}

		private void NotifySuggestionChanged(EntityFieldPath path, AbstractEntity oldSuggestion, AbstractEntity newSuggestion)
		{
			if (oldSuggestion != newSuggestion)
			{
				this.OnSuggestionChanged (new DialogDataEventArgs (path, null, oldSuggestion, newSuggestion));
			}
		}

		/// <summary>
		/// Notifies the dialog search controller that the suggestion changed and
		/// that the dialog data validty changed as a result.
		/// </summary>
		/// <param name="suggestion">The suggestion.</param>
		/// <param name="oldValidity">Old dialog data validity.</param>
		/// <param name="newValidity">New dialog data validity.</param>
		private void NotifySuggestionChangedDialogDataValidty(AbstractEntity suggestion, bool oldValidity, bool newValidity)
		{
			System.Diagnostics.Debug.WriteLine ("Suggestion set to " + (suggestion == null ? "<null>" : suggestion.Dump ()));

			if ((oldValidity != newValidity) &&
				(this.dialogPanel != null))
			{
				Widgets.CommandContext    commandContext    = Widgets.Helpers.VisualTree.GetCommandContext (this.dialogPanel);
				Widgets.ValidationContext validationContext = Widgets.Helpers.VisualTree.GetValidationContext (this.dialogPanel);

				if ((commandContext != null) &&
					(validationContext != null))
				{
					commandContext.SetGroupEnable (validationContext, "Accept", newValidity);
				}
			}
		}

		/// <summary>
		/// Runs the real search, resolve to a given suggestion and set it as
		/// the current suggestion in the active search context; this method
		/// is called asynchronously from the main event loop.
		/// </summary>
		private void AsyncResolveSearch()
		{
			CancelEventArgs e = new CancelEventArgs ();
			this.OnResolving (e);

			if (e.Cancel)
			{
				//	Do nothing more - the event was canceled.
			}
			else
			{
				if ((this.activeSearchContext != null) &&
					(this.entityResolver != null))
				{
					this.activeSearchContext.Resolve (this.entityResolver, this.searchEntityId, this.searchCriteria);
				}

				this.OnResolved ();
			}
		}

		/// <summary>
		/// Raises the <see cref="E:Resolving"/> event.
		/// </summary>
		/// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
		private void OnResolving(CancelEventArgs e)
		{
			if (this.Resolving != null)
			{
				this.Resolving (this, e);
			}
		}

		/// <summary>
		/// Raises the <see cref="E:Resolved"/> event.
		/// </summary>
		private void OnResolved()
		{
			if (this.Resolved != null)
			{
				this.Resolved (this);
			}
		}

		private void OnUserInteraction()
		{
			if (this.UserInteraction != null)
			{
				this.UserInteraction (this);
			}
		}

		private void ActivatePlaceholder(AbstractPlaceholder placeholder)
		{
			AbstractPlaceholder newPlaceholder = placeholder;
			AbstractPlaceholder oldPlaceholder = this.activePlaceholder;

			if (oldPlaceholder != newPlaceholder)
			{
				this.activePlaceholder = placeholder;

				this.OnActivePlaceholderChanged (new DependencyPropertyChangedEventArgs ("Placeholder", oldPlaceholder, newPlaceholder));
			}
		}

		/// <summary>
		/// Activates the specified search context.
		/// </summary>
		/// <param name="newContext">The new context.</param>
		private void ActivateSearchContext(SearchContext context)
		{
			SearchContext newContext = context;
			SearchContext oldContext = this.activeSearchContext;

			if (oldContext != newContext)
			{
				if (oldContext != null)
				{
					SearchContext.Deactivate (oldContext);
				}

				this.activeSearchContext = context;

				if (newContext != null)
				{
					SearchContext.Activate (newContext);
				}
				else
				{
					DialogSearchController.globalContext.NotifyActivity (null);
				}

				this.OnSearchContextChanged (new DependencyPropertyChangedEventArgs ("SearchContext", oldContext, newContext));
			}
		}

		private void OnActivePlaceholderChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.ActivePlaceholderChanged != null)
			{
				this.ActivePlaceholderChanged (this, e);
			}
		}

		/// <summary>
		/// Raises the <see cref="E:SearchContextChanged"/> event when the active
		/// search context changes.
		/// </summary>
		/// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance
		/// containing the event data.</param>
		private void OnSearchContextChanged(DependencyPropertyChangedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine (e.ToString ());
 
			if (this.SearchContextChanged != null)
			{
				this.SearchContextChanged (this, e);
			}

			if (this.dialogPanel != null)
			{
				this.dialogPanel.Invalidate ();
			}
		}

		/// <summary>
		/// Raises the <see cref="E:SuggestionChanged"/> event.
		/// </summary>
		/// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private void OnSuggestionChanged(DialogDataEventArgs e)
		{
			if (this.SuggestionChanged != null)
			{
				this.SuggestionChanged (this, e);
			}
		}

		/// <summary>
		/// Called when the dialog window changes. This is used to attach or detach
		/// event handlers to track the keyboard focus.
		/// </summary>
		/// <param name="oldWindow">The old window.</param>
		/// <param name="newWindow">The new window.</param>
		private void OnDialogWindowChanged(Widgets.Window oldWindow, Widgets.Window newWindow)
		{
			if (oldWindow != null)
			{
				oldWindow.FocusedWidgetChanged -= this.HandleWindowFocusedWidgetChanged;
				oldWindow.FocusedWidgetChanging -= this.HandleWindowFocusedWidgetChanging;
				oldWindow.PaintFilter = null;
			}
			if (newWindow != null)
			{
				newWindow.FocusedWidgetChanged += this.HandleWindowFocusedWidgetChanged;
				newWindow.FocusedWidgetChanging += this.HandleWindowFocusedWidgetChanging;
				newWindow.PaintFilter = this;
			}
		}

		private void OnPlaceholderPostProcessing(AbstractPlaceholder sender, Widgets.MessageEventArgs e)
		{
			if (this.PlaceholderPostProcessing != null)
			{
				this.PlaceholderPostProcessing (sender, e);
			}
		}

		private void OnDialogDataChanging(DialogDataEventArgs e)
		{
			this.dialogDataEventArgsCache = e;

			if (this.DialogDataChanging != null)
			{
				this.DialogDataChanging (this, e);
			}
		}

		private void OnDialogDataChanged(DialogDataEventArgs e)
		{
			this.dialogDataEventArgsCache = null;

			if (this.DialogDataChanged != null)
			{
				this.DialogDataChanged (this, e);
			}
		}

		private void OnDialogFocusChanged(DialogFocusEventArgs e)
		{
			if (this.DialogFocusChanged != null)
			{
				this.DialogFocusChanged (this, e);
			}
		}



		/// <summary>
		/// Temporarily disables the search handler.
		/// </summary>
		/// <returns>A disposable object which must be used in a <c>using</c> block.</returns>
		private System.IDisposable SuspendSearchHandler()
		{
			return new SuspendSearchHandlerHelper (this);
		}

		/// <summary>
		/// Gets the entity data and associated field name for the specified
		/// placeholder. This uses binding analysis to retrieve the information.
		/// </summary>
		/// <param name="placeholder">The placeholder.</param>
		/// <returns>The entity data and field.</returns>
		private static EntityField GetEntityDataAndField(AbstractPlaceholder placeholder)
		{
			BindingExpression   binding     = placeholder.ValueBindingExpression;
			DataSourceType      sourceType  = binding == null ? DataSourceType.None : binding.GetSourceType ();

			if (sourceType == DataSourceType.StructuredData)
			{
				return new EntityField ()
				{
					Entity = binding.GetSourceObject () as AbstractEntity,
					Field = binding.GetSourceProperty () as string
				};
			}
			else
			{
				return new EntityField ();
			}
		}

		#region IPaintFilter Members

		bool IPaintFilter.IsWidgetFullyDiscarded(Widget widget)
		{
			return false;
		}

		bool IPaintFilter.IsWidgetPaintDiscarded(Widget widget)
		{
			return false;
		}

		void IPaintFilter.NotifyAboutToProcessChildren(Widget sender, PaintEventArgs e)
		{
			if ((this.activeSearchContext != null) &&
				(this.dialogData.Mode != DialogDataMode.Search))
			{
				if (Widgets.Helpers.VisualTree.IsAncestor (this.dialogPanel, sender))
				{
					AbstractPlaceholder placeholder = this.activePlaceholder;
					
					Drawing.Rectangle rootRect  = placeholder.MapClientToRoot (placeholder.Client.Bounds);
					Drawing.Rectangle localRect = Drawing.Rectangle.Deflate (sender.MapRootToClient (rootRect), 0.5, 0.5);
					
					using (Drawing.Path path = new Drawing.Path ())
					{
						path.AppendRoundedRectangle (localRect, 6);
						e.Graphics.Rasterizer.AddSurface (path);
						e.Graphics.RenderSolid (Drawing.Color.FromRgb (255.0/255.0, 186.0/255.0, 1.0/255.0));
						e.Graphics.Rasterizer.AddOutline (path, 1, Drawing.CapStyle.Round, Drawing.JoinStyle.Round);
						e.Graphics.RenderSolid (Drawing.Color.FromBrightness (0.4));
					}
				}
			}
		}

		void IPaintFilter.NotifyChildrenProcessed(Widget sender, PaintEventArgs e)
		{
		}

		#endregion

		#region Node Structure

		private struct Node : System.IEquatable<Node>
		{
			public AbstractPlaceholder Placeholder
			{
				get;
				set;
			}
			
			public EntityFieldPath Path
			{
				get;
				set;
			}
			
			public SearchContext Context
			{
				get;
				set;
			}

			public bool IsEmpty
			{
				get
				{
					return this.Placeholder == null;
				}
			}

			#region IEquatable<Node> Members

			public bool Equals(Node other)
			{
				return this.Placeholder == other.Placeholder;
			}

			#endregion

			public override int GetHashCode()
			{
				return this.Placeholder == null ? 0 : this.Placeholder.GetHashCode ();
			}

			public override bool Equals(object obj)
			{
				if (obj is Node)
				{
					return this.Equals ((Node) obj);
				}
				else
				{
					return false;
				}
			}
		}

		#endregion

		#region EntityField Structure

		public struct EntityField
		{
			public AbstractEntity Entity
			{
				get;
				set;
			}
			public string Field
			{
				get;
				set;
			}
		}

		#endregion

		#region SuspendSearchHandlerHelper Class

		private sealed class SuspendSearchHandlerHelper : System.IDisposable
		{
			public SuspendSearchHandlerHelper(DialogSearchController host)
			{
				this.host = host;
				System.Threading.Interlocked.Increment (ref this.host.suspendSearchHandler);
			}

			#region IDisposable Members

			public void Dispose()
			{
				System.Threading.Interlocked.Decrement (ref this.host.suspendSearchHandler);
				System.GC.SuppressFinalize (this);
			}

			#endregion

			private readonly DialogSearchController host;
		}

		#endregion

		#region SearchContext Class

		/// <summary>
		/// The <c>SearchContext</c> class maintains information about a (sub-)search
		/// in a dialog.
		/// </summary>
		private sealed class SearchContext : System.IDisposable, ISearchContext
		{
			static int next = 0;
			private int id = SearchContext.next++;
			public SearchContext(DialogSearchController searchController, AbstractEntity rootData, EntityFieldPath rootPath)
			{
				this.searchController = searchController;
				this.nodes = new List<Node> ();
				this.searchRootData = rootData;
				this.searchRootPath = rootPath;
			}

			public IList<Node> Nodes
			{
				get
				{
					return this.nodes;
				}
			}

			public EntityResolverResult ResolverResult
			{
				get
				{
					return this.resolverResult;
				}
			}

			public void AnalyzePlaceholderGraph(Widgets.Widget root, bool initialSearchValues)
			{
				Druid entityId = this.searchRootData.GetEntityStructuredTypeId ();
				EntityContext  context = this.searchController.DialogData.Data.GetEntityContext ();
				AbstractEntity entityData;
				
				if (this.searchRootPath.IsEmpty)
				{
					entityData = this.searchRootData;
				}
				else
				{
					entityId   = this.searchRootPath.NavigateReadSchema (this.searchRootData).TypeId;
					entityData = this.searchRootPath.NavigateRead (this.searchRootData) as AbstractEntity;
				}

				this.searchTemplate = context.CreateSearchEntity (entityId);
				this.searchTemplate.DisableCalculations ();

				foreach (Node node in this.GetPlaceholderGraph (root))
				{
					this.nodes.Add (node);
					node.Placeholder.PostProcessing += this.HandlePlaceholderPostProcessing;

					if ((initialSearchValues) &&
						(entityData != null))
					{
						node.Path.NavigateWrite (this.searchTemplate, node.Path.NavigateRead (entityData));
					}
				}
			}

			public IEnumerable<Node> GetPlaceholderGraph(Widgets.Widget root)
			{
				foreach (AbstractPlaceholder placeholder in root.FindAllChildren (child => child is AbstractPlaceholder))
				{
					EntityField info = DialogSearchController.GetEntityDataAndField (placeholder);

					AbstractEntity entity = info.Entity;
					string         field  = info.Field;

					if ((entity == null) ||
						(field == null))
					{
						continue;
					}

					IEntityProxyProvider  proxyProvider = entity;
					DialogData.FieldProxy proxy = proxyProvider.GetEntityProxy () as DialogData.FieldProxy;

					if (proxy == null)
					{
						//	Are we currently working with a special full-search dialog? If so,
						//	there won't be a proxy for the root.

						if ((this.searchController.DialogData.Mode == DialogDataMode.Search) &&
							(this.searchRootData == entity) &&
							(this.searchRootPath.IsEmpty))
						{
							yield return new Node ()
							{
								Placeholder = placeholder,
								Path = EntityFieldPath.CreateRelativePath (field),
								Context = this
							};
						}
					}
					else
					{
						EntityFieldPath fieldPath = EntityFieldPath.CreateRelativePath (proxy.GetFieldPath (), field);

						if (fieldPath.StartsWith (this.searchRootPath))
						{
							yield return new Node ()
							{
								Placeholder = placeholder,
								Path = fieldPath,
								Context = this
							};
						}
					}
				}
			}

			public void Clear()
			{
				foreach (Node node in this.nodes)
				{
					if (node.Placeholder.SuggestionMode == PlaceholderSuggestionMode.DisplayActiveHint)
					{
						node.Placeholder.SuggestionMode = PlaceholderSuggestionMode.DisplayHintResetText;
						node.Placeholder.Value = UndefinedValue.Value;
						node.Placeholder.SuggestionMode = PlaceholderSuggestionMode.DisplayActiveHint;

						this.SetTemplateValue (node, UndefinedValue.Value);
					}
				}
			}

			public void SetTemplateValue(AbstractPlaceholder placeholder, object value)
			{
				if (value == UndefinedValue.Value)
				{
					//	Do nothing - this can happen if the user did not type
					//	anything yet but simply set the focus on one of the
					//	placeholders participating in the search.

					DialogSearchController.globalContext.NotifyActivity (this);
				}
				else
				{
					this.SetTemplateValue (this.FindNode (placeholder), value);
				}
			}

			public void SetTemplateValue(Node node, object value)
			{
				System.Diagnostics.Debug.Assert (node.IsEmpty == false);

				EntityFieldPath writePath = node.Path.StripStart (this.searchRootPath);

				writePath.CreateMissingNodes (this.searchTemplate);
				writePath.NavigateWrite (this.searchTemplate, value);

				DialogSearchController.globalContext.NotifyActivity (this);
			}

			public void SetSuggestionValue(Node node, AbstractEntity entity)
			{
				System.Diagnostics.Debug.Assert (node.Placeholder.SuggestionMode == PlaceholderSuggestionMode.DisplayActiveHint);
				EntityFieldPath readPath = node.Path.StripStart (this.searchRootPath);
				
				object oldValue = node.Placeholder.Value;
				object newValue = entity == null ? UndefinedValue.Value : readPath.NavigateRead (entity);

				if (DependencyObject.EqualObjectValues (oldValue, newValue))
				{
					//	This is the same value as previously. Since setting the
					//	value would not refresh the user interface, we force the
					//	update manually.

					//	Note: this is required since the value can map both to the
					//	user entered value or to the hint value; one might change
					//	while the other stays the same. However, when the value does
					//	not change in a visible manner, the controller associated
					//	with the placeholder cannot be informed of the change.

					node.Placeholder.InternalUpdateValue (oldValue, newValue);
				}
				else
				{
					//	Update the value displayed by the placeholder. This will
					//	use the "hint" display mode.

					BindingExpression expression = node.Placeholder.ValueBindingExpression;

					if (expression != null)
					{
						expression.SetSourceValue (newValue);
					}
					else
					{
						node.Placeholder.Value = newValue;
					}
				}
			}

			public int FindNodeIndex(AbstractPlaceholder placeholder)
			{
				return this.nodes.FindIndex (node => node.Placeholder == placeholder);
			}

			public Node FindNode(AbstractPlaceholder placeholder)
			{
				int index = this.FindNodeIndex (placeholder);

				if (index < 0)
				{
					return new Node ();
				}
				else
				{
					return this.nodes[index];
				}
			}

			public bool ContainsNode(AbstractPlaceholder placeholder)
			{
				return this.FindNodeIndex (placeholder) < 0 ? false : true;
			}

			#region IDisposable Members

			public void Dispose()
			{
				foreach (Node node in this.nodes)
				{
					node.Placeholder.PostProcessing -= this.HandlePlaceholderPostProcessing;
				}
			}

			#endregion

			#region ISearchContext Members

			public DialogSearchController SearchController
			{
				get
				{
					return this.searchController;
				}
			}

			/// <summary>
			/// Gets the search template data used by this search controller.
			/// </summary>
			/// <value>The search template data.</value>
			public AbstractEntity SearchTemplate
			{
				get
				{
					return this.searchTemplate;
				}
			}

			/// <summary>
			/// Gets the active suggestion.
			/// </summary>
			/// <value>The active suggestion.</value>
			public AbstractEntity ActiveSuggestion
			{
				get
				{
					return this.activeSuggestion;
				}
			}

			/// <summary>
			/// Gets the active placeholders used by this search controller.
			/// </summary>
			/// <returns>
			/// A collection of <see cref="AbstractPlaceholder"/> instances.
			/// </returns>
			public IEnumerable<AbstractPlaceholder> GetActivePlaceholders()
			{
				foreach (Node node in this.nodes)
				{
					yield return node.Placeholder;
				}
			}

			/// <summary>
			/// Sets the suggestion.
			/// </summary>
			/// <param name="suggestion">The suggestion.</param>
			public void SetSuggestion(AbstractEntity suggestion)
			{
				foreach (Node node in this.nodes)
				{
					this.SetSuggestionValue (node, suggestion);
				}

				bool oldValidity = this.searchController.dialogData.AreReferenceReplacementsValid ();
				this.searchController.dialogData.SetReferenceReplacement (this.searchRootPath, suggestion);
				bool newValidity = this.searchController.dialogData.AreReferenceReplacementsValid ();

				if (oldValidity != newValidity)
				{
					this.searchController.NotifySuggestionChangedDialogDataValidty (suggestion, oldValidity, newValidity);
				}

				AbstractEntity oldSuggestion = this.activeSuggestion;
				AbstractEntity newSuggestion = suggestion;
				
				if (oldSuggestion != newSuggestion)
				{
					this.activeSuggestion = suggestion;

					if (this.searchController.DialogData.Mode != DialogDataMode.Search)
					{
						ReferencePlaceholder reference = this.searchController.activePlaceholder as ReferencePlaceholder;

						if (reference != null)
						{
							if (suggestion == null)
							{
								EntityContext context = this.searchController.DialogData.EntityContext;

								using (context.SuspendConstraintChecking ())
								{
									reference.Value = null;
								}
							}
							else
							{
								reference.Value = suggestion;
							}
						}
					}

					this.searchController.NotifySuggestionChanged (this.searchRootPath, oldSuggestion, newSuggestion);
				}
			}

			/// <summary>
			/// Gets the ids of the entity types which are currently targeted by
			/// the search context.
			/// </summary>
			/// <returns>A collection of entity ids.</returns>
			public IEnumerable<Druid> GetEntityIds()
			{
				if (this.searchRootPath.IsEmpty)
				{
					return new Druid[] { this.searchRootData.GetEntityStructuredTypeId () };
				}
				else
				{
					return new Druid[] { this.searchRootPath.NavigateReadSchema (this.searchRootData).TypeId };
				}
			}

			#endregion

			/// <summary>
			/// Searches the associated collection using the specified resolver.
			/// </summary>
			/// <param name="entityResolver">The resolver.</param>
			/// <param name="searchEntityId">The search entity id.</param>
			/// <param name="searchCriteria">The search criteria.</param>
			public void Resolve(IEntityResolver entityResolver, Druid searchEntityId, string searchCriteria)
			{
				AbstractEntity template = this.searchTemplate;

				if (string.IsNullOrEmpty (searchCriteria))
				{
					this.resolverResult = EntityResolver.Resolve (entityResolver, template);
				}
				else
				{
					this.resolverResult = EntityResolver.Resolve (entityResolver, searchEntityId, searchCriteria);
				}

				AbstractEntity suggestion = this.searchController.defaultSuggestion ?? this.activeSuggestion ?? this.GetDefaultSuggestion ();

				this.searchController.defaultSuggestion = null;

				if ((suggestion != null) &&
					(this.resolverResult.AllResults.Contains (suggestion)))
				{
					this.SetSuggestion (suggestion);
				}
				else
				{
					this.SetSuggestion (this.resolverResult.FirstResult);
				}
			}

			/// <summary>
			/// Activates the specified context by making all its placeholders
			/// display an active hint.
			/// </summary>
			/// <param name="context">The search context.</param>
			public static void Activate(SearchContext context)
			{
				foreach (Node node in context.nodes)
				{
					node.Placeholder.SuggestionMode = PlaceholderSuggestionMode.DisplayActiveHint;
				}
			}

			/// <summary>
			/// Deactivates the specified context by making all its placeholders
			/// display a passive hint.
			/// </summary>
			/// <param name="context">The search context.</param>
			public static void Deactivate(SearchContext context)
			{
				foreach (Node node in context.nodes)
				{
					node.Placeholder.SuggestionMode = PlaceholderSuggestionMode.DisplayPassiveHint;
					node.Placeholder.SimulateEdition ();
				}
			}

			private void HandlePlaceholderPostProcessing(object sender, Widgets.MessageEventArgs e)
			{
				if ((e.Message.Handled == false) &&
					(e.Message.IsKeyType))
				{
					this.searchController.OnPlaceholderPostProcessing (sender as AbstractPlaceholder, e);
				}
			}

			private AbstractEntity GetDefaultSuggestion()
			{
				AbstractEntity item = this.searchRootPath.NavigateRead (this.searchController.dialogData.Data) as AbstractEntity;

				int pos = Types.Collection.FindIndex (this.resolverResult.AllResults, x => EntityContext.CompareEqual (x, item));

				if (pos < 0)
				{
					return null;
				}
				else
				{
					return this.resolverResult.AllResults[pos];
				}
			}

			private readonly List<Node>			nodes;
			private readonly DialogSearchController searchController;
			private AbstractEntity				searchTemplate;
			private EntityFieldPath				searchRootPath;
			private AbstractEntity				searchRootData;
			private AbstractEntity				activeSuggestion;
			private EntityResolverResult		resolverResult;
		}

		#endregion

		#region GlobalContext Class

		/// <summary>
		/// The <c>GlobalContext</c> class manages thread safe global settings
		/// for the <see cref="DialogSearchController"/>.
		/// </summary>
		private sealed class GlobalContext
		{
			/// <summary>
			/// Notifies activity with the specified search context.
			/// </summary>
			/// <param name="context">The active search context.</param>
			public void NotifyActivity(ISearchContext context)
			{
				ISearchContext oldContext;
				ISearchContext newContext;
				
				lock (this)
				{
					oldContext = this.activeContext;
					newContext = context;
					
					this.activeContext = context;
				}

				if (oldContext != newContext)
				{
					this.OnActiveContextChanged (new DependencyPropertyChangedEventArgs ("ActiveContext", oldContext, newContext));
				}
			}

			/// <summary>
			/// Gets the active search context.
			/// </summary>
			/// <value>The active search context.</value>
			public ISearchContext ActiveContext
			{
				get
				{
					return this.activeContext;
				}
			}

			private void OnActiveContextChanged(DependencyPropertyChangedEventArgs e)
			{
				EventHandler<DependencyPropertyChangedEventArgs> handler;

				lock (this)
				{
					handler = this.contextChanged;
				}

				if (handler != null)
				{
					handler (this, e);
				}
			}

			public event EventHandler<DependencyPropertyChangedEventArgs> ContextChanged
			{
				add
				{
					lock (this)
					{
						this.contextChanged += value;
					}
				}
				remove
				{
					lock (this)
					{
						this.contextChanged -= value;
					}
				}
			}

			private EventHandler<DependencyPropertyChangedEventArgs> contextChanged;
			private ISearchContext				activeContext;
		}

		#endregion

		public event EventHandler<DependencyPropertyChangedEventArgs> ActivePlaceholderChanged;
		
		/// <summary>
		/// Occurs when the search context changed within this instance of
		/// <see cref="DialogSearchController"/>.
		/// </summary>
		public event EventHandler<DependencyPropertyChangedEventArgs> SearchContextChanged;

		/// <summary>
		/// Occurs immediately before resolving a template from the active
		/// search context into a suggestion. The event can be canceled,
		/// which aborts the resolution.
		/// </summary>
		public event EventHandler<CancelEventArgs>			Resolving;

		/// <summary>
		/// Occurs when a template from the active search context was
		/// resolved into a suggestion.
		/// </summary>
		public event EventHandler							Resolved;

		public event EventHandler							UserInteraction;

		/// <summary>
		/// Occurs when the suggestion for the active search context changed.
		/// </summary>
		public event EventHandler<DialogDataEventArgs>		SuggestionChanged;

		public event EventHandler<Widgets.MessageEventArgs> PlaceholderPostProcessing;

		public event EventHandler<DialogDataEventArgs>		DialogDataChanging;
		public event EventHandler<DialogDataEventArgs>		DialogDataChanged;
		public event EventHandler<DialogFocusEventArgs>		DialogFocusChanged;

		/// <summary>
		/// Occurs when the search context changed, globally. This is signalled
		/// internally by the <see cref="GlobalContext"/> singleton.
		/// </summary>
		public static event EventHandler<DependencyPropertyChangedEventArgs> GlobalSearchContextChanged
		{
			add
			{
				DialogSearchController.globalContext.ContextChanged += value;
			}
			remove
			{
				DialogSearchController.globalContext.ContextChanged -= value;
			}
		}

		public static ISearchContext GetGlobalSearchContext()
		{
			return DialogSearchController.globalContext.ActiveContext;
		}


		private static readonly GlobalContext	globalContext = new GlobalContext ();

		private int								suspendSearchHandler;
		private IEntityResolver					entityResolver;

		private readonly List<SearchContext>	searchContexts;
		private SearchContext					activeSearchContext;
		private AbstractPlaceholder				activePlaceholder;
		private DialogData						dialogData;
		private Widgets.Window					dialogWindow;
		private UI.Panel						dialogPanel;
		private DialogDataEventArgs				dialogDataEventArgsCache;
		private AbstractEntity					defaultSuggestion;
		private Druid							searchEntityId;
		private string							searchCriteria;
	}
}
