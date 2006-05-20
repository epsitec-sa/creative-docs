//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			}

			public void Attach(PropertyChangedEventHandler handler)
			{
				switch (this.type)
				{
					case NodeType.DependencyObject:
						this.AttachDependencyObject (handler);
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
				}
			}

			private void AttachDependencyObject(PropertyChangedEventHandler handler)
			{
				DependencyObject source   = (DependencyObject) this.source;
				DependencyProperty property = (DependencyProperty) this.property;

				source.AddEventHandler (property, handler);
			}

			private void DetachDependencyObject(PropertyChangedEventHandler handler)
			{
				DependencyObject source   = (DependencyObject) this.source;
				DependencyProperty property = (DependencyProperty) this.property;

				source.RemoveEventHandler (property, handler);
			}

			private NodeType type;
			private object source;
			private object property;
		}

		private enum NodeType
		{
			None,
			DependencyObject,
			StructuredData,
		}

		private List<Node> nodes = new List<Node> ();
		private PropertyChangedEventHandler handler;
	}
}
