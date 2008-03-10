//	Copyright © 2006-2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.UI.MetaButton))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// La classe MetaButton est un Button pouvant contenir une icône et/ou un texte.
	/// </summary>
	public class MetaButton : Button
	{
		public MetaButton()
		{
		}

		static MetaButton()
		{
			Types.DependencyPropertyMetadata metadataButtonStyle = Button.ButtonStyleProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataAlign = Visual.ContentAlignmentProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataButtonStyle.MakeNotSerializable ();
			metadataAlign.DefineDefaultValue (Drawing.ContentAlignment.MiddleLeft);
			metadataDy.DefineDefaultValue (Widget.DefaultFontHeight+10);

			Button.ButtonStyleProperty.OverrideMetadata(typeof(MetaButton), metadataButtonStyle);
			Visual.ContentAlignmentProperty.OverrideMetadata(typeof(MetaButton), metadataAlign);
			Visual.PreferredHeightProperty.OverrideMetadata(typeof(MetaButton), metadataDy);
		}

		public ButtonClass						ButtonClass
		{
			//	Aspect du bouton.
			get
			{
				return (ButtonClass) this.GetValue(MetaButton.ButtonClassProperty);
			}

			set
			{
				this.SetValue (MetaButton.ButtonClassProperty, value);
			}
		}

		public ButtonDisplayMode				DisplayMode
		{
			//	Mode d'affichage du contenu du bouton.
			//	Pas de 'set' disponible; il faut utilser ButtonClass.
			get
			{
				return (ButtonDisplayMode) this.GetValue(MetaButton.DisplayModeProperty);
			}
		}

		public ButtonMarkDisposition			MarkDisposition
		{
			//	Emplacement de la marque.
			get
			{
				return (ButtonMarkDisposition) this.GetValue(MetaButton.MarkDispositionProperty);
			}

			set
			{
				this.SetValue(MetaButton.MarkDispositionProperty, value);
			}
		}

		public double							MarkLength
		{
			//	Dimension de la marque.
			get
			{
				return (double) this.GetValue(MetaButton.MarkLengthProperty);
			}

			set
			{
				this.SetValue(MetaButton.MarkLengthProperty, value);
			}
		}

		public Color							BulletColor
		{
			//	Couleur de la puce éventuelle (si différent de Color.Empty).
			get
			{
				return (Color) this.GetValue(MetaButton.BulletColorProperty);
			}

			set
			{
				this.SetValue(MetaButton.BulletColorProperty, value);
			}
		}

		public string							PreferredIconLanguage
		{
			get
			{
				return (string) this.GetValue(MetaButton.PreferredIconLanguageProperty);
			}

			set
			{
				this.SetValue(MetaButton.PreferredIconLanguageProperty, value);
			}
		}

		public string							PreferredIconStyle
		{
			get
			{
				return (string) this.GetValue(MetaButton.PreferredIconStyleProperty);
			}

			set
			{
				this.SetValue(MetaButton.PreferredIconStyleProperty, value);
			}
		}

		public override ButtonStyle				ButtonStyle
		{
			get
			{
				return base.ButtonStyle;
			}
			set
			{
				throw new System.InvalidOperationException ("Use ButtonClass instead");
			}
		}

		public override ContentAlignment		ContentAlignment
		{
			get
			{
				return base.ContentAlignment;
			}
			set
			{
				throw new System.InvalidOperationException ("Use ButtonClass instead");
			}
		}


		protected override Size GetTextLayoutSize()
		{
			Rectangle bounds = this.GetTextBounds ();
			return bounds.Size;
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			this.UpdateIcon();
		}

		protected override void OnIconNameChanged(string oldIconName, string newIconName)
		{
			base.OnIconNameChanged(oldIconName, newIconName);

			if (string.IsNullOrEmpty(oldIconName) &&
				string.IsNullOrEmpty(newIconName))
			{
				//	Nothing to do. Change is not significant : the text remains
				//	empty if we swap "" for null.
			}
			else
			{
				this.UpdateIcon();
			}
		}


		protected void UpdateIcon()
		{
			//	Met à jour le texte du bouton, qui est un tag <img.../> contenant le nom de l'image
			//	suivi des différentes préférences (taille, langue et style).

			string iconName = this.IconName;

			if ((string.IsNullOrEmpty (iconName)) || 
				(this.DisplayMode == ButtonDisplayMode.TextOnly))
			{
				if (this.iconLayout != null)
				{
					this.iconLayout = null;
					this.Invalidate ();
				}
			}
			else
			{
				Rectangle iconBounds = this.GetIconBounds ();
				string    iconSource = IconButton.GetSourceForIconText (iconName, iconBounds.Size, this.PreferredIconLanguage, this.PreferredIconStyle);

				if (this.iconLayout == null)
				{
					this.iconLayout = new TextLayout ();
				}
				else
				{
					if ((this.iconLayout.Text == iconSource) &&
						(this.iconLayout.Alignment == ContentAlignment.MiddleCenter))
					{
						return;
					}
				}

				this.iconLayout.Text      = iconSource;
				this.iconLayout.Alignment = ContentAlignment.MiddleCenter;
				
				this.Invalidate ();
			}
		}

		protected void UpdateButtonClass(ButtonClass aspect)
		{
			//	Met à jour le bouton lorsque son aspect a changé.
			switch (aspect)
			{
				case ButtonClass.DialogButton:
					this.SetValue (MetaButton.DisplayModeProperty, ButtonDisplayMode.TextOnly);
					base.ButtonStyle = ButtonStyle.Normal;
					base.ContentAlignment = ContentAlignment.MiddleCenter;
					break;

				case ButtonClass.IconButton:
					this.SetValue (MetaButton.DisplayModeProperty, ButtonDisplayMode.Automatic);
					base.ButtonStyle = ButtonStyle.ToolItem;
					base.ContentAlignment = ContentAlignment.MiddleLeft;
					break;

				default:
					throw new System.NotSupportedException (string.Format ("ButtonClass.{0} not supported", aspect));
			}
		}

		protected Rectangle GetIconBounds()
		{
			//	Donne le rectangle carré à utiliser pour l'icône du bouton.
			if (this.iconLayout == null)
			{
				return Rectangle.Empty;
			}
			else
			{
				Rectangle rect = this.GetInnerBounds ();

				if (rect.Width < rect.Height*2)  // place seulement pour l'icône ?
				{
					rect.Left += System.Math.Floor((rect.Width-rect.Height)/2);
				}

				rect.Width = rect.Height;  // forcément un carré
				return rect;
			}
		}

		protected Rectangle GetTextBounds()
		{
			//	Donne le rectangle à utiliser pour le texte du bouton.
			Rectangle rect = this.GetInnerBounds ();

			if (this.DisplayMode == ButtonDisplayMode.Automatic && rect.Width < rect.Height*2)  // place seulement pour l'icône ?
			{
				return Rectangle.Empty;
			}

			if (this.iconLayout != null)
			{
				rect.Left += rect.Height;
			}

			switch (this.ContentAlignment)
			{
				case ContentAlignment.BottomLeft:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.TopLeft:
					rect.Left += 5;  // espace entre le bord gauche ou l'icône et le texte
					break;
			}

			return rect;
		}

		protected Rectangle GetInnerBounds()
		{
			//  Donne le rectangle utilisable à l'intérieur du cadre du bouton.
			Rectangle bounds = IconButtonMark.GetFrameBounds (this.Client.Bounds, this.MarkDisposition, this.MarkLength);
			
			switch (this.ButtonStyle)
			{
				case ButtonStyle.Normal:
				case ButtonStyle.DefaultAccept:
				case ButtonStyle.DefaultAcceptAndCancel:
				case ButtonStyle.DefaultCancel:
					return Rectangle.Deflate (bounds, new Margins (4));
				default:
					return bounds;
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le bouton.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			WidgetPaintState state = this.PaintState;

			bool enable = ((state & WidgetPaintState.Enabled) != 0);
			if (!enable)
			{
				state &= ~WidgetPaintState.Focused;
				state &= ~WidgetPaintState.Entered;
				state &= ~WidgetPaintState.Engaged;
			}

			if ((this.ActiveState == ActiveState.Yes) && 
				(this.MarkDisposition != ButtonMarkDisposition.None))  // dessine la marque triangulaire ?
			{
				adorner.PaintButtonMark (graphics, rect, state, this.MarkDisposition, this.MarkLength);
			}

			rect  = IconButtonMark.GetFrameBounds (rect, this.MarkDisposition, this.MarkLength);
			state &= ~WidgetPaintState.Selected;
			adorner.PaintButtonBackground (graphics, rect, state, Direction.Down, this.ButtonStyle);
			adorner.PaintButtonBullet (graphics, ref rect, state, this.BulletColor);

			//	Dessine l'icône.
			if (this.iconLayout != null)
			{
				Rectangle ricon = this.GetIconBounds ();
				if (!ricon.IsSurfaceZero)
				{
					if (this.innerZoom != 1.0)
					{
						double zoom = (this.innerZoom-1)/2+1;
						this.iconLayout.LayoutSize = ricon.Size/this.innerZoom;
						Transform transform = graphics.Transform;
						graphics.ScaleTransform (zoom, zoom, 0, -this.Client.Size.Height*zoom);
						adorner.PaintButtonTextLayout (graphics, ricon.BottomLeft, this.iconLayout, state, this.ButtonStyle);
						graphics.Transform = transform;
					}
					else
					{
						this.iconLayout.LayoutSize = ricon.Size;
						adorner.PaintButtonTextLayout (graphics, ricon.BottomLeft, this.iconLayout, state, this.ButtonStyle);
					}
				}
			}

			//	Dessine le texte.
			rect = this.GetTextBounds ();
			if ((!rect.IsSurfaceZero) &&
				(this.TextLayout != null))
			{
				if (this.innerZoom != 1.0)
				{
					this.TextLayout.LayoutSize = rect.Size/this.innerZoom;
					Transform transform = graphics.Transform;
					graphics.ScaleTransform (this.innerZoom, this.innerZoom, this.Client.Size.Width / 2, this.Client.Size.Height / 2);
					adorner.PaintButtonTextLayout (graphics, rect.BottomLeft, this.TextLayout, state, this.ButtonStyle);
					graphics.Transform = transform;
				}
				else
				{
					this.TextLayout.LayoutSize = rect.Size;
					adorner.PaintButtonTextLayout (graphics, rect.BottomLeft, this.TextLayout, state, this.ButtonStyle);
				}
			}
		}

		private static void HandleButtonClassChanged(DependencyObject obj, object oldValue, object newValue)
		{
			MetaButton that = (MetaButton) obj;
			that.UpdateButtonClass((ButtonClass) newValue);
		}

		private static void HandleIconDefinitionChanged(DependencyObject obj, object oldValue, object newValue)
		{
			MetaButton that = obj as MetaButton;

			that.UpdateIcon();
		}

		public static readonly DependencyProperty ButtonClassProperty			= DependencyProperty.Register ("ButtonClass", typeof (ButtonClass), typeof (MetaButton), new DependencyPropertyMetadata (ButtonClass.None, MetaButton.HandleButtonClassChanged));
		public static readonly DependencyProperty DisplayModeProperty			= DependencyProperty.RegisterReadOnly("DisplayMode", typeof(ButtonDisplayMode), typeof(MetaButton), new DependencyPropertyMetadata(ButtonDisplayMode.Automatic, MetaButton.HandleIconDefinitionChanged));
		public static readonly DependencyProperty MarkDispositionProperty		= DependencyProperty.Register("MarkDisposition", typeof(ButtonMarkDisposition), typeof(MetaButton), new DependencyPropertyMetadata(ButtonMarkDisposition.None, MetaButton.HandleIconDefinitionChanged));
		public static readonly DependencyProperty MarkLengthProperty			= DependencyProperty.Register("MarkLength", typeof(double), typeof(MetaButton), new DependencyPropertyMetadata(8.0, MetaButton.HandleIconDefinitionChanged));
		public static readonly DependencyProperty BulletColorProperty			= DependencyProperty.Register("BulletColor", typeof(Color), typeof(MetaButton), new DependencyPropertyMetadata(Color.Empty, MetaButton.HandleIconDefinitionChanged));
		public static readonly DependencyProperty PreferredIconLanguageProperty	= DependencyProperty.Register("PreferredIconLanguage", typeof(string), typeof(MetaButton), new DependencyPropertyMetadata(System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName, MetaButton.HandleIconDefinitionChanged));
		public static readonly DependencyProperty PreferredIconStyleProperty	= DependencyProperty.Register("PreferredIconStyle", typeof(string), typeof(MetaButton), new DependencyPropertyMetadata(null, MetaButton.HandleIconDefinitionChanged));

		protected TextLayout			iconLayout;
	}
}
