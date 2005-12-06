using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Justif permet de choisir un mode de justification.
	/// </summary>
	[SuppressBundleSupport]
	public class Justif : Abstract
	{
		public Justif(Document document) : base(document)
		{
			this.label.Text = Res.Strings.TextPanel.Justif.Title;

			this.fixIcon.Text = Misc.Image("TextJustif");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Justif.Title);

			this.buttonAlignLeft   = this.CreateIconButton(Misc.Icon("JustifHLeft"),   Res.Strings.Action.Text.Paragraph.AlignLeft,   new MessageEventHandler(this.HandleJustifClicked));
			this.buttonAlignCenter = this.CreateIconButton(Misc.Icon("JustifHCenter"), Res.Strings.Action.Text.Paragraph.AlignCenter, new MessageEventHandler(this.HandleJustifClicked));
			this.buttonAlignRight  = this.CreateIconButton(Misc.Icon("JustifHRight"),  Res.Strings.Action.Text.Paragraph.AlignRight,  new MessageEventHandler(this.HandleJustifClicked));
			this.buttonAlignJustif = this.CreateIconButton(Misc.Icon("JustifHJustif"), Res.Strings.Action.Text.Paragraph.AlignJustif, new MessageEventHandler(this.HandleJustifClicked));
			this.buttonAlignAll    = this.CreateIconButton(Misc.Icon("JustifHAll"),    Res.Strings.Action.Text.Paragraph.AlignAll,    new MessageEventHandler(this.HandleJustifClicked));

			this.buttonHyphen = this.CreateIconButton(Misc.Icon("TextHyphen"), Res.Strings.Action.Text.Paragraph.Hyphen, new MessageEventHandler(this.HandleHyphenClicked));

			this.buttonClear = this.CreateClearButton(new MessageEventHandler(this.HandleClearClicked));

			this.document.ParagraphWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
			this.document.ParagraphWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.document.ParagraphWrapper.Active.Changed  -= new EventHandler(this.HandleWrapperChanged);
				this.document.ParagraphWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return this.LabelHeight + 30;
			}
		}

		// Met à jour après un changement du wrapper.
		protected override void UpdateAfterChanging()
		{
			base.UpdateAfterChanging();

			Common.Text.Wrappers.JustificationMode justif = this.document.ParagraphWrapper.Active.JustificationMode;
			bool isJustif = this.document.ParagraphWrapper.Defined.IsJustificationModeDefined;

			bool hyphen = this.document.ParagraphWrapper.Active.Hyphenation;
			bool isHyphen = this.document.ParagraphWrapper.Defined.IsHyphenationDefined;

			this.ignoreChanged = true;

			this.ActiveIconButton(this.buttonAlignLeft,   justif == Common.Text.Wrappers.JustificationMode.AlignLeft,        isJustif);
			this.ActiveIconButton(this.buttonAlignCenter, justif == Common.Text.Wrappers.JustificationMode.Center,           isJustif);
			this.ActiveIconButton(this.buttonAlignRight,  justif == Common.Text.Wrappers.JustificationMode.AlignRight,       isJustif);
			this.ActiveIconButton(this.buttonAlignJustif, justif == Common.Text.Wrappers.JustificationMode.JustifyAlignLeft, isJustif);
			this.ActiveIconButton(this.buttonAlignAll,    justif == Common.Text.Wrappers.JustificationMode.JustifyJustfy,    isJustif);

			this.ActiveIconButton(this.buttonHyphen, hyphen, isHyphen);

			this.ignoreChanged = false;
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

			if ( this.buttonAlignLeft == null )  return;

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Left = rect.Left;
			r.Width = 20;
			this.buttonAlignLeft.Bounds = r;
			r.Offset(20, 0);
			this.buttonAlignCenter.Bounds = r;
			r.Offset(20, 0);
			this.buttonAlignRight.Bounds = r;
			r.Offset(20, 0);
			this.buttonAlignJustif.Bounds = r;
			r.Offset(20, 0);
			this.buttonAlignAll.Bounds = r;
			r.Offset(25, 0);
			this.buttonHyphen.Bounds = r;

			r.Left = rect.Right-20;
			r.Width = 20;
			this.buttonClear.Bounds = r;
		}


		private void HandleJustifClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			IconButton button = sender as IconButton;
			if ( button == null )  return;

			Common.Text.Wrappers.JustificationMode justif = Common.Text.Wrappers.JustificationMode.Unknown;
			if ( this.buttonAlignLeft   == button )  justif = Common.Text.Wrappers.JustificationMode.AlignLeft;
			if ( this.buttonAlignCenter == button )  justif = Common.Text.Wrappers.JustificationMode.Center;
			if ( this.buttonAlignRight  == button )  justif = Common.Text.Wrappers.JustificationMode.AlignRight;
			if ( this.buttonAlignJustif == button )  justif = Common.Text.Wrappers.JustificationMode.JustifyAlignLeft;
			if ( this.buttonAlignAll    == button )  justif = Common.Text.Wrappers.JustificationMode.JustifyJustfy;

			if ( justif == Common.Text.Wrappers.JustificationMode.Unknown )
			{
				this.document.ParagraphWrapper.Defined.ClearJustificationMode();
			}
			else
			{
				this.document.ParagraphWrapper.Defined.JustificationMode = justif;
			}
		}

		private void HandleHyphenClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			bool hyphen = (this.buttonHyphen.ActiveState == ActiveState.No);
			this.document.ParagraphWrapper.Defined.Hyphenation = hyphen;
		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			this.document.ParagraphWrapper.SuspendSynchronisations();
			this.document.ParagraphWrapper.Defined.ClearJustificationMode();
			this.document.ParagraphWrapper.Defined.ClearHyphenation();
			this.document.ParagraphWrapper.ResumeSynchronisations();
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
