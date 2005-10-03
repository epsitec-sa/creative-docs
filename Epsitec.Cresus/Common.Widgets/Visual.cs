//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Visual.
	/// </summary>
	public class Visual : Types.Object
	{
		public Visual()
		{
		}
		
		
		public string							Name
		{
			get
			{
				return (string) this.GetValue (Visual.NameProperty);
			}
			set
			{
				this.SetValue (Visual.NameProperty, value);
			}
		}
		
		public Visual							Parent
		{
			get
			{
				if (this.parent_layer == null)
				{
					return null;
				}
				else
				{
					return this.parent_layer.Visual;
				}
			}
		}
		
		public Layouts.Layer					ParentLayer
		{
			get
			{
				return this.parent_layer;
			}
		}
		
		
		internal bool							HasLayerCollection
		{
			get
			{
				return this.layer_collection == null ? false : true;
			}
		}
		
		
		internal void SetParentLayer(Layouts.Layer parent_layer)
		{
			this.parent_layer = parent_layer;
		}
		
		internal Collections.LayerCollection GetLayerCollection()
		{
			if (this.layer_collection == null)
			{
				lock (this)
				{
					if (this.layer_collection == null)
					{
						this.layer_collection = new Collections.LayerCollection (this);
					}
				}
			}
			
			return this.layer_collection;
		}
		
		
		internal void NotifyChildrenChanged(Layouts.Layer layer)
		{
		}
		
		
		private static object GetParentValue(Object o)
		{
			Visual that = o as Visual;
			return that.Parent;
		}
		
		private static object GetParentLayerValue(Object o)
		{
			Visual that = o as Visual;
			return that.ParentLayer;
		}
		
		
		public static readonly Property NameProperty = Property.Register ("Name", typeof (string), typeof (Visual));
		public static readonly Property LayerProperty = Property.Register ("Layer", typeof (string), typeof (Visual));

		public static readonly Property ParentProperty = Property.RegisterReadOnly ("Parent", typeof (Visual), typeof (Visual), new PropertyMetadata (null, new GetValueOverrideCallback (Visual.GetParentValue)));
		public static readonly Property ParentLayerProperty = Property.RegisterReadOnly ("ParentLayer", typeof (Layouts.Layer), typeof (Visual), new PropertyMetadata (null, new GetValueOverrideCallback (Visual.GetParentLayerValue)));
		public static readonly Property ChildrenProperty = Property.Register ("Children", typeof (Collections.VisualCollection), typeof (Visual));
		
		
		private Collections.LayerCollection		layer_collection;
		private Layouts.Layer					parent_layer;
	}
}
