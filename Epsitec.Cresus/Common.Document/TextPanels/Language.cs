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

			this.buttonHyphen = this.CreateIconButton(Misc.Icon("TextHyphen"), Res.Strings.Action.ParagraphHyphen, new MessageEventHandler(this.HandleHyphenClicked));

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

		
		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return this.LabelHeight+30;
			}
		}

		
		protected void HandleWrapperChanged(object sender)
		{
			//	Le wrapper associé a changé.
			this.UpdateAfterChanging();
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
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


		protected void UpdateComboLanguage()
		{
			//	Met à jour la liste des langues.
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


		protected override void UpdateAfterChanging()
		{
			//	Met à jour après un changement du wrapper.
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

			this.document.TextWrapper.SuspendSynchronizations();

			if ( hyphen )
			{
				this.document.TextWrapper.Defined.LanguageHyphenation = 1.0;
			}
			else
			{
				this.document.TextWrapper.Defined.LanguageHyphenation = 0.0;
			}

			this.document.TextWrapper.DefineOperationName("TextLanguageHyphen", Res.Strings.Action.ParagraphHyphen);
			this.document.TextWrapper.ResumeSynchronizations();
		}

		private void HandleLanguageChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;

			this.document.TextWrapper.SuspendSynchronizations();

			string language = Language.LanguageLongToShort(this.fieldLanguage.Text);
			if ( language != "" )
			{
				this.document.TextWrapper.Defined.LanguageLocale = language;
			}
			else
			{
				this.document.TextWrapper.Defined.ClearLanguageLocale();
			}
			
			this.document.TextWrapper.DefineOperationName("TextLanguage", Res.Strings.TextPanel.Language.Title);
			this.document.TextWrapper.ResumeSynchronizations();
		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;

			this.document.TextWrapper.SuspendSynchronizations();
			this.document.TextWrapper.Defined.ClearLanguageHyphenation();
			this.document.TextWrapper.Defined.ClearLanguageLocale();
			this.document.TextWrapper.DefineOperationName("TextLanguageClear", Res.Strings.TextPanel.Clear);
			this.document.TextWrapper.ResumeSynchronizations();
		}


		protected IconButton				buttonHyphen;
		protected TextFieldCombo			fieldLanguage;
		protected IconButton				buttonClear;
	}
}
