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

			this.buttonScaleDiv2 = this.CreateIconButton("ScaleDiv2", Misc.Icon("OperScaleDiv2"), Res.Strings.Action.ScaleDiv2);
			this.buttonScaleMul2 = this.CreateIconButton("ScaleMul2", Misc.Icon("OperScaleMul2"), Res.Strings.Action.ScaleMul2);
			this.buttonMirrorH = this.CreateIconButton("MirrorH", Misc.Icon("OperMirrorH"), Res.Strings.Action.MirrorH);
			this.buttonMirrorV = this.CreateIconButton("MirrorV", Misc.Icon("OperMirrorV"), Res.Strings.Action.MirrorV);
			this.separator = new IconSeparator(this);
			this.buttonScaleDivFree = this.CreateIconButton("ScaleDivFree", Misc.Icon("OperScaleDivFree"), Res.Strings.Action.ScaleDivFree);
			this.buttonScaleMulFree = this.CreateIconButton("ScaleMulFree", Misc.Icon("OperScaleMulFree"),  Res.Strings.Action.ScaleMulFree);
			this.CreateFieldScale(ref this.fieldScale, Res.Strings.Action.ScaleValue);
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

			this.AdaptFieldScale(this.fieldScale);
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

			if ( this.buttonScaleDiv2 == null )  return;

			double dx = this.buttonScaleDiv2.DefaultWidth;
			double dy = this.buttonScaleDiv2.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*2;
			rect.Width = this.separatorWidth;
			this.separator.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy);
			this.buttonScaleDiv2.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonScaleMul2.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			rect.Width = 25;
			this.buttonScaleDivFree.Bounds = rect;
			rect.Offset(25, 0);
			this.buttonScaleMulFree.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonMirrorH.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonMirrorV.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			rect.Width = 50;
			this.fieldScale.Bounds = rect;
		}


		// Crée un champ éditable pour une échelle.
		protected void CreateFieldScale(ref TextFieldReal field, string tooltip)
		{
			field = new TextFieldReal(this);
			field.Width = 50;
			field.TabIndex = tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.ValueChanged += new EventHandler(this.HandleFieldValueChanged);
			ToolTip.Default.SetToolTip(field, tooltip);
		}

		// Adapte un champ éditable pour une échelle.
		protected void AdaptFieldScale(TextFieldReal field)
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
				field.InternalValue = (decimal) this.document.Modifier.ScaleFactor;
				this.ignoreChange = false;
			}
		}

		private void HandleFieldValueChanged(object sender)
		{
			if ( this.ignoreChange )  return;
			TextFieldReal field = sender as TextFieldReal;
			if ( field == this.fieldScale )
			{
				this.document.Modifier.ScaleFactor = (double) field.InternalValue;
			}
		}


		protected IconButton				buttonScaleDiv2;
		protected IconButton				buttonScaleMul2;
		protected IconButton				buttonMirrorH;
		protected IconButton				buttonMirrorV;
		protected IconSeparator				separator;
		protected IconButton				buttonScaleDivFree;
		protected IconButton				buttonScaleMulFree;
		protected TextFieldReal				fieldScale;
	}
}
