//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>DialogSearchController</c> class manages searches based on
	/// <see cref="DialogData"/> and a set of <see cref="Placeholder"/>
	/// instances.
	/// </summary>
	public sealed class DialogSearchController : System.IDisposable
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
		/// Clears the suggestions and the text typed in by the user in the
		/// associated <see cref="Placeholder"/> widgets.
		/// </summary>
		public void ClearSuggestions()
		{
			using (this.SuspendSearchHandler ())
			{
				foreach (SearchContext context in this.searchContexts)
				{
					context.Clear ();
					context.Resolve (this.entityResolver);
				}

				this.searchContexts.Clear ();
				this.ActivateSearchContext (null);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
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
			if (PlaceholderContext.Depth == 1)
			{
				UI.Controllers.AbstractController controller = sender as UI.Controllers.AbstractController;
				object value = controller.GetActualValue ();

				this.UpdateSearchTemplate (PlaceholderContext.InteractivePlaceholder, value);
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

			if ((this.activeSearchContext != null) &&
				(this.activeSearchContext.ContainsNode (newPlaceholder)))
			{
				//	Nothing to do : the currently active search context still has
				//	the focus.
			}
			else
			{
				this.UpdateSearchTemplate (newPlaceholder, UndefinedValue.Value);
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
					IEntityProxyProvider  proxyProvider = DialogSearchController.GetEntityDataAndField (placeholder).Entity;
					DialogData.FieldProxy proxy = proxyProvider.GetEntityProxy () as DialogData.FieldProxy;

					if (proxy != null)
					{
						EntityFieldPath rootPath   = proxy.GetFieldPath ().GetRootPath ();
						AbstractEntity  rootData   = proxy.DialogData.Data;
						Widgets.Widget  rootWidget = Panel.GetParentPanel (placeholder);

						System.Diagnostics.Debug.Assert (rootPath != null);
						System.Diagnostics.Debug.Assert (rootPath.Count == 1);
						System.Diagnostics.Debug.Assert (rootData != null);
						System.Diagnostics.Debug.Assert (rootWidget != null);

						newContext = new SearchContext (this, rootData, rootPath);
						newContext.AnalysePlaceholderGraph (rootWidget);

						this.searchContexts.Add (newContext);
					}
				}

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
			if ((this.activeSearchContext != null) &&
				(this.entityResolver != null))
			{
				this.activeSearchContext.Resolve (this.entityResolver);
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

				this.OnSearchContextChanged (new DependencyPropertyChangedEventArgs ("SearchContext", oldContext, newContext));
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
			}
			if (newWindow != null)
			{
				newWindow.FocusedWidgetChanged += this.HandleWindowFocusedWidgetChanged;
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
			DataSourceType      sourceType  = binding.GetSourceType ();

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

			public void AnalysePlaceholderGraph(Widgets.Widget root)
			{
				foreach (Node node in this.GetPlaceholderGraph (root))
				{
					this.nodes.Add (node);
				}

				AbstractEntity entityData = this.searchRootPath.NavigateRead (this.searchRootData) as AbstractEntity;
				EntityContext  context = entityData.GetEntityContext ();

				this.searchTemplate = context.CreateEmptyEntity (entityData.GetEntityStructuredTypeId ());
				this.searchTemplate.DisableCalculations ();
			}

			public void Clear()
			{
				foreach (Node node in this.nodes)
				{
					System.Diagnostics.Debug.Assert (node.Placeholder.SuggestionMode == PlaceholderSuggestionMode.DisplayActiveHint);

					node.Placeholder.SuggestionMode = PlaceholderSuggestionMode.DisplayHintResetText;
					node.Placeholder.Value = UndefinedValue.Value;
					node.Placeholder.SuggestionMode = PlaceholderSuggestionMode.DisplayActiveHint;

					this.SetTemplateValue (node, UndefinedValue.Value);
				}
			}

			public void SetTemplateValue(AbstractPlaceholder placeholder, object value)
			{
				if (value == UndefinedValue.Value)
				{
					//	Do nothing - this can happen if the user did not type
					//	anything yet but simply set the focus on one of the
					//	placeholders participating in the search.
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

					node.Placeholder.Value = newValue;
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

			public void Resolve(IEntityResolver entityResolver)
			{
				AbstractEntity result = EntityResolver.Resolve (entityResolver, this.searchTemplate);
				this.SetSuggestion (result);
			}

			#region IDisposable Members

			public void Dispose()
			{
				throw new System.NotImplementedException ();
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

			public AbstractEntity SearchTemplate
			{
				get
				{
					return this.searchTemplate;
				}
			}

			public IEnumerable<AbstractPlaceholder> GetActivePlaceholders()
			{
				foreach (Node node in this.nodes)
				{
					yield return node.Placeholder;
				}
			}

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
			}

			#endregion

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

			private IEnumerable<Node> GetPlaceholderGraph(Widgets.Widget root)
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
						continue;
					}

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

			private readonly List<Node>			nodes;
			private readonly DialogSearchController searchController;
			private AbstractEntity				searchTemplate;
			private EntityFieldPath				searchRootPath;
			private AbstractEntity				searchRootData;
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
						this.ContextChanged -= value;
					}
				}
			}

			private EventHandler<DependencyPropertyChangedEventArgs> contextChanged;
			private ISearchContext				activeContext;
		}

		#endregion

		/// <summary>
		/// Occurs when the search context changed within this instance of
		/// <see cref="DialogSearchController"/>.
		/// </summary>
		public event EventHandler<DependencyPropertyChangedEventArgs> SearchContextChanged;

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


		private static readonly GlobalContext	globalContext = new GlobalContext ();

		private int								suspendSearchHandler;
		private IEntityResolver					entityResolver;

		private readonly List<SearchContext>	searchContexts;
		private SearchContext					activeSearchContext;
		private DialogData						dialogData;
		private Widgets.Window					dialogWindow;
		private UI.Panel						dialogPanel;
	}
}
