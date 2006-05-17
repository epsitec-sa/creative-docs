using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Font permet de choisir la fonte du texte.
	/// </summary>
	public class Font : Abstract
	{
		public Font() : base()
		{
			this.Title = Res.Strings.Action.FontMain;
			this.PreferredWidth = 240;

			this.comboFont = this.CreateIconButtonsCombo("TextEditing");
			this.comboFont.SetColumnsAndRows(2, 2);
			this.comboFont.MenuDrawFrame = true;
			this.comboFont.AllLinesWidthSameWidth = true;
			this.comboFont.AutoFocus = false;
			this.comboFont.TabIndex = this.tabIndex++;
			this.comboFont.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.comboFont.SelectedIndexChanged += new EventHandler(this.HandleSelectedIndexChanged);
			this.comboFont.FirstIconChanged += new EventHandler(this.HandleFirstIconChanged);

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

//			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		public override void SetDocument(DocumentType type, InstallType install, DebugMode debug, Settings.GlobalSettings gs, Document document)
		{
			//	Indique quel est le document actif, puisque les rubans sont globaux � l'application.
			base.SetDocument(type, install, debug, gs, document);

			if (this.debugMode == DebugMode.DebugCommands)
			{
				this.PreferredWidth = 240+40;
			}

			if (this.document == null)
			{
				this.comboFont.SelectedName = null;
				this.comboFont.Enable = false;
				this.comboFont.Items.Clear();
				this.comboFont.UpdateButtons();
			}
			else
			{
				this.comboFont.Enable = true;
				this.UpdateAfterFontListChanged();
				this.UpdateSelectedFont();
				
				this.ignoreChange = true;
				this.comboFont.FirstIconVisible = this.document.Wrappers.RibbonFontFirst;
				this.ignoreChange = false;
			}
		}


		public override void NotifyChanged(string changed)
		{
			if ( changed == "FontsSettingsChanged" )
			{
				this.UpdateAfterFontListChanged();
				this.UpdateSelectedFont();
			}
		}

		public override void NotifyTextStylesChanged()
		{
			//	Appel� lorsque le style courant a �t� chang�, par exemple suite au
			//	d�placement du curseur.
			if ( this.document == null )  return;
			this.UpdateSelectedFont();
		}


		protected void UpdateAfterFontListChanged()
		{
			//	Met � jour la liste des styles.
			if ( this.document == null )  return;
			this.document.Wrappers.FontFaceComboUpdate(this.comboFont);
			this.comboFont.UpdateButtons();
		}

		protected void UpdateSelectedFont()
		{
			//	Met � jour la police s�lectionn�e en fonction du texte en �dition.
			if ( this.document == null )  return;
			if ( this.document.Wrappers.TextFlow == null )  return;

			string face = this.document.Wrappers.TextWrapper.Active.FontFace;
			this.ignoreChange = true;
			this.comboFont.SelectedName = face;
			this.ignoreChange = false;
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.buttonBold == null )  return;

			Rectangle rect;
			double dx = this.buttonBold.PreferredWidth;
			double dy = this.buttonBold.PreferredHeight;

			rect = this.UsefulZone;
			rect.Width = 75;
			this.comboFont.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Height = dy;
			rect.Width = dx;
			rect.Offset(80, dy+5);
			this.buttonBold.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonItalic.SetManualBounds(rect);
			rect.Offset(dx+5, 0);
			this.buttonFontSizeMinus.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonFontSizePlus.SetManualBounds(rect);
			rect.Offset(dx+35, 0);
			this.buttonShowControl.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Height = dy;
			rect.Width = dx;
			rect.Offset(80, 0);
			this.buttonUnderlined.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonOverlined.SetManualBounds(rect);
			rect.Offset(dx+5, 0);
			this.buttonStrikeout.SetManualBounds(rect);
			rect.Offset(dx+5, 0);
			this.buttonSubscript.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonSuperscript.SetManualBounds(rect);
			rect.Offset(dx+10, 0);
			this.buttonClear.SetManualBounds(rect);

			rect.Offset(dx+10, 0);
			this.buttonStyle.SetManualBounds(rect);
			rect.Offset(0, dy+5);
			this.buttonMW.SetManualBounds(rect);
		}


		private void HandleSelectedIndexChanged(object sender)
		{
			if ( this.ignoreChange )  return;

			string face = this.comboFont.SelectedName;
			if ( face == null )  return;

			this.document.Wrappers.TextWrapper.SuspendSynchronizations();
			this.document.Wrappers.TextWrapper.Defined.FontFace  = face;
			this.document.Wrappers.TextWrapper.Defined.FontStyle = Misc.DefaultFontStyle(face);
			this.document.Wrappers.TextWrapper.DefineOperationName("QuickFont", face);
			this.document.Wrappers.TextWrapper.ResumeSynchronizations();
		}

		private void HandleFirstIconChanged(object sender)
		{
			if ( this.ignoreChange )  return;

			this.document.Wrappers.RibbonFontFirst = this.comboFont.FirstIconVisible;
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
			//	Cr�e un bouton pour une police.
			CommandState cs = CommandState.Get (command);
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


		protected IconButtonsCombo			comboFont;
		protected IconButton				buttonFontSizeMinus;
		protected IconButton				buttonFontSizePlus;
		protected IconButton				buttonShowControl;
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
