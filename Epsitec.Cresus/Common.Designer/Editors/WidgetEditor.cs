//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Editors
{
	using WidgetCollection = Epsitec.Common.Widgets.Helpers.WidgetCollection;
	
	/// <summary>
	/// La classe WidgetEditor...
	/// </summary>
	public class WidgetEditor : Support.ICommandDispatcherHost
	{
		public WidgetEditor(BuilderController builder_controller)
		{
			this.builder_controller = builder_controller;
			
			this.hilite_adorner = new HiliteWidgetAdorner ();
			this.grips_overlay  = new Widgets.GripsOverlay (this);
			this.tab_o_overlay  = new Widgets.TabOrderOverlay ();
			
			this.grips_overlay.SelectedTarget    += new SelectionEventHandler (this.HandleSelectedTarget);
			this.grips_overlay.DeselectingTarget += new SelectionEventHandler (this.HandleDeselectingTarget);
			this.grips_overlay.DeselectedTarget  += new SelectionEventHandler (this.HandleDeselectedTarget);
		}
		
		
		public Widget							Root
		{
			get
			{
				return this.root;
			}
			
			set
			{
				if (this.root != value)
				{
					this.DetachRoot (this.root);
					this.DetachWindow (this.window);
					
					this.root = value;
					this.window = value == null ? null : value.Window;
					
					this.AttachWindow (this.window);
					this.AttachRoot (this.root);
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
		
		public bool								IsActiveEditor
		{
			get
			{
				return this.is_active_editor;
			}
			set
			{
				if (this.is_active_editor != value)
				{
					this.is_active_editor = value;
					this.OnActiveEditorChanged ();
				}
			}
		}
		
		public BuilderController				BuilderController
		{
			get
			{
				return this.builder_controller;
			}
		}
		
		public Widgets.GripsOverlay				GripsOverlay
		{
			get
			{
				return this.grips_overlay;
			}
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
				this.tab_o_overlay.RootWidget     = this.root;
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
		
		
		public static WidgetEditor FromWidget(Widget widget)
		{
			return widget == null ? null : WidgetEditor.FromWindow (widget.Window);
		}
		
		public static WidgetEditor FromWindow(Window window)
		{
			return window == null ? null : window.GetProperty (WidgetEditor.prop_widget_editor) as Editors.WidgetEditor;
		}
		
		
		protected virtual void AttachRoot(Widget root)
		{
			if (root != null)
			{
				root.PreProcessing += new MessageEventHandler (this.HandleRootPreProcessing);
				root.TextChanged   += new EventHandler (this.HandleRootTextChanged);
			}
		}
		
		protected virtual void DetachRoot(Widget root)
		{
			if (root != null)
			{
				root.PreProcessing -= new MessageEventHandler (this.HandleRootPreProcessing);
				root.TextChanged   -= new EventHandler (this.HandleRootTextChanged);
				
				this.hot_widget = null;
				this.hilite_adorner.Widget = null;
				
				this.tab_o_overlay.RootWidget = null;
			}
		}
		
		protected virtual void AttachWindow(Window window)
		{
			if (window != null)
			{
				window.SetProperty (WidgetEditor.prop_widget_editor, this);
			}
		}
		
		protected virtual void DetachWindow(Window window)
		{
			if (window != null)
			{
				window.ClearProperty (WidgetEditor.prop_widget_editor);
			}
		}
		
		
		private void HandleRootPreProcessing(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.root == sender);
			
			if ((this.tab_o_overlay != null) &&
				(this.tab_o_overlay.RootWidget != null))
			{
				//	Si l'overlay pour la gestion des TabIndex est actif, on ne doit pas manger
				//	les événements, car ils lui sont destinés :
				
				return;
			}
			
			if (e.Message.IsMouseType)
			{
				Drawing.Point     pos  = e.Point;
				Drawing.Rectangle clip = this.root.GetClipStackBounds ();
				Widget            hot  = null;
				
				if (clip.Contains (pos))
				{
					if (this.grips_overlay.FindChild (pos) != null)
					{
						//	La souris survole une poignée; on doit donc permettre à l'événement d'être
						//	traité librement, ou sinon, Grip ne va jamais voir la souris bouger :
						
						return;
					}
					
					Widget.ChildFindMode mode = Widget.ChildFindMode.SkipHidden
						/**/				  | Widget.ChildFindMode.SkipEmbedded
						/**/				  | Widget.ChildFindMode.Deep;
					
					hot = this.root.FindChild (pos, mode);
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
		
		private void HandleRootTextChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.root == sender);
			
			this.UpdateWindowTitle ();
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
					
					this.builder_controller.ActivateEditor (hot, false);
				}
			}
		}
		
		
		protected virtual void UpdateWindowTitle()
		{
			if (this.IsActiveEditor)
			{
				this.window.Text = string.Format ("[ {0} ]", this.root.Text);
			}
			else
			{
				this.window.Text = this.root.Text;
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
		
		protected virtual void OnActiveEditorChanged()
		{
			if ((this.window != null) &&
				(this.root != null))
			{
				this.UpdateWindowTitle ();
			}
			
			if (this.ActiveEditorChanged != null)
			{
				this.ActiveEditorChanged (this);
			}
		}
		
		
		public event SelectionEventHandler		Selected;
		public event SelectionEventHandler		Deselecting;
		public event SelectionEventHandler		Deselected;
		public event Support.EventHandler		ActiveEditorChanged;
		
		
		protected Support.CommandDispatcher		dispatcher;
		protected BuilderController				builder_controller;
		
		protected Widget						hot_widget;
		protected Widget						root;
		protected Window						window;
		
		protected bool							is_active_editor;
		
		protected HiliteWidgetAdorner			hilite_adorner;
		protected Widgets.GripsOverlay			grips_overlay;
		protected Widgets.TabOrderOverlay		tab_o_overlay;
		
		private const string					prop_widget_editor = "$widget editor$editor$";
	}
}
