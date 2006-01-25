using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe TextStyles permet de choisir un style de paragraphe ou de caractère.
	/// </summary>
	[SuppressBundleSupport]
	public class TextStyles : Abstract
	{
		public TextStyles() : base()
		{
			this.title.Text = Res.Strings.Action.TextStylesMain;

			this.buttonParagraph = new IconButton(this);
			this.buttonParagraph.IconName = Misc.Icon("TextFilterParagraph");
			this.buttonParagraph.PreferredIconSize = Misc.IconPreferredSize("Normal");
			this.buttonParagraph.AutoFocus = false;
			this.buttonParagraph.TabIndex = this.tabIndex++;
			this.buttonParagraph.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.buttonParagraph.Clicked += new MessageEventHandler(this.HandleParagraphClicked);
			ToolTip.Default.SetToolTip(this.buttonParagraph, Res.Strings.Panel.Style.ParagraphDefinition);

			this.buttonCharacter = new IconButton(this);
			this.buttonCharacter.IconName = Misc.Icon("TextFilterCharacter");
			this.buttonCharacter.PreferredIconSize = Misc.IconPreferredSize("Normal");
			this.buttonCharacter.AutoFocus = false;
			this.buttonCharacter.TabIndex = this.tabIndex++;
			this.buttonCharacter.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.buttonCharacter.Clicked += new MessageEventHandler(this.HandleCharacterClicked);
			ToolTip.Default.SetToolTip(this.buttonCharacter, Res.Strings.Panel.Style.CharacterDefinition);

			this.styleParagraph = new Widgets.StyleCombo(this);
			this.styleParagraph.Command = "TextEditing";
			this.styleParagraph.StyleCategory = StyleCategory.Paragraph;
			this.styleParagraph.IsDeep = true;
			this.styleParagraph.IsReadOnly = true;
			this.styleParagraph.AutoFocus = false;
			this.styleParagraph.TabIndex = this.tabIndex++;
			this.styleParagraph.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.styleParagraph.ClosedCombo += new EventHandler(this.HandleStyleClosedCombo);
			ToolTip.Default.SetToolTip(this.styleParagraph, Res.Strings.Panel.Style.ParagraphChoice);

			this.styleCharacter = new Widgets.StyleCombo(this);
			this.styleCharacter.Command = "TextEditing";
			this.styleCharacter.StyleCategory = StyleCategory.Character;
			this.styleCharacter.IsDeep = true;
			this.styleCharacter.IsReadOnly = true;
			this.styleCharacter.AutoFocus = false;
			this.styleCharacter.TabIndex = this.tabIndex++;
			this.styleCharacter.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.styleCharacter.ClosedCombo += new EventHandler(this.HandleStyleClosedCombo);
			ToolTip.Default.SetToolTip(this.styleCharacter, Res.Strings.Panel.Style.CharacterChoice);

			this.UpdateClientGeometry();
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

				this.styleParagraph.Document = null;
				this.styleCharacter.Document = null;
			}
			else
			{
				this.buttonParagraph.Enable = true;
				this.buttonCharacter.Enable = true;
				
				this.styleParagraph.Document = this.document;
				this.styleCharacter.Document = this.document;
			}
		}

		public override void NotifyChanged(string changed)
		{
		}

		public override void NotifyTextStylesChanged()
		{
			string paragraph = "";
			string character = "";

			if ( this.document.Wrappers.TextFlow != null )
			{
				Text.TextStyle[] styles = this.document.Wrappers.TextFlow.TextNavigator.TextStyles;
				foreach ( Text.TextStyle style in styles )
				{
					string text = this.document.TextContext.StyleList.StyleMap.GetCaption(style);

					if ( style.TextStyleClass == Common.Text.TextStyleClass.Paragraph )
					{
						TextStyles.StyleAppend(ref paragraph, text);
					}

					if ( style.TextStyleClass == Common.Text.TextStyleClass.Text )
					{
						TextStyles.StyleAppend(ref character, text);
					}
				}
			}

			this.styleParagraph.Text = paragraph;
			this.styleCharacter.Text = character;
		}

		protected static void StyleAppend(ref string initial, string add)
		{
			if ( initial == "" )
			{
				initial = add;
			}
			else
			{
				initial = initial + " + " + add;
			}
		}


		public override double DefaultWidth
		{
			//	Retourne la largeur standard.
			get
			{
				return 10+22+135;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.styleParagraph == null )  return;

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
			rect.Left += dx;
			rect.Bottom += 28;
			rect.Height = 20;
			this.styleParagraph.Bounds = rect;

			rect = this.UsefulZone;
			rect.Left += dx;
			rect.Bottom += 1;
			rect.Height = 20;
			this.styleCharacter.Bounds = rect;
		}


		private void HandleParagraphClicked(object sender, MessageEventArgs e)
		{
			this.document.Notifier.NotifyBookPanelShowPage("Styles", "Paragraph");
		}

		private void HandleCharacterClicked(object sender, MessageEventArgs e)
		{
			this.document.Notifier.NotifyBookPanelShowPage("Styles", "Character");
		}

		private void HandleStyleClosedCombo(object sender)
		{
			//	Combo des styles fermé.
			Widgets.StyleCombo combo = sender as Widgets.StyleCombo;
			int sel = combo.SelectedIndex;
			if ( sel == -1 )  return;

			Common.Text.TextStyle[] styles = this.document.TextStyles(combo.StyleCategory);
			Common.Text.TextStyle style = styles[sel];
			this.document.Modifier.SetTextStyle(style);
		}


		protected IconButton				buttonParagraph;
		protected IconButton				buttonCharacter;
		protected Widgets.StyleCombo		styleParagraph;
		protected Widgets.StyleCombo		styleCharacter;
	}
}
