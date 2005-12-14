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

			this.buttonBold          = this.CreateIconButton("FontBold",       Misc.Icon("FontBold"),       Res.Strings.Action.Text.Font.Bold,       true);
			this.buttonItalic        = this.CreateIconButton("FontItalic",     Misc.Icon("FontItalic"),     Res.Strings.Action.Text.Font.Italic,     true);
			this.buttonFontSizeMinus = this.CreateIconButton("FontSizeMinus",  Misc.Icon("FontSizeMinus"),  Res.Strings.Action.Text.Font.SizeMinus,  false);
			this.buttonFontSizePlus  = this.CreateIconButton("FontSizePlus",   Misc.Icon("FontSizePlus"),   Res.Strings.Action.Text.Font.SizePlus,   false);
			this.buttonUnderlined    = this.CreateIconButton("FontUnderlined", Misc.Icon("FontUnderlined"), Res.Strings.Action.Text.Font.Underlined, true);
			this.buttonOverlined     = this.CreateIconButton("FontOverlined",  Misc.Icon("FontOverlined"),  Res.Strings.Action.Text.Font.Overlined,  true);
			this.buttonStrikeout     = this.CreateIconButton("FontStrikeout",  Misc.Icon("FontStrikeout"),  Res.Strings.Action.Text.Font.Strikeout,  true);

			this.buttonClear = this.CreateIconButton("FontClear", Misc.Icon("Nothing"), Res.Strings.Action.Text.Font.Clear, false);

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


		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 200;
			}
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
			rect.Offset(dx+5, 0);
			this.buttonFontSizeMinus.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonFontSizePlus.Bounds = rect;

			rect = this.UsefulZone;
			rect.Height = dy;
			rect.Width = dx;
			this.buttonUnderlined.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonOverlined.Bounds = rect;
			rect.Offset(dx+5, 0);
			this.buttonStrikeout.Bounds = rect;
			rect.Offset(dx+20, 0);
			this.buttonClear.Bounds = rect;

			rect.Offset(dx+50, 0);
			this.buttonStyle.Bounds = rect;
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


		protected IconButton				buttonFontSizeMinus;
		protected IconButton				buttonFontSizePlus;
		protected IconButton				buttonBold;
		protected IconButton				buttonItalic;
		protected IconButton				buttonUnderlined;
		protected IconButton				buttonOverlined;
		protected IconButton				buttonStrikeout;
		protected IconButton				buttonClear;

		protected Button					buttonStyle;
	}
}
