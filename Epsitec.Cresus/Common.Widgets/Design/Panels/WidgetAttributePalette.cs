namespace Epsitec.Common.Widgets.Design.Panels
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
			parent.SuspendLayout ();
			
			this.parent   = parent;
			this.title    = new StaticText (this.parent);
			this.panel    = new ScrollablePanel (this.parent);
			
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
		
		
		
		protected Widget				parent;
		protected StaticText			title;
		protected ScrollablePanel		panel;
		protected object				active;
		protected System.Type			type;
	}
}
