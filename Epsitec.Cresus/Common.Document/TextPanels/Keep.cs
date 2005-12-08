using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Keep permet de choisir les groupements de paragraphes.
	/// </summary>
	[SuppressBundleSupport]
	public class Keep : Abstract
	{
		public Keep(Document document) : base(document)
		{
			this.label.Text = Res.Strings.TextPanel.Keep.Title;

			this.fixIcon.Text = Misc.Image("TextKeep");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Keep.Title);

			this.buttonKeepNext = this.CreateIconButton(Misc.Icon("ParaKeepNext"), Res.Strings.TextPanel.Keep.Tooltip.KeepNext, new MessageEventHandler(this.HandleButtonKeepNextClicked));
			this.buttonKeepPrev = this.CreateIconButton(Misc.Icon("ParaKeepPrev"), Res.Strings.TextPanel.Keep.Tooltip.KeepPrev, new MessageEventHandler(this.HandleButtonKeepPrevClicked));

			this.fieldKeepStart = this.CreateTextFieldLabel(Res.Strings.TextPanel.Keep.Tooltip.KeepStart, Res.Strings.TextPanel.Keep.Short.KeepStart, Res.Strings.TextPanel.Keep.Long.KeepStart, 0.0, 100.0, 1.0, false, new EventHandler(this.HandleKeepStartChanged));
			this.fieldKeepEnd   = this.CreateTextFieldLabel(Res.Strings.TextPanel.Keep.Tooltip.KeepEnd,   Res.Strings.TextPanel.Keep.Short.KeepEnd,   Res.Strings.TextPanel.Keep.Long.KeepEnd,   0.0, 100.0, 1.0, false, new EventHandler(this.HandleKeepEndChanged));
			
			this.fieldKeepStart.TextFieldReal.Resolution = 1.0M;
			this.fieldKeepStart.TextFieldReal.Scale      = 1.0M;
			this.fieldKeepStart.TextFieldReal.UnitType   = RealUnitType.Scalar;
			this.fieldKeepStart.TextFieldReal.MinValue   = 1M;
			this.fieldKeepStart.TextFieldReal.MaxValue   = 19M;
			
			this.fieldKeepEnd.TextFieldReal.Resolution = 1.0M;
			this.fieldKeepEnd.TextFieldReal.Scale      = 1.0M;
			this.fieldKeepEnd.TextFieldReal.UnitType   = RealUnitType.Scalar;
			this.fieldKeepEnd.TextFieldReal.MinValue   = 1M;
			this.fieldKeepEnd.TextFieldReal.MaxValue   = 19M;

			this.buttonClear = this.CreateClearButton(new MessageEventHandler(this.HandleClearClicked));

			this.document.ParagraphWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
			this.document.ParagraphWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

			this.isNormalAndExtended = true;
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

		
		// Indique si ce panneau est visible pour un filtre donné.
		public override bool IsFilterShow(string filter)
		{
			return ( filter == "All" || filter == "Paragraph" );
		}


		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau étendu ?
				{
					if ( this.IsLabelProperties )  // étendu/détails ?
					{
						h += 80;
					}
					else	// étendu/compact ?
					{
						h += 30;
					}
				}
				else	// panneau réduit ?
				{
					h += 30;
				}

				return h;
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

			if ( this.buttonKeepNext == null )  return;

			Rectangle rect = this.UsefulZone;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				if ( this.IsLabelProperties )
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonKeepNext.Bounds = r;
					r.Offset(20, 0);
					this.buttonKeepPrev.Bounds = r;
					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.Bounds = r;

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Right;
					this.fieldKeepStart.Bounds = r;
					this.fieldKeepStart.Visibility = true;

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Right;
					this.fieldKeepEnd.Bounds = r;
					this.fieldKeepEnd.Visibility = true;
				}
				else
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonKeepNext.Bounds = r;
					r.Offset(20, 0);
					this.buttonKeepPrev.Bounds = r;
					r.Offset(20, 0);
					r.Width = 60;
					this.fieldKeepStart.Bounds = r;
					r.Offset(60, 0);
					this.fieldKeepEnd.Bounds = r;

					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.Bounds = r;
				}
			}
			else
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				r.Left = rect.Left;
				r.Width = 20;
				this.buttonKeepNext.Bounds = r;
				r.Offset(20, 0);
				this.buttonKeepPrev.Bounds = r;
				r.Offset(20, 0);
				r.Width = 60;
				this.fieldKeepStart.Bounds = r;
				r.Offset(60, 0);
				this.fieldKeepEnd.Bounds = r;

				r.Left = rect.Right-20;
				r.Width = 20;
				this.buttonClear.Bounds = r;
			}
		}


		// Met à jour après un changement du wrapper.
		protected override void UpdateAfterChanging()
		{
			base.UpdateAfterChanging();

			bool keepNext    = this.document.ParagraphWrapper.Active.KeepWithNextParagraph;
			bool keepPrev    = this.document.ParagraphWrapper.Active.KeepWithPreviousParagraph;
			int  keepStart   = this.document.ParagraphWrapper.Active.KeepStartLines;
			int  keepEnd     = this.document.ParagraphWrapper.Active.KeepEndLines;
			bool isKeepNext  = this.document.ParagraphWrapper.Defined.IsKeepWithNextParagraphDefined;
			bool isKeepPrev  = this.document.ParagraphWrapper.Defined.IsKeepWithPreviousParagraphDefined;
			bool isKeepStart = this.document.ParagraphWrapper.Defined.IsKeepStartLinesDefined;
			bool isKeepEnd   = this.document.ParagraphWrapper.Defined.IsKeepEndLinesDefined;

			this.ignoreChanged = true;

			this.ActiveIconButton(this.buttonKeepNext, keepNext, isKeepNext);
			this.ActiveIconButton(this.buttonKeepPrev, keepPrev, isKeepPrev);

			this.fieldKeepStart.TextFieldReal.InternalValue = (decimal) keepStart;
			this.fieldKeepEnd  .TextFieldReal.InternalValue = (decimal) keepEnd;
			this.ProposalTextFieldLabel(this.fieldKeepStart, !isKeepStart);
			this.ProposalTextFieldLabel(this.fieldKeepEnd,   !isKeepEnd);

			this.ignoreChanged = false;
		}



		private void HandleButtonKeepNextClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			bool value = (this.buttonKeepNext.ActiveState == ActiveState.No);
			this.document.ParagraphWrapper.Defined.KeepWithNextParagraph = value;
		}

		private void HandleButtonKeepPrevClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			bool value = (this.buttonKeepPrev.ActiveState == ActiveState.No);
			this.document.ParagraphWrapper.Defined.KeepWithPreviousParagraph = value;
		}

		private void HandleKeepStartChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			double value = (double) field.InternalValue;
			bool isDefined = field.Text != "";

			if ( isDefined )
			{
				this.document.ParagraphWrapper.Defined.KeepStartLines = (int) value;
			}
			else
			{
				this.document.ParagraphWrapper.Defined.ClearKeepStartLines();
			}
		}
		
		private void HandleKeepEndChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			double value = (double) field.InternalValue;
			bool isDefined = field.Text != "";

			if ( isDefined )
			{
				this.document.ParagraphWrapper.Defined.KeepEndLines = (int) value;
			}
			else
			{
				this.document.ParagraphWrapper.Defined.ClearKeepEndLines();
			}
		}
		
		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.ParagraphWrapper.IsAttached )  return;

			this.document.ParagraphWrapper.SuspendSynchronisations();
			this.document.ParagraphWrapper.Defined.ClearKeepWithNextParagraph();
			this.document.ParagraphWrapper.Defined.ClearKeepWithPreviousParagraph();
			this.document.ParagraphWrapper.Defined.ClearKeepStartLines();
			this.document.ParagraphWrapper.Defined.ClearKeepEndLines();
			this.document.ParagraphWrapper.ResumeSynchronisations();
		}

		
		protected IconButton				buttonKeepNext;
		protected IconButton				buttonKeepPrev;
		protected Widgets.TextFieldLabel	fieldKeepStart;
		protected Widgets.TextFieldLabel	fieldKeepEnd;
		protected IconButton				buttonClear;
	}
}
