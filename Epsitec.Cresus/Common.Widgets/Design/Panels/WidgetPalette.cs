namespace Epsitec.Common.Widgets.Design.Panels
{
	/// <summary>
	/// La classe WidgetPalette permet de remplir un panel servant de base à la
	/// palette des widgets servant à la construction de la GUI.
	/// </summary>
	public class WidgetPalette
	{
		public WidgetPalette(PreferredLayout preference)
		{
			this.preference = preference;
			
			//	TODO: déterminer la taille utile en fonction de l'orientation préférentielle
			//	passée en entrée, au cas où on voudrait mettre la palette dans une barre
			//	vericale, etc.
			
			this.size = new Drawing.Size (172, 125);
		}
		
		public Drawing.Size				Size
		{
			get
			{
				return this.size;
			}
		}
		
		public PreferredLayout			PreferredLayout
		{
			get
			{
				return this.preference;
			}
		}
		
		
		public void CreateWidgets(Widget parent, Drawing.Point origin)
		{
			System.Diagnostics.Debug.Assert (this.preference != PreferredLayout.None);
			System.Diagnostics.Debug.Assert (this.parent == null);
			
			//	TODO: gérer l'orientation préférentielle passée en entrée, au cas où
			//	on voudrait mettre la palette dans une barre vericale, etc.
			
			this.parent = parent;
			this.origin = origin;
			
			this.CreateDragSource (typeof (Button),          "Button",   0,   0, 86, 23+6);
			this.CreateDragSource (typeof (CheckButton),     "Check",    0,  30, 66, 14+6);
			this.CreateDragSource (typeof (RadioButton),     "Radio",    0,  50, 66, 14+6);
			this.CreateDragSource (typeof (TextField),       "",         0,  70, 86, 21+6);
			this.CreateDragSource (typeof (TextFieldUpDown), "10",       0,  98, 43, 21+6);
			this.CreateDragSource (typeof (TextFieldSlider), "40",      43,  98, 43, 21+6);
			this.CreateDragSource (typeof (TextFieldMulti),  "",        86,  70, 86, 55);
			this.CreateDragSource (typeof (GroupBox),        "Group",   86,   0, 86, 69);
			
//			this.CreateDragSource (typeof (VScroller), x, ref y, 0, dy);
//			this.CreateDragSource (typeof (TextFieldCombo),  x, ref y, dx2, 0);
//			this.CreateDragSource (typeof (HScroller),       x, ref y, dx2, 0);
		}
		
		protected void CreateDragSource(System.Type type, string text, double x, double y, double dx, double dy)
		{
			Widget     widget = System.Activator.CreateInstance (type) as Widget;
			DragSource source = new DragSource (parent);
			
			widget.Text = text;
			
			source.Widget   = widget;
			source.Parent   = this.parent;
			source.Location = new Drawing.Point (this.origin.X + x, this.origin.Y + this.size.Height - y - dy);
			source.Size     = new Drawing.Size (dx, dy);
		}
		
		
		public static IGuideAlign		Guide
		{
			get { return new GuideAlign (); }
		}
		
		
		protected class GuideAlign : IGuideAlign
		{
			#region IGuideAlign Members
			public Drawing.Margins GetInnerMargins(Widget widget)
			{
				Widget parent = widget.Parent;
				
				while ((parent != null) && (parent.Client.Bounds == widget.Bounds))
				{
					widget = parent;
					parent = widget.Parent;
				}
				
				System.Type type = widget.GetType ();
				
				if (WidgetPalette.MatchClass (type, typeof (WindowRoot)))
				{
					return new Drawing.Margins (8, 8, 16, 16);;
				}
			
				if (WidgetPalette.MatchClass (type, typeof (GroupBox)) ||
					WidgetPalette.MatchClass (type, typeof (TabPage)) ||
					WidgetPalette.MatchClass (type, typeof (PanePage)) ||
					WidgetPalette.MatchClass (type, typeof (Panel)))
				{
					return new Drawing.Margins (4, 4, 6, 6);
				}
				
				return new Drawing.Margins (0, 0, 0, 0);
			}
		
			public Drawing.Margins GetAlignMargins(Widget widget_a, Widget widget_b)
			{
				System.Type type_a = widget_a.GetType ();
				System.Type type_b = widget_b.GetType ();
				
				SpaceClass a = WidgetPalette.GetSpaceClass (type_a);
				SpaceClass b = WidgetPalette.GetSpaceClass (type_b);
				SpaceClass x = WidgetPalette.Combine (a, b);
				
				switch (x)
				{
					case SpaceClass.Button:
						return new Drawing.Margins (8, 8, 6, 6);
					
					case SpaceClass.TextField:
						return new Drawing.Margins (4, 4, 2, 2);
					
					case SpaceClass.Compact:
						return new Drawing.Margins (2, 2, 2, 2);
					
					case SpaceClass.Tight:
						return new Drawing.Margins (0, 0, 0, 0);
				}
				
				return new Drawing.Margins (2, 2, 2, 2);
			}
			#endregion
		}
		
		public static bool MatchClass(System.Type type, System.Type model)
		{
			return (type == model) || (type.IsSubclassOf (model));
		}
		
		public static SpaceClass GetSpaceClass(System.Type type)
		{
			if (WidgetPalette.MatchClass (type, typeof (TextFieldCombo)) ||
				WidgetPalette.MatchClass (type, typeof (Button)))
			{
				return SpaceClass.Button;
			}
			
			if (WidgetPalette.MatchClass (type, typeof (AbstractTextField)))
			{
				return SpaceClass.TextField;
			}
			
			if (WidgetPalette.MatchClass (type, typeof (CheckButton)) ||
				WidgetPalette.MatchClass (type, typeof (RadioButton)) ||
				WidgetPalette.MatchClass (type, typeof (StaticText)) ||
				WidgetPalette.MatchClass (type, typeof (GroupBox)) ||
				WidgetPalette.MatchClass (type, typeof (TabBook)))
			{
				return SpaceClass.Compact;
			}
			
			if (WidgetPalette.MatchClass (type, typeof (HScroller)) ||
				WidgetPalette.MatchClass (type, typeof (VScroller)) ||
				WidgetPalette.MatchClass (type, typeof (PaneBook)) ||
				WidgetPalette.MatchClass (type, typeof (Panel)))
			{
				return SpaceClass.Tight;
			}
			
			return SpaceClass.Generic;
		}
		
		public static SpaceClass Combine(SpaceClass a, SpaceClass b)
		{
			if (a == b) return a;
			
			if ((a == SpaceClass.Button) ||
				(b == SpaceClass.Button))
			{
				return SpaceClass.Button;
			}
			
			if ((a == SpaceClass.TextField) ||
				(b == SpaceClass.TextField))
			{
				return SpaceClass.TextField;
			}
			
			if ((a == SpaceClass.Compact) ||
				(b == SpaceClass.Compact))
			{
				return SpaceClass.Compact;
			}
			
			if ((a == SpaceClass.Tight) ||
				(b == SpaceClass.Tight))
			{
				return SpaceClass.Tight;
			}
			
			return SpaceClass.Generic;
		}
		
		
		public enum SpaceClass
		{
			Generic,
			Button,
			TextField,
			Compact,
			Tight,
		}
		
		protected Widget				parent;
		protected PreferredLayout		preference;
		protected Drawing.Size			size;
		protected Drawing.Point			origin;
	}
}
