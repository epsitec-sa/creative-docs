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

		public bool IsDuplicate
		{
			//	Indique si le widget doit être dupliqué.
			get
			{
				return this.isDuplicate;
			}
			set
			{
				if (this.isDuplicate != value)
				{
					this.isDuplicate = value;
					this.Invalidate();
				}
			}
		}

		protected enum ArrowHilite
		{
			None,
			Before,
			After,
		}

		private ArrowHilite SourceHilite
		{
			//	Indique comment le widget doit être dessiné comme source du drag & drop.
			get
			{
				return this.sourceHilite;
			}
			set
			{
				if (this.sourceHilite != value)
				{
					this.sourceHilite = value;
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
			Widget widget = this.Window.Root.FindChild (this.MapClientToRoot (mouse), WidgetChildFindMode.SkipHidden | WidgetChildFindMode.Deep | WidgetChildFindMode.SkipDisabled);
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
				switch (message.MessageType)
				{
					case MessageType.KeyDown:
						if (message.KeyCode == KeyCode.ShiftKey ||
							message.KeyCode == KeyCode.ControlKey)
						{
							if (this.dragInfo != null)
							{
								this.IsDuplicate = true;
								message.Consumer = this;
								return;
							}
						}
						break;

					case MessageType.KeyUp:
						if (message.KeyCode == KeyCode.ShiftKey ||
							message.KeyCode == KeyCode.ControlKey)
						{
							if (this.dragInfo != null)
							{
								this.IsDuplicate = false;
								message.Consumer = this;
								return;
							}
						}
						break;
				}

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

			if (this.sourceHilite != ArrowHilite.None)  // source du drag & drop ?
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(Color.FromBrightness(0.9));  // dessine le fond gris très clair

				double dim = System.Math.Min(rect.Width, rect.Height);
				Rectangle square = new Rectangle(rect.Center.X-dim/2, rect.Center.Y-dim/2, dim, dim);
				Point goal;

				graphics.LineWidth = dim*0.15;  // trait très épais...
				graphics.LineCap = CapStyle.Round;  // ...à bords ronds

				if (this.isLeftRightPlacement)
				{
					Point p1 = new Point(square.Left+square.Width*0.2, square.Center.Y);
					Point p2 = new Point(square.Left+square.Width*0.8, square.Center.Y);
					double len = square.Width*0.2;

					if (this.sourceHilite == ArrowHilite.Before)  // flèche <- ?
					{
						graphics.AddLine(p1, p2);
						graphics.AddLine(p1, new Point(p1.X+len, p1.Y-len));
						graphics.AddLine(p1, new Point(p1.X+len, p1.Y+len));
						goal = p1;
					}
					else  // flèche -> ?
					{
						graphics.AddLine(p2, p1);
						graphics.AddLine(p2, new Point(p2.X-len, p2.Y-len));
						graphics.AddLine(p2, new Point(p2.X-len, p2.Y+len));
						goal = p2;
					}
				}
				else
				{
					Point p1 = new Point(square.Center.X, square.Bottom+square.Height*0.2);
					Point p2 = new Point(square.Center.X, square.Bottom+square.Height*0.8);
					double len = square.Height*0.2;

					if (this.sourceHilite == ArrowHilite.Before)  // flèche v ?
					{
						graphics.AddLine(p1, p2);
						graphics.AddLine(p1, new Point(p1.X-len, p1.Y+len));
						graphics.AddLine(p1, new Point(p1.X+len, p1.Y+len));
						goal = p1;
					}
					else  // flèche ^ ?
					{
						graphics.AddLine(p2, p1);
						graphics.AddLine(p2, new Point(p2.X-len, p2.Y-len));
						graphics.AddLine(p2, new Point(p2.X+len, p2.Y-len));
						goal = p2;
					}
				}

				graphics.RenderSolid(Color.FromBrightness(1.0));  // dessine la grosse flèche blanche
				graphics.LineWidth = 1;
				graphics.LineCap = CapStyle.Square;

				if (this.isDuplicate)
				{
					double d = square.Width*0.14;
					graphics.LineWidth = dim*0.10;  // trait épais
					graphics.AddLine(new Point(goal.X-d, goal.Y), new Point(goal.X+d, goal.Y));
					graphics.AddLine(new Point(goal.X, goal.Y-d), new Point(goal.X, goal.Y+d));
					graphics.RenderSolid(Color.FromRgb(1, 0, 0));  // dessine le gros "+" rouge à l'extrémité de la flèche
					graphics.LineWidth = 1;
				}
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

				Path path = new Path();
				if (r.Width > r.Height)  // bande horizontale ?
				{
					path.MoveTo(r.Left, r.Center.Y);
					path.LineTo(r.Right, r.Center.Y);
				}
				else  // bande verticale ?
				{
					path.MoveTo(r.Center.X, r.Bottom);
					path.LineTo(r.Center.X, r.Top);
				}
				Drawer.DrawPathDash(graphics, 1.0, path, 2.0, 4.0, 5.0, Color.FromRgb(1, 0, 0));  // dessine une bande latérale traitillée rouge
				path.Dispose();
			}

			if (this.IsEnabled && this.DragHost != null)  // contenu de la fenêtre flottante ?
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(Color.FromBrightness(0.8));  // fond gris clair

				rect.Deflate(0.5, 0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(Color.FromBrightness(0.3));  // cadre gris foncé
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

			this.isDuplicate = false;

			//	Crée un échantillon utilisable pour l'opération de drag & drop (il
			//	va représenter visuellement l'échantillon de couleur). On le place
			//	dans un DragWindow et hop.
			MiniatureFrame box = new MiniatureFrame();
			box.Padding = this.Padding;
			box.DragHost = this;

			this.originalViewer = Containers.PageMiniatures.GetViewer(this);
			this.originalLabel  = Containers.PageMiniatures.GetLabel(this);
			this.originalViewer.SetParent(box);
			this.originalLabel.SetParent(box);

			this.dragInfo = new DragInfo(cursor, box, this.ActualSize);

			return true;
		}

		public void OnDragging(DragEventArgs e)
		{
			this.dragInfo.Window.WindowLocation = this.dragInfo.Origin + e.Offset;

			Point mouse = e.ToPoint;
			MiniatureFrame cs = this.FindDropTarget(mouse);

			ArrowHilite arrow = ArrowHilite.None;
			PartialHilite hilite = PartialHilite.None;

			if (cs != null)
			{
				Point m = this.MapClientToScreen(mouse);
				Rectangle b = cs.MapClientToScreen(cs.Client.Bounds);
				Rectangle s = this.MapClientToScreen(this.Client.Bounds);

				if (this.isLeftRightPlacement)
				{
					arrow  = (m.X < s.Center.X) ? ArrowHilite.Before : ArrowHilite.After;
					hilite = (m.X < b.Center.X) ? PartialHilite.Left : PartialHilite.Right;
				}
				else
				{
					arrow  = (m.Y < s.Center.Y) ? ArrowHilite.Before : ArrowHilite.After;
					hilite = (m.Y < b.Center.Y) ? PartialHilite.Bottom : PartialHilite.Top;
				}
			}

			if (cs == this)
			{
				arrow = ArrowHilite.None;
			}

			this.SourceHilite = arrow;

			if (cs != this.dragInfo.Target || this.DragTargetHilite != hilite)
			{
				this.DragHilite(PartialHilite.None);
				this.dragInfo.Target = cs;
				this.DragHilite(hilite);
			}
		}

		public void OnDragEnd()
		{
			this.originalViewer.SetParent(this);
			this.originalLabel.SetParent(this);

			this.SourceHilite = ArrowHilite.None;
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
			public DragInfo(Point cursor, MiniatureFrame widget, Size size)
			{
				System.Diagnostics.Debug.Assert(widget.DragHost != null);

				this.host   = widget.DragHost;
				this.window = new DragWindow();

				this.target = null;
				this.origin = widget.DragHost.MapClientToScreen(new Point(-System.Math.Floor(size.Width/2), -System.Math.Floor(size.Height/2)));

				this.window = new DragWindow();
				this.window.Alpha = 0.6;
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
		public event EventHandler<MiniatureFrameEventArgs> DragAndDropDoing
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
			EventHandler<MiniatureFrameEventArgs> handler = this.GetUserEventHandler<MiniatureFrameEventArgs>("DragAndDropDoing");

			if (handler != null)
			{
				handler (this, new MiniatureFrameEventArgs (dst));
			}
		}
		#endregion


		public static readonly DependencyProperty DragHostProperty         = DependencyProperty.Register("DragHost", typeof(MiniatureFrame), typeof(MiniatureFrame), new DependencyPropertyMetadata().MakeNotSerializable());
		public static readonly DependencyProperty DragSourceFrameProperty  = DependencyProperty.Register("DragSourceFrame", typeof(bool), typeof(MiniatureFrame), new Common.Widgets.Helpers.VisualPropertyMetadata(false, Common.Widgets.Helpers.VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty DragSourceEnableProperty = DependencyProperty.Register("DragSourceEnable", typeof(bool), typeof(MiniatureFrame), new DependencyPropertyMetadata(true));

		private bool									isLeftRightPlacement;
		private ArrowHilite								sourceHilite;
		private PartialHilite							targetHilite;
		private bool									isBefore;
		private bool									isDuplicate;
		private Common.Widgets.Behaviors.DragBehavior	dragBehavior;
		private DragInfo								dragInfo;
		private Viewer									originalViewer;
		private StaticText								originalLabel;
	}

	public class MiniatureFrameEventArgs : Support.EventArgs
	{
		public MiniatureFrameEventArgs(MiniatureFrame frame)
		{
			this.frame = frame;
		}


		public MiniatureFrame Frame
		{
			get
			{
				return this.frame;
			}
		}

		private readonly MiniatureFrame frame;
	}
}
