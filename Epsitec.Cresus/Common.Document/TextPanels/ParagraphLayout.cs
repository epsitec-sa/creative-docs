using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe ParagraphLayout permet de choisir une mise en page.
	/// </summary>
	[SuppressBundleSupport]
	public class ParagraphLayout : Abstract
	{
		public ParagraphLayout(Document document) : base(document)
		{
			this.label.Text = Res.Strings.Property.Abstract.TextJustif;

			this.fixIcon.Text = Misc.Image("PropertyTextJustif");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.Property.Abstract.TextJustif);

			this.buttonAlignLeft   = this.CreateIconButton(Misc.Icon("JustifHLeft"),   Res.Strings.Action.Text.Paragraph.AlignLeft,   new MessageEventHandler(this.HandleJustifClicked));
			this.buttonAlignCenter = this.CreateIconButton(Misc.Icon("JustifHCenter"), Res.Strings.Action.Text.Paragraph.AlignCenter, new MessageEventHandler(this.HandleJustifClicked));
			this.buttonAlignRight  = this.CreateIconButton(Misc.Icon("JustifHRight"),  Res.Strings.Action.Text.Paragraph.AlignRight,  new MessageEventHandler(this.HandleJustifClicked));
			this.buttonAlignJustif = this.CreateIconButton(Misc.Icon("JustifHJustif"), Res.Strings.Action.Text.Paragraph.AlignJustif, new MessageEventHandler(this.HandleJustifClicked));

			this.fieldLeftMarginFirst = this.CreateTextFieldLabel(Res.Strings.Action.Text.Ruler.HandleLeftFirst, "P", "Marge d�but",  0.0, 100.0, 1.0, false, new EventHandler(this.HandleMarginChanged));
			this.fieldLeftMarginBody  = this.CreateTextFieldLabel(Res.Strings.Action.Text.Ruler.HandleLeftBody,  "G", "Marge gauche", 0.0, 100.0, 1.0, false, new EventHandler(this.HandleMarginChanged));
			this.fieldRightMargin     = this.CreateTextFieldLabel(Res.Strings.Action.Text.Ruler.HandleRight,     "D", "Marge droite", 0.0, 100.0, 1.0, false, new EventHandler(this.HandleMarginChanged));

			this.document.ParagraphLayoutWrapper.Active.Changed += new EventHandler(this.HandleWrapperChanged);
			this.document.ParagraphLayoutWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.document.ParagraphLayoutWrapper.Active.Changed -= new EventHandler(this.HandleWrapperChanged);
				this.document.ParagraphLayoutWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau �tendu ?
				{
					if ( this.IsLabelProperties )  // �tendu/d�tails ?
					{
						h += 105;
					}
					else	// �tendu/compact ?
					{
						h += 55;
					}
				}
				else	// panneau r�duit ?
				{
					h += 30;
				}

				return h;
			}
		}

		// Met � jour apr�s un changement du wrapper.
		protected override void UpdateAfterChanging()
		{
			base.UpdateAfterChanging();

			Common.Text.Wrappers.JustificationMode justif = this.document.ParagraphLayoutWrapper.Defined.JustificationMode;
			if ( justif == Common.Text.Wrappers.JustificationMode.Unknown )
			{
				justif = this.document.ParagraphLayoutWrapper.Active.JustificationMode;
			}

			double leftMarginFirst = this.document.ParagraphLayoutWrapper.Defined.LeftMarginFirst;
			if ( double.IsNaN(leftMarginFirst) )
			{
				leftMarginFirst = this.document.ParagraphLayoutWrapper.Active.LeftMarginFirst;

				if ( double.IsNaN(leftMarginFirst) )
				{
					leftMarginFirst = 0;
				}
			}

			double leftMarginBody = this.document.ParagraphLayoutWrapper.Defined.LeftMarginBody;
			if ( double.IsNaN(leftMarginBody) )
			{
				leftMarginBody = this.document.ParagraphLayoutWrapper.Active.LeftMarginBody;

				if ( double.IsNaN(leftMarginBody) )
				{
					leftMarginBody = 0;
				}
			}

			double rightMargin = this.document.ParagraphLayoutWrapper.Defined.RightMarginBody;
			if ( double.IsNaN(rightMargin) )
			{
				rightMargin = this.document.ParagraphLayoutWrapper.Active.RightMarginBody;

				if ( double.IsNaN(rightMargin) )
				{
					rightMargin = 0;
				}
			}

			this.ignoreChanged = true;

			this.buttonAlignLeft.ActiveState   = (justif == Common.Text.Wrappers.JustificationMode.AlignLeft)        ? ActiveState.Yes : ActiveState.No;
			this.buttonAlignCenter.ActiveState = (justif == Common.Text.Wrappers.JustificationMode.Center)           ? ActiveState.Yes : ActiveState.No;
			this.buttonAlignRight.ActiveState  = (justif == Common.Text.Wrappers.JustificationMode.AlignRight)       ? ActiveState.Yes : ActiveState.No;
			this.buttonAlignJustif.ActiveState = (justif == Common.Text.Wrappers.JustificationMode.JustifyAlignLeft) ? ActiveState.Yes : ActiveState.No;

			this.fieldLeftMarginFirst.TextFieldReal.InternalValue = (decimal) leftMarginFirst;
			this.fieldLeftMarginBody.TextFieldReal.InternalValue  = (decimal) leftMarginBody;
			this.fieldRightMargin.TextFieldReal.InternalValue     = (decimal) rightMargin;
			
			this.ignoreChanged = false;
		}


		// Le wrapper associ� a chang�.
		protected void HandleWrapperChanged(object sender)
		{
			this.UpdateAfterChanging();
		}

		
		// Met � jour la g�om�trie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonAlignLeft == null )  return;

			Rectangle rect = this.UsefulZone;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				if ( this.IsLabelProperties )
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonAlignLeft.Bounds = r;
					r.Offset(20, 0);
					this.buttonAlignCenter.Bounds = r;
					r.Offset(20, 0);
					this.buttonAlignRight.Bounds = r;
					r.Offset(20, 0);
					this.buttonAlignJustif.Bounds = r;

					r.Left = rect.Left;
					r.Right = rect.Right;
					r.Offset(0, -25);
					this.fieldLeftMarginFirst.Bounds = r;
					this.fieldLeftMarginFirst.SetVisible(true);
					r.Offset(0, -25);
					this.fieldLeftMarginBody.Bounds = r;
					this.fieldLeftMarginBody.SetVisible(true);
					r.Offset(0, -25);
					this.fieldRightMargin.Bounds = r;
					this.fieldRightMargin.SetVisible(true);
				}
				else
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonAlignLeft.Bounds = r;
					r.Offset(20, 0);
					this.buttonAlignCenter.Bounds = r;
					r.Offset(20, 0);
					this.buttonAlignRight.Bounds = r;
					r.Offset(20, 0);
					this.buttonAlignJustif.Bounds = r;

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Width = 60;
					this.fieldLeftMarginFirst.Bounds = r;
					this.fieldLeftMarginFirst.SetVisible(true);
					r.Offset(60, 0);
					this.fieldLeftMarginBody.Bounds = r;
					this.fieldLeftMarginBody.SetVisible(true);
					r.Offset(60, 0);
					this.fieldRightMargin.Bounds = r;
					this.fieldRightMargin.SetVisible(true);
				}
			}
			else
			{
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

				this.fieldLeftMarginFirst.SetVisible(false);
				this.fieldLeftMarginBody.SetVisible(false);
				this.fieldRightMargin.SetVisible(false);
			}
		}


		private void HandleJustifClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphLayoutWrapper.IsAttached )  return;

			IconButton button = sender as IconButton;
			if ( button == null )  return;
			button.ActiveState = (button.ActiveState == ActiveState.Yes) ? ActiveState.No : ActiveState.Yes;

			if ( this.buttonAlignLeft   != button )  this.buttonAlignLeft.ActiveState   = ActiveState.No;
			if ( this.buttonAlignCenter != button )  this.buttonAlignCenter.ActiveState = ActiveState.No;
			if ( this.buttonAlignRight  != button )  this.buttonAlignRight.ActiveState  = ActiveState.No;
			if ( this.buttonAlignJustif != button )  this.buttonAlignJustif.ActiveState = ActiveState.No;

			Common.Text.Wrappers.JustificationMode justif = Common.Text.Wrappers.JustificationMode.Unknown;

			if ( this.buttonAlignLeft.ActiveState   ==  ActiveState.Yes )  justif = Common.Text.Wrappers.JustificationMode.AlignLeft;
			if ( this.buttonAlignCenter.ActiveState ==  ActiveState.Yes )  justif = Common.Text.Wrappers.JustificationMode.Center;
			if ( this.buttonAlignRight.ActiveState  ==  ActiveState.Yes )  justif = Common.Text.Wrappers.JustificationMode.AlignRight;
			if ( this.buttonAlignJustif.ActiveState ==  ActiveState.Yes )  justif = Common.Text.Wrappers.JustificationMode.JustifyAlignLeft;

			this.document.ParagraphLayoutWrapper.Defined.JustificationMode = justif;
		}

		private void HandleMarginChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphLayoutWrapper.IsAttached )  return;

			Widgets.TextFieldLabel field = sender as Widgets.TextFieldLabel;
			if ( field == null )  return;

			double value = (double) field.TextFieldReal.InternalValue;

			if ( field == this.fieldLeftMarginFirst )
			{
				this.document.ParagraphLayoutWrapper.Defined.LeftMarginFirst = value;
			}

			if ( field == this.fieldLeftMarginBody )
			{
				this.document.ParagraphLayoutWrapper.Defined.LeftMarginBody = value;
			}

			if ( field == this.fieldRightMargin )
			{
				this.document.ParagraphLayoutWrapper.SuspendSynchronisations();
				this.document.ParagraphLayoutWrapper.Defined.RightMarginFirst = value;
				this.document.ParagraphLayoutWrapper.Defined.RightMarginBody  = value;
				this.document.ParagraphLayoutWrapper.ResumeSynchronisations();
			}
		}


		protected IconButton				buttonAlignLeft;
		protected IconButton				buttonAlignCenter;
		protected IconButton				buttonAlignRight;
		protected IconButton				buttonAlignJustif;
		protected Widgets.TextFieldLabel	fieldLeftMarginFirst;
		protected Widgets.TextFieldLabel	fieldLeftMarginBody;
		protected Widgets.TextFieldLabel	fieldRightMargin;
	}
}
