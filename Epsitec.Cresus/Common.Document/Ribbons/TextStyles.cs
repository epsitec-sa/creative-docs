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

			this.styleParagraph = new Widgets.StyleCombo(this);
			this.styleParagraph.StyleCategory = StyleCategory.Paragraph;
			this.styleParagraph.IsDeep = true;
			this.styleParagraph.IsReadOnly = true;
			this.styleParagraph.ClosedCombo += new EventHandler(this.HandleStyleClosedCombo);
			//?ToolTip.Default.SetToolTip(this.styleParagraph, Res.Strings.Container.Principal.Button.AggregateCombo);

			this.styleCharacter = new Widgets.StyleCombo(this);
			this.styleCharacter.StyleCategory = StyleCategory.Character;
			this.styleCharacter.IsDeep = true;
			this.styleCharacter.IsReadOnly = true;
			this.styleCharacter.ClosedCombo += new EventHandler(this.HandleStyleClosedCombo);
			//?ToolTip.Default.SetToolTip(this.styleCharacter, Res.Strings.Container.Principal.Button.AggregateCombo);

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
				this.styleParagraph.Enable = false;
				this.styleCharacter.Enable = false;
				this.styleParagraph.Document = null;
				this.styleCharacter.Document = null;
			}
			else
			{
				this.styleParagraph.Enable = true;
				this.styleCharacter.Enable = true;
				this.styleParagraph.Document = this.document;
				this.styleCharacter.Document = this.document;
			}
		}

		public override void NotifyChanged(string changed)
		{
		}


		public override double DefaultWidth
		{
			//	Retourne la largeur standard.
			get
			{
				return 135+10;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.styleParagraph == null )  return;

			Rectangle rect = this.UsefulZone;
			rect.Bottom += 28;
			rect.Height = 20;
			this.styleParagraph.Bounds = rect;

			rect = this.UsefulZone;
			rect.Bottom += 1;
			rect.Height = 20;
			this.styleCharacter.Bounds = rect;
		}


		private void HandleStyleClosedCombo(object sender)
		{
			//	Combo des styles fermé.
			Widgets.StyleCombo combo = sender as Widgets.StyleCombo;
			int sel = combo.SelectedIndex;
			if ( sel == -1 )  return;

			Common.Text.TextStyle[] styles = this.document.TextStyles(combo.StyleCategory);
			Common.Text.TextStyle style = styles[sel];
			//	TODO: modifier le style du texte...
		}

		
		protected Widgets.StyleCombo		styleParagraph;
		protected Widgets.StyleCombo		styleCharacter;
	}
}
