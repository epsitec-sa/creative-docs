namespace Epsitec.Common.Widgets.Design.Panels
{
	/// <summary>
	/// La classe WidgetPalette permet de remplir un panel servant de base à la
	/// palette des widgets servant à la construction de la GUI.
	/// </summary>
	public class WidgetPalette : AbstractPalette
	{
		public WidgetPalette(PreferredLayout preference)
		{
			this.preference = preference;
			
			//	TODO: déterminer la taille utile en fonction de l'orientation préférentielle
			//	passée en entrée, au cas où on voudrait mettre la palette dans une barre
			//	vericale, etc.
			
			this.size = new Drawing.Size (172+2*10, 125+2*10);
		}
		
		
		public PreferredLayout			PreferredLayout
		{
			get
			{
				return this.preference;
			}
		}
		
		public DragSource				ActiveDragSource
		{
			get
			{
				return this.active_drag_source;
			}
		}
		
		
		protected override Widget CreateWidget()
		{
			Widget host = new Widget ();
			
			host.Size    = this.Size;
			host.MinSize = this.Size;
			
			this.CreateWidgets (host);
			
			return host;
		}

		protected void CreateWidgets(Widget parent)
		{
			System.Diagnostics.Debug.Assert (this.preference != PreferredLayout.None);
			System.Diagnostics.Debug.Assert (this.parent == null);
			
			//	TODO: gérer l'orientation préférentielle passée en entrée, au cas où
			//	on voudrait mettre la palette dans une barre vericale, etc.
			
			this.parent = parent;
			
			this.CreateDragSource (typeof (Button),          "Button",  10,  10, 86, 23+6);
			this.CreateDragSource (typeof (CheckButton),     "Check",   10,  40, 66, 14+6);
			this.CreateDragSource (typeof (RadioButton),     "Radio",   10,  60, 66, 14+6);
			this.CreateDragSource (typeof (TextField),       "",        10,  80, 86, 21+6);
			this.CreateDragSource (typeof (TextFieldUpDown), "10",      10, 108, 43, 21+6);
			this.CreateDragSource (typeof (TextFieldSlider), "40",      53, 108, 43, 21+6);
			this.CreateDragSource (typeof (TextFieldMulti),  "",        96,  80, 86, 55);
			this.CreateDragSource (typeof (GroupBox),        "Group",   96,  10, 86, 69);
			
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
			source.Location = new Drawing.Point (x, this.size.Height - y - dy);
			source.Size     = new Drawing.Size (dx, dy);
			
			source.DragBegin += new EventHandler (this.HandleSourceDragBegin);
			source.DragEnd   += new EventHandler (this.HandleSourceDragEnd);
		}
		
		
		private void HandleSourceDragBegin(object sender)
		{
			System.Diagnostics.Debug.Assert (this.active_drag_source == null);
			
			this.active_drag_source = sender as DragSource;
			
			if (this.DragBegin != null)
			{
				this.DragBegin (this);
			}
		}
		
		private void HandleSourceDragEnd(object sender)
		{
			System.Diagnostics.Debug.Assert (this.active_drag_source == sender);
			
			if (this.DragEnd != null)
			{
				this.DragEnd (this);
			}
			
			this.active_drag_source = null;
		}
		
		
		public event EventHandler		DragBegin;
		public event EventHandler		DragEnd;
		
		public static IGuideAlign		Guide
		{
			get { return new GuideAlign (); }
		}
		
		
		#region GuideAlign class
		protected class GuideAlign : IGuideAlign
		{
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
		}
		#endregion
		
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
		protected DragSource			active_drag_source;
	}
}
