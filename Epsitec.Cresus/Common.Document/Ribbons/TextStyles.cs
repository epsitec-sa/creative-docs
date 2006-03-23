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

			this.comboParagraph = this.CreateIconButtonCombo("TextEditing");  // (*)
			this.comboParagraph.ComboOpening += new CancelEventHandler(this.HandleParagraphOpening);
			this.comboParagraph.ComboClosed += new EventHandler(this.HandleParagraphClosed);
			ToolTip.Default.SetToolTip(this.comboParagraph, Res.Strings.Panel.Style.ParagraphChoice);

			this.comboCharacter = this.CreateIconButtonCombo("TextEditing");  // (*)
			this.comboCharacter.ComboOpening += new CancelEventHandler(this.HandleCharacterOpening);
			this.comboCharacter.ComboClosed += new EventHandler(this.HandleCharacterClosed);
			ToolTip.Default.SetToolTip(this.comboCharacter, Res.Strings.Panel.Style.CharacterChoice);

			// (*)	Ce nom permet de griser automatiquement les widgets lorsqu'il n'y a
			//		pas de texte en édition.

			this.UpdateClientGeometry();
		}
		
		private void DynamicImageXyz(Drawing.Graphics graphics, Drawing.Size size, string argument, Drawing.GlyphPaintStyle style, Drawing.Color color, object adorner)
		{
			//	Méthode de test pour peindre une image dynamique selon un
			//	modèle nommé "Xyz"; l'argument reçu en entrée permet de
			//	déterminer exactement ce qui doit être peint.
			
			int    hue; 
			double saturation = (style == Drawing.GlyphPaintStyle.Disabled) ? 0.2 : 1.0;
			double value      = (style == Drawing.GlyphPaintStyle.Disabled) ? 0.7 : 1.0;
			
			if (argument == "random")
			{
				System.Random random = new System.Random ();
				hue = random.Next (360);
			}
			else
			{
				hue = int.Parse (argument);
			}
			
			graphics.AddFilledRectangle (0, 0, size.Width, size.Height);
			graphics.RenderSolid (Drawing.Color.FromHsv (hue, saturation, value));
			graphics.LineWidth = 2.0;
			graphics.AddRectangle (1, 1, size.Width-2, size.Height-2);
			graphics.RenderSolid (Drawing.Color.FromBrightness (0));
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
				this.comboParagraph.SelectedName = null;
				this.comboCharacter.SelectedName = null;

				this.comboParagraph.Enable = false;
				this.comboCharacter.Enable = false;
			}
			else
			{
				this.comboParagraph.Enable = true;
				this.comboCharacter.Enable = true;
			}
		}

		public override void NotifyChanged(string changed)
		{
		}

		public override void NotifyTextStylesChanged()
		{
			if ( this.document.Wrappers.TextFlow != null )
			{
				Text.TextStyle[] styles = this.document.Wrappers.TextFlow.TextNavigator.TextStyles;
				foreach ( Text.TextStyle style in styles )
				{
					string text = this.document.TextContext.StyleList.StyleMap.GetCaption(style);

					if ( style.TextStyleClass == Common.Text.TextStyleClass.Paragraph )
					{
						string briefIcon = string.Concat(this.document.UniqueName, ".TextStyleBrief");
						string parameter = string.Concat(style.Name, ".Paragraph");
						this.BriefIconButtonComboDyn(this.comboParagraph, briefIcon, parameter);
					}

					if ( style.TextStyleClass == Common.Text.TextStyleClass.Text )
					{
						string briefIcon = string.Concat(this.document.UniqueName, ".TextStyleBrief");
						string parameter = string.Concat(style.Name, ".Character");
						this.BriefIconButtonComboDyn(this.comboCharacter, briefIcon, parameter);
					}
				}
			}
		}


		public override double DefaultWidth
		{
			//	Retourne la largeur standard.
			get
			{
				return 5+70+5+70+5;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.comboParagraph == null )  return;

			Rectangle rect = this.UsefulZone;
			rect.Width = 70;
			this.comboParagraph.Bounds = rect;
			rect.Offset(75, 0);
			this.comboCharacter.Bounds = rect;
		}


		private void HandleParagraphOpening(object sender, CancelEventArgs e)
		{
			this.comboParagraph.Items.Clear();

			Text.TextStyle[] styles = this.document.TextStyles(StyleCategory.Paragraph);
			foreach ( Text.TextStyle style in styles )
			{
				string name      = style.Name;
				string briefIcon = string.Concat(this.document.UniqueName, ".TextStyleBrief");
				string menuIcon  = string.Concat(this.document.UniqueName, ".TextStyleMenu");
				string parameter = string.Concat(style.Name, ".Paragraph");
				this.AddIconButtonComboDyn(this.comboParagraph, name, briefIcon, menuIcon, parameter);
			}
		}

		private void HandleParagraphClosed(object sender)
		{
			IconButtonCombo combo = sender as IconButtonCombo;
			string name = combo.SelectedName;
			if ( name == null )  return;

			Text.TextStyle style = this.document.TextContext.StyleList.GetTextStyle(name, Common.Text.TextStyleClass.Paragraph);
			this.document.Modifier.SetTextStyle(style);
		}

		private void HandleCharacterOpening(object sender, CancelEventArgs e)
		{
			this.comboCharacter.Items.Clear();

			Text.TextStyle[] styles = this.document.TextStyles(StyleCategory.Character);
			foreach ( Text.TextStyle style in styles )
			{
				string name      = style.Name;
				string briefIcon = string.Concat(this.document.UniqueName, ".TextStyleBrief");
				string menuIcon  = string.Concat(this.document.UniqueName, ".TextStyleMenu");
				string parameter = string.Concat(style.Name, ".Character");
				this.AddIconButtonComboDyn(this.comboCharacter, name, briefIcon, menuIcon, parameter);
			}
		}

		private void HandleCharacterClosed(object sender)
		{
			IconButtonCombo combo = sender as IconButtonCombo;
			string name = combo.SelectedName;
			if ( name == null )  return;

			Text.TextStyle style = this.document.TextContext.StyleList.GetTextStyle(name, Common.Text.TextStyleClass.Text);
			this.document.Modifier.SetTextStyle(style);
		}


		protected IconButtonCombo			comboParagraph;
		protected IconButtonCombo			comboCharacter;
	}
}
