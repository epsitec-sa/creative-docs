using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Numerator permet de choisir les groupements de paragraphes.
	/// </summary>
	[SuppressBundleSupport]
	public class Numerator : Abstract
	{
		public Numerator(Document document, bool isStyle, StyleCategory styleCategory) : base(document, isStyle, styleCategory)
		{
			this.label.Text = Res.Strings.TextPanel.Numerator.Title;

			this.fixIcon.Text = Misc.Image("TextNumerator");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Numerator.Title);

			this.labelStart = new StaticText(this);
			this.labelStart.ContentAlignment = ContentAlignment.MiddleRight;
			this.labelStart.Text = Res.Strings.TextPanel.Numerator.Label.Start;

			this.fieldStart = new TextField(this);
			this.fieldStart.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.fieldStart.AutoSelectOnFocus = true;
			this.fieldStart.SwallowEscape = true;
			this.fieldStart.SwallowReturn = true;
			this.fieldStart.EditionAccepted += new EventHandler(this.HandleStartEditionAccepted);
			ToolTip.Default.SetToolTip(this.fieldStart, Res.Strings.TextPanel.Numerator.Tooltip.Start);

			this.buttonClear = this.CreateClearButton(new MessageEventHandler(this.HandleClearClicked));

			this.ParagraphWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
			this.ParagraphWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

			this.isNormalAndExtended = false;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.ParagraphWrapper.Active.Changed  -= new EventHandler(this.HandleWrapperChanged);
				this.ParagraphWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);
			}
			
			base.Dispose(disposing);
		}

		
		public override void UpdateAfterAttach()
		{
			//	Mise � jour apr�s avoir attach� le wrappers.
			this.buttonClear.Visibility = !this.ParagraphWrapper.IsAttachedToDefaultParagraphStyle;
		}


		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				double h = this.LabelHeight;
				h += 30;
				return h;
			}
		}


		protected void HandleWrapperChanged(object sender)
		{
			//	Le wrapper associ� a chang�.
			this.UpdateAfterChanging();
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.fieldStart == null )  return;

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;

			r.Left = rect.Left;
			r.Width = 80;
			this.labelStart.SetManualBounds(r);

			r.Left = rect.Left+80+3;
			r.Right = rect.Right-25;
			this.fieldStart.SetManualBounds(r);
			
			r.Left = rect.Right-20;
			r.Width = 20;
			this.buttonClear.SetManualBounds(r);
		}


		protected override void UpdateAfterChanging()
		{
			//	Met � jour apr�s un changement du wrapper.
			base.UpdateAfterChanging();
			
			if ( this.ParagraphWrapper.IsAttached == false )  return;

			string text = "";
			if ( this.ParagraphWrapper.Defined.IsItemListInfoDefined )
			{
				text = this.ParagraphWrapper.Defined.ItemListInfo;

				if ( text.StartsWith("set ") )
				{
					text = text.Substring(4);
				}

				if ( text == "cont" )
				{
					text = "";
				}
			}

			this.ignoreChanged = true;
			this.fieldStart.Text = text;
			this.ignoreChanged = false;
		}



		private void HandleStartEditionAccepted(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			this.ParagraphWrapper.SuspendSynchronizations();

			string text = this.fieldStart.Text;
			if ( text == "" )
			{
				this.ParagraphWrapper.Defined.ClearItemListInfo();
			}
			else
			{
				this.ParagraphWrapper.Defined.ItemListInfo = "set " + text;
			}

			this.ParagraphWrapper.DefineOperationName("ParagraphNumerator", Res.Strings.TextPanel.Numerator.Title);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}
		
		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			this.ParagraphWrapper.SuspendSynchronizations();
			this.ParagraphWrapper.Defined.ClearItemListInfo();
			this.ParagraphWrapper.DefineOperationName("ParagraphNumerator", Res.Strings.TextPanel.Clear);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		
		protected StaticText				labelStart;
		protected TextField					fieldStart;
		protected IconButton				buttonClear;
	}
}
