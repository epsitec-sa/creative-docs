//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Panels
{
	/// <summary>
	/// La classe WidgetAttributePalette offre l'accès aux propriétés d'un
	/// widget.
	/// </summary>
	public class WidgetAttributePalette : AbstractPalette
	{
		public WidgetAttributePalette()
		{
			this.size = new Drawing.Size (250, 600);
		}
		
		
		public object					ActiveObject
		{
			get
			{
				return this.active;
			}
			set
			{
				if (this.active != value)
				{
					this.active = value;
					this.type   = value == null ? null : this.active.GetType ();
					this.UpdateContents ();
				}
			}
		}
		
		protected override void CreateWidgets(Widget parent)
		{
			System.Diagnostics.Debug.Assert (this.widget == parent);
			
			parent.SuspendLayout ();
			
			this.title    = new StaticText (parent);
			this.panel    = new ScrollablePanel (parent);
			
			this.title.Height    = 50;
			this.title.Dock      = DockStyle.Top;
			this.title.Text      = "<br/>";
			this.title.Alignment = Drawing.ContentAlignment.MiddleCenter;
			
			this.panel.Dock = DockStyle.Fill;
			
			this.UpdateContents ();
			
			parent.ResumeLayout ();
		}
		
		protected void UpdateContents()
		{
			if (this.active == null)
			{
				this.title.Text = @"<font size=""120%""><b>Object Attributes</b><br/>(no selected object)</font>";
			}
			else
			{
				this.title.Text = string.Format (@"<font size=""120%""><b>Object Attributes</b><br/><i>{0}</i></font>", this.type.Name);
			}
		}
		
		
		
		protected StaticText			title;
		protected ScrollablePanel		panel;
		protected object				active;
		protected System.Type			type;
	}
}
