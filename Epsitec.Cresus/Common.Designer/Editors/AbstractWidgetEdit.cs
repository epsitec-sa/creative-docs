//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Editors
{
	using WidgetCollection = Epsitec.Common.Widgets.Helpers.WidgetCollection;
	
	/// <summary>
	/// La classe AbstractWidgetEdit...
	/// </summary>
	public abstract class AbstractWidgetEdit : Support.ICommandDispatcherHost
	{
		public AbstractWidgetEdit()
		{
			this.hilite_adorner = new HiliteWidgetAdorner ();
			this.grips_overlay  = new Widgets.GripsOverlay ();
			this.tab_o_overlay  = new Widgets.TabOrderOverlay ();
			
			this.grips_overlay.SelectedTarget    += new SelectionEventHandler (this.HandleSelectedTarget);
			this.grips_overlay.DeselectingTarget += new SelectionEventHandler (this.HandleDeselectingTarget);
			this.grips_overlay.DeselectedTarget  += new SelectionEventHandler (this.HandleDeselectedTarget);
		}

		//	TODO: Dispose
		
		
		public Panel							Panel
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
		
		public WidgetCollection					SelectedWidgets
		{
			get
			{
				return this.grips_overlay.SelectedWidgets;
			}
		}
		
		public Widget							Root
		{
			get { return this.root; }
			set { this.root = value; }
		}
		
		
		#region ICommandDispatcherHost Members
		public Support.CommandDispatcher		CommandDispatcher
		{
			get
			{
				return this.dispatcher;
			}
			set
			{
				if (this.dispatcher != value)
				{
					this.dispatcher = value;
					this.OnCommandDispatcherChanged ();
				}
			}
		}
		#endregion
		
		
		public void SetTabIndexSetterMode(bool enable)
		{
			if (enable)
			{
				this.tab_o_overlay.RootWidget     = this.panel;
				this.tab_o_overlay.IsSetterActive = true;
			}
			else
			{
				this.tab_o_overlay.RootWidget     = null;
				this.tab_o_overlay.IsSetterActive = false;
			}
		}
		
		public void SetTabIndexPickerMode(bool enable)
		{
			if (this.tab_o_overlay != null)
			{
				this.tab_o_overlay.IsPickerActive = enable;
			}
		}
		
		public void DefineTabIndex(int tab_index)
		{
			if (this.tab_o_overlay != null)
			{
				this.tab_o_overlay.DefineTabIndex (tab_index);
			}
		}

		public void ResetTabIndexSeq()
		{
			if (this.tab_o_overlay != null)
			{
				this.tab_o_overlay.ResetTabIndexSeq ();
			}
		}
		
		public void StartTabIndexSeq()
		{
			if (this.tab_o_overlay != null)
			{
				this.tab_o_overlay.StartTabIndexSeq ();
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
				
				this.tab_o_overlay.RootWidget = null;
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
				
				if (hot != null)
				{
					if ((e.Message.ModifierKeys & ModifierKeys.Shift) != 0)
					{
						//	Evite que l'on puisse sélectionner simultanément un widget qui serait un
						//	descendant d'un autre widget sélectionné. Si on est sur un enfant d'un
						//	widget sélectionné, on retourne le widget sélectionné en lieu et place.
						
						foreach (Widget sel in this.SelectedWidgets)
						{
							if (hot.IsAncestorWidget (sel))
							{
								hot = sel;
								break;
							}
						}
					}
				}
				
				this.hot_widget = null;
				
				if ((Message.State.Buttons == MouseButtons.None) &&
					(!this.SelectedWidgets.Contains (hot)) &&
					(e.Message.Type != MessageType.MouseLeave))
				{
					//	Ne met en évidence le widget "chaud" que si celui-ci n'est pas sélectionné comme
					//	cible. Si l'utilisateur survole la poignée d'un objet sélectionné, c'est celle-ci
					//	qui est prioritaire par rapport au mécanisme de détection.
					
					Widgets.Grip grip = this.grips_overlay.FindChild (e.Message.Cursor) as Widgets.Grip;
					
					if (grip == null)
					{
						this.hot_widget = hot;
					}
				}
				
				this.hilite_adorner.Widget = this.hot_widget;
				this.hilite_adorner.HiliteMode = WidgetHiliteMode.SelectCandidate;
				
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
					if ((message.ModifierKeys & ModifierKeys.Shift) == 0)
					{
						this.SelectedWidgets.Clear ();
					}
					else
					{
						this.SelectedWidgets.Remove (hot);
					}
				}
				else
				{
					if ((message.ModifierKeys & ModifierKeys.Shift) == 0)
					{
						this.SelectedWidgets.Clear ();
					}
					if (hot != null)
					{
						//	Vérifie si ce widget n'est pas le parent d'un des widgets déjà
						//	sélectionnés. Si c'est le cas, on retire les enfants trouvés de
						//	la liste :
						
						if (this.SelectedWidgets.Count > 0)
						{
							Widget[] sel = new Widget[this.SelectedWidgets.Count];
							this.SelectedWidgets.CopyTo (sel, 0);
							
							for (int i = 0; i < sel.Length; i++)
							{
								//	Le widget 'hot' est-il un ancêtre du widget sélectionné ?
								
								if (sel[i].IsAncestorWidget (hot))
								{
									this.SelectedWidgets.Remove (sel[i]);
								}
							}
						}
						
						this.SelectedWidgets.Add (hot);
					}
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
		
		protected virtual void HandleDeselectedTarget(object sender, object o)
		{
			if (this.Deselected != null)
			{
				this.Deselected (this, o);
			}
		}
		
		
		protected virtual void OnCommandDispatcherChanged()
		{
			if (this.tab_o_overlay != null)
			{
				this.tab_o_overlay.CommandDispatcher = this.CommandDispatcher;
			}
		}
		
		
		public event SelectionEventHandler		Selected;
		public event SelectionEventHandler		Deselecting;
		public event SelectionEventHandler		Deselected;
		
		
		protected Support.CommandDispatcher		dispatcher;
		
		protected Panel							panel;
		protected Widget						hot_widget;
		protected Widget						root;
		
		protected HiliteWidgetAdorner			hilite_adorner;
		protected Widgets.GripsOverlay			grips_overlay;
		protected Widgets.TabOrderOverlay		tab_o_overlay;
	}
}
