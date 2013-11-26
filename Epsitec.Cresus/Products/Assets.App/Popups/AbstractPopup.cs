//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Un popup permet de réaliser des dialogues modaux, sans qu'il soit dit.
	/// Dès que la souris quitte la surface, le popup est fermé.
	/// A la création, un popup s'attache à la fenêtre parent nommée "PopupParentFrame",
	/// qui doit remplir toute la fenêtre. Le popup lui-même occupe toute la surfce.
	/// </summary>
	public abstract class AbstractPopup : Widget
	{
		public void Create(Widget target, bool leftOrRight = false)
		{
			//	Crée le popup, dont la queue pointera vers le widget target.
			this.target = target;

			var parent = this.GetParent ();

			this.Parent = parent;
			this.Anchor = AnchorStyles.All;
			this.Name   = "PopupWidget";

			var r1 = parent.MapClientToScreen (parent.ActualBounds);
			var r2 = target.MapClientToScreen (new Rectangle (0, 0, target.ActualWidth, target.ActualHeight));

			var x = r2.Left - r1.Left;
			var y = r2.Bottom - r1.Bottom;

			this.targetReal = new Rectangle (x, y, target.ActualWidth, target.ActualHeight);

			this.targetRect = this.targetReal;
			this.targetRect.Deflate ((int) (System.Math.Min (this.targetRect.Width, this.targetRect.Height) * 0.2));

			this.InitializeDialogRect (leftOrRight);
			this.CreateUI ();
		}

		private void InitializeDialogRect(bool leftOrRight)
		{
			//	Calcule la position du popup.
			//	leftOrRight = false  ->  en haut ou en bas
			//	leftOrRight = true   ->  à gauche ou à droite
			if (leftOrRight)  // à gauche ou à droite ?
			{
				var y = this.targetRect.Center.Y - this.DialogSize.Height/2;

				y = System.Math.Max (y, AbstractPopup.FrameThickness);
				y = System.Math.Min (y, this.Parent.ActualHeight - this.DialogSize.Height - AbstractPopup.FrameThickness);
				y = System.Math.Floor (y);

				if (this.targetRect.Center.X > this.Parent.ActualWidth/2)  // popup à gauche ?
				{
					var x = this.targetRect.Left - AbstractPopup.Spacing - this.DialogSize.Width;
					this.dialogRect = new Rectangle (x, y, this.DialogSize.Width, this.DialogSize.Height);
				}
				else  // popup à droite ?
				{
					var x = this.targetRect.Right + AbstractPopup.Spacing;
					this.dialogRect = new Rectangle (x, y, this.DialogSize.Width, this.DialogSize.Height);
				}
			}
			else  // en haut ou en bas ?
			{
				var x = this.targetRect.Center.X - this.DialogSize.Width/2;

				x = System.Math.Max (x, AbstractPopup.FrameThickness);
				x = System.Math.Min (x, this.Parent.ActualWidth - this.DialogSize.Width - AbstractPopup.FrameThickness);
				x = System.Math.Floor (x);

				if (this.targetRect.Center.Y > this.Parent.ActualHeight/2)  // popup en dessous ?
				{
					var y = this.targetRect.Bottom - AbstractPopup.Spacing - this.DialogSize.Height;
					this.dialogRect = new Rectangle (x, y, this.DialogSize.Width, this.DialogSize.Height);
				}
				else  // popup en dessus ?
				{
					var y = this.targetRect.Top + AbstractPopup.Spacing;
					this.dialogRect = new Rectangle (x, y, this.DialogSize.Width, this.DialogSize.Height);
				}
			}

			this.initialDistance = this.Distance;

			this.mainFrameBox = new FrameBox
			{
				Parent        = this,
				Anchor        = AnchorStyles.BottomLeft,
				PreferredSize = this.dialogRect.Size,
				Margins       = new Margins (this.dialogRect.Left, 0, 0, this.dialogRect.Bottom),
			};
		}


		protected virtual Size DialogSize
		{
			get
			{
				return Size.Empty;
			}
		}

		public virtual void CreateUI()
		{
		}

		protected void ChangeDialogSize(Size size)
		{
			this.dialogRect.Size = size;
			this.mainFrameBox.PreferredSize = size;
			this.Invalidate ();
		}

		protected void CreateCloseButton()
		{
			//	Crée le bouton de fermeture en haut à droite.
			int size = AbstractPopup.TitleHeight - 1;

			var button = new IconButton
			{
				Parent        = this.mainFrameBox,
				IconUri       = AbstractCommandToolbar.GetResourceIconUri ("Popup.Close"),
				AutoFocus     = false,
				Anchor        = AnchorStyles.TopRight,
				PreferredSize = new Size (size, size),
				Margins       = new Margins (0, 0, 0, 0),
			};

			ToolTip.Default.SetToolTip (button, "Ferme la fenêtre");

			button.Clicked += delegate
			{
				this.ClosePopup ();
			};
		}

		protected FrameBox CreateTitle(string text)
		{
			var frame = new FrameBox
			{
				Parent           = this.mainFrameBox,
				Dock             = DockStyle.Top,
				PreferredHeight  = AbstractPopup.TitleHeight - 1,
			};

			if (!string.IsNullOrEmpty (text))
			{
				new StaticText
				{
					Parent           = frame,
					Text             = text,
					ContentAlignment = ContentAlignment.MiddleLeft,
					Dock             = DockStyle.Fill,
					Margins          = new Margins (10, 0, 0, 0),
				};
			}

			//	Trait horizontal sous le titre.
			new FrameBox
			{
				Parent           = this.mainFrameBox,
				Dock             = DockStyle.Top,
				PreferredHeight  = 1,
				BackColor        = ColorManager.PopupBorderColor,
			};

			return frame;
		}

		protected FrameBox CreateFooter()
		{
			return new FrameBox
			{
				Parent              = this.mainFrameBox,
				Dock                = DockStyle.Bottom,
				PreferredHeight     = AbstractPopup.FooterHeight,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};
		}

		protected Button CreateFooterButton(FrameBox parent, DockStyle style, string name, string text, string tooltip = null)
		{
			int lm = (style == DockStyle.Left ) ? 0 : 5;
			int rm = (style == DockStyle.Right) ? 0 : 5;

			var button = new Button
			{
				Parent      = parent,
				Name        = name,
				Text        = text,
				ButtonStyle = ButtonStyle.Icon,
				AutoFocus   = false,
				Dock        = DockStyle.Fill,
				Margins     = new Margins (lm, rm, 0, 0),
			};

			if (!string.IsNullOrEmpty (tooltip))
			{
				ToolTip.Default.SetToolTip (button, tooltip);
			}

			button.Clicked += delegate
			{
				this.ClosePopup ();
				this.OnButtonClicked (button.Name);
			};

			return button;
		}

		protected FrameBox CreateFrame(int x, int y, int dx, int dy)
		{
			var frame = new FrameBox
			{
				Parent        = this.mainFrameBox,
				Anchor        = AnchorStyles.BottomLeft,
				PreferredSize = new Size (dx, dy),
				Margins       = new Margins (x, 0, 0, y),
			};

			return frame;
		}

		protected RadioButton CreateRadio(int x, int y, int dx, int dy, string name, string text, string tooltip = null, bool activate = false)
		{
			var button = new RadioButton
			{
				Parent        = this.mainFrameBox,
				Name          = name,
				Text          = text,
				AutoFocus     = false,
				Anchor        = AnchorStyles.BottomLeft,
				PreferredSize = new Size (dx, dy),
				Margins       = new Margins (x, 0, 0, y),
				ActiveState   = activate ? ActiveState.Yes : ActiveState.No,
			};

			if (!string.IsNullOrEmpty (tooltip))
			{
				ToolTip.Default.SetToolTip (button, tooltip);
			}

			return button;
		}

		protected ColoredButton CreateItem(int x, int y, int dx, int dy, string text)
		{
			var button = new ColoredButton
			{
				Parent           = this.mainFrameBox,
				Text             = text,
				ContentAlignment = ContentAlignment.MiddleLeft,
				Anchor           = AnchorStyles.BottomLeft,
				PreferredSize    = new Size (dx, dy),
				Margins          = new Margins (x, 0, 0, y),
				NormalColor      = Color.Empty,
				SelectedColor    = ColorManager.SelectionColor,
				HoverColor       = ColorManager.HoverColor,
			};

			return button;
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			if (message.MessageType == MessageType.MouseDown)
			{
				this.MouseDown (pos);
			}
			else if (message.MessageType == MessageType.MouseMove)
			{
				this.MouseMove (pos);
			}
			else if (message.MessageType == MessageType.MouseUp)
			{
				this.MouseUp (pos);
			}
			else if (message.MessageType == MessageType.KeyPress)  // TODO: ne fonctionne pas toujours !
			{
				if (message.KeyCode == KeyCode.Escape)
				{
					this.ClosePopup ();
				}
			}

			message.Captured = true;
			message.Consumer = this;

			base.ProcessMessage (message, pos);
		}


		private void MouseDown(Point pos)
		{
			if (this.ExternalRect.Contains (pos))
			{
				this.isDragging = true;
				this.lastPos = pos;
			}
		}

		private void MouseMove(Point pos)
		{
			if (this.isDragging)
			{
				var delta = pos - this.lastPos;
				this.lastPos = pos;

				//	Déplace le rectangle du popup.
				var p = this.dialogRect.BottomLeft;

				this.dialogRect.Offset (delta);

				var master = this.ActualBounds;
				master.Deflate (AbstractPopup.FrameThickness);
				this.dialogRect = AbstractPopup.ForceInside (this.dialogRect, master);

				delta = this.dialogRect.BottomLeft - p;  // recalcule le delta réel

				//	Déplace le FrameBox parent de tous les enfants.
				if (!delta.IsZero)
				{
					this.mainFrameBox.Margins = new Margins
					(
						this.mainFrameBox.Margins.Left + delta.X,
						0,
						0,
						this.mainFrameBox.Margins.Bottom + delta.Y
					);
				}

				this.Invalidate ();
			}
		}

		private void MouseUp(Point pos)
		{
			if (this.isDragging)
			{
				this.isDragging = false;
			}
			else
			{
				//	Un clic de la souris hors du popup le ferme.
				if (!this.ExternalRect.Contains (pos))
				{
					this.ClosePopup ();
				}
			}
		}


		protected void ClosePopup()
		{
			var parent = this.GetParent ();
			parent.Children.Remove (this);
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			this.PaintFrame (graphics);
		}


		private void PaintFrame(Graphics graphics, bool smoothShadow = true)
		{
			if (smoothShadow)
			{
				//	Dessine une jolie ombre douce.
				SmoothShadow.Paint (graphics, this.ExternalRect);
			}
			else
			{
				//	Grise toute la fenêtre.
				graphics.AddFilledRectangle (this.ActualBounds);
				graphics.RenderSolid (Color.FromAlphaRgb (0.5, 0.5, 0.5, 0.5));
			}

			//	Met en évidence le bouton target à l'origine du popup.
			this.PaintTarget (graphics);

			//	Dessine le cadre jaune du popup, avec la queue.
			var alpha = this.Alpha;

			if (alpha > 0.0)  // y a-t-il une queue ?
			{
				//	Gros bord orange.
				graphics.AddFilledRectangle (this.ExternalRect);
				graphics.RenderSolid (ColorManager.SelectionColor);

				//	Intérieur blanc avec la queue.
				var rect = this.dialogRect;
				graphics.AddFilledPath (BalloonPath.GetPath (rect, this.targetRect, this.QueueThickness));
				graphics.RenderSolid (Color.FromAlphaColor (alpha, ColorManager.GetBackgroundColor ()));

				//	Intérieur blanc rectangulaire.
				if (alpha < 1.0)
				{
					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());
				}

				//	Petit filet sombre.
				if (alpha == 1.0)
				{
					rect.Inflate (0.5);
					graphics.AddPath (BalloonPath.GetPath (rect, this.targetRect, this.QueueThickness));
					graphics.RenderSolid (Color.FromAlphaColor (alpha, ColorManager.PopupBorderColor));
				}
				else
				{
					//	Dessine la queue.
					rect.Inflate (0.5);
					graphics.AddPath (BalloonPath.GetPath (rect, this.targetRect, this.QueueThickness, onlyQueue: true, onlyRect: false, onlyBase: false));
					graphics.RenderSolid (Color.FromAlphaColor (alpha, ColorManager.PopupBorderColor));

					//	Dessine le rectangle, sans la base de la queue.
					graphics.AddPath (BalloonPath.GetPath (rect, this.targetRect, this.QueueThickness, onlyQueue: false, onlyRect: true, onlyBase: false));
					graphics.RenderSolid (ColorManager.PopupBorderColor);

					//	Dessine la base de la queue.
					graphics.AddPath (BalloonPath.GetPath (rect, this.targetRect, this.QueueThickness, onlyQueue: false, onlyRect: false, onlyBase: true));
					graphics.RenderSolid (Color.FromAlphaColor (1.0-alpha, ColorManager.PopupBorderColor));
				}
			}
			else  // pas de queue ?
			{
				//	Gros bord orange.
				graphics.AddFilledRectangle (this.ExternalRect);
				graphics.RenderSolid (ColorManager.SelectionColor);

				//	Intérieur blanc rectangulaire.
				var rect = this.dialogRect;
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (ColorManager.GetBackgroundColor ());

				//	Petit filet sombre.
				rect.Inflate (0.5);
				graphics.AddRectangle (rect);
				graphics.RenderSolid (ColorManager.PopupBorderColor);
			}
		}

		private void PaintTarget(Graphics graphics)
		{
			graphics.AddFilledRectangle (this.targetReal);
			graphics.RenderSolid (Color.FromAlphaColor (0.4, ColorManager.SelectionColor));
		}


		private double QueueThickness
		{
			get
			{
				var delta = this.Distance - this.initialDistance + AbstractPopup.Spacing;
				return System.Math.Min (delta / 2.0, 10.0);
			}
		}

		private double Alpha
		{
			get
			{
				var delta = this.Distance - this.initialDistance;
				var factor = (delta-30.0) / 50.0;

				factor = System.Math.Max (factor, 0.0);
				factor = System.Math.Min (factor, 1.0);

				return 1.0 - factor;
			}
		}

		private double Distance
		{
			get
			{
				return BalloonPath.GetDistance (this.ExternalRect, this.targetRect);
			}
		}

		private Rectangle ExternalRect
		{
			//	Retourne le rectangle extérieur, auquel on ajoute une queue.
			get
			{
				var rect = this.dialogRect;
				rect.Inflate (AbstractPopup.FrameThickness);
				return rect;
			}
		}


		protected Widget GetParent()
		{
			//	Retourne un widget parent quelconque, dont la seule caractéristique
			//	importante est qu'il doit occuper toute la fenêtre.
			Widget parent = this.target;

			while (true)
			{
				if (parent.Name == "PopupParentFrame")
				{
					return parent;
				}

				parent = parent.Parent;
			}
		}


		private static Rectangle ForceInside(Rectangle rect, Rectangle master)
		{
			if (rect.Left < master.Left)
			{
				rect.Offset (master.Left-rect.Left, 0);
			}

			if (rect.Right > master.Right)
			{
				rect.Offset (master.Right-rect.Right, 0);
			}

			if (rect.Bottom < master.Bottom)
			{
				rect.Offset (0, master.Bottom-rect.Bottom);
			}

			if (rect.Top > master.Top)
			{
				rect.Offset (0, master.Top-rect.Top);
			}

			return rect;
		}


		#region Events handler
		protected void OnButtonClicked(string name)
		{
			this.ButtonClicked.Raise (this, name);
		}

		public event EventHandler<string> ButtonClicked;
		#endregion


		public  static readonly int				TitleHeight    = 24 + 1;
		private static readonly int				FooterHeight   = 30;
		private static readonly double			FrameThickness = 8;
		private static readonly double			Spacing        = 20;

		private Widget							target;
		private Rectangle						dialogRect;
		private Rectangle						targetReal;
		private Rectangle						targetRect;
		protected FrameBox						mainFrameBox;

		private double							initialDistance;
		private bool							isDragging;
		private Point							lastPos;
	}
}