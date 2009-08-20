//	Copyright � 2006-2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

[assembly: DependencyClass (typeof (MetaButton))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>MetaButton</c> class implements a button which can behave like
	/// a plain button or like an icon button, depending on its settings.
	/// </summary>
	public sealed class MetaButton : Button
	{
		public MetaButton()
		{
		}

		public ButtonClass						ButtonClass
		{
			get
			{
				ButtonClass value = (ButtonClass) this.GetValue(MetaButton.ButtonClassProperty);

				return (value == ButtonClass.None) ? this.cachedButtonClass : value;
			}
			set
			{
				if (value == ButtonClass.None)
				{
					this.ClearValue (MetaButton.ButtonClassProperty);
				}
				else
				{
					this.SetValue (MetaButton.ButtonClassProperty, value);
				}
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

		public Drawing.Color					BulletColor
		{
			//	Couleur de la puce �ventuelle (si diff�rent de Color.Empty).
			get
			{
				return (Drawing.Color) this.GetValue(MetaButton.BulletColorProperty);
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

		public override Drawing.ContentAlignment ContentAlignment
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

		public override bool					HasTextLabel
		{
			get
			{
				return this.textVisibility;
			}
		}


		protected override Drawing.Size GetTextLayoutSize()
		{
			Drawing.Rectangle bounds = this.GetTextBounds ();
			return bounds.Size;
		}

		protected override string GetCaptionDescription(Caption caption)
		{
			if (this.textVisibility)
			{
				return base.GetCaptionDescription (caption);
			}
			else
			{
				if (caption.HasDescription)
				{
					return caption.Description;
				}
				else if (caption.HasLabels)
				{
					var labels = caption.Labels;
					return labels[labels.Count-1];
				}
				else
				{
					return null;
				}
			}
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.UpdateTextVisibility ();
			this.UpdateIcon ();
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

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Dessine le bouton.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect = this.Client.Bounds;
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
			adorner.PaintButtonBullet (graphics, rect, state, this.BulletColor);

			//	Dessine l'ic�ne.
			if (this.iconLayout != null)
			{
				rect = this.GetIconBounds ();
				
				if (!rect.IsSurfaceZero)
				{
					if (this.innerZoom != 1.0)
					{
						double zoom = (this.innerZoom-1)/2+1;
						this.iconLayout.LayoutSize = rect.Size/this.innerZoom;
						Drawing.Transform transform = graphics.Transform;
						graphics.ScaleTransform (zoom, zoom, 0, -this.Client.Size.Height*zoom);
						adorner.PaintButtonTextLayout (graphics, rect.BottomLeft, this.iconLayout, state, this.ButtonStyle);
						graphics.Transform = transform;
					}
					else
					{
						this.iconLayout.LayoutSize = rect.Size;
						adorner.PaintButtonTextLayout (graphics, rect.BottomLeft, this.iconLayout, state, this.ButtonStyle);
					}
				}
			}

			//	Dessine le texte.
			if (this.TextLayout != null)
			{
				rect = this.GetTextBounds ();
				
				if (!rect.IsSurfaceZero)
				{
					if (this.innerZoom != 1.0)
					{
						Drawing.Point center = this.Client.Bounds.Center;
						this.TextLayout.LayoutSize = rect.Size/this.innerZoom;
						Drawing.Transform transform = graphics.Transform;
						graphics.ScaleTransform (this.innerZoom, this.innerZoom, center.X, center.Y);
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
		}

		protected override void OnCommandObjectChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnCommandObjectChanged (e);

			Command command = e.NewValue as Command;

			if ((command != null) &&
				(!this.ContainsValue (MetaButton.ButtonClassProperty)))
			{
				string buttonClassParam = command.CommandParameters["ButtonClass"];

				if (string.IsNullOrEmpty (buttonClassParam))
				{
					this.cachedButtonClass = ButtonClass.FlatButton;
				}
				else
				{
					this.cachedButtonClass = buttonClassParam.ToEnum<ButtonClass> ();
				}

				this.UpdateButtonClass (this.cachedButtonClass);
			}
		}


		private void UpdateTextVisibility()
		{
			Drawing.Rectangle rect = this.GetInnerBounds ();

			bool oldVisibility = this.textVisibility;

			if (rect.Width < rect.Height*2)
			{
				//	Not enough room for the text, but only for the icon.

				this.textVisibility = false;
			}
			else
			{
				this.textVisibility = true;
			}

			if (oldVisibility != this.textVisibility)
			{
				this.UpdateCaption ();
			}
		}

		private void UpdateIcon()
		{
			//	Met � jour le texte du bouton, qui est un tag <img.../> contenant le nom de l'image
			//	suivi des diff�rentes pr�f�rences (taille, langue et style).

			string iconName = this.IconName;

			if ((string.IsNullOrEmpty (iconName)) || 
				(this.ButtonClass == ButtonClass.DialogButton))
			{
				if (this.iconLayout != null)
				{
					this.iconLayout = null;
					this.Invalidate ();
				}
			}
			else
			{
				Drawing.Rectangle iconBounds = this.GetIconBounds ();
				string            iconSource = IconButton.GetSourceForIconText (iconName, iconBounds.Size, this.PreferredIconLanguage, this.PreferredIconStyle);

				if (this.iconLayout == null)
				{
					this.iconLayout = new TextLayout ();
				}
				else
				{
					if ((this.iconLayout.Text == iconSource) &&
						(this.iconLayout.Alignment == Drawing.ContentAlignment.MiddleCenter))
					{
						return;
					}
				}

				this.iconLayout.Text      = iconSource;
				this.iconLayout.Alignment = Drawing.ContentAlignment.MiddleCenter;
				
				this.Invalidate ();
			}
		}

		private void UpdateButtonClass(ButtonClass aspect)
		{
			//	Met � jour le bouton lorsque son aspect a chang�.
			switch (aspect)
			{
				case ButtonClass.DialogButton:
				case ButtonClass.RichDialogButton:
					base.ButtonStyle = ButtonStyle.Normal;
					base.ContentAlignment = Drawing.ContentAlignment.MiddleCenter;
					break;

				case ButtonClass.FlatButton:
					base.ButtonStyle = ButtonStyle.ToolItem;
					base.ContentAlignment = Drawing.ContentAlignment.MiddleLeft;
					break;

				default:
					throw new System.NotSupportedException (string.Format ("ButtonClass.{0} not supported", aspect));
			}
		}

		
		private Drawing.Rectangle GetIconBounds()
		{
			//	Donne le rectangle carr� � utiliser pour l'ic�ne du bouton.
			if (this.iconLayout == null)
			{
				return Drawing.Rectangle.Empty;
			}
			else
			{
				Drawing.Rectangle rect = this.GetInnerBounds ();

				if (this.textVisibility == false)
				{
					//	Center the icon in the available space; else just left align it.

					rect.Left += System.Math.Floor((rect.Width-rect.Height)/2);
				}

				rect.Width = rect.Height;  // forc�ment un carr�
				return rect;
			}
		}

		private Drawing.Rectangle GetTextBounds()
		{
			//	Donne le rectangle � utiliser pour le texte du bouton.
			Drawing.Rectangle rect = this.GetInnerBounds ();
			ButtonClass buttonClass = this.ButtonClass;

			if ((buttonClass == ButtonClass.FlatButton) ||
				(buttonClass == ButtonClass.RichDialogButton))
			{
				if (this.textVisibility == false)
				{
					return Drawing.Rectangle.Empty;
				}
			}

			if (this.iconLayout != null)
			{
				if (buttonClass == ButtonClass.FlatButton)
				{
					rect.Left += rect.Height;
				}
			}

			switch (this.ContentAlignment)
			{
				case Drawing.ContentAlignment.BottomLeft:
				case Drawing.ContentAlignment.MiddleLeft:
				case Drawing.ContentAlignment.TopLeft:
					rect.Left += 5;  // espace entre le bord gauche ou l'ic�ne et le texte
					break;
				
				case Drawing.ContentAlignment.BottomRight:
				case Drawing.ContentAlignment.MiddleRight:
				case Drawing.ContentAlignment.TopRight:
					rect.Right -= 5;  // espace entre le bord droite et le texte
					break;
			}

			return rect;
		}

		private Drawing.Rectangle GetInnerBounds()
		{
			//  Donne le rectangle utilisable � l'int�rieur du cadre du bouton.
			Drawing.Rectangle bounds = IconButtonMark.GetFrameBounds (this.Client.Bounds, this.MarkDisposition, this.MarkLength);

			if (!this.BulletColor.IsEmpty)
			{
				bounds.Left += bounds.Height;
			}
			
			switch (this.ButtonStyle)
			{
				case ButtonStyle.Normal:
				case ButtonStyle.DefaultAccept:
				case ButtonStyle.DefaultAcceptAndCancel:
				case ButtonStyle.DefaultCancel:
					return Drawing.Rectangle.Deflate (bounds, new Drawing.Margins (4));
				
				default:
					return bounds;
			}
		}


		static MetaButton()
		{
			DependencyPropertyMetadata metadataButtonStyle = Button.ButtonStyleProperty.DefaultMetadata.Clone ();
			DependencyPropertyMetadata metadataAlign = Visual.ContentAlignmentProperty.DefaultMetadata.Clone ();
			DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataButtonStyle.MakeNotSerializable ();
			metadataAlign.DefineDefaultValue (Drawing.ContentAlignment.MiddleLeft);
			metadataDy.DefineDefaultValue (Widget.DefaultFontHeight+10);

			Button.ButtonStyleProperty.OverrideMetadata(typeof(MetaButton), metadataButtonStyle);
			Visual.ContentAlignmentProperty.OverrideMetadata(typeof(MetaButton), metadataAlign);
			Visual.PreferredHeightProperty.OverrideMetadata(typeof(MetaButton), metadataDy);
		}


		private static void HandleButtonClassChanged(DependencyObject obj, object oldValue, object newValue)
		{
			MetaButton that = (MetaButton) obj;
			that.UpdateButtonClass((ButtonClass) newValue);
		}

		private static void HandleIconDefinitionChanged(DependencyObject obj, object oldValue, object newValue)
		{
			MetaButton that = (MetaButton) obj;
			that.UpdateIcon ();
		}

		public static readonly DependencyProperty ButtonClassProperty			= DependencyProperty.Register ("ButtonClass", typeof (ButtonClass), typeof (MetaButton), new DependencyPropertyMetadata (ButtonClass.None, MetaButton.HandleButtonClassChanged));
		public static readonly DependencyProperty MarkDispositionProperty		= DependencyProperty.Register ("MarkDisposition", typeof (ButtonMarkDisposition), typeof (MetaButton), new DependencyPropertyMetadata (ButtonMarkDisposition.None, MetaButton.HandleIconDefinitionChanged));
		public static readonly DependencyProperty MarkLengthProperty			= DependencyProperty.Register ("MarkLength", typeof (double), typeof (MetaButton), new DependencyPropertyMetadata (8.0, MetaButton.HandleIconDefinitionChanged));
		public static readonly DependencyProperty BulletColorProperty			= DependencyProperty.Register ("BulletColor", typeof (Drawing.Color), typeof (MetaButton), new DependencyPropertyMetadata (Drawing.Color.Empty, MetaButton.HandleIconDefinitionChanged));
		public static readonly DependencyProperty PreferredIconLanguageProperty	= DependencyProperty.Register ("PreferredIconLanguage", typeof (string), typeof (MetaButton), new DependencyPropertyMetadata (System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName, MetaButton.HandleIconDefinitionChanged));
		public static readonly DependencyProperty PreferredIconStyleProperty	= DependencyProperty.Register ("PreferredIconStyle", typeof (string), typeof (MetaButton), new DependencyPropertyMetadata (null, MetaButton.HandleIconDefinitionChanged));

		private TextLayout			iconLayout;
		private ButtonClass			cachedButtonClass;
		private bool				textVisibility;
	}
}
