﻿//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

namespace Epsitec.Common.Dialogs
{
	public sealed class DialogSearchController
	{
		public DialogSearchController()
		{
			this.searchContexts = new List<SearchContext> ();
		}

		/// <summary>
		/// Gets or sets the entity resolver.
		/// </summary>
		/// <value>The entity resolver.</value>
		public IEntityResolver Resolver
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

		public void NotifySearchTemplateChanged(DialogData dialogData, AbstractEntity entityData, EntityFieldPath path, DependencyPropertyChangedEventArgs e)
		{
			if ((this.suspendSearchHandler > 0) ||
				(this.entityResolver == null))
			{
				return;
			}
			
			Placeholder placeholder = PlaceholderContext.InteractivePlaceholder;

			if (placeholder == null)
			{
				//	Non-interactive update of the template; we won't kick in here.

				return;
			}

			if ((this.activeSearchContext != null) &&
				(this.activeSearchContext.ContainsNode (placeholder)))
			{
				//	The placeholder already belongs to the active search context.

				this.activeSearchContext.SetTemplateValue (placeholder);
			}
			else
			{
				//	The placeholder does not belong to the active search context:
				//	activate the matching search context or create a new one if
				//	required.

				SearchContext oldContext = this.activeSearchContext;
				SearchContext newContext = null;
				
				foreach (SearchContext context in this.searchContexts)
				{
					if (context.ContainsNode (placeholder))
					{
						newContext = context;
						break;
					}
				}

				if (newContext == null)
				{
					IEntityProxyProvider  proxyProvider = entityData;
					DialogData.FieldProxy proxy = proxyProvider.GetEntityProxy () as DialogData.FieldProxy;

					System.Diagnostics.Debug.Assert (proxy != null);

					EntityFieldPath rootPath   = proxy.GetFieldPath ().GetRootPath ();
					AbstractEntity  rootData   = proxy.DialogData.Data;
					Widgets.Widget  rootWidget = placeholder.RootParent;

					newContext = new SearchContext (rootData, rootPath);
					newContext.AnalysePlaceholderGraph (rootWidget);

					this.searchContexts.Add (newContext);
				}

				if (oldContext != newContext)
				{
					this.activeSearchContext = newContext;
					this.activeSearchContext.SetTemplateValue (placeholder);
					this.OnSearchContextChanged (new DependencyPropertyChangedEventArgs ("SearchContext", oldContext, newContext));
				}
			}

			Widgets.Application.QueueAsyncCallback (delegate
				{
					if (this.activeSearchContext != null)
					{
						this.activeSearchContext.Resolve (this.entityResolver);
					}
				});
			
#if false			
			IEntityProxyProvider  proxyProvider = entityData;
			DialogData.FieldProxy proxy = proxyProvider.GetEntityProxy () as DialogData.FieldProxy;

			using (this.SuspendSearchHandler ())
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Search contents changed: path={0}, id={1}, value={2}", path, e.PropertyName, e.NewValue ?? "<null>"));
				System.Diagnostics.Debug.WriteLine (string.Format (" field options={0}, field relation={1}", proxy.FieldOptions, proxy.FieldRelation));

				EntityContext  context  = entityData.GetEntityContext ();
				AbstractEntity template = context.CreateEmptyEntity (entityData.GetEntityStructuredTypeId ());

				template.DisableCalculations ();
				template.InternalSetValue (e.PropertyName, e.NewValue);

				AbstractEntity result = EntityResolver.Resolve (this.entityResolver, template);

				if (result != null)
				{
					//	TODO: ...
				}
			}
#endif
		}

		private void OnSearchContextChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		public void ClearSuggestions()
		{
			using (this.SuspendSearchHandler ())
			{
				foreach (SearchContext context in this.searchContexts)
				{
					context.Clear ();
				}

				this.searchContexts.Clear ();

				if (this.activeSearchContext != null)
				{
					SearchContext oldContext = this.activeSearchContext;
					SearchContext newContext = null;

					this.activeSearchContext = null;
					
					this.OnSearchContextChanged (new DependencyPropertyChangedEventArgs ("SearchContext", oldContext, newContext));
				}
			}
		}

		private System.IDisposable SuspendSearchHandler()
		{
			return new SuspendSearchHandlerHelper (this);
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

		private sealed class SearchContext
		{
			public SearchContext(AbstractEntity rootData, EntityFieldPath rootPath)
			{
				this.activeNodes = new List<Node> ();
				this.searchRootData = rootData;
				this.searchRootPath = rootPath;
			}

			public IList<Node> Nodes
			{
				get
				{
					return this.activeNodes;
				}
			}

			public void AnalysePlaceholderGraph(Widgets.Widget root)
			{
				foreach (Node node in this.GetPlaceholderGraph (root))
				{
					this.activeNodes.Add (node);
				}

				AbstractEntity entityData = this.searchRootPath.NavigateRead (this.searchRootData) as AbstractEntity;
				EntityContext  context = entityData.GetEntityContext ();

				this.searchTemplate = context.CreateEmptyEntity (entityData.GetEntityStructuredTypeId ());
				this.searchTemplate.DisableCalculations ();
			}

			public void Clear()
			{
				foreach (Node node in this.activeNodes)
				{
					System.Diagnostics.Debug.Assert (node.Placeholder.SuggestionMode == PlaceholderSuggestionMode.DisplayHint);

					node.Placeholder.SuggestionMode = PlaceholderSuggestionMode.DisplayHintResetText;
					node.Placeholder.Value = UndefinedValue.Value;
					node.Placeholder.SuggestionMode = PlaceholderSuggestionMode.DisplayHint;
				}
			}

			public void SetTemplateValue(AbstractPlaceholder placeholder)
			{
				Node node = this.FindNode (placeholder);

				System.Diagnostics.Debug.Assert (node.IsEmpty == false);
				
				EntityFieldPath readPath  = node.Path;
				EntityFieldPath writePath = node.Path.StripStart (this.searchRootPath);

				object value = readPath.NavigateRead (this.searchRootData);

				writePath.CreateMissingNodes (this.searchTemplate);
				writePath.NavigateWrite (this.searchTemplate, value);
			}

			public void SetSuggestionValue(Node node, AbstractEntity entity)
			{
				EntityFieldPath readPath = node.Path.StripStart (this.searchRootPath);
				
				object value = entity == null ? UndefinedValue.Value : readPath.NavigateRead (entity);

				node.Placeholder.Value = value;
			}

			public int FindNodeIndex(AbstractPlaceholder placeholder)
			{
				return this.activeNodes.FindIndex (node => node.Placeholder == placeholder);
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
					return this.activeNodes[index];
				}
			}

			public bool ContainsNode(AbstractPlaceholder placeholder)
			{
				return this.FindNodeIndex (placeholder) < 0 ? false : true;
			}

			public void Resolve(IEntityResolver entityResolver)
			{
				AbstractEntity result = EntityResolver.Resolve (entityResolver, this.searchTemplate);

				foreach (Node node in this.activeNodes)
				{
					this.SetSuggestionValue (node, result);
				}
			}
			
			private IEnumerable<Node> GetPlaceholderGraph(Widgets.Widget root)
			{
				foreach (AbstractPlaceholder placeholder in root.FindAllChildren (child => child is AbstractPlaceholder))
				{
					BindingExpression binding = placeholder.ValueBindingExpression;
					DataSourceType sourceType = binding.GetSourceType ();

					if (sourceType == DataSourceType.StructuredData)
					{
						AbstractEntity entity = binding.GetSourceObject () as AbstractEntity;
						string         field  = binding.GetSourceProperty () as string;

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
			}

			private readonly List<Node>			activeNodes;
			private AbstractEntity				searchTemplate;
			private EntityFieldPath				searchRootPath;
			private AbstractEntity				searchRootData;
		}
		
		private int								suspendSearchHandler;
		private IEntityResolver					entityResolver;

		private readonly List<SearchContext>	searchContexts;
		private SearchContext					activeSearchContext;
	}
}
