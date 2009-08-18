using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Justif permet de choisir un mode de justification.
	/// </summary>
	public class Justif : Abstract
	{
		public Justif(Document document, bool isStyle, StyleCategory styleCategory) : base(document, isStyle, styleCategory)
		{
			this.label.Text = Res.Strings.TextPanel.Justif.Title;

			this.fixIcon.Text = Misc.Image("TextJustif");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Justif.Title);

			this.buttonAlignLeft   = this.CreateIconButton(Misc.Icon("JustifHLeft"),   Res.Strings.Action.ParagraphAlignLeft,   this.HandleJustifClicked);
			this.buttonAlignCenter = this.CreateIconButton(Misc.Icon("JustifHCenter"), Res.Strings.Action.ParagraphAlignCenter, this.HandleJustifClicked);
			this.buttonAlignRight  = this.CreateIconButton(Misc.Icon("JustifHRight"),  Res.Strings.Action.ParagraphAlignRight,  this.HandleJustifClicked);
			this.buttonAlignJustif = this.CreateIconButton(Misc.Icon("JustifHJustif"), Res.Strings.Action.ParagraphAlignJustif, this.HandleJustifClicked);
			this.buttonAlignAll    = this.CreateIconButton(Misc.Icon("JustifHAll"),    Res.Strings.Action.ParagraphAlignAll,    this.HandleJustifClicked);

			this.buttonHyphen = this.CreateIconButton(Misc.Icon("TextHyphen"), Res.Strings.Action.ParagraphHyphen, this.HandleHyphenClicked);

			this.buttonClear = this.CreateClearButton(this.HandleClearClicked);

			this.ParagraphWrapper.Active.Changed  += this.HandleWrapperChanged;
			this.ParagraphWrapper.Defined.Changed += this.HandleWrapperChanged;

			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.ParagraphWrapper.Active.Changed  -= this.HandleWrapperChanged;
				this.ParagraphWrapper.Defined.Changed -= this.HandleWrapperChanged;
			}
			
			base.Dispose(disposing);
		}

		
		public override void UpdateAfterAttach()
		{
			//	Mise à jour après avoir attaché le wrappers.
			this.buttonClear.Visibility = !this.ParagraphWrapper.IsAttachedToDefaultParagraphStyle;
		}


		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return this.LabelHeight + 30;
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

			if ( this.buttonAlignLeft == null )  return;

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Left = rect.Left;
			r.Width = 20;
			this.buttonAlignLeft.SetManualBounds(r);
			r.Offset(20, 0);
			this.buttonAlignCenter.SetManualBounds(r);
			r.Offset(20, 0);
			this.buttonAlignRight.SetManualBounds(r);
			r.Offset(20, 0);
			this.buttonAlignJustif.SetManualBounds(r);
			r.Offset(20, 0);
			this.buttonAlignAll.SetManualBounds(r);
			r.Offset(25, 0);
			this.buttonHyphen.SetManualBounds(r);

			this.buttonHyphen.Visibility = (r.Right<rect.Right-22);

			r.Left = rect.Right-20;
			r.Width = 20;
			this.buttonClear.SetManualBounds(r);
		}


		protected override void UpdateAfterChanging()
		{
			//	Met à jour après un changement du wrapper.
			base.UpdateAfterChanging();

			if ( this.ParagraphWrapper.IsAttached == false )  return;
			
			Common.Text.Wrappers.JustificationMode justif = this.ParagraphWrapper.Active.JustificationMode;
			bool isJustif = this.ParagraphWrapper.Defined.IsJustificationModeDefined;

			bool hyphen = this.ParagraphWrapper.Active.Hyphenation;
			bool isHyphen = this.ParagraphWrapper.Defined.IsHyphenationDefined;

			this.ignoreChanged = true;

			this.ActiveIconButton(this.buttonAlignLeft,   justif == Common.Text.Wrappers.JustificationMode.AlignLeft,        isJustif);
			this.ActiveIconButton(this.buttonAlignCenter, justif == Common.Text.Wrappers.JustificationMode.Center,           isJustif);
			this.ActiveIconButton(this.buttonAlignRight,  justif == Common.Text.Wrappers.JustificationMode.AlignRight,       isJustif);
			this.ActiveIconButton(this.buttonAlignJustif, justif == Common.Text.Wrappers.JustificationMode.JustifyAlignLeft, isJustif);
			this.ActiveIconButton(this.buttonAlignAll,    justif == Common.Text.Wrappers.JustificationMode.JustifyJustfy,    isJustif);

			this.ActiveIconButton(this.buttonHyphen, hyphen, isHyphen);

			this.ignoreChanged = false;
		}


		private void HandleJustifClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			IconButton button = sender as IconButton;
			if ( button == null )  return;

			Common.Text.Wrappers.JustificationMode justif = Common.Text.Wrappers.JustificationMode.Unknown;
			if ( this.buttonAlignLeft   == button )  justif = Common.Text.Wrappers.JustificationMode.AlignLeft;
			if ( this.buttonAlignCenter == button )  justif = Common.Text.Wrappers.JustificationMode.Center;
			if ( this.buttonAlignRight  == button )  justif = Common.Text.Wrappers.JustificationMode.AlignRight;
			if ( this.buttonAlignJustif == button )  justif = Common.Text.Wrappers.JustificationMode.JustifyAlignLeft;
			if ( this.buttonAlignAll    == button )  justif = Common.Text.Wrappers.JustificationMode.JustifyJustfy;

			this.ParagraphWrapper.SuspendSynchronizations();

			if ( justif == Common.Text.Wrappers.JustificationMode.Unknown )
			{
				this.ParagraphWrapper.Defined.ClearJustificationMode();
			}
			else
			{
				this.ParagraphWrapper.Defined.JustificationMode = justif;
			}

			this.ParagraphWrapper.DefineOperationName("ParagraphJustif", Res.Strings.Action.ParagraphJustif);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleHyphenClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			bool hyphen = (this.buttonHyphen.ActiveState == ActiveState.No);

			this.ParagraphWrapper.SuspendSynchronizations();
			this.ParagraphWrapper.Defined.Hyphenation = hyphen;
			this.ParagraphWrapper.DefineOperationName("ParagraphHyphen", Res.Strings.Action.ParagraphHyphen);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			this.ParagraphWrapper.SuspendSynchronizations();
			this.ParagraphWrapper.Defined.ClearJustificationMode();
			this.ParagraphWrapper.Defined.ClearHyphenation();
			this.ParagraphWrapper.DefineOperationName("ParagraphJustifClear", Res.Strings.TextPanel.Clear);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}


		protected IconButton				buttonAlignLeft;
		protected IconButton				buttonAlignCenter;
		protected IconButton				buttonAlignRight;
		protected IconButton				buttonAlignJustif;
		protected IconButton				buttonAlignAll;
		protected IconButton				buttonHyphen;
		protected IconButton				buttonClear;
	}
}
