//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			
			this.grips_overlay.SelectedTarget    += new SelectionEventHandler (this.HandleSelectedTarget);
			this.grips_overlay.DeselectingTarget += new SelectionEventHandler (this.HandleDeselectingTarget);
		}

		//	TODO: Dispose
		
		
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
		
		public Helpers.WidgetCollection	SelectedWidgets
		{
			get
			{
				return this.grips_overlay.SelectedWidgets;
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
				
				if (clip.Contains (pos))
				{
					Widget.ChildFindMode mode = Widget.ChildFindMode.SkipHidden
						/**/				  | Widget.ChildFindMode.SkipEmbedded
						/**/				  | Widget.ChildFindMode.Deep;
					
					hot = this.panel.FindChild (pos, mode);
				}
				
				this.hot_widget = null;
				
				if ((Message.State.Buttons == MouseButtons.None) &&
					(!this.SelectedWidgets.Contains (hot)))
				{
					//	Ne met en évidence le widget "chaud" que si celui-ci n'est pas sélectionné comme
					//	cible. Si l'utilisateur survole la poignée d'un objet sélectionné, c'est celle-ci
					//	qui est prioritaire par rapport au mécanisme de détection.
					
					Grip grip = this.grips_overlay.FindChild (e.Message.Cursor) as Grip;
					
					if (grip == null)
					{
						this.hot_widget = hot;
					}
				}
				
				this.hilite_adorner.Widget = this.hot_widget;
				this.hilite_adorner.HiliteMode = HiliteMode.SelectCandidate;
				
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
		
		private void HandleMouseDown(Message message, Drawing.Point pos, Widget hot)
		{
			if (message.Button == MouseButtons.Left)
			{
				if (this.SelectedWidgets.Contains (hot))
				{
					return;
				}
				
				if ((message.ModifierKeys & ModifierKeys.Shift) == 0)
				{
					this.SelectedWidgets.Clear ();
				}
				
				if (hot != null)
				{
					this.SelectedWidgets.Add (hot);
					System.Diagnostics.Debug.WriteLine ("Click on " + hot.Name + " (" + hot.GetType ().Name + ")");
				}
			}
		}
		
		
		
		protected virtual void HandleSelectedTarget(object sender, object o)
		{
			if (this.Selected != null)
			{
				this.Selected (this, o);
			}
		}
		
		protected virtual void HandleDeselectingTarget(object sender, object o)
		{
			if (this.Deselecting != null)
			{
				this.Deselecting (this, o);
			}
		}
		
		
		public event SelectionEventHandler	Selected;
		public event SelectionEventHandler	Deselecting;
		
		protected Panel						panel;
		protected Widget					hot_widget;
		
		protected HiliteAdorner				hilite_adorner;
		protected GripsOverlay				grips_overlay;
	}
}
