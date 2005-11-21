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

			this.buttonAlignLeft   = this.CreateIconButton(Misc.Icon("JustifHLeft"),   Res.Strings.Action.Text.Paragraph.AlignLeft,   new MessageEventHandler(this.HandleButtonJustifClicked));
			this.buttonAlignCenter = this.CreateIconButton(Misc.Icon("JustifHCenter"), Res.Strings.Action.Text.Paragraph.AlignCenter, new MessageEventHandler(this.HandleButtonJustifClicked));
			this.buttonAlignRight  = this.CreateIconButton(Misc.Icon("JustifHRight"),  Res.Strings.Action.Text.Paragraph.AlignRight,  new MessageEventHandler(this.HandleButtonJustifClicked));
			this.buttonAlignJustif = this.CreateIconButton(Misc.Icon("JustifHJustif"), Res.Strings.Action.Text.Paragraph.AlignJustif, new MessageEventHandler(this.HandleButtonJustifClicked));

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

				if ( this.isExtendedSize )  // panneau étendu ?
				{
					if ( this.IsLabelProperties )  // étendu/détails ?
					{
						h += 105;
					}
					else	// étendu/compact ?
					{
						h += 80;
					}
				}
				else	// panneau réduit ?
				{
					h += 30;
				}

				return h;
			}
		}

		// Met à jour après un changement du wrapper.
		protected override void UpdateAfterChanging()
		{
			base.UpdateAfterChanging();

			Common.Text.Wrappers.JustificationMode justif = this.document.ParagraphLayoutWrapper.Defined.JustificationMode;
			if ( justif == Common.Text.Wrappers.JustificationMode.Unknown )
			{
				justif = this.document.ParagraphLayoutWrapper.Active.JustificationMode;
			}

			this.ignoreChanged = true;

			this.buttonAlignLeft.ActiveState   = (justif == Common.Text.Wrappers.JustificationMode.AlignLeft)        ? ActiveState.Yes : ActiveState.No;
			this.buttonAlignCenter.ActiveState = (justif == Common.Text.Wrappers.JustificationMode.Center)           ? ActiveState.Yes : ActiveState.No;
			this.buttonAlignRight.ActiveState  = (justif == Common.Text.Wrappers.JustificationMode.AlignRight)       ? ActiveState.Yes : ActiveState.No;
			this.buttonAlignJustif.ActiveState = (justif == Common.Text.Wrappers.JustificationMode.JustifyAlignLeft) ? ActiveState.Yes : ActiveState.No;
			
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
			}
		}


		private void HandleButtonJustifClicked(object sender, MessageEventArgs e)
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


		protected IconButton				buttonAlignLeft;
		protected IconButton				buttonAlignCenter;
		protected IconButton				buttonAlignRight;
		protected IconButton				buttonAlignJustif;
	}
}
