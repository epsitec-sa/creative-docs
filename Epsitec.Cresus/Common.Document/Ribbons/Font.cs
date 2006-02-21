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

			//	Création dans l'ordre inverse, pour que les cadres soient plus jolis
			//	lorsqu'un bouton est disable.
			this.buttonQuick4        = this.CreateButtonFontFace("FontQuick4");
			this.buttonQuick3        = this.CreateButtonFontFace("FontQuick3");
			this.buttonQuick2        = this.CreateButtonFontFace("FontQuick2");
			this.buttonQuick1        = this.CreateButtonFontFace("FontQuick1");

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

			this.buttonStyle = new Button(this);
			this.buttonStyle.Text = "S";
			this.buttonStyle.Clicked += new MessageEventHandler(this.HandleButtonStyleClicked);

			this.buttonMW = new Button(this);
			this.buttonMW.Text = "MW";
			this.buttonMW.Clicked += new MessageEventHandler(this.HandleButtonMWClicked);

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


		public override double DefaultWidth
		{
			//	Retourne la largeur standard.
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


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
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

			rect.Offset(dx+10, 0);
			this.buttonStyle.Bounds = rect;
			rect.Offset(0, dy+5);
			this.buttonMW.Bounds = rect;
		}


		protected void UpdateQuickFonts()
		{
			//	Met à jour les noms des polices rapides.
			this.UpdateQuickButton(this.buttonQuick1, 0);
			this.UpdateQuickButton(this.buttonQuick2, 1);
			this.UpdateQuickButton(this.buttonQuick3, 2);
			this.UpdateQuickButton(this.buttonQuick4, 3);

			if ( this.document != null )
			{
				this.document.Wrappers.UpdateQuickButtons();
			}
		}

		protected void UpdateQuickButton(Widgets.ButtonFontFace button, int i)
		{
			//	Met à jour un bouton pour une police rapide.
			if ( this.document == null )
			{
				button.FontIdentity = null;
			}
			else
			{
				OpenType.FontIdentity id = this.document.Wrappers.GetQuickFonts(i);
				button.FontIdentity = id;
				if ( id != null )
				{
					ToolTip.Default.SetToolTip(button, id.InvariantFaceName);
				}
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

		private void HandleButtonMWClicked(object sender, MessageEventArgs e)
		{
			Objects.AbstractText text = this.document.Modifier.RetEditObject() as Objects.AbstractText;
			if ( text != null )
			{
				TextFlow flow = text.TextFlow;
				Text.TextStory story = flow.TextStory;
				Text.TextNavigator navigator = flow.TextNavigator;
				
				Epsitec.Common.Text.Exchange.Rosetta.TestCode(story, navigator);
			}
		}


		protected Widgets.ButtonFontFace CreateButtonFontFace(string command)
		{
			//	Crée un bouton pour une police.
			CommandState cs = CommandDispatcher.GetFocusedPrimaryDispatcher().GetCommandState(command);
			Widgets.ButtonFontFace button = new Widgets.ButtonFontFace(this);

			button.Command = command;
			button.AutoFocus = false;

			if ( cs.Statefull )
			{
				button.ButtonStyle = ButtonStyle.ActivableIcon;
			}

			button.TabIndex = this.tabIndex++;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			return button;
		}


		protected IconButton				buttonFontSizeMinus;
		protected IconButton				buttonFontSizePlus;
		protected IconButton				buttonShowControl;
		protected Widgets.ButtonFontFace	buttonQuick1;
		protected Widgets.ButtonFontFace	buttonQuick2;
		protected Widgets.ButtonFontFace	buttonQuick3;
		protected Widgets.ButtonFontFace	buttonQuick4;
		protected IconButton				buttonBold;
		protected IconButton				buttonItalic;
		protected IconButton				buttonUnderlined;
		protected IconButton				buttonOverlined;
		protected IconButton				buttonStrikeout;
		protected IconButton				buttonSubscript;
		protected IconButton				buttonSuperscript;
		protected IconButton				buttonClear;
		protected Button					buttonStyle;
		protected Button					buttonMW;
	}
}
