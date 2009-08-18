using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Move permet de déplacer la sélection.
	/// </summary>
	public class Move : Abstract
	{
		public Move() : base()
		{
			this.Title = Res.Strings.Action.MoveMain;
			this.PreferredWidth = 8 + 22*2 + 50;

			this.buttonMoveHi = this.CreateIconButton("MoveLeftFree");
			this.buttonMoveH  = this.CreateIconButton("MoveRightFree");
			this.buttonMoveVi = this.CreateIconButton("MoveDownFree");
			this.buttonMoveV  = this.CreateIconButton("MoveUpFree");
			this.CreateFieldMove(ref this.fieldMoveH, Res.Strings.Action.MoveValueX);
			this.CreateFieldMove(ref this.fieldMoveV, Res.Strings.Action.MoveValueY);
			
//			this.UpdateClientGeometry();
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


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonMoveH == null )  return;

			double dx = this.buttonMoveH.PreferredWidth;
			double dy = this.buttonMoveH.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonMoveHi.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonMoveH.SetManualBounds(rect);
			rect.Offset(dx, 0);
			rect.Width = 50;
			this.fieldMoveH.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonMoveVi.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonMoveV.SetManualBounds(rect);
			rect.Offset(dx, 0);
			rect.Width = 50;
			this.fieldMoveV.SetManualBounds(rect);
		}


		protected void CreateFieldMove(ref TextFieldReal field, string tooltip)
		{
			//	Crée un champ éditable pour un déplacement.
			field = new TextFieldReal(this);
			field.PreferredWidth = 50;
			field.TabIndex = this.tabIndex++;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			field.ValueChanged += this.HandleFieldValueChanged;
			ToolTip.Default.SetToolTip(field, tooltip);
		}

		protected void AdaptFieldMove(TextFieldReal field)
		{
			//	Adapte un champ éditable pour un déplacement.
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
			if ( this.document.Modifier == null )  return;
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
