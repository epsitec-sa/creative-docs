using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Language permet de choisir la langue.
	/// </summary>
	public class Language : Abstract
	{
		public Language(Document document, bool isStyle, StyleCategory styleCategory) : base(document, isStyle, styleCategory)
		{
			this.label.Text = Res.Strings.TextPanel.Language.Title;

			this.fixIcon.Text = Misc.Image("TextLanguage");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Font.Title);

			this.fieldLanguage = new TextFieldCombo(this);
			this.fieldLanguage.IsReadOnly = true;
			this.fieldLanguage.TextChanged += this.HandleLanguageChanged;
			this.fieldLanguage.TabIndex = this.tabIndex++;
			this.fieldLanguage.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldLanguage, Res.Strings.TextPanel.Language.Tooltip.Language);

			this.buttonHyphen = this.CreateIconButton(Misc.Icon("TextHyphen"), Res.Strings.Action.ParagraphHyphen, this.HandleHyphenClicked);

			this.buttonClear = this.CreateClearButton(this.HandleClearClicked);

			this.TextWrapper.Active.Changed  += this.HandleWrapperChanged;
			this.TextWrapper.Defined.Changed += this.HandleWrapperChanged;

			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldLanguage.TextChanged -= this.HandleLanguageChanged;

				this.TextWrapper.Active.Changed  -= this.HandleWrapperChanged;
				this.TextWrapper.Defined.Changed -= this.HandleWrapperChanged;

				this.fieldLanguage = null;
			}
			
			base.Dispose(disposing);
		}

		
		public override void UpdateAfterAttach()
		{
			//	Mise à jour après avoir attaché le wrappers.
			this.buttonClear.Visibility = !this.TextWrapper.IsAttachedToDefaultParagraphStyle;
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
			this.fieldLanguage.SetManualBounds(r);

			r.Offset(105, 0);
			r.Width = 20;
			this.buttonHyphen.SetManualBounds(r);

			r.Left = rect.Right-20;
			r.Width = 20;
			this.buttonClear.SetManualBounds(r);
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

		public static string LanguageShortToLong(string shortLanguage)
		{
			if ( shortLanguage != null )
			{
				if ( shortLanguage.StartsWith("fr") )  return Res.Strings.TextPanel.Language.List.fr;
				if ( shortLanguage.StartsWith("de") )  return Res.Strings.TextPanel.Language.List.de;
				if ( shortLanguage.StartsWith("en") )  return Res.Strings.TextPanel.Language.List.en;
			}
			
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

			if ( this.TextWrapper.IsAttached == false )  return;
			
			bool hyphen = false;
			bool isHyphen = false;
			if ( this.TextWrapper.Defined.IsLanguageHyphenationDefined )
			{
				hyphen = (this.TextWrapper.Active.LanguageHyphenation == 1.0);
				isHyphen = true;
			}

			string language = Language.LanguageShortToLong(this.TextWrapper.Active.LanguageLocale);
			bool isLanguage = this.TextWrapper.Defined.IsLanguageLocaleDefined;

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
			if ( !this.TextWrapper.IsAttached )  return;

			this.buttonHyphen.ActiveState = (this.buttonHyphen.ActiveState == ActiveState.Yes) ? ActiveState.No : ActiveState.Yes;
			bool hyphen = (this.buttonHyphen.ActiveState == ActiveState.Yes);

			this.TextWrapper.SuspendSynchronizations();

			if ( hyphen )
			{
				this.TextWrapper.Defined.LanguageHyphenation = 1.0;
			}
			else
			{
				this.TextWrapper.Defined.LanguageHyphenation = 0.0;
			}

			this.TextWrapper.DefineOperationName("TextLanguageHyphen", Res.Strings.Action.ParagraphHyphen);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleLanguageChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;

			this.TextWrapper.SuspendSynchronizations();

			string language = Language.LanguageLongToShort(this.fieldLanguage.Text);
			if ( language != "" )
			{
				this.TextWrapper.Defined.LanguageLocale = language;
			}
			else
			{
				this.TextWrapper.Defined.ClearLanguageLocale();
			}
			
			this.TextWrapper.DefineOperationName("TextLanguage", Res.Strings.TextPanel.Language.Title);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;

			this.TextWrapper.SuspendSynchronizations();
			this.TextWrapper.Defined.ClearLanguageHyphenation();
			this.TextWrapper.Defined.ClearLanguageLocale();
			this.TextWrapper.DefineOperationName("TextLanguageClear", Res.Strings.TextPanel.Clear);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}


		protected IconButton				buttonHyphen;
		protected TextFieldCombo			fieldLanguage;
		protected IconButton				buttonClear;
	}
}
