//	Copyright � 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// La classe Layer permet de repr�senter un calque logique pour un widget
	/// o� les enfants doivent �tre organis�s de mani�re avanc�e.
	/// </summary>
	public class Layer : Types.DependencyObject, Collections.IVisualCollectionHost
	{
		public Layer(Visual parent)
		{
			this.parent = parent;
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
		public Visual							ParentVisual
		{
			get
			{
				return this.parent;
			}
		}
		
		private static object GetChildrenValue(DependencyObject o)
		{
			Layer layer = o as Layer;
			return layer.Children;
		}
		private static object GetParentVisualValue(DependencyObject o)
		{
			Layer layer = o as Layer;
			return layer.ParentVisual;
		}
		
		#region IVisualCollectionHost Members
		void Collections.IVisualCollectionHost.NotifyVisualCollectionBeforeInsertion(Collections.VisualCollection collection, Visual visual)
		{
			if ((visual.ParentLayer != null) &&
				(visual.ParentLayer != this))
			{
				//	Le widget a d�j� un layer parent, autre que nous. Il faut le
				//	d�senregistrer avant que l'insertion n'ait lieu :
				
				visual.ParentLayer.Children.Remove (visual);
				
				System.Diagnostics.Debug.Assert (visual.ParentLayer == null);
			}
		}
		void Collections.IVisualCollectionHost.NotifyVisualCollectionAfterInsertion(Collections.VisualCollection collection, Visual visual)
		{
			System.Diagnostics.Debug.Assert (this.parent != null);
			
			if (visual.ParentLayer == null)
			{
				visual.SetParentLayer (this);
			}
			
			System.Diagnostics.Debug.Assert (visual.ParentLayer == this);

			this.parent.NotifyChildrenChanged (this);
		}
		
		void Collections.IVisualCollectionHost.NotifyVisualCollectionBeforeRemoval(Collections.VisualCollection collection, Visual visual)
		{
			System.Diagnostics.Debug.Assert (visual.ParentLayer == this);
		}
		void Collections.IVisualCollectionHost.NotifyVisualCollectionAfterRemoval(Collections.VisualCollection collection, Visual visual)
		{
			System.Diagnostics.Debug.Assert (visual.ParentLayer == this);
			
			visual.SetParentLayer (null);
			
			System.Diagnostics.Debug.Assert (visual.ParentLayer == null);

			this.parent.NotifyChildrenChanged (this);
		}
		
		void Collections.IVisualCollectionHost.NotifyVisualCollectionChanged(Collections.VisualCollection collection)
		{
		}
		#endregion
		
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register ("Name", typeof (string), typeof (Layer));
		public static readonly DependencyProperty ChildrenProperty = DependencyProperty.RegisterReadOnly ("Children", typeof (Collections.VisualCollection), typeof (Layer), new DependencyPropertyMetadata (new GetValueOverrideCallback (Layer.GetChildrenValue)));
		public static readonly DependencyProperty VisualParentProperty = DependencyProperty.RegisterReadOnly ("VisualParent", typeof (Visual), typeof (Layer), new DependencyPropertyMetadata (new GetValueOverrideCallback (Layer.GetParentVisualValue)));

		private Visual							parent;
		private Collections.VisualCollection	children;
	}
}
