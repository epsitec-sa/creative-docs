//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Widgets
{
	using IDragBehaviorHost = Epsitec.Common.Widgets.Helpers.IDragBehaviorHost;
	using DragBehavior      = Epsitec.Common.Widgets.Helpers.DragBehavior;
	
	/// <summary>
	/// La classe DragSource abrite un widget qui, lorsqu'il est "dragged" vient
	/// automatiquement clôné dans un DragWindow.
	/// </summary>
	public class DragSource : Widget, IDragBehaviorHost
	{
		public DragSource()
		{
			this.drag_behavior = new Epsitec.Common.Widgets.Helpers.DragBehavior (this);
			this.drop_behavior = new Behaviors.DropBehavior (this);
			
			this.DockPadding   = this.drop_behavior.Margins;
			
			this.drop_behavior.DropTargetHiliteMode = WidgetHiliteMode.DropCandidate;
		}
		
		public DragSource(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
		}
		
		
		public Widget						Widget
		{
			get { return this.widget; }
			set
			{
				if (this.widget != value)
				{
					this.DetachWidget (this.widget);
					this.widget = value;
					this.AttachWidget (this.widget);
				}
			}
		}
		
		public Widget						DroppedWidget
		{
			get { return this.dropped_widget; }
		}
		
		
		protected void AttachWidget(Widget widget)
		{
			if (widget != null)
			{
				widget.Parent = this;
				widget.Bounds = this.Client.Bounds;
				widget.Dock   = DockStyle.Fill;
				widget.SetFrozen (true);
			}
		}
		
		protected void DetachWidget(Widget widget)
		{
			if (widget != null)
			{
				widget.Parent = null;
			}
		}
		
		
		protected override void ProcessMessage(Message message, Epsitec.Common.Drawing.Point pos)
		{
			if (! this.drag_behavior.ProcessMessage (message, pos))
			{
				base.ProcessMessage (message, pos);
			}
		}
		
		
		#region Interface IDragBehaviorHost
		public Drawing.Point				DragLocation
		{
			get
			{
				return this.drop_behavior.DragWindowLocation;
			}
		}
		
		
		void IDragBehaviorHost.OnDragBegin(Drawing.Point cursor)
		{
			//	L'utilisateur aimerait déplacer le widget pour faire du drag & drop. Il faut créer
			//	la fenêtre miniature qui contient le widget en déplacement :
			
			Widget copy = Behaviors.DropBehavior.CloneWidget (this.widget);
			
			copy.Dock   = DockStyle.None;
			copy.Anchor = AnchorStyles.None;
			copy.Bounds = this.widget.Bounds;
			copy.Parent = this.widget.Parent;
			
			this.drop_behavior.Widget     = copy;
			this.drop_behavior.DropTarget = null;
			
			this.drop_behavior.StartWidgetDragging ();
			
			if (this.DragBegin != null)
			{
				this.DragBegin (this);
			}
		}
		
		
		void IDragBehaviorHost.OnDragging(DragEventArgs e)
		{
			Drawing.Point drag_cursor = this.Window.MapWindowToScreen (Message.State.LastPosition) - new Drawing.Point (1, 1);
			
			this.drop_behavior.DragWindowLocation += e.Offset;
			this.drop_behavior.ProcessDragging (drag_cursor);
			
			if (this.Dragging != null)
			{
				this.Dragging (this, e);
			}
		}
		
		void IDragBehaviorHost.OnDragEnd()
		{
			//	Voilà... L'utilisateur a relâché le bouton de la souris. Il faut déterminer où
			//	le widget devra être inséré (pour autant qu'une cible soit connue) :
			
			if (this.drop_behavior.ValidateWidgetDragging ())
			{
				this.dropped_widget = this.drop_behavior.Widget;
			}
			else
			{
				this.dropped_widget = null;
			}
			
			this.drop_behavior.Widget = null;
			
			if (this.DragEnd != null)
			{
				this.DragEnd (this);
			}
		}
		#endregion
		
		public event Support.EventHandler		DragBegin;
		public event Support.EventHandler		DragEnd;
		public event DragEventHandler			Dragging;
		
		
		private Widget							widget;
		private DragBehavior					drag_behavior;
		private Behaviors.DropBehavior			drop_behavior;
		
		private Widget							dropped_widget;
	}
}
