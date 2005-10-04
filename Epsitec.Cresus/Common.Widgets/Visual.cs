//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets.Helpers;

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
		
		
		public AnchorStyles						Anchor
		{
			get
			{
				return AnchorStyles.None;
			}
			set
			{
			}
		}
		
		public Drawing.Margins					AnchorMargins
		{
			get
			{
				return Drawing.Margins.Zero;
			}
			set
			{
			}
		}
		
		public DockStyle						Dock
		{
			get
			{
				return DockStyle.None;
			}
			set
			{
			}
		}
		
		public Drawing.Margins					DockPadding
		{
			get
			{
				return Drawing.Margins.Zero;
			}
			set
			{
			}
		}
		
		public Drawing.Margins					DockMargins
		{
			get
			{
				return Drawing.Margins.Zero;
			}
			set
			{
			}
		}
		
		
		public Drawing.Rectangle				Bounds
		{
			get
			{
				return Drawing.Rectangle.Empty;
			}
			set
			{
			}
		}
		
		public Widget.ClientInfo				Client
		{
			get
			{
				return new Widget.ClientInfo ();
			}
		}
		
		public Drawing.Margins					InternalPadding
		{
			get
			{
				return Drawing.Margins.Zero;
			}
		}
		
		public double							Width
		{
			get
			{
				return this.Bounds.Width;
			}
			set
			{
			}
		}
		
		public double							Height
		{
			get
			{
				return this.Bounds.Height;
			}
			set
			{
			}
		}
		
		public Drawing.Size						MinSize
		{
			get
			{
				return Drawing.Size.Empty;
			}
		}
		
		public Drawing.Size						MaxSize
		{
			get
			{
				return Drawing.Size.Empty;
			}
		}
		
		public Drawing.Size						ResultingMinSize
		{
			get
			{
				return this.MinSize;
			}
		}
		
		public Drawing.Size						ResultingMaxSize
		{
			get
			{
				return this.MaxSize;
			}
		}
		
		public ContainerLayoutMode				ContainerLayoutMode
		{
			get
			{
				return ContainerLayoutMode.None;
			}
			set
			{
			}
		}
		
		
		public bool								Visibility
		{
			get
			{
				return true;
			}
			set
			{
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
		
		internal void SetBounds(Drawing.Rectangle bounds)
		{

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
		
		internal void NotifyLayoutChanged()
		{
		}
		
		internal void NotifyParentLayoutChanged()
		{
			Visual parent = this.Parent;
			
			if (parent != null)
			{
				parent.NotifyLayoutChanged ();
			}
		}
		
		
		static Visual()
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
		
		
		public static readonly Property NameProperty			= Property.Register ("Name", typeof (string), typeof (Visual));
		public static readonly Property ParentProperty			= Property.RegisterReadOnly ("Parent", typeof (Visual), typeof (Visual), new PropertyMetadata (null, new GetValueOverrideCallback (Visual.GetParentValue)));
		public static readonly Property ParentLayerProperty		= Property.RegisterReadOnly ("ParentLayer", typeof (Layouts.Layer), typeof (Visual), new PropertyMetadata (null, new GetValueOverrideCallback (Visual.GetParentLayerValue)));
		
		public static readonly Property AnchorProperty			= Property.Register ("Anchor", typeof (AnchorStyles), typeof (Visual), new VisualPropertyMetadata (VisualPropertyFlags.AffectsParentLayout));
		public static readonly Property AnchorMarginsProperty	= Property.Register ("AnchorMargins", typeof (Drawing.Margins), typeof (Visual), new VisualPropertyMetadata (VisualPropertyFlags.AffectsParentLayout));
		public static readonly Property DockProperty			= Property.Register ("Dock", typeof (DockStyle), typeof (Visual), new VisualPropertyMetadata (VisualPropertyFlags.AffectsParentLayout));
		public static readonly Property DockPaddingProperty		= Property.Register ("DockPadding", typeof (Drawing.Margins), typeof (Visual), new VisualPropertyMetadata (VisualPropertyFlags.AffectsParentLayout));
		public static readonly Property DockMarginsProperty		= Property.Register ("DockMargins", typeof (Drawing.Margins), typeof (Visual), new VisualPropertyMetadata (VisualPropertyFlags.AffectsParentLayout));
		
		
		public static readonly Property ChildrenProperty = Property.Register ("Children", typeof (Collections.VisualCollection), typeof (Visual));
		
		
		private Collections.LayerCollection		layer_collection;
		private Layouts.Layer					parent_layer;
	}
}
