using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe TextStyles permet de choisir un style de paragraphe ou de caract�re.
	/// </summary>
	[SuppressBundleSupport]
	public class TextStyles : Abstract
	{
		public TextStyles() : base()
		{
			this.title.Text = Res.Strings.Action.TextStylesMain;

			this.buttonParagraph = new IconButtonMark(this);
			//?this.buttonParagraph.Command = "TextEditing";  // (*)
			this.buttonParagraph.MarkDimension = 5;
			this.buttonParagraph.SiteMark = SiteMark.OnRight;
			this.buttonParagraph.IconName = Misc.Icon("TextFilterParagraph");
			this.buttonParagraph.PreferredIconSize = Misc.IconPreferredSize("Normal");
			this.buttonParagraph.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonParagraph.AutoFocus = false;
			this.buttonParagraph.TabIndex = this.tabIndex++;
			this.buttonParagraph.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.buttonParagraph.Clicked += new MessageEventHandler(this.HandleParagraphClicked);
			ToolTip.Default.SetToolTip(this.buttonParagraph, Res.Strings.Panel.Style.ParagraphDefinition);

			this.buttonCharacter = new IconButtonMark(this);
			//?this.buttonCharacter.Command = "TextEditing";  // (*)
			this.buttonCharacter.MarkDimension = 5;
			this.buttonCharacter.SiteMark = SiteMark.OnRight;
			this.buttonCharacter.IconName = Misc.Icon("TextFilterCharacter");
			this.buttonCharacter.PreferredIconSize = Misc.IconPreferredSize("Normal");
			this.buttonCharacter.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonCharacter.AutoFocus = false;
			this.buttonCharacter.TabIndex = this.tabIndex++;
			this.buttonCharacter.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.buttonCharacter.Clicked += new MessageEventHandler(this.HandleCharacterClicked);
			ToolTip.Default.SetToolTip(this.buttonCharacter, Res.Strings.Panel.Style.CharacterDefinition);

			this.comboStyle = this.CreateIconButtonsCombo("TextEditing");  // (*)
			this.comboStyle.SetColumnsAndRows(3, 1);
			//?this.comboStyle.SetColumnsAndRows(4, 2);
			this.comboStyle.MenuDrawFrame = true;
			this.comboStyle.AllLinesWidthSameWidth = true;
			this.comboStyle.AutoFocus = false;
			this.comboStyle.TabIndex = this.tabIndex++;
			this.comboStyle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.comboStyle.SelectedIndexChanged += new EventHandler(this.HandleSelectedIndexChanged);
			this.comboStyle.FirstIconChanged += new EventHandler(this.HandleFirstIconChanged);

			// (*)	Ce nom permet de griser automatiquement les widgets lorsqu'il n'y a
			//		pas de texte en �dition.

			this.UpdateClientGeometry();
			this.UpdateMode();
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

			if ( this.document == null )
			{
				this.buttonParagraph.Enable = false;
				this.buttonCharacter.Enable = false;

				this.comboStyle.SelectedName = null;
				this.comboStyle.Enable = false;
			}
			else
			{
				this.buttonParagraph.Enable = true;
				this.buttonCharacter.Enable = true;
				
				this.comboStyle.Enable = true;
				this.UpdateAfterTextStyleListChanged();
				this.UpdateSelectedStyle();
				
				this.ignoreChange = true;
				this.comboStyle.FirstIconVisible = this.RibbonStyleFirst;
				this.ignoreChange = false;
			}
		}

		public override void NotifyChanged(string changed)
		{
			//	Appel� lorsqu'un style � �t� cr��, supprim� ou d�plac�.
			if ( changed == "TextStyleListChanged" )
			{
				this.UpdateAfterTextStyleListChanged();
				this.UpdateSelectedStyle();
			}
		}

		public override void NotifyTextStylesChanged(System.Collections.ArrayList textStyleList)
		{
			//	Appel� lorsque les d�finitions d'un style ont chang�s, et que l'�chantillon
			//	correspondant doit �tre redessin�.
			this.document.Wrappers.ClearDynamicImageStyle();

			foreach ( Text.TextStyle textStyle in textStyleList )
			{
				int max = this.comboStyle.Columns*this.comboStyle.Rows;
				for ( int i=0 ; i<max ; i++ )
				{
					int rank = this.comboStyle.FirstIconVisible+i;
					if ( rank >= this.comboStyle.Items.Count )  break;

					IconButtonsCombo.Item item = this.comboStyle.Items[rank] as IconButtonsCombo.Item;
					if ( textStyle.Name == item.Name )
					{
						IconButton button = this.comboStyle.IconButton(i);
						button.Invalidate();
					}
				}
			}
		}

		public override void NotifyTextStylesChanged()
		{
			//	Appel� lorsque le style courant a �t� chang�, par exemple suite au
			//	d�placement du curseur.
			if ( this.document == null )  return;
			this.UpdateSelectedStyle();
		}


		public override double DefaultWidth
		{
			//	Retourne la largeur standard.
			get
			{
				return 5+20+5+60*3+5;
			}
		}


		protected void UpdateAfterTextStyleListChanged()
		{
			//	Met � jour la liste des styles.
			this.comboStyle.Items.Clear();

			Text.TextStyle[] styles = this.document.TextStyles(this.characterMode ? StyleCategory.Character : StyleCategory.Paragraph);
			string tt = this.characterMode ? Res.Strings.Action.Apply.Tooltip.StyleCharacter : Res.Strings.Action.Apply.Tooltip.StyleParagraph;
			foreach ( Text.TextStyle style in styles )
			{
				string name      = style.Name;
				string briefIcon = string.Concat(this.document.UniqueName, ".TextStyleBrief");
				string menuIcon  = string.Concat(this.document.UniqueName, ".TextStyleMenu");
				string parameter = string.Concat(style.Name, '\t', this.characterMode ? "Character" : "Paragraph");
				string tooltip   = string.Format(tt, Misc.UserTextStyleName(this.document.TextContext.StyleList.StyleMap.GetCaption(style)));
				this.AddIconButtonsComboDyn(this.comboStyle, name, briefIcon, menuIcon, parameter, tooltip);
			}

			this.comboStyle.UpdateButtons();
		}

		protected void UpdateSelectedStyle()
		{
			//	Met � jour le style s�lectionn� en fonction du texte en �dition.
			if ( this.document.Wrappers.TextFlow != null )
			{
				Text.TextStyle[] styles = this.document.Wrappers.TextFlow.TextNavigator.TextStyles;
				foreach ( Text.TextStyle style in styles )
				{
					if ( style.TextStyleClass == Common.Text.TextStyleClass.Paragraph && !this.characterMode )
					{
						this.ignoreChange = true;
						this.comboStyle.SelectedName = style.Name;
						this.ignoreChange = false;
					}

					if ( style.TextStyleClass == Common.Text.TextStyleClass.Text && this.characterMode )
					{
						this.ignoreChange = true;
						this.comboStyle.SelectedName = style.Name;
						this.ignoreChange = false;
					}
				}
			}
		}

		protected void UpdateMode()
		{
			//	Met � jour les 2 boutons pour choisir style de paragraphe ou de caract�re.
			this.buttonParagraph.ActiveState = this.characterMode ? ActiveState.No  : ActiveState.Yes;
			this.buttonCharacter.ActiveState = this.characterMode ? ActiveState.Yes : ActiveState.No;
		}


		protected int RibbonStyleFirst
		{
			//	Premi�re ic�ne visible, en fonction des boutons prev/next de IconButtonsCombo.
			get
			{
				if ( this.characterMode )  return this.document.Wrappers.RibbonCharacterStyleFirst;
				else                       return this.document.Wrappers.RibbonParagraphStyleFirst;
			}

			set
			{
				if ( this.characterMode )  this.document.Wrappers.RibbonCharacterStyleFirst = value;
				else                       this.document.Wrappers.RibbonParagraphStyleFirst = value;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.comboStyle == null )  return;

			Rectangle rect;
			double dx = this.buttonParagraph.DefaultWidth;
			double dy = this.buttonParagraph.DefaultHeight;

			rect = this.UsefulZone;
			rect.Width  = dx+5;
			rect.Height = dy;
			rect.Offset(0, 3);
			this.buttonCharacter.Bounds = rect;
			rect.Offset(0, rect.Height-1);
			this.buttonParagraph.Bounds = rect;

			rect = this.UsefulZone;
			rect.Left += dx+5;
			rect.Width = 60*3;
			this.comboStyle.Bounds = rect;
		}


		private void HandleParagraphClicked(object sender, MessageEventArgs e)
		{
			this.RibbonStyleFirst = this.comboStyle.FirstIconVisible;

			this.characterMode = false;
			this.UpdateMode();
			this.UpdateAfterTextStyleListChanged();
			this.UpdateSelectedStyle();

			this.comboStyle.FirstIconVisible = this.RibbonStyleFirst;
		}

		private void HandleCharacterClicked(object sender, MessageEventArgs e)
		{
			this.RibbonStyleFirst = this.comboStyle.FirstIconVisible;

			this.characterMode = true;
			this.UpdateMode();
			this.UpdateAfterTextStyleListChanged();
			this.UpdateSelectedStyle();

			this.comboStyle.FirstIconVisible = this.RibbonStyleFirst;
		}

		private void HandleSelectedIndexChanged(object sender)
		{
			if ( this.ignoreChange )  return;

			string name = this.comboStyle.SelectedName;
			if ( name == null )  return;

			Text.TextStyle style = this.document.TextContext.StyleList.GetTextStyle(name, this.characterMode ? Common.Text.TextStyleClass.Text : Common.Text.TextStyleClass.Paragraph);
			this.document.Modifier.SetTextStyle(style);
		}

		private void HandleFirstIconChanged(object sender)
		{
			if ( this.ignoreChange )  return;

			this.RibbonStyleFirst = this.comboStyle.FirstIconVisible;
		}


		protected IconButtonMark			buttonParagraph;
		protected IconButtonMark			buttonCharacter;
		protected IconButtonsCombo			comboStyle;
		protected bool						characterMode = false;
	}
}
