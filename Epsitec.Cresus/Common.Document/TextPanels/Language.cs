using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Language permet de choisir la langue.
	/// </summary>
	[SuppressBundleSupport]
	public class Language : Abstract
	{
		public Language(Document document) : base(document)
		{
			this.label.Text = Res.Strings.TextPanel.Language.Title;

			this.fixIcon.Text = Misc.Image("TextLanguage");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Font.Title);

			this.fieldLanguage = new TextFieldCombo(this);
			this.fieldLanguage.IsReadOnly = true;
			this.fieldLanguage.TextChanged += new EventHandler(this.HandleLanguageChanged);
			this.fieldLanguage.TabIndex = this.tabIndex++;
			this.fieldLanguage.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldLanguage, Res.Strings.TextPanel.Language.Tooltip.Language);

			this.buttonHyphen = this.CreateIconButton(Misc.Icon("TextHyphen"), Res.Strings.Action.Text.Paragraph.Hyphen, new MessageEventHandler(this.HandleHyphenClicked));

			this.buttonClear = this.CreateClearButton(new MessageEventHandler(this.HandleClearClicked));

			this.document.TextWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
			this.document.TextWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldLanguage.TextChanged -= new EventHandler(this.HandleLanguageChanged);

				this.document.TextWrapper.Active.Changed  -= new EventHandler(this.HandleWrapperChanged);
				this.document.TextWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);

				this.fieldLanguage = null;
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return this.LabelHeight+30;
			}
		}

		
		// Le wrapper associé a changé.
		protected void HandleWrapperChanged(object sender)
		{
			this.UpdateAfterChanging();
		}

		
		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonHyphen == null )  return;

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;

			r.Left = rect.Left;
			r.Width = 100;
			this.fieldLanguage.Bounds = r;

			r.Offset(105, 0);
			r.Width = 20;
			this.buttonHyphen.Bounds = r;

			r.Left = rect.Right-20;
			r.Width = 20;
			this.buttonClear.Bounds = r;
		}


		// Met à jour la liste des langues.
		protected void UpdateComboLanguage()
		{
			if ( this.fieldLanguage.Items.Count == 0 )
			{
				this.fieldLanguage.Items.Add(Res.Strings.TextPanel.Language.List.None);
				this.fieldLanguage.Items.Add(Res.Strings.TextPanel.Language.List.fr);
				this.fieldLanguage.Items.Add(Res.Strings.TextPanel.Language.List.de);
				this.fieldLanguage.Items.Add(Res.Strings.TextPanel.Language.List.en);
			}
		}

		protected static string LanguageShortToLong(string shortLanguage)
		{
			if ( shortLanguage.StartsWith("fr") )  return Res.Strings.TextPanel.Language.List.fr;
			if ( shortLanguage.StartsWith("de") )  return Res.Strings.TextPanel.Language.List.de;
			if ( shortLanguage.StartsWith("en") )  return Res.Strings.TextPanel.Language.List.en;
			
			return Res.Strings.TextPanel.Language.List.None;
		}

		protected static string LanguageLongToShort(string longLanguage)
		{
			if ( longLanguage == Res.Strings.TextPanel.Language.List.fr )  return "fr";
			if ( longLanguage == Res.Strings.TextPanel.Language.List.de )  return "de";
			if ( longLanguage == Res.Strings.TextPanel.Language.List.en )  return "en";
			
			return "xx";
		}


		// Met à jour après un changement du wrapper.
		protected override void UpdateAfterChanging()
		{
			base.UpdateAfterChanging();

			bool hyphen = false;
			bool isHyphen = false;
			if ( this.document.TextWrapper.Defined.IsLanguageHyphenationDefined )
			{
				hyphen = (this.document.TextWrapper.Active.LanguageHyphenation == 1.0);
				isHyphen = true;
			}

			string language = Language.LanguageShortToLong(this.document.TextWrapper.Active.LanguageLocale);
			bool isLanguage = this.document.TextWrapper.Defined.IsLanguageLocaleDefined;

			this.ignoreChanged = true;
			
			this.ActiveIconButton(this.buttonHyphen, hyphen, isHyphen);
			
			this.UpdateComboLanguage();
			this.fieldLanguage.Text = language;
			this.ProposalTextFieldCombo(this.fieldLanguage, !isLanguage);

			this.ignoreChanged = false;
		}


		private void HandleHyphenClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;

			this.buttonHyphen.ActiveState = (this.buttonHyphen.ActiveState == ActiveState.Yes) ? ActiveState.No : ActiveState.Yes;
			bool hyphen = (this.buttonHyphen.ActiveState == ActiveState.Yes);

			if ( hyphen )
			{
				this.document.TextWrapper.Defined.LanguageHyphenation = 1.0;
			}
			else
			{
				this.document.TextWrapper.Defined.LanguageHyphenation = 0.0;
			}
		}

		// Un champ a été changé.
		private void HandleLanguageChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			this.document.TextWrapper.SuspendSynchronisations();

			string language = Language.LanguageLongToShort(this.fieldLanguage.Text);
			if ( language != "" )
			{
				this.document.TextWrapper.Defined.LanguageLocale = language;
			}
			else
			{
				this.document.TextWrapper.Defined.ClearLanguageLocale();
			}
			
			this.document.TextWrapper.ResumeSynchronisations();
		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;

			this.document.TextWrapper.SuspendSynchronisations();
			this.document.TextWrapper.Defined.ClearLanguageHyphenation();
			this.document.TextWrapper.Defined.ClearLanguageLocale();
			this.document.TextWrapper.ResumeSynchronisations();
		}


		protected IconButton				buttonHyphen;
		protected TextFieldCombo			fieldLanguage;
		protected IconButton				buttonClear;
	}
}
