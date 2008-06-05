using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe MiniatureFrame est un widget servant de cadre pour une miniature, avec possibilité de drag & drop.
	/// Normalement, MiniatureFrame a deux enfants: un Viewer et un StaticText.
	/// </summary>
	public class MiniatureFrame : FrameBox, Common.Widgets.Behaviors.IDragBehaviorHost
	{
		public MiniatureFrame() : base()
		{
			this.dragBehavior = new Common.Widgets.Behaviors.DragBehavior(this, true, true);
		}

		public MiniatureFrame(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public bool IsLeftRightPlacement
		{
			//	Indique si le drag & drop s'effectue sur les bord gauche/droite ou supérieur/inférieur.
			get
			{
				return this.isLeftRightPlacement;
			}
			set
			{
				this.isLeftRightPlacement = value;
			}
		}

		public bool IsBefore
		{
			//	Si le widget a été utilisé comme destination d'un drag & drop, indique la moitié visée.
			get
			{
				return this.isBefore;
			}
		}


		public bool DragSourceFrame
		{
			get
			{
				return (bool) this.GetValue(MiniatureFrame.DragSourceFrameProperty);
			}
			set
			{
				this.SetValue(MiniatureFrame.DragSourceFrameProperty, value);
			}
		}

		public bool DragSourceEnable
		{
			get
			{
				return (bool) this.GetValue(MiniatureFrame.DragSourceEnableProperty);
			}
			set
			{
				if (value)
				{
					this.ClearValue(MiniatureFrame.DragSourceEnableProperty);
				}
				else
				{
					this.SetValue(MiniatureFrame.DragSourceEnableProperty, value);
				}
			}
		}


		public MiniatureFrame DragHost
		{
			get
			{
				return this.GetValue(MiniatureFrame.DragHostProperty) as MiniatureFrame;
			}
			set
			{
				if (value == null)
				{
					this.ClearValue(MiniatureFrame.DragHostProperty);
				}
				else
				{
					this.SetValue(MiniatureFrame.DragHostProperty, value);
				}
			}
		}

		private bool IsSource
		{
			//	Indique si le widget doit être dessiné comme la source du drag & drop.
			get
			{
				return this.isSource;
			}
			set
			{
				if (this.isSource != value)
				{
					this.isSource = value;
					this.Invalidate();
				}
			}
		}

		protected enum PartialHilite
		{
			None,
			Left,
			Right,
			Bottom,
			Top,
		}

		private PartialHilite TargetHilite
		{
			//	Indique comment le widget doit être dessiné comme destination du drag & drop.
			get
			{
				return this.targetHilite;
			}
			set
			{
				if (this.targetHilite != value)
				{
					this.targetHilite = value;

					if (this.targetHilite != PartialHilite.None)
					{
						this.isBefore = (this.targetHilite == PartialHilite.Left || this.targetHilite == PartialHilite.Bottom);
					}

					this.Invalidate();
				}
			}
		}


		private MiniatureFrame FindDropTarget(Point mouse)
		{
			//	Cherche un widget MiniatureFrame destinataire du drag & drop.
			//	Si on est dans le Viewer ou le StaticText enfant du MiniatureFrame, considère que l'on
			//	est dans le MiniatureFrame parent.
			Widget widget = this.Window.Root.FindChild(this.MapClientToRoot(mouse), Widget.ChildFindMode.SkipHidden | Widget.ChildFindMode.Deep | Widget.ChildFindMode.SkipDisabled);
			MiniatureFrame finded = null;

			if (widget != null)
			{
				if (widget is MiniatureFrame)
				{
					finded = widget as MiniatureFrame;
				}
				else
				{
					if (widget.Parent is MiniatureFrame)  // Viewer ou StaticText enfant du MiniatureFrame ?
					{
						finded = widget.Parent as MiniatureFrame;
					}
				}
			}

			if (finded != null && finded.isLeftRightPlacement != this.isLeftRightPlacement)
			{
				finded = null;  // refuse de dragger une page dans un calque (par exemple) !
			}

			return finded;
		}

		private void DragHilite(PartialHilite hilite)
		{
			//	Met en évidence le widget MiniatureFrame destinataire du drag & drop.
			if (this.dragInfo.Target == this)
			{
				return;
			}

			this.DragTargetHilite = hilite;
		}

		private PartialHilite DragTargetHilite
		{
			get
			{
				if (this.dragInfo == null || this.dragInfo.Target == null)
				{
					return PartialHilite.None;
				}
				else
				{
					return this.dragInfo.Target.TargetHilite;
				}
			}
			set
			{
				if (this.dragInfo != null && this.dragInfo.Target != null)
				{
					this.dragInfo.Target.TargetHilite = value;
				}
			}
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			MiniatureFrame dragHost = this.DragHost;

			//	Est-ce que l'événement clavier est reçu dans un échantillon en
			//	cours de drag dans un DragWindow ? C'est possible, car le focus
			//	clavier change quand on montre le DragWindow.
			if (dragHost != null && message.IsKeyType)
			{
				//	Signalons l'événement clavier à l'auteur du drag :
				dragHost.ProcessMessage(message, pos);
			}
			else
			{
				if (this.DragSourceEnable == false || !this.dragBehavior.ProcessMessage(message, pos))
				{
					base.ProcessMessage(message, pos);
				}
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation(graphics, clipRect);

			Rectangle rect = this.Client.Bounds;

			if (this.isSource)  // source du drag & drop ?
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(Color.FromBrightness(0.7));
			}

			if (this.targetHilite != PartialHilite.None)  // destination du drag & drop ?
			{
				Rectangle r = rect;

				switch (this.targetHilite)
				{
					case PartialHilite.Left:
						r.Right = r.Left+2;
						break;

					case PartialHilite.Right:
						r.Left = r.Right-2;
						break;

					case PartialHilite.Bottom:
						r.Top = r.Bottom+2;
						break;

					case PartialHilite.Top:
						r.Bottom = r.Top-2;
						break;
				}

				graphics.AddFilledRectangle(r);
				graphics.RenderSolid(Color.FromRgb(1, 0, 0));  // dessine une bande latérale rouge
			}

			if (this.IsEnabled && this.DragHost != null)  // contenu de la fenêtre flottante ?
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(Color.FromBrightness(0.95));  // fond gris très clair

				rect.Deflate(0.5, 0.5);
				Path path = new Path(rect);
				Drawer.DrawPathDash(graphics, 1.0, path, 1.0, 4.0, 5.0, Color.FromBrightness(0.0));  // cadre traitillé
				path.Dispose();
			}
		}

	
		#region IDragBehaviorHost Members
		public Point DragLocation
		{
			get
			{
				return Point.Zero;
			}
		}


		public bool OnDragBegin(Point cursor)
		{
			if (this.DragSourceEnable == false)
			{
				return false;
			}

			this.IsSource = true;

			//	Crée un échantillon utilisable pour l'opération de drag & drop (il
			//	va représenter visuellement l'échantillon de couleur). On le place
			//	dans un DragWindow et hop.
			MiniatureFrame widget = new MiniatureFrame();
			widget.DragHost = this;

			this.dragInfo = new DragInfo(cursor, widget, this.ActualSize);

			return true;
		}

		public void OnDragging(DragEventArgs e)
		{
			this.dragInfo.Window.WindowLocation = this.dragInfo.Origin + e.Offset;

			Point mouse = e.ToPoint;
			MiniatureFrame cs = this.FindDropTarget(mouse);

			PartialHilite hilite = PartialHilite.None;
			if (cs != null)
			{
				Point m = this.MapClientToScreen(mouse);
				Rectangle b = cs.MapClientToScreen(cs.Client.Bounds);

				if (this.isLeftRightPlacement)
				{
					hilite = (m.X < b.Center.X) ? PartialHilite.Left : PartialHilite.Right;
				}
				else
				{
					hilite = (m.Y < b.Center.Y) ? PartialHilite.Bottom : PartialHilite.Top;
				}
			}

			if (cs != this.dragInfo.Target || this.DragTargetHilite != hilite)
			{
				this.DragHilite(PartialHilite.None);
				this.dragInfo.Target = cs;
				this.DragHilite(hilite);
			}
		}

		public void OnDragEnd()
		{
			this.IsSource = false;
			this.DragHilite(PartialHilite.None);

			if (this.dragInfo.Target == null)
			{
				this.dragInfo.DissolveAndDispose();
				this.dragInfo = null;
			}
			else
			{
				if (this.dragInfo.Target != this)  // pas un simple clic ?
				{
					this.OnDragAndDropDoing(this.dragInfo.Target);
				}

				this.dragInfo.Dispose();
				this.dragInfo = null;
			}
		}
		#endregion

		#region DragInfo Class
		/// <summary>
		/// The <c>DragInfo</c> classe stores information needed only while drag
		/// and drop is in progress.
		/// </summary>
		private class DragInfo
		{
			public DragInfo(MiniatureFrame host)
			{
				this.host = host;
			}

			public DragInfo(Point cursor, MiniatureFrame widget, Size size)
			{
				System.Diagnostics.Debug.Assert(widget.DragHost != null);

				this.host   = widget.DragHost;
				this.window = new DragWindow();

				this.target = null;
				this.origin = widget.DragHost.MapClientToScreen(new Point(-System.Math.Floor(size.Width/2), -System.Math.Floor(size.Height/2)));

				this.window = new DragWindow();
				this.window.Alpha = 0.5;
				this.window.DefineWidget(widget, size, Margins.Zero);
				this.window.WindowLocation = this.Origin + cursor;
				this.window.Owner = widget.DragHost.Window;
				this.window.FocusWidget(widget);
				this.window.Show();
			}

			public DragWindow Window
			{
				get
				{
					return this.window;
				}
			}

			public Point Origin
			{
				get
				{
					return this.origin;
				}
			}

			public MiniatureFrame Target
			{
				get
				{
					return this.target;
				}
				set
				{
					this.target = value;
				}
			}

			public void Dispose()
			{
				if (this.Window != null)
				{
					this.window.Hide();
					this.window.Dispose();
					this.window = null;
				}
			}

			public void DissolveAndDispose()
			{
				if (this.window != null)
				{
					this.window.DissolveAndDisposeWindow();
					this.window = null;
				}
			}

			private MiniatureFrame		host;
			private DragWindow			window;
			private Point				origin;
			private MiniatureFrame		target;
		}
		#endregion


		#region Events
		public event EventHandler<MiniatureFrame> DragAndDropDoing
		{
			add
			{
				this.AddUserEventHandler("DragAndDropDoing", value);
			}
			remove
			{
				this.RemoveUserEventHandler("DragAndDropDoing", value);
			}
		}

		private void OnDragAndDropDoing(MiniatureFrame dst)
		{
			EventHandler<MiniatureFrame> handler = (EventHandler<MiniatureFrame>) this.GetUserEventHandler("DragAndDropDoing");

			if (handler != null)
			{
				handler(this, dst);
			}
		}
		#endregion


		public static readonly DependencyProperty DragHostProperty         = DependencyProperty.Register("DragHost", typeof(MiniatureFrame), typeof(MiniatureFrame), new DependencyPropertyMetadata().MakeNotSerializable());
		public static readonly DependencyProperty DragSourceFrameProperty  = DependencyProperty.Register("DragSourceFrame", typeof(bool), typeof(MiniatureFrame), new Common.Widgets.Helpers.VisualPropertyMetadata(false, Common.Widgets.Helpers.VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty DragSourceEnableProperty = DependencyProperty.Register("DragSourceEnable", typeof(bool), typeof(MiniatureFrame), new DependencyPropertyMetadata(true));

		private bool									isLeftRightPlacement;
		private bool									isSource;
		private PartialHilite							targetHilite;
		private bool									isBefore;
		private Common.Widgets.Behaviors.DragBehavior	dragBehavior;
		private DragInfo								dragInfo;
	}
}
