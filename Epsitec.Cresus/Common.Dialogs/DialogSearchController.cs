//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			this.activeNodes = new List<Node> ();
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

			if (this.ContainsNode (placeholder))
			{
				//	The placeholder is already known and an active search element.
			}
			else
			{
				foreach (Node node in this.activeNodes)
				{
					this.ReleaseNode (node);
				}

				List<Node> nodes = new List<Node> ();

				foreach (Node node in this.GetPlaceholderGraph (placeholder.RootParent, path))
				{
					nodes.Add (node);
					this.AcquireNode (node);
				}

				this.activeNodes.Clear ();
				this.activeNodes.AddRange (nodes);
			}
			
			
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
		}

		private void AcquireNode(Node node)
		{
			System.Diagnostics.Debug.Assert (node.Placeholder.SuggestionMode == PlaceholderSuggestionMode.None);

			node.Placeholder.SuggestionMode = PlaceholderSuggestionMode.DisplayHint;
		}

		private void ReleaseNode(Node node)
		{
			System.Diagnostics.Debug.Assert (node.Placeholder.SuggestionMode == PlaceholderSuggestionMode.DisplayHint);
			
			node.Placeholder.SuggestionMode = PlaceholderSuggestionMode.None;
		}

		private IEnumerable<Node> GetPlaceholderGraph(Widgets.Widget root, EntityFieldPath path)
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

					if (fieldPath.StartsWith (path))
					{
						yield return new Node ()
						{
							Placeholder = placeholder,
							Path = fieldPath
						};
					}
				}
			}
		}

		private int FindNodeIndex(AbstractPlaceholder placeholder)
		{
			return this.activeNodes.FindIndex (node => node.Placeholder == placeholder);
		}

		private Node FindNode(AbstractPlaceholder placeholder)
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

		private bool ContainsNode(AbstractPlaceholder placeholder)
		{
			return this.FindNodeIndex (placeholder) < 0 ? false : true;
		}

		private struct Node
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
		}

		private System.IDisposable SuspendSearchHandler()
		{
			return new SuspendSearchHandlerHelper (this);
		}


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
		
		private int								suspendSearchHandler;
		private IEntityResolver					entityResolver;

		private readonly List<Node>				activeNodes;
	}
}
