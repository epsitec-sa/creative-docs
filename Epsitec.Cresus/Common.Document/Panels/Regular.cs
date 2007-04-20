using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Regular permet de choisir un type de polygone régulier.
	/// </summary>
	public class Regular : Abstract
	{
		public Regular(Document document) : base(document)
		{
			this.grid = new RadioIconGrid(this);
			this.grid.SelectionChanged += new EventHandler(HandleTypeChanged);
			this.grid.TabIndex = 0;
			this.grid.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(0, "Norm");
			this.AddRadioIcon(1, "Star");
			this.AddRadioIcon(2, "Flower");

			this.fieldNbFaces = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldNbFaces.LabelShortText = Res.Strings.Panel.Regular.Short.Faces;
			this.fieldNbFaces.LabelLongText  = Res.Strings.Panel.Regular.Long.Faces;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldNbFaces.TextFieldReal);
			this.fieldNbFaces.TextFieldReal.InternalMinValue = 3;
			this.fieldNbFaces.TextFieldReal.InternalMaxValue = 24;
			this.fieldNbFaces.TextFieldReal.Step = 1;
			this.fieldNbFaces.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldNbFaces.TabIndex = 1;
			this.fieldNbFaces.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldNbFaces, Res.Strings.Panel.Regular.Tooltip.Faces);

			this.fieldDeep = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldDeep.LabelShortText = Res.Strings.Panel.Regular.Short.Deep;
			this.fieldDeep.LabelLongText  = Res.Strings.Panel.Regular.Long.Deep;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldDeep.TextFieldReal);
			this.fieldDeep.TextFieldReal.InternalMinValue = 0;
			this.fieldDeep.TextFieldReal.InternalMaxValue = 100;
			this.fieldDeep.TextFieldReal.Step = 5;
			this.fieldDeep.TextFieldReal.TextSuffix = "%";
			this.fieldDeep.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldDeep.TabIndex = 3;
			this.fieldDeep.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldDeep, Res.Strings.Panel.Regular.Tooltip.Deep);

			this.fieldTwirlAngle = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldTwirlAngle.LabelShortText = "T"; //Res.Strings.Panel.Regular.Short.Deep;
			this.fieldTwirlAngle.LabelLongText  = "Torsion"; //Res.Strings.Panel.Regular.Long.Deep;
			this.document.Modifier.AdaptTextFieldRealAngle(this.fieldTwirlAngle.TextFieldReal);
			this.fieldTwirlAngle.TextFieldReal.InternalMinValue = -90;
			this.fieldTwirlAngle.TextFieldReal.InternalMaxValue = 90;
			this.fieldTwirlAngle.TextFieldReal.Step = 1.0M;
			this.fieldTwirlAngle.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldTwirlAngle.TabIndex = 4;
			this.fieldTwirlAngle.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldTwirlAngle, Res.Strings.Panel.Regular.Tooltip.Deep);

			this.fieldTwirlAngleE1 = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldTwirlAngleE1.LabelShortText = "e1"; //Res.Strings.Panel.Regular.Short.Deep;
			this.fieldTwirlAngleE1.LabelLongText  = "Angle extérieur 1"; //Res.Strings.Panel.Regular.Long.Deep;
			this.document.Modifier.AdaptTextFieldRealAngle(this.fieldTwirlAngleE1.TextFieldReal);
			this.fieldTwirlAngleE1.TextFieldReal.InternalMinValue = -90;
			this.fieldTwirlAngleE1.TextFieldReal.InternalMaxValue = 90;
			this.fieldTwirlAngleE1.TextFieldReal.Step = 1.0M;
			this.fieldTwirlAngleE1.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldTwirlAngleE1.TabIndex = 4;
			this.fieldTwirlAngleE1.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldTwirlAngleE1, Res.Strings.Panel.Regular.Tooltip.Deep);

			this.fieldTwirlAngleE2 = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldTwirlAngleE2.LabelShortText = "e2"; //Res.Strings.Panel.Regular.Short.Deep;
			this.fieldTwirlAngleE2.LabelLongText  = "Angle extérieur 2"; //Res.Strings.Panel.Regular.Long.Deep;
			this.document.Modifier.AdaptTextFieldRealAngle(this.fieldTwirlAngleE2.TextFieldReal);
			this.fieldTwirlAngleE2.TextFieldReal.InternalMinValue = -90;
			this.fieldTwirlAngleE2.TextFieldReal.InternalMaxValue = 90;
			this.fieldTwirlAngleE2.TextFieldReal.Step = 1.0M;
			this.fieldTwirlAngleE2.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldTwirlAngleE2.TabIndex = 4;
			this.fieldTwirlAngleE2.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldTwirlAngleE2, Res.Strings.Panel.Regular.Tooltip.Deep);

			this.fieldTwirlAngleI1 = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldTwirlAngleI1.LabelShortText = "i1"; //Res.Strings.Panel.Regular.Short.Deep;
			this.fieldTwirlAngleI1.LabelLongText  = "Angle intérieur 1"; //Res.Strings.Panel.Regular.Long.Deep;
			this.document.Modifier.AdaptTextFieldRealAngle(this.fieldTwirlAngleI1.TextFieldReal);
			this.fieldTwirlAngleI1.TextFieldReal.InternalMinValue = -90;
			this.fieldTwirlAngleI1.TextFieldReal.InternalMaxValue = 90;
			this.fieldTwirlAngleI1.TextFieldReal.Step = 1.0M;
			this.fieldTwirlAngleI1.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldTwirlAngleI1.TabIndex = 4;
			this.fieldTwirlAngleI1.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldTwirlAngleI1, Res.Strings.Panel.Regular.Tooltip.Deep);

			this.fieldTwirlAngleI2 = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldTwirlAngleI2.LabelShortText = "i2"; //Res.Strings.Panel.Regular.Short.Deep;
			this.fieldTwirlAngleI2.LabelLongText  = "Angle intérieur 2"; //Res.Strings.Panel.Regular.Long.Deep;
			this.document.Modifier.AdaptTextFieldRealAngle(this.fieldTwirlAngleI2.TextFieldReal);
			this.fieldTwirlAngleI2.TextFieldReal.InternalMinValue = -90;
			this.fieldTwirlAngleI2.TextFieldReal.InternalMaxValue = 90;
			this.fieldTwirlAngleI2.TextFieldReal.Step = 1.0M;
			this.fieldTwirlAngleI2.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldTwirlAngleI2.TabIndex = 4;
			this.fieldTwirlAngleI2.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldTwirlAngleI2, Res.Strings.Panel.Regular.Tooltip.Deep);

			this.fieldTwirlScaleE1 = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldTwirlScaleE1.LabelShortText = "e1"; //Res.Strings.Panel.Regular.Short.Deep;
			this.fieldTwirlScaleE1.LabelLongText  = "Décalage extérieur 1"; //Res.Strings.Panel.Regular.Long.Deep;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldTwirlScaleE1.TextFieldReal);
			this.fieldTwirlScaleE1.TextFieldReal.InternalMinValue = -100;
			this.fieldTwirlScaleE1.TextFieldReal.InternalMaxValue = 100;
			this.fieldTwirlScaleE1.TextFieldReal.Step = 5;
			this.fieldTwirlScaleE1.TextFieldReal.TextSuffix = "%";
			this.fieldTwirlScaleE1.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldTwirlScaleE1.TabIndex = 4;
			this.fieldTwirlScaleE1.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldTwirlScaleE1, Res.Strings.Panel.Regular.Tooltip.Deep);

			this.fieldTwirlScaleE2 = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldTwirlScaleE2.LabelShortText = "e2"; //Res.Strings.Panel.Regular.Short.Deep;
			this.fieldTwirlScaleE2.LabelLongText  = "Décalage extérieur 2"; //Res.Strings.Panel.Regular.Long.Deep;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldTwirlScaleE2.TextFieldReal);
			this.fieldTwirlScaleE2.TextFieldReal.InternalMinValue = -100;
			this.fieldTwirlScaleE2.TextFieldReal.InternalMaxValue = 100;
			this.fieldTwirlScaleE2.TextFieldReal.Step = 5;
			this.fieldTwirlScaleE2.TextFieldReal.TextSuffix = "%";
			this.fieldTwirlScaleE2.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldTwirlScaleE2.TabIndex = 4;
			this.fieldTwirlScaleE2.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldTwirlScaleE2, Res.Strings.Panel.Regular.Tooltip.Deep);

			this.fieldTwirlScaleI1 = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldTwirlScaleI1.LabelShortText = "i1"; //Res.Strings.Panel.Regular.Short.Deep;
			this.fieldTwirlScaleI1.LabelLongText  = "Décalage intérieur 1"; //Res.Strings.Panel.Regular.Long.Deep;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldTwirlScaleI1.TextFieldReal);
			this.fieldTwirlScaleI1.TextFieldReal.InternalMinValue = -100;
			this.fieldTwirlScaleI1.TextFieldReal.InternalMaxValue = 100;
			this.fieldTwirlScaleI1.TextFieldReal.Step = 5;
			this.fieldTwirlScaleI1.TextFieldReal.TextSuffix = "%";
			this.fieldTwirlScaleI1.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldTwirlScaleI1.TabIndex = 4;
			this.fieldTwirlScaleI1.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldTwirlScaleI1, Res.Strings.Panel.Regular.Tooltip.Deep);

			this.fieldTwirlScaleI2 = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldTwirlScaleI2.LabelShortText = "i2"; //Res.Strings.Panel.Regular.Short.Deep;
			this.fieldTwirlScaleI2.LabelLongText  = "Décalage intérieur 2"; //Res.Strings.Panel.Regular.Long.Deep;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldTwirlScaleI2.TextFieldReal);
			this.fieldTwirlScaleI2.TextFieldReal.InternalMinValue = -100;
			this.fieldTwirlScaleI2.TextFieldReal.InternalMaxValue = 100;
			this.fieldTwirlScaleI2.TextFieldReal.Step = 5;
			this.fieldTwirlScaleI2.TextFieldReal.TextSuffix = "%";
			this.fieldTwirlScaleI2.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldTwirlScaleI2.TabIndex = 4;
			this.fieldTwirlScaleI2.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldTwirlScaleI2, Res.Strings.Panel.Regular.Tooltip.Deep);

			this.isNormalAndExtended = true;
		}

		protected void AddRadioIcon(int rank, string type)
		{
			this.grid.AddRadioIcon(Misc.Icon(Properties.Regular.GetIconText(type)), Properties.Regular.GetName(type), rank, false);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.grid.SelectionChanged -= new EventHandler(HandleTypeChanged);
				this.fieldNbFaces.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldDeep.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldTwirlAngle.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldTwirlAngleE1.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldTwirlAngleE2.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldTwirlAngleI1.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldTwirlAngleI2.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldTwirlScaleE1.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldTwirlScaleE2.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldTwirlScaleI1.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldTwirlScaleI2.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);

				this.grid = null;
				this.fieldNbFaces = null;
				this.fieldDeep = null;
				this.fieldTwirlAngle = null;
				this.fieldTwirlAngleE1 = null;
				this.fieldTwirlAngleE2 = null;
				this.fieldTwirlAngleI1 = null;
				this.fieldTwirlAngleI2 = null;
				this.fieldTwirlScaleE1 = null;
				this.fieldTwirlScaleE2 = null;
				this.fieldTwirlScaleI1 = null;
				this.fieldTwirlScaleI2 = null;
			}
			
			base.Dispose(disposing);
		}

		
		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau étendu ?
				{
					if ( this.IsLabelProperties )  // étendu/détails ?
					{
						h += 5+25*11;
					}
					else	// étendu/compact ?
					{
						h += 5+25*5;
					}
				}
				else	// panneau réduit ?
				{
					h += 30;
				}

				return h;
			}
		}

		protected override void PropertyToWidgets()
		{
			//	Propriété -> widgets.
			base.PropertyToWidgets();

			Properties.Regular p = this.property as Properties.Regular;
			if ( p == null )  return;

			this.ignoreChanged = true;

			int sv = 0;
			if (p.Star)  sv = 1;
			if (p.Flower)  sv = 2;
			this.grid.SelectedValue = sv;

			this.fieldNbFaces.TextFieldReal.InternalValue = p.NbFaces;
			this.fieldDeep.TextFieldReal.InternalValue = (decimal) p.Deep.R*100;
			this.fieldTwirlAngle.TextFieldReal.InternalValue = (decimal) p.Deep.A;
			this.fieldTwirlAngleE1.TextFieldReal.InternalValue = (decimal) p.E1.A;
			this.fieldTwirlAngleE2.TextFieldReal.InternalValue = (decimal) p.E2.A;
			this.fieldTwirlAngleI1.TextFieldReal.InternalValue = (decimal) p.I1.A;
			this.fieldTwirlAngleI2.TextFieldReal.InternalValue = (decimal) p.I2.A;
			this.fieldTwirlScaleE1.TextFieldReal.InternalValue = (decimal) p.E1.R*100;
			this.fieldTwirlScaleE2.TextFieldReal.InternalValue = (decimal) p.E2.R*100;
			this.fieldTwirlScaleI1.TextFieldReal.InternalValue = (decimal) p.I1.R*100;
			this.fieldTwirlScaleI2.TextFieldReal.InternalValue = (decimal) p.I2.R*100;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Regular p = this.property as Properties.Regular;
			if ( p == null )  return;

			if (this.grid.SelectedValue == 0)
			{
				p.Star = false;
				p.Flower = false;
			}
			else if (this.grid.SelectedValue == 1)
			{
				p.Star = true;
				p.Flower = false;
			}
			else if (this.grid.SelectedValue == 2)
			{
				p.Star = false;
				p.Flower = true;
			}

			p.NbFaces = (int)this.fieldNbFaces.TextFieldReal.InternalValue;
			p.Deep = new Polar((double) this.fieldDeep.TextFieldReal.InternalValue/100, (double) this.fieldTwirlAngle.TextFieldReal.InternalValue);
			p.E1 = new Polar((double) this.fieldTwirlScaleE1.TextFieldReal.InternalValue/100, (double) this.fieldTwirlAngleE1.TextFieldReal.InternalValue);
			p.E2 = new Polar((double) this.fieldTwirlScaleE2.TextFieldReal.InternalValue/100, (double) this.fieldTwirlAngleE2.TextFieldReal.InternalValue);
			p.I1 = new Polar((double) this.fieldTwirlScaleI1.TextFieldReal.InternalValue/100, (double) this.fieldTwirlAngleI1.TextFieldReal.InternalValue);
			p.I2 = new Polar((double) this.fieldTwirlScaleI2.TextFieldReal.InternalValue/100, (double) this.fieldTwirlAngleI2.TextFieldReal.InternalValue);
		}

		protected void EnableWidgets()
		{
			//	Grise les widgets nécessaires.
			bool star = (this.grid.SelectedValue == 1);
			bool flower = (this.grid.SelectedValue == 2);
			this.fieldDeep.Enable = (this.isExtendedSize && (star || flower));
			this.fieldTwirlAngle.Enable = (this.isExtendedSize && (star || flower));
			this.fieldTwirlAngleE1.Enable = (this.isExtendedSize && flower);
			this.fieldTwirlAngleE2.Enable = (this.isExtendedSize && flower);
			this.fieldTwirlAngleI1.Enable = (this.isExtendedSize && flower);
			this.fieldTwirlAngleI2.Enable = (this.isExtendedSize && flower);
			this.fieldTwirlScaleE1.Enable = (this.isExtendedSize && flower);
			this.fieldTwirlScaleE2.Enable = (this.isExtendedSize && flower);
			this.fieldTwirlScaleI1.Enable = (this.isExtendedSize && flower);
			this.fieldTwirlScaleI2.Enable = (this.isExtendedSize && flower);
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.grid == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Width = 22*3;
			r.Inflate(1);
			this.grid.SetManualBounds(r);

			if ( this.isExtendedSize && this.IsLabelProperties )
			{
				r = rect;
				r.Bottom = r.Top-20;
				r.Left = rect.Left+22*3;
				r.Right = rect.Right;
				this.fieldNbFaces.SetManualBounds(r);

				rect.Top = r.Bottom-5;
				rect.Bottom = rect.Top-20;
				r = rect;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldDeep.SetManualBounds(r);

				rect.Top = r.Bottom-5;
				rect.Bottom = rect.Top-20;
				r = rect;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldTwirlAngle.SetManualBounds(r);

				rect.Top = r.Bottom-5;
				rect.Bottom = rect.Top-20;
				r = rect;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldTwirlAngleE1.SetManualBounds(r);

				rect.Top = r.Bottom-5;
				rect.Bottom = rect.Top-20;
				r = rect;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldTwirlAngleE2.SetManualBounds(r);

				rect.Top = r.Bottom-5;
				rect.Bottom = rect.Top-20;
				r = rect;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldTwirlAngleI1.SetManualBounds(r);

				rect.Top = r.Bottom-5;
				rect.Bottom = rect.Top-20;
				r = rect;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldTwirlAngleI2.SetManualBounds(r);

				rect.Top = r.Bottom-5;
				rect.Bottom = rect.Top-20;
				r = rect;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldTwirlScaleE1.SetManualBounds(r);

				rect.Top = r.Bottom-5;
				rect.Bottom = rect.Top-20;
				r = rect;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldTwirlScaleE2.SetManualBounds(r);

				rect.Top = r.Bottom-5;
				rect.Bottom = rect.Top-20;
				r = rect;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldTwirlScaleI1.SetManualBounds(r);

				rect.Top = r.Bottom-5;
				rect.Bottom = rect.Top-20;
				r = rect;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldTwirlScaleI2.SetManualBounds(r);
			}
			else
			{
				r = rect;
				r.Bottom = r.Top-20;
				r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth;
				r.Right = rect.Right;
				this.fieldNbFaces.SetManualBounds(r);

				r.Top = rect.Top-25;
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fieldDeep.SetManualBounds(r);

				r.Left = r.Right;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fieldTwirlAngle.SetManualBounds(r);

				r.Left = r.Right;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fieldTwirlAngleE1.SetManualBounds(r);

				r.Left = r.Right;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fieldTwirlAngleE2.SetManualBounds(r);

				r.Left = r.Right;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fieldTwirlAngleI1.SetManualBounds(r);

				r.Left = r.Right;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fieldTwirlAngleI2.SetManualBounds(r);

				r.Left = r.Right;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fieldTwirlScaleE1.SetManualBounds(r);

				r.Left = r.Right;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fieldTwirlScaleE2.SetManualBounds(r);

				r.Left = r.Right;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fieldTwirlScaleI1.SetManualBounds(r);

				r.Left = r.Right;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fieldTwirlScaleI2.SetManualBounds(r);
			}
		}
		
		private void HandleFieldChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		private void HandleTypeChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;
			this.EnableWidgets();
			this.OnChanged();
		}


		protected RadioIconGrid				grid;
		protected Widgets.TextFieldLabel	fieldNbFaces;
		protected Widgets.TextFieldLabel	fieldDeep;
		protected Widgets.TextFieldLabel	fieldTwirlAngle;
		protected Widgets.TextFieldLabel	fieldTwirlAngleE1;
		protected Widgets.TextFieldLabel	fieldTwirlAngleE2;
		protected Widgets.TextFieldLabel	fieldTwirlAngleI1;
		protected Widgets.TextFieldLabel	fieldTwirlAngleI2;
		protected Widgets.TextFieldLabel	fieldTwirlScaleE1;
		protected Widgets.TextFieldLabel	fieldTwirlScaleE2;
		protected Widgets.TextFieldLabel	fieldTwirlScaleI1;
		protected Widgets.TextFieldLabel	fieldTwirlScaleI2;
	}
}
