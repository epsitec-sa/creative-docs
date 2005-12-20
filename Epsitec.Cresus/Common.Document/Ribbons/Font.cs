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
			this.title.Text = Res.Strings.Action.FontMain;

			this.buttonQuick1        = this.CreateIconButton("FontQuick1");
			this.buttonQuick2        = this.CreateIconButton("FontQuick2");
			this.buttonQuick3        = this.CreateIconButton("FontQuick3");
			this.buttonQuick4        = this.CreateIconButton("FontQuick4");
			this.buttonBold          = this.CreateIconButton("FontBold");
			this.buttonItalic        = this.CreateIconButton("FontItalic");
			this.buttonFontSizeMinus = this.CreateIconButton("FontSizeMinus");
			this.buttonFontSizePlus  = this.CreateIconButton("FontSizePlus");
			this.buttonShowControl   = this.CreateIconButton("TextShowControlCharacters");
			this.buttonUnderlined    = this.CreateIconButton("FontUnderlined");
			this.buttonOverlined     = this.CreateIconButton("FontOverlined");
			this.buttonStrikeout     = this.CreateIconButton("FontStrikeout");
			this.buttonSubscript     = this.CreateIconButton("FontSubscript");
			this.buttonSuperscript   = this.CreateIconButton("FontSuperscript");
			this.buttonClear         = this.CreateIconButton("FontClear");

			if ( this.debugMode == DebugMode.DebugCommands )
			{
				this.buttonStyle = new Button(this);
				this.buttonStyle.Text = "S";
				this.buttonStyle.Clicked += new MessageEventHandler(this.HandleButtonStyleClicked);
			}

			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		public override void NotifyChanged(string changed)
		{
			if ( changed == "FontsSettingsChanged" )
			{
				this.UpdateQuickFonts();
			}
		}


		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				if ( this.debugMode == DebugMode.DebugCommands )
				{
					return 200+76;
				}
				else
				{
					return 160+76;
				}
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
			rect.Width  = 33;
			rect.Height = 25;
			rect.Offset(0, 25-1);
			this.buttonQuick1.Bounds = rect;
			rect.Offset(33, 0);
			this.buttonQuick2.Bounds = rect;
			rect = this.UsefulZone;
			rect.Width  = 33;
			rect.Height = 25;
			rect.Offset(0, 0);
			this.buttonQuick3.Bounds = rect;
			rect.Offset(33, 0);
			this.buttonQuick4.Bounds = rect;

			rect = this.UsefulZone;
			rect.Height = dy;
			rect.Width = dx;
			rect.Offset(dx*1.5*2+10, dy+5);
			this.buttonBold.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonItalic.Bounds = rect;
			rect.Offset(dx+5, 0);
			this.buttonFontSizeMinus.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonFontSizePlus.Bounds = rect;
			rect.Offset(dx+35, 0);
			this.buttonShowControl.Bounds = rect;

			rect = this.UsefulZone;
			rect.Height = dy;
			rect.Width = dx;
			rect.Offset(dx*1.5*2+10, 0);
			this.buttonUnderlined.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonOverlined.Bounds = rect;
			rect.Offset(dx+5, 0);
			this.buttonStrikeout.Bounds = rect;
			rect.Offset(dx+5, 0);
			this.buttonSubscript.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonSuperscript.Bounds = rect;
			rect.Offset(dx+10, 0);
			this.buttonClear.Bounds = rect;

			if ( this.debugMode == DebugMode.DebugCommands )
			{
				rect.Offset(dx+10, 0);
				this.buttonStyle.Bounds = rect;
			}
		}


		// Met à jour les noms des polices rapides.
		protected void UpdateQuickFonts()
		{
			this.UpdateQuickButton(this.buttonQuick1, 0);
			this.UpdateQuickButton(this.buttonQuick2, 1);
			this.UpdateQuickButton(this.buttonQuick3, 2);
			this.UpdateQuickButton(this.buttonQuick4, 3);

			if ( this.document != null )
			{
				this.document.Wrappers.UpdateQuickButtons();
			}
		}

		// Met à jour un bouton pour une police rapide.
		protected void UpdateQuickButton(IconButton button, int i)
		{
			// Cherche l'identificateur de la police pour ce bouton.
			OpenType.FontIdentity id = null;
			if ( this.document != null )
			{
				id = this.document.Wrappers.GetQuickFonts(i);
			}

			// Si le bouton a déjà l'échantillon pour la bonne police, ne fait rien.
			if ( id != null )
			{
				Widgets.FontSample current = button.FindChild("Sample") as Widgets.FontSample;
				if ( current != null )
				{
					if ( current.FontIdentity.InvariantFaceName == id.InvariantFaceName )  return;
				}
			}

			// Supprime l'échantillon dans le bouton.
			foreach ( Widget widget in button.Children.Widgets )
			{
				widget.Dispose();
			}

			// Crée le nouvel échantillon pour le bouton.
			if ( this.document != null && id != null )
			{
				Widgets.FontSample sample = new Widgets.FontSample(button);
				sample.Name = "Sample";
				sample.FontIdentity = id;
				sample.SampleAbc = true;
				sample.Dock = DockStyle.Fill;
				sample.DockMargins = new Margins(0, 0, 3, 2);

				ToolTip.Default.SetToolTip(button, id.InvariantFaceName);
			}
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
		protected IconButton				buttonShowControl;
		protected IconButton				buttonQuick1;
		protected IconButton				buttonQuick2;
		protected IconButton				buttonQuick3;
		protected IconButton				buttonQuick4;
		protected IconButton				buttonBold;
		protected IconButton				buttonItalic;
		protected IconButton				buttonUnderlined;
		protected IconButton				buttonOverlined;
		protected IconButton				buttonStrikeout;
		protected IconButton				buttonSubscript;
		protected IconButton				buttonSuperscript;
		protected IconButton				buttonClear;
		protected Button					buttonStyle;
	}
}
