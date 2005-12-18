using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Move permet de déplacer la sélection.
	/// </summary>
	[SuppressBundleSupport]
	public class Move : Abstract
	{
		public Move() : base()
		{
			this.title.Text = Res.Strings.Action.MoveMain;

			this.buttonMoveHi = this.CreateIconButton("MoveLeftFree");
			this.buttonMoveH  = this.CreateIconButton("MoveRightFree");
			this.buttonMoveVi = this.CreateIconButton("MoveDownFree");
			this.buttonMoveV  = this.CreateIconButton("MoveUpFree");
			this.CreateFieldMove(ref this.fieldMoveH, Res.Strings.Action.MoveValueX);
			this.CreateFieldMove(ref this.fieldMoveV, Res.Strings.Action.MoveValueY);
			
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

			this.AdaptFieldMove(this.fieldMoveH);
			this.AdaptFieldMove(this.fieldMoveV);
		}

		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 8 + 22*2 + 50;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonMoveH == null )  return;

			double dx = this.buttonMoveH.DefaultWidth;
			double dy = this.buttonMoveH.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonMoveHi.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonMoveH.Bounds = rect;
			rect.Offset(dx, 0);
			rect.Width = 50;
			this.fieldMoveH.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonMoveVi.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonMoveV.Bounds = rect;
			rect.Offset(dx, 0);
			rect.Width = 50;
			this.fieldMoveV.Bounds = rect;
		}


		// Crée un champ éditable pour un déplacement.
		protected void CreateFieldMove(ref TextFieldReal field, string tooltip)
		{
			field = new TextFieldReal(this);
			field.Width = 50;
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.ValueChanged += new EventHandler(this.HandleFieldValueChanged);
			ToolTip.Default.SetToolTip(field, tooltip);
		}

		// Adapte un champ éditable pour un déplacement.
		protected void AdaptFieldMove(TextFieldReal field)
		{
			if ( this.document == null )
			{
				field.Enable = false;
			}
			else
			{
				field.Enable = true;

				this.document.Modifier.AdaptTextFieldRealDimension(field);

				this.ignoreChange = true;
				field.InternalMinValue = 0;
				if ( field == this.fieldMoveH )
				{
					field.InternalValue = (decimal) this.document.Modifier.MoveDistanceH;
				}
				if ( field == this.fieldMoveV )
				{
					field.InternalValue = (decimal) this.document.Modifier.MoveDistanceV;
				}
				this.ignoreChange = false;
			}
		}

		private void HandleFieldValueChanged(object sender)
		{
			if ( this.ignoreChange )  return;
			TextFieldReal field = sender as TextFieldReal;

			if ( field == this.fieldMoveH )
			{
				this.document.Modifier.MoveDistanceH = (double) field.InternalValue;
			}
			if ( field == this.fieldMoveV )
			{
				this.document.Modifier.MoveDistanceV = (double) field.InternalValue;
			}
		}


		protected IconButton				buttonMoveH;
		protected IconButton				buttonMoveHi;
		protected IconButton				buttonMoveV;
		protected IconButton				buttonMoveVi;
		protected TextFieldReal				fieldMoveH;
		protected TextFieldReal				fieldMoveV;
	}
}
