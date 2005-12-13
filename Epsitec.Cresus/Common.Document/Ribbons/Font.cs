using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Font permet de choisir la fonte du texte.
	/// </summary>
	[SuppressBundleSupport]
	public class Font : Abstract
	{
		public Font() : base()
		{
			this.title.Text = Res.Strings.Action.Text.Font.Main;

			//?this.buttonBold       = this.CreateIconButton(Misc.Icon("FontBold"),       Res.Strings.Action.Text.Font.Bold,       new MessageEventHandler(this.HandleButtonBoldClicked));
			//?this.buttonItalic     = this.CreateIconButton(Misc.Icon("FontItalic"),     Res.Strings.Action.Text.Font.Italic,     new MessageEventHandler(this.HandleButtonItalicClicked));
			//?this.buttonUnderlined = this.CreateIconButton(Misc.Icon("FontUnderlined"), Res.Strings.Action.Text.Font.Underlined, new MessageEventHandler(this.HandleButtonUnderlinedClicked));
			this.buttonBold       = this.CreateIconButton("FontBold",       Misc.Icon("FontBold"),       Res.Strings.Action.Text.Font.Bold,       true);
			this.buttonItalic     = this.CreateIconButton("FontItalic",     Misc.Icon("FontItalic"),     Res.Strings.Action.Text.Font.Italic,     true);
			this.buttonUnderlined = this.CreateIconButton("FontUnderlined", Misc.Icon("FontUnderlined"), Res.Strings.Action.Text.Font.Underlined, true);
			this.buttonOverlined  = this.CreateIconButton("FontOverlined",  Misc.Icon("FontOverlined"),  Res.Strings.Action.Text.Font.Overlined,  true);
			this.buttonStrikeout  = this.CreateIconButton("FontStrikeout",  Misc.Icon("FontStrikeout"),  Res.Strings.Action.Text.Font.Strikeout,  true);

			this.buttonStyle = new Button(this);
			this.buttonStyle.Text = "S";
			this.buttonStyle.Clicked += new MessageEventHandler(this.HandleButtonStyleClicked);

			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

#if false
		public override void SetDocument(DocumentType type, InstallType install, Settings.GlobalSettings gs, Document document)
		{
			if ( this.document != null )
			{
				this.document.TextWrapper.Active.Changed  -= new EventHandler(this.HandleWrapperChanged);
				this.document.TextWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);
			}

			base.SetDocument(type, install, gs, document);

			if ( this.document != null )
			{
				this.document.TextWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
				this.document.TextWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);
			}

			this.HandleWrapperChanged(null);
		}
#endif

		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 200;
			}
		}


		protected void UpdateButtonBold()
		{
			bool enabled = false;
			bool state   = false;

			if ( this.document != null && this.document.TextWrapper.IsAttached )
			{
				string face  = this.document.TextWrapper.Defined.FontFace;
				if ( face == null )
				{
					face = this.document.TextWrapper.Active.FontFace;
				}

				string style = this.document.TextWrapper.Defined.FontStyle;
				if ( style == null )
				{
					style = this.document.TextWrapper.Active.FontStyle;
				}

				OpenType.FontWeight weight = OpenType.FontWeight.Medium;
				if ( face != null && style != null )
				{
					OpenType.Font font = TextContext.GetFont(face, style);
					weight = font.FontIdentity.FontWeight;
				}

				enabled = true;
				state   = ((int)weight > (int)OpenType.FontWeight.Medium);
				state  ^= this.document.TextWrapper.Defined.InvertBold;
			}

			this.buttonBold.Enable = enabled;
			this.buttonBold.ActiveState = state ? ActiveState.Yes : ActiveState.No;
		}

		protected void UpdateButtonItalic()
		{
			bool enabled = false;
			bool state   = false;

			if ( this.document != null && this.document.TextWrapper.IsAttached )
			{
				string face  = this.document.TextWrapper.Defined.FontFace;
				if ( face == null )
				{
					face = this.document.TextWrapper.Active.FontFace;
				}

				string style = this.document.TextWrapper.Defined.FontStyle;
				if ( style == null )
				{
					style = this.document.TextWrapper.Active.FontStyle;
				}

				OpenType.FontStyle italic = OpenType.FontStyle.Normal;
				if ( face != null && style != null )
				{
					OpenType.Font font = TextContext.GetFont(face, style);
					italic = font.FontIdentity.FontStyle;
				}

				enabled = true;
				state   = italic != OpenType.FontStyle.Normal;
				state  ^= this.document.TextWrapper.Defined.InvertItalic;
			}

			this.buttonItalic.Enable = enabled;
			this.buttonItalic.ActiveState = state ? ActiveState.Yes : ActiveState.No;
		}

		protected void UpdateButtonUnderlined()
		{
			bool enabled = false;
			bool state   = false;

			if ( this.document != null && this.document.TextWrapper.IsAttached )
			{
			}

			this.buttonUnderlined.Enable = enabled;
			this.buttonUnderlined.ActiveState = state ? ActiveState.Yes : ActiveState.No;
		}

		
		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonBold == null )  return;

			double dx = this.buttonBold.DefaultWidth;
			double dy = this.buttonBold.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Height = dy;
			rect.Width = dx;
			rect.Offset(0, dy+5);
			this.buttonBold.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonItalic.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonUnderlined.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonOverlined.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonStrikeout.Bounds = rect;
			rect.Offset(dx+5, 0);
			this.buttonStyle.Bounds = rect;
		}


		// Le wrapper associé a changé.
		private void HandleWrapperChanged(object sender)
		{
			this.UpdateButtonBold();
			this.UpdateButtonItalic();
			this.UpdateButtonUnderlined();
		}

		private void HandleButtonBoldClicked(object sender, MessageEventArgs e)
		{
			if ( !this.document.TextWrapper.IsAttached )  return;
			this.document.TextWrapper.Defined.InvertBold = !this.document.TextWrapper.Defined.InvertBold;
		}

		private void HandleButtonItalicClicked(object sender, MessageEventArgs e)
		{
			if ( !this.document.TextWrapper.IsAttached )  return;
			this.document.TextWrapper.Defined.InvertItalic = !this.document.TextWrapper.Defined.InvertItalic;
		}

		private void HandleButtonUnderlinedClicked(object sender, MessageEventArgs e)
		{
			if ( !this.document.TextWrapper.IsAttached )  return;
		}

		private void HandleButtonStyleClicked(object sender, MessageEventArgs e)
		{
			Objects.TextBox2 text = this.document.Modifier.RetEditObject() as Objects.TextBox2;
			if ( text != null )
			{
				TextFlow flow = text.TextFlow;
				
				Common.Text.TextStyle  style = this.document.TextContext.StyleList["Default", Common.Text.TextStyleClass.Paragraph];
				Common.Text.Property[] props = flow.TextNavigator.AccumulatedTextProperties;
				
				System.Collections.ArrayList list = new	System.Collections.ArrayList();
				
				foreach ( Common.Text.Property property in props )
				{
					if ( property.PropertyType != Common.Text.Properties.PropertyType.LocalSetting &&
						 property.WellKnownType != Common.Text.Properties.WellKnownType.Styles )
					{
						if ( property.WellKnownType == Common.Text.Properties.WellKnownType.Tabs )
						{
							list.Add(this.document.TextContext.TabList.PromoteToSharedTabs(property as Common.Text.Properties.TabsProperty));
						}
						else
						{
							list.Add(property);
						}
					}
				}
				
				props = (Common.Text.Property[]) list.ToArray(typeof(Common.Text.Property));
				
				this.document.TextContext.StyleList.RedefineTextStyle(flow.TextStory.OpletQueue, style, props);
				this.document.TextContext.StyleList.UpdateTextStyles();
				flow.TextNavigator.ExternalNotifyTextChanged();
			}
		}


		protected IconButton				buttonBold;
		protected IconButton				buttonItalic;
		protected IconButton				buttonUnderlined;
		protected IconButton				buttonOverlined;
		protected IconButton				buttonStrikeout;

		protected Button					buttonStyle;
	}
}
