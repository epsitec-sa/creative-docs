//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;
	
	internal sealed class BindingBreadcrumbs : System.IDisposable
	{
		public BindingBreadcrumbs(PropertyChangedEventHandler handler)
		{
			this.handler = handler;
		}

		public void AddNode(DependencyObject source, DependencyProperty property)
		{
			if ((source != null) &&
				(source != Binding.DoNothing))
			{
				Node node = new Node (source, property);
				
				node.Attach (this.handler);
				
				this.nodes.Add (node);
			}
		}

		public void AddNode(IStructuredData source, string name)
		{
			if ((source != null) &&
				(source != Binding.DoNothing))
			{
				Node node = new Node (source, name);

				node.Attach (this.handler);

				this.nodes.Add (node);
			}
		}
		
		public void AddNode(ICollectionView source)
		{
			if (source != null)
			{
				Node node = new Node (source);

				node.Attach (this.handler);

				this.nodes.Add (node);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			foreach (Node item in this.nodes)
			{
				item.Detach (this.handler);
			}

			this.nodes.Clear ();
		}

		#endregion

		private struct Node
		{
			public Node(DependencyObject source, DependencyProperty property)
			{
				this.type     = NodeType.DependencyObject;
				this.source   = source;
				this.property = property;
				
				this.handlerRelay = null;
			}

			public Node(IStructuredData source, string name)
			{
				this.type     = NodeType.StructuredData;
				this.source   = source;
				this.property = name;
				
				this.handlerRelay = null;
			}

			public Node(ICollectionView source)
			{
				this.type     = NodeType.CollectionView;
				this.source   = source;
				this.property = null;
				
				this.handlerRelay = null;
			}

			public void Attach(PropertyChangedEventHandler handler)
			{
				switch (this.type)
				{
					case NodeType.DependencyObject:
						this.AttachDependencyObject (handler);
						break;
					case NodeType.StructuredData:
						this.AttachStructuredData (handler);
						break;
					case NodeType.CollectionView:
						this.AttachCollectionView (handler);
						break;
				}
			}

			public void Detach(PropertyChangedEventHandler handler)
			{
				switch (this.type)
				{
					case NodeType.DependencyObject:
						this.DetachDependencyObject (handler);
						break;
					case NodeType.StructuredData:
						this.DetachStructuredData (handler);
						break;
					case NodeType.CollectionView:
						this.DetachCollectionView (handler);
						break;
				}
			}

			private void AttachDependencyObject(PropertyChangedEventHandler handler)
			{
				DependencyObject   source   = (DependencyObject) this.source;
				DependencyProperty property = (DependencyProperty) this.property;

				source.AddEventHandler (property, handler);
			}

			private void AttachStructuredData(PropertyChangedEventHandler handler)
			{
				IStructuredData source = (IStructuredData) this.source;
				string          name   = (string) this.property;

				source.AttachListener (name, handler);
			}

			private void AttachCollectionView(PropertyChangedEventHandler handler)
			{
				ICollectionView source = (ICollectionView) this.source;

				System.Diagnostics.Debug.Assert (this.handlerRelay == null);

				this.handlerRelay =
					delegate (object sender)
					{
						handler (sender, new DependencyPropertyChangedEventArgs ("CurrentItem"));
					};

				source.CurrentChanged += this.handlerRelay;
			}

			private void DetachDependencyObject(PropertyChangedEventHandler handler)
			{
				DependencyObject source   = (DependencyObject) this.source;
				DependencyProperty property = (DependencyProperty) this.property;

				source.RemoveEventHandler (property, handler);
			}

			private void DetachStructuredData(PropertyChangedEventHandler handler)
			{
				IStructuredData source = (IStructuredData) this.source;
				string name   = (string) this.property;

				source.DetachListener (name, handler);
			}

			private void DetachCollectionView(PropertyChangedEventHandler handler)
			{
				ICollectionView source = (ICollectionView) this.source;

				System.Diagnostics.Debug.Assert (this.handlerRelay != null);

				source.CurrentChanged -= this.handlerRelay;
				this.handlerRelay = null;
			}

			private NodeType type;
			private object source;
			private object property;
			private Support.EventHandler handlerRelay;
		}

		private enum NodeType
		{
			None,
			DependencyObject,
			StructuredData,
			CollectionView,
		}

		private List<Node> nodes = new List<Node> ();
		private PropertyChangedEventHandler handler;
	}
}
