using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Scale permet de mettre à l'échelle la sélection.
	/// </summary>
	[SuppressBundleSupport]
	public class Scale : Abstract
	{
		public Scale() : base()
		{
			this.title.Text = Res.Strings.Action.ScaleMain;

			this.buttonZoomDiv2 = this.CreateIconButton("ZoomDiv2", Misc.Icon("OperZoomDiv2"), Res.Strings.Action.ZoomDiv2);
			this.buttonZoomMul2 = this.CreateIconButton("ZoomMul2", Misc.Icon("OperZoomMul2"), Res.Strings.Action.ZoomMul2);
			this.buttonMirrorH = this.CreateIconButton("MirrorH", Misc.Icon("OperMirrorH"), Res.Strings.Action.MirrorH);
			this.buttonMirrorV = this.CreateIconButton("MirrorV", Misc.Icon("OperMirrorV"), Res.Strings.Action.MirrorV);
			this.separator = new IconSeparator(this);
			this.buttonZoomDivFree = this.CreateIconButton("ZoomDivFree", Misc.Icon("OperZoomi"), Res.Strings.Action.ZoomFreeM);
			this.buttonZoomMulFree = this.CreateIconButton("ZoomMulFree", Misc.Icon("OperZoom"),  Res.Strings.Action.ZoomFreeP);
			this.CreateFieldZoom(ref this.fieldZoom, Res.Strings.Action.ZoomValue);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		public override void SetDocument(DocumentType type, Settings.GlobalSettings gs, Document document)
		{
			base.SetDocument(type, gs, document);

			this.AdaptFieldZoom(this.fieldZoom);
		}

		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 8 + 22*2 + this.separatorWidth + 50;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonZoomDiv2 == null )  return;

			double dx = this.buttonZoomDiv2.DefaultWidth;
			double dy = this.buttonZoomDiv2.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*2;
			rect.Width = this.separatorWidth;
			this.separator.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy);
			this.buttonZoomDiv2.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonZoomMul2.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			rect.Width = 25;
			this.buttonZoomDivFree.Bounds = rect;
			rect.Offset(25, 0);
			this.buttonZoomMulFree.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonMirrorH.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonMirrorV.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			rect.Width = 50;
			this.fieldZoom.Bounds = rect;
		}


		// Crée un champ éditable pour un zoom.
		protected void CreateFieldZoom(ref TextFieldReal field, string tooltip)
		{
			field = new TextFieldReal(this);
			field.Width = 50;
			field.TabIndex = tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.ValueChanged += new EventHandler(this.HandleFieldValueChanged);
			ToolTip.Default.SetToolTip(field, tooltip);
		}

		// Adapte un champ éditable pour un zoom.
		protected void AdaptFieldZoom(TextFieldReal field)
		{
			if ( this.document == null )
			{
				field.SetEnabled(false);
			}
			else
			{
				field.SetEnabled(true);

				this.ignoreChange = true;
				this.document.Modifier.AdaptTextFieldRealScalar(field);
				field.InternalMinValue = 1.0M;
				field.InternalMaxValue = 2.0M;
				field.DefaultValue = 1.0M;
				field.Step = 0.1M;
				field.Resolution = 0.01M;
				field.InternalValue = (decimal) this.document.Modifier.ZoomFactor;
				this.ignoreChange = false;
			}
		}

		private void HandleFieldValueChanged(object sender)
		{
			if ( this.ignoreChange )  return;
			TextFieldReal field = sender as TextFieldReal;
			if ( field == this.fieldZoom )
			{
				this.document.Modifier.ZoomFactor = (double) field.InternalValue;
			}
		}


		protected IconButton				buttonZoomDiv2;
		protected IconButton				buttonZoomMul2;
		protected IconButton				buttonMirrorH;
		protected IconButton				buttonMirrorV;
		protected IconSeparator				separator;
		protected IconButton				buttonZoomDivFree;
		protected IconButton				buttonZoomMulFree;
		protected TextFieldReal				fieldZoom;
	}
}
