//	Copyright © 2006-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
				var value = this.GetValue<ButtonClass > (MetaButton.ButtonClassProperty);

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
			get
			{
				return this.GetValue<ButtonMarkDisposition > (MetaButton.MarkDispositionProperty);
			}
			set
			{
				this.SetValue (MetaButton.MarkDispositionProperty, value);
			}
		}

		public double							MarkLength
		{
			get
			{
				return this.GetValue<double> (MetaButton.MarkLengthProperty);
			}
			set
			{
				this.SetValue (MetaButton.MarkLengthProperty, value);
			}
		}

		public Drawing.Color					BulletColor
		{
			get
			{
				return this.GetValue<Drawing.Color> (MetaButton.BulletColorProperty);
			}

			set
			{
				this.SetValue (MetaButton.BulletColorProperty, value);
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
				return this.GetValue<string> (MetaButton.PreferredIconStyleProperty);
			}

			set
			{
				this.SetValue (MetaButton.PreferredIconStyleProperty, value);
			}
		}

		public Drawing.Size PreferredIconSize
		{
			get
			{
				return this.GetValue<Drawing.Size> (MetaButton.PreferredIconSizeProperty);
			}
			set
			{
				this.SetValue (MetaButton.PreferredIconSizeProperty, value);
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

		protected override void OnIconUriChanged(string oldIconUri, string newIconUri)
		{
			base.OnIconUriChanged(oldIconUri, newIconUri);

			if (string.IsNullOrEmpty(oldIconUri) &&
				string.IsNullOrEmpty(newIconUri))
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

			//	Dessine l'icône.
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
				this.cachedButtonClass = command.CommandParameters.GetValueOrDefault (ButtonClass.FlatButton);
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
				this.UpdateCaption (updateIconUri: true);
			}
		}

		private void UpdateIcon()
		{
			//	Met à jour le texte du bouton, qui est un tag <img.../> contenant le nom de l'image
			//	suivi des différentes préférences (taille, langue et style).

			string iconUri = this.IconUri;

			if ((string.IsNullOrEmpty (iconUri)) || 
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
				string            iconSource = IconButton.GetSourceForIconText (iconUri, iconBounds.Size, this.PreferredIconLanguage, this.PreferredIconStyle);

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
			//	Met à jour le bouton lorsque son aspect a changé.
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
			//	Donne le rectangle carré à utiliser pour l'icône du bouton.
			if (this.iconLayout == null)
			{
				return Drawing.Rectangle.Empty;
			}
			else
			{
				var rect = this.GetInnerBounds ();
				var image = Support.ImageProvider.Default.GetImage (this.IconUri, Support.Resources.DefaultManager);
				var size  = image.Size;

				if (this.ContainsValue (MetaButton.PreferredIconSizeProperty))
				{
					size = this.PreferredIconSize;
				}

				if ((image != null) &&
					(size.Width != size.Height))
				{
					double ox = rect.X + System.Math.Floor ((rect.Width - size.Width) / 2);
					double oy = rect.Y + System.Math.Floor ((rect.Height - size.Height) / 2);

					if (this.textVisibility == false)
					{
						rect = new Drawing.Rectangle (ox, oy, size.Width, size.Height);
					}
					else
					{
						rect = new Drawing.Rectangle (rect.X, oy, size.Width, size.Height);
					}
				}
				else
				{
					if (this.textVisibility == false)
					{
						//	Center the icon in the available space; else just left align it.
						
						rect.Left += System.Math.Floor ((rect.Width-rect.Height)/2);
					}

					if (rect.Height > rect.Width)
					{
						rect.Width = rect.Height;
					}
				}
				
				return rect;
			}
		}

		private Drawing.Rectangle GetTextBounds()
		{
			//	Donne le rectangle à utiliser pour le texte du bouton.
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
					rect.Left += 5;  // espace entre le bord gauche ou l'icône et le texte
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
			//  Donne le rectangle utilisable à l'intérieur du cadre du bouton.
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
		public static readonly DependencyProperty PreferredIconSizeProperty		= DependencyProperty.Register ("PreferredIconSize", typeof (Drawing.Size), typeof (MetaButton), new DependencyPropertyMetadata (Drawing.Size.Zero, MetaButton.HandleIconDefinitionChanged));

		private TextLayout			iconLayout;
		private ButtonClass			cachedButtonClass;
		private bool				textVisibility;
	}
}
