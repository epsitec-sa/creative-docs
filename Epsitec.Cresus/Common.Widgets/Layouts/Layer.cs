//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// La classe Layer permet de représenter un calque logique pour un widget
	/// où les enfants doivent être organisés de manière avancée.
	/// </summary>
	public class Layer : Types.Object, Collections.IVisualCollectionHost
	{
		public Layer(Visual visual)
		{
			this.visual = visual;
		}
		
		
		public string							Name
		{
			get
			{
				return (string) this.GetValue (Layer.NameProperty);
			}
			set
			{
				this.SetValue (Layer.NameProperty, value);
			}
		}
		
		public Collections.VisualCollection		Children
		{
			get
			{
				if (this.children == null)
				{
					lock (this)
					{
						if (this.children == null)
						{
							this.children = new Collections.VisualCollection (this);
						}
					}
				}
			
				return this.children;
			}
		}
		
		public Visual							Visual
		{
			get
			{
				return this.visual;
			}
		}
		
		
		private static object GetChildrenValue(Object o)
		{
			Layer layer = o as Layer;
			return layer.Children;
		}
		
		private static object GetVisualValue(Object o)
		{
			Layer layer = o as Layer;
			return layer.Visual;
		}
		
		
		#region IVisualCollectionHost Members
		void Collections.IVisualCollectionHost.NotifyVisualCollectionInsertion(Collections.VisualCollection collection, Visual visual)
		{
			if (this.visual != null)
			{
				this.visual.NotifyVisualInsertion (this, visual);
			}
		}
		
		void Collections.IVisualCollectionHost.NotifyVisualCollectionRemoval(Collections.VisualCollection collection, Visual visual)
		{
			if (this.visual != null)
			{
				this.visual.NotifyVisualRemoval (this, visual);
			}
		}
		
		void Collections.IVisualCollectionHost.NotifyVisualCollectionChanged(Collections.VisualCollection collection)
		{
		}
		#endregion
		
		public static readonly Property NameProperty = Property.Register ("Name", typeof (string), typeof (Layer));
		public static readonly Property ChildrenProperty = Property.RegisterReadOnly ("Children", typeof (Collections.VisualCollection), typeof (Layer), new PropertyMetadata (null, new GetValueOverrideCallback (Layer.GetChildrenValue)));
		public static readonly Property VisualProperty = Property.RegisterReadOnly ("Visual", typeof (Visual), typeof (Layer), new PropertyMetadata (null, new GetValueOverrideCallback (Layer.GetVisualValue)));
		
		private Visual							visual;
		private Collections.VisualCollection	children;
	}
}
