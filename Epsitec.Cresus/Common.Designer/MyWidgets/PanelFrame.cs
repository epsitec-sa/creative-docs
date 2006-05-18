using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget de type 'groupe' avec un cadre permettant d'éditer les widgets enfants.
	/// </summary>
	public class PanelFrame : AbstractGroup
	{
		protected enum MouseCursorType
		{
			Arrow,
			ArrowPlus,
			Global,
			Hand,
			Edit,
			Pen,
			Zoom,
		}


		public PanelFrame() : base()
		{
		}

		public PanelFrame(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		static PanelFrame()
		{
			Widgets.Helpers.VisualPropertyMetadata metadata = new Widgets.Helpers.VisualPropertyMetadata(ContentAlignment.TopLeft, Widgets.Helpers.VisualPropertyMetadataOptions.AffectsTextLayout);
			Widgets.Visual.ContentAlignmentProperty.OverrideMetadata(typeof(PanelFrame), metadata);
		}


		public string					Tool
		{
			get
			{
				return this.tool;
			}

			set
			{
				this.tool = value;
			}
		}


		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch (message.Type)
			{
				case MessageType.MouseDown:
					this.ProcessMouseDown(pos);
					message.Captured = true;
					message.Consumer = this;
					break;

				case MessageType.MouseMove:
					this.ProcessMouseMove(pos);
					message.Captured = true;
					message.Consumer = this;
					break;

				case MessageType.MouseUp:
					this.ProcessMouseUp(pos);
					message.Captured = true;
					message.Consumer = this;
					break;
			}
		}

		void ProcessMouseDown(Point pos)
		{
			//	La souris a été pressée.
			if (this.tool == "ToolSelect")
			{
				this.SelectDown(pos);
			}

			if (this.tool == "ToolGlobal")
			{
				this.GlobalDown(pos);
			}

			if (this.tool == "ToolEdit")
			{
				this.EditDown(pos);
			}

			if (this.tool == "ToolZoom")
			{
				this.ZoomDown(pos);
			}

			if (this.tool == "ToolHand")
			{
				this.HandDown(pos);
			}

			if (this.tool.StartsWith("Object"))
			{
				this.ObjectDown(this.tool, pos);
			}
		}

		void ProcessMouseMove(Point pos)
		{
			//	La souris a été déplacée.
			if (this.tool == "ToolSelect")
			{
				this.SelectMove(pos);
			}

			if (this.tool == "ToolGlobal")
			{
				this.GlobalMove(pos);
			}

			if (this.tool == "ToolEdit")
			{
				this.EditMove(pos);
			}

			if (this.tool == "ToolZoom")
			{
				this.ZoomMove(pos);
			}

			if (this.tool == "ToolHand")
			{
				this.HandMove(pos);
			}

			if (this.tool.StartsWith("Object"))
			{
				this.ObjectMove(this.tool, pos);
			}
		}

		void ProcessMouseUp(Point pos)
		{
			//	La souris a été relâchée.
			if (this.tool == "ToolSelect")
			{
				this.SelectUp(pos);
			}

			if (this.tool == "ToolGlobal")
			{
				this.GlobalUp(pos);
			}

			if (this.tool == "ToolEdit")
			{
				this.EditUp(pos);
			}

			if (this.tool == "ToolZoom")
			{
				this.ZoomUp(pos);
			}

			if (this.tool == "ToolHand")
			{
				this.HandUp(pos);
			}

			if (this.tool.StartsWith("Object"))
			{
				this.ObjectUp(this.tool, pos);
			}
		}


		#region Drawing select
		protected void SelectDown(Point pos)
		{
			//	Sélection ponctuelle, souris pressée.
		}

		protected void SelectMove(Point pos)
		{
			//	Sélection ponctuelle, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Arrow);
		}

		protected void SelectUp(Point pos)
		{
			//	Sélection ponctuelle, souris relâchée.
		}
		#endregion

		#region Drawing global
		protected void GlobalDown(Point pos)
		{
			//	Sélection rectangulaire, souris pressée.
		}

		protected void GlobalMove(Point pos)
		{
			//	Sélection rectangulaire, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Global);
		}

		protected void GlobalUp(Point pos)
		{
			//	Sélection rectangulaire, souris relâchée.
		}
		#endregion

		#region Drawing edit
		protected void EditDown(Point pos)
		{
			//	Edition, souris pressée.
		}

		protected void EditMove(Point pos)
		{
			//	Edition, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Edit);
		}

		protected void EditUp(Point pos)
		{
			//	Edition, souris relâchée.
		}
		#endregion

		#region Drawing zoom
		protected void ZoomDown(Point pos)
		{
			//	Loupe, souris pressée.
		}

		protected void ZoomMove(Point pos)
		{
			//	Loupe, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Zoom);
		}

		protected void ZoomUp(Point pos)
		{
			//	Loupe, souris relâchée.
		}
		#endregion

		#region Drawing hand
		protected void HandDown(Point pos)
		{
			//	Main, souris pressée.
		}

		protected void HandMove(Point pos)
		{
			//	Main, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Hand);
		}

		protected void HandUp(Point pos)
		{
			//	Main, souris relâchée.
		}
		#endregion

		#region Drawing object
		protected void ObjectDown(string tool, Point pos)
		{
			//	Dessin d'un objet, souris pressée.
			this.ContainerLock();

			if (tool == "ObjectLine")
			{
				this.creatingObject = new Separator(this);
				this.creatingObject.PreferredHeight = 1;
			}

			if (tool == "ObjectButton")
			{
				this.creatingObject = new Button(this);
				this.creatingObject.Text = "Button";
			}
			
			if (tool == "ObjectText")
			{
				this.creatingObject = new TextField(this);
				this.creatingObject.Text = "TextField";
			}

			this.creatingObject.Anchor = AnchorStyles.BottomLeft;
			this.creatingObject.Margins = new Margins(pos.X, 0, 0, pos.Y);
		}

		protected void ObjectMove(string tool, Point pos)
		{
			//	Dessin d'un objet, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Pen);

			if (this.creatingObject != null)
			{
				this.creatingObject.Margins = new Margins(pos.X, 0, 0, pos.Y);
			}
		}

		protected void ObjectUp(string tool, Point pos)
		{
			//	Dessin d'un objet, souris relâchée.
			this.creatingObject = null;
			this.ContainerUnlock();
		}
		#endregion

		protected void ContainerLock()
		{
#if false  // TODO: génère des asserts 'dirty layout' !
			this.container.MinSize = this.container.ActualSize;
			this.container.MaxSize = this.container.ActualSize;
#endif
		}

		protected void ContainerUnlock()
		{
#if false
			this.container.MinSize = new Size(100, 100);
			this.container.MaxSize = new Size(10000, 10000);
#endif
		}



		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le texte.
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(adorner.ColorBorder);
		}


		#region Mouse events
		protected virtual void OnMouseDown(Point pos)
		{
			EventHandler<Point> handler = (EventHandler<Point>) this.GetUserEventHandler("MouseDown");
			if (handler != null)
			{
				handler(this, pos);
			}
		}

		public event EventHandler<Point> MouseDown
		{
			add
			{
				this.AddUserEventHandler("MouseDown", value);
			}
			remove
			{
				this.RemoveUserEventHandler("MouseDown", value);
			}
		}

		protected virtual void OnMouseMove(Point pos)
		{
			EventHandler<Point> handler = (EventHandler<Point>) this.GetUserEventHandler("MouseMove");
			if (handler != null)
			{
				handler(this, pos);
			}
		}

		public event EventHandler<Point> MouseMove
		{
			add
			{
				this.AddUserEventHandler("MouseMove", value);
			}
			remove
			{
				this.RemoveUserEventHandler("MouseMove", value);
			}
		}

		protected virtual void OnMouseUp(Point pos)
		{
			EventHandler<Point> handler = (EventHandler<Point>) this.GetUserEventHandler("MouseUp");
			if (handler != null)
			{
				handler(this, pos);
			}
		}

		public event EventHandler<Point> MouseUp
		{
			add
			{
				this.AddUserEventHandler("MouseUp", value);
			}
			remove
			{
				this.RemoveUserEventHandler("MouseUp", value);
			}
		}
		#endregion

		#region MouseCursor
		protected void ChangeMouseCursor(MouseCursorType cursor)
		{
			//	Change le sprite de la souris.
			switch (cursor)
			{
				case MouseCursorType.Arrow:
					this.MouseCursorImage(ref this.mouseCursorArrow, Misc.Icon("CursorArrow"));
					break;

				case MouseCursorType.ArrowPlus:
					this.MouseCursorImage(ref this.mouseCursorArrowPlus, Misc.Icon("CursorArrowPlus"));
					break;

				case MouseCursorType.Global:
					this.MouseCursorImage(ref this.mouseCursorGlobal, Misc.Icon("CursorGlobal"));
					break;

				case MouseCursorType.Edit:
					this.MouseCursorImage(ref this.mouseCursorEdit, Misc.Icon("CursorEdit"));
					break;

				case MouseCursorType.Hand:
					this.MouseCursorImage(ref this.mouseCursorHand, Misc.Icon("CursorHand"));
					break;

				case MouseCursorType.Pen:
					this.MouseCursorImage(ref this.mouseCursorPen, Misc.Icon("CursorPen"));
					break;

				case MouseCursorType.Zoom:
					this.MouseCursorImage(ref this.mouseCursorZoom, Misc.Icon("CursorZoom"));
					break;

				default:
					this.MouseCursor = MouseCursor.AsArrow;
					break;
			}

			this.Window.MouseCursor = this.MouseCursor;
		}

		protected void MouseCursorImage(ref Image image, string name)
		{
			//	Choix du sprite de la souris.
			if (image == null)
			{
				image = Support.Resources.DefaultManager.GetImage(name);
			}

			this.MouseCursor = MouseCursor.FromImage(image);
		}
		#endregion


		protected string					tool;
		protected Widget					creatingObject;

		protected Image						mouseCursorArrow = null;
		protected Image						mouseCursorArrowPlus = null;
		protected Image						mouseCursorGlobal = null;
		protected Image						mouseCursorEdit = null;
		protected Image						mouseCursorPen = null;
		protected Image						mouseCursorZoom = null;
		protected Image						mouseCursorHand = null;
	}
}
