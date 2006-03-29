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

			this.buttonParagraph = new IconButton(this);
			//?this.buttonParagraph.Command = "TextEditing";  // (*)
			this.buttonParagraph.IconName = Misc.Icon("TextFilterParagraph");
			this.buttonParagraph.PreferredIconSize = Misc.IconPreferredSize("Normal");
			this.buttonParagraph.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonParagraph.AutoFocus = false;
			this.buttonParagraph.TabIndex = this.tabIndex++;
			this.buttonParagraph.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.buttonParagraph.Clicked += new MessageEventHandler(this.HandleParagraphClicked);
			ToolTip.Default.SetToolTip(this.buttonParagraph, Res.Strings.Panel.Style.ParagraphDefinition);

			this.buttonCharacter = new IconButton(this);
			//?this.buttonCharacter.Command = "TextEditing";  // (*)
			this.buttonCharacter.IconName = Misc.Icon("TextFilterCharacter");
			this.buttonCharacter.PreferredIconSize = Misc.IconPreferredSize("Normal");
			this.buttonCharacter.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonCharacter.AutoFocus = false;
			this.buttonCharacter.TabIndex = this.tabIndex++;
			this.buttonCharacter.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.buttonCharacter.Clicked += new MessageEventHandler(this.HandleCharacterClicked);
			ToolTip.Default.SetToolTip(this.buttonCharacter, Res.Strings.Panel.Style.CharacterDefinition);

			this.comboStyle = this.CreateIconButtonsCombo("TextEditing");  // (*)
			this.comboStyle.TotalButtons = 3;
			this.comboStyle.MenuDrawFrame = true;
			this.comboStyle.AllLinesWidthSameWidth = true;
			this.comboStyle.SelectedIndexChanged += new EventHandler(this.HandleParagraphSelected);
			ToolTip.Default.SetToolTip(this.comboStyle, Res.Strings.Panel.Style.Choice);

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
			}
		}

		protected void UpdateAfterTextStyleListChanged()
		{
			this.comboStyle.Items.Clear();

			Text.TextStyle[] styles = this.document.TextStyles(this.characterMode ? StyleCategory.Character : StyleCategory.Paragraph);
			foreach ( Text.TextStyle style in styles )
			{
				string name      = style.Name;
				string briefIcon = string.Concat(this.document.UniqueName, ".TextStyleBrief");
				string menuIcon  = string.Concat(this.document.UniqueName, ".TextStyleMenu");
				string parameter = string.Concat(style.Name, '\t', this.characterMode ? "Character" : "Paragraph");
				this.AddIconButtonsComboDyn(this.comboStyle, name, briefIcon, menuIcon, parameter);
			}

			this.comboStyle.UpdateButtons();
		}

		public override void NotifyChanged(string changed)
		{
			if ( changed == "TextStyleListChanged" )
			{
				this.UpdateAfterTextStyleListChanged();
			}
		}

		public override void NotifyTextStylesChanged(System.Collections.ArrayList textStyleList)
		{
			foreach ( Text.TextStyle textStyle in textStyleList )
			{
				for ( int i=0 ; i<this.comboStyle.TotalButtons ; i++ )
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
			if ( this.document == null )  return;

			if ( this.document.Wrappers.TextFlow != null )
			{
				Text.TextStyle[] styles = this.document.Wrappers.TextFlow.TextNavigator.TextStyles;
				foreach ( Text.TextStyle style in styles )
				{
					if ( style.TextStyleClass == Common.Text.TextStyleClass.Paragraph && !this.characterMode )
					{
						this.comboStyle.SelectedName = style.Name;
					}

					if ( style.TextStyleClass == Common.Text.TextStyleClass.Text && this.characterMode )
					{
						this.comboStyle.SelectedName = style.Name;
					}
				}
			}
		}


		public override double DefaultWidth
		{
			//	Retourne la largeur standard.
			get
			{
				return 5+20+5+60*3+5;
			}
		}


		protected void UpdateMode()
		{
			this.buttonParagraph.ActiveState = this.characterMode ? ActiveState.No  : ActiveState.Yes;
			this.buttonCharacter.ActiveState = this.characterMode ? ActiveState.Yes : ActiveState.No;
			//?this.NotifyTextStylesChanged();
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
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonParagraph.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonCharacter.Bounds = rect;

			rect = this.UsefulZone;
			rect.Left += dx+5;
			rect.Width = 60*3;
			this.comboStyle.Bounds = rect;
		}


		private void HandleParagraphClicked(object sender, MessageEventArgs e)
		{
			this.characterMode = false;
			this.UpdateMode();
			this.UpdateAfterTextStyleListChanged();
		}

		private void HandleCharacterClicked(object sender, MessageEventArgs e)
		{
			this.characterMode = true;
			this.UpdateMode();
			this.UpdateAfterTextStyleListChanged();
		}

		private void HandleParagraphSelected(object sender)
		{
			IconButtonsCombo combo = sender as IconButtonsCombo;
			string name = combo.SelectedName;
			if ( name == null )  return;

			Text.TextStyle style = this.document.TextContext.StyleList.GetTextStyle(name, this.characterMode ? Common.Text.TextStyleClass.Text : Common.Text.TextStyleClass.Paragraph);
			this.document.Modifier.SetTextStyle(style);
		}


		protected IconButton				buttonParagraph;
		protected IconButton				buttonCharacter;
		protected IconButtonsCombo			comboStyle;
		protected bool						characterMode = false;
	}
}
