//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Panels
{
	/// <summary>
	/// La classe WidgetSourcePalette permet de remplir un panel servant de base à la
	/// palette des widgets servant à la construction de la GUI.
	/// </summary>
	public class WidgetSourcePalette : AbstractPalette, IDropSource
	{
		public WidgetSourcePalette()
		{
			this.size = new Drawing.Size (172+2*10, 145+2*10);
		}
		
		
		#region IDropSource Members
		public Widget							DroppedWidget
		{
			get
			{
				if (this.active_drag_source != null)
				{
					return this.active_drag_source.DroppedWidget;
				}
				
				return null;
			}
		}
		#endregion
		
		
		internal void NotifyActiveEditorChanged(Editors.WidgetEditor editor)
		{
		}
		
		protected override void CreateWidgets(Widget parent)
		{
			System.Diagnostics.Debug.Assert (this.widget == parent);
			
			this.CreateDragSource (typeof (StaticText),      "Texte",		 10,  12, 33, 14+6);
			this.CreateDragSource (typeof (StaticTextSmall), "Petit texte",  43,  14, 54, 12+6);
			this.CreateDragSource (typeof (StaticTextLarge), "Grand texte",  97,  10, 78, 17+6);
			this.CreateDragSource (typeof (Button),          "Bouton",		 10,  35, 86, 23+6);
			this.CreateDragSource (typeof (CheckButton),     "Coche",		 10,  65, 66, 14+6);
			this.CreateDragSource (typeof (RadioButton),     "Radio",		 10,  85, 66, 14+6);
			this.CreateDragSource (typeof (TextField),       "",			 10, 105, 86, 21+6);
			this.CreateDragSource (typeof (TextFieldUpDown), "10",			 10, 133, 43, 21+6);
			this.CreateDragSource (typeof (TextFieldSlider), "40",			 53, 133, 43, 21+6);
			this.CreateDragSource (typeof (TextFieldMulti),  "",			 96, 105, 86, 55);
			this.CreateDragSource (typeof (GroupBox),        "Groupe",		 96,  35, 86, 69);
			
//			this.CreateDragSource (typeof (VScroller), x, ref y, 0, dy);
//			this.CreateDragSource (typeof (TextFieldCombo),  x, ref y, dx2, 0);
//			this.CreateDragSource (typeof (HScroller),       x, ref y, dx2, 0);
		}
		
		protected void CreateDragSource(System.Type type, string text, double x, double y, double dx, double dy)
		{
			Widget             widget = System.Activator.CreateInstance (type) as Widget;
			Widgets.DragSource source = new Widgets.DragSource (this.widget);
			
			widget.Text = text;
			
			source.Widget   = widget;
			source.Parent   = this.widget;
			source.Location = new Drawing.Point (x, this.size.Height - y - dy);
			source.Size     = new Drawing.Size (dx, dy);
			
			source.DragBegin += new Support.EventHandler (this.HandleSourceDragBegin);
			source.DragEnd   += new Support.EventHandler (this.HandleSourceDragEnd);
		}
		
		
		private void HandleSourceDragBegin(object sender)
		{
			System.Diagnostics.Debug.Assert (this.active_drag_source == null);
			
			this.active_drag_source = sender as Widgets.DragSource;
			
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
		
		
		public event Support.EventHandler		DragBegin;
		public event Support.EventHandler		DragEnd;
		
		public static Behaviors.IGuideAlignHint	Guide
		{
			get { return new GuideAlign (); }
		}
		
		
		#region GuideAlign class
		protected class GuideAlign : Behaviors.IGuideAlignHint
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
				
				if (WidgetSourcePalette.MatchClass (type, typeof (WindowRoot)))
				{
					return new Drawing.Margins (8, 8, 16, 16);;
				}
			
				if (WidgetSourcePalette.MatchClass (type, typeof (GroupBox)) ||
					WidgetSourcePalette.MatchClass (type, typeof (TabPage)) ||
					WidgetSourcePalette.MatchClass (type, typeof (PanePage)) ||
					WidgetSourcePalette.MatchClass (type, typeof (Panel)))
				{
					return new Drawing.Margins (4, 4, 6, 6);
				}
				
				return new Drawing.Margins (0, 0, 0, 0);
			}
		
			public Drawing.Margins GetAlignMargins(Widget widget_a, Widget widget_b)
			{
				System.Type type_a = widget_a.GetType ();
				System.Type type_b = widget_b.GetType ();
				
				SpaceClass a = WidgetSourcePalette.GetSpaceClass (type_a);
				SpaceClass b = WidgetSourcePalette.GetSpaceClass (type_b);
				SpaceClass x = WidgetSourcePalette.Combine (a, b);
				
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
			if (WidgetSourcePalette.MatchClass (type, typeof (TextFieldCombo)) ||
				WidgetSourcePalette.MatchClass (type, typeof (Button)))
			{
				return SpaceClass.Button;
			}
			
			if (WidgetSourcePalette.MatchClass (type, typeof (AbstractTextField)))
			{
				return SpaceClass.TextField;
			}
			
			if (WidgetSourcePalette.MatchClass (type, typeof (CheckButton)) ||
				WidgetSourcePalette.MatchClass (type, typeof (RadioButton)) ||
				WidgetSourcePalette.MatchClass (type, typeof (StaticText)) ||
				WidgetSourcePalette.MatchClass (type, typeof (GroupBox)) ||
				WidgetSourcePalette.MatchClass (type, typeof (TabBook)))
			{
				return SpaceClass.Compact;
			}
			
			if (WidgetSourcePalette.MatchClass (type, typeof (HScroller)) ||
				WidgetSourcePalette.MatchClass (type, typeof (VScroller)) ||
				WidgetSourcePalette.MatchClass (type, typeof (PaneBook)) ||
				WidgetSourcePalette.MatchClass (type, typeof (Panel)))
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
		
		protected Widgets.DragSource			active_drag_source;
	}
}
