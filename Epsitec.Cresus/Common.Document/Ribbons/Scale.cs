using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Scale permet de mettre � l'�chelle la s�lection.
	/// </summary>
	[SuppressBundleSupport]
	public class Scale : Abstract
	{
		public Scale() : base()
		{
			this.title.Text = Res.Strings.Action.ScaleMain;

			this.buttonScaleDiv2    = this.CreateIconButton("ScaleDiv2");
			this.buttonScaleMul2    = this.CreateIconButton("ScaleMul2");
			this.buttonMirrorH      = this.CreateIconButton("MirrorH");
			this.buttonMirrorV      = this.CreateIconButton("MirrorV");
			this.separator          = new IconSeparator(this);
			this.buttonScaleDivFree = this.CreateIconButton("ScaleDivFree");
			this.buttonScaleMulFree = this.CreateIconButton("ScaleMulFree");
			this.CreateFieldScale(ref this.fieldScale, Res.Strings.Action.ScaleValue);
			
			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		public override void SetDocument(DocumentType type, InstallType install, DebugMode debug, Settings.GlobalSettings gs, Document document)
		{
			base.SetDocument(type, install, debug, gs, document);

			this.AdaptFieldScale(this.fieldScale);
		}

		public override double DefaultWidth
		{
			//	Retourne la largeur standard.
			get
			{
				return 8 + 22*2 + this.separatorWidth + 50;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.buttonScaleDiv2 == null )  return;

			double dx = this.buttonScaleDiv2.DefaultWidth;
			double dy = this.buttonScaleDiv2.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*2;
			rect.Width = this.separatorWidth;
			this.separator.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonScaleDiv2.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonScaleMul2.SetManualBounds(rect);
			rect.Offset(dx+this.separatorWidth, 0);
			rect.Width = 25;
			this.buttonScaleDivFree.SetManualBounds(rect);
			rect.Offset(25, 0);
			this.buttonScaleMulFree.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonMirrorH.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonMirrorV.SetManualBounds(rect);
			rect.Offset(dx+this.separatorWidth, 0);
			rect.Width = 50;
			this.fieldScale.SetManualBounds(rect);
		}


		protected void CreateFieldScale(ref TextFieldReal field, string tooltip)
		{
			//	Cr�e un champ �ditable pour une �chelle.
			field = new TextFieldReal(this);
			field.PreferredWidth = 50;
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.ValueChanged += new EventHandler(this.HandleFieldValueChanged);
			ToolTip.Default.SetToolTip(field, tooltip);
		}

		protected void AdaptFieldScale(TextFieldReal field)
		{
			//	Adapte un champ �ditable pour une �chelle.
			if ( this.document == null )
			{
				field.Enable = false;
			}
			else
			{
				field.Enable = true;

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
