//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Widgets.Design
{
	/// <summary>
	/// La classe AbstractWidgetEdit...
	/// </summary>
	public abstract class AbstractWidgetEdit
	{
		public AbstractWidgetEdit()
		{
			this.hilite_adorner = new HiliteAdorner ();
			this.grips_overlay  = new GripsOverlay ();
		}
		
		public Panel					Panel
		{
			get
			{
				return this.panel;
			}
			
			set
			{
				if (this.panel != value)
				{
					if (this.panel != null)
					{
						this.DetachPanel (this.panel);
					}
					
					this.panel = value;
					
					if (this.panel != null)
					{
						this.AttachPanel (this.panel);
					}
				}
			}
		}
		
		
		protected virtual void AttachPanel(Panel panel)
		{
			if (panel != null)
			{
				panel.PreProcessing += new MessageEventHandler (this.HandlePanelPreProcessing);
			}
		}
		
		protected virtual void DetachPanel(Panel panel)
		{
			if (panel != null)
			{
				panel.PreProcessing -= new MessageEventHandler (this.HandlePanelPreProcessing);
				
				this.hot_widget = null;
				this.hilite_adorner.Widget = null;
			}
		}
		
		
		private void HandlePanelPreProcessing(object sender, MessageEventArgs e)
		{
			if (e.Message.IsMouseType)
			{
				Drawing.Point     pos  = e.Point;
				Drawing.Rectangle clip = this.panel.GetClipStackBounds ();
				Widget            hot  = null;
				
				System.Diagnostics.Debug.WriteLine ("Clip: " + clip.ToString () + ", pos: " + pos.ToString () + ", message: " + e.Message.ToString ());
				
				if (clip.Contains (pos))
				{
					hot = this.panel.FindChild (pos);
				}
				
				if (Message.State.Buttons == MouseButtons.None)
				{
					this.hot_widget = hot;
				}
				else
				{
					this.hot_widget = null;
				}
				
				this.hilite_adorner.Widget = this.hot_widget;
				
				switch (e.Message.Type)
				{
					case MessageType.MouseDown:
						if (clip.Contains (pos))
						{
							this.HandleMouseDown (e.Message, e.Point, hot);
						}
						break;
				}
			}
			
			e.Suppress = true;
		}
		
		protected virtual void HandleMouseDown(Message message, Drawing.Point pos, Widget hot)
		{
			if (message.Button == MouseButtons.Left)
			{
				this.grips_overlay.TargetWidget = hot;
				
				if (hot != null)
				{
					System.Diagnostics.Debug.WriteLine ("Click on " + hot.Name + " (" + hot.GetType ().Name + ")");
				}
			}
		}
		
		
		
		protected Panel					panel;
		protected Widget				hot_widget;
		
		protected HiliteAdorner			hilite_adorner;
		protected GripsOverlay			grips_overlay;
	}
}
