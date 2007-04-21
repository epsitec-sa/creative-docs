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

			this.fieldDeep = new Widgets.TextFieldPolar(this);
			this.fieldDeep.LabelText = Res.Strings.Panel.Regular.Label.Deep;
			this.fieldDeep.TextFieldR.InternalMinValue = 0.0M;
			this.fieldDeep.TextFieldR.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldDeep.TextFieldA.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldDeep.TabIndex = 2;
			this.fieldDeep.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldDeep, Res.Strings.Panel.Regular.Tooltip.Deep);

			this.fieldE1 = new Widgets.TextFieldPolar(this);
			this.fieldE1.LabelText = Res.Strings.Panel.Regular.Label.E1;
			this.fieldE1.TextFieldR.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldE1.TextFieldA.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldE1.TabIndex = 3;
			this.fieldE1.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldE1, Res.Strings.Panel.Regular.Tooltip.E1);

			this.fieldE2 = new Widgets.TextFieldPolar(this);
			this.fieldE2.LabelText = Res.Strings.Panel.Regular.Label.E2;
			this.fieldE2.TextFieldR.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldE2.TextFieldA.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldE2.TabIndex = 4;
			this.fieldE2.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldE2, Res.Strings.Panel.Regular.Tooltip.E2);

			this.fieldI1 = new Widgets.TextFieldPolar(this);
			this.fieldI1.LabelText = Res.Strings.Panel.Regular.Label.I1;
			this.fieldI1.TextFieldR.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldI1.TextFieldA.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldI1.TabIndex = 5;
			this.fieldI1.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldI1, Res.Strings.Panel.Regular.Tooltip.I1);

			this.fieldI2 = new Widgets.TextFieldPolar(this);
			this.fieldI2.LabelText = Res.Strings.Panel.Regular.Label.I2;
			this.fieldI2.TextFieldR.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldI2.TextFieldA.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldI2.TabIndex = 6;
			this.fieldI2.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldI2, Res.Strings.Panel.Regular.Tooltip.I2);

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
				this.fieldDeep.TextFieldR.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldDeep.TextFieldA.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldE1.TextFieldR.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldE1.TextFieldA.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldE2.TextFieldR.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldE2.TextFieldA.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldI1.TextFieldR.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldI1.TextFieldA.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldI2.TextFieldR.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldI2.TextFieldA.EditionAccepted -= new EventHandler(this.HandleFieldChanged);

				this.grid = null;
				this.fieldNbFaces = null;
				this.fieldDeep = null;
				this.fieldE1 = null;
				this.fieldE2 = null;
				this.fieldI1 = null;
				this.fieldI2 = null;
			}
			
			base.Dispose(disposing);
		}

		
		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return ( this.isExtendedSize ? this.LabelHeight+5+25*6 : this.LabelHeight+5+25*1 );
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

			this.fieldDeep.TextFieldR.InternalValue = (decimal) p.Deep.R*100;
			this.fieldDeep.TextFieldA.InternalValue = (decimal) p.Deep.A;

			this.fieldE1.TextFieldR.InternalValue = (decimal) p.E1.R*100;
			this.fieldE1.TextFieldA.InternalValue = (decimal) p.E1.A;

			this.fieldE2.TextFieldR.InternalValue = (decimal) p.E2.R*100;
			this.fieldE2.TextFieldA.InternalValue = (decimal) p.E2.A;

			this.fieldI1.TextFieldR.InternalValue = (decimal) p.I1.R*100;
			this.fieldI1.TextFieldA.InternalValue = (decimal) p.I1.A;

			this.fieldI2.TextFieldR.InternalValue = (decimal) p.I2.R*100;
			this.fieldI2.TextFieldA.InternalValue = (decimal) p.I2.A;

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
			p.Deep = new Polar((double) this.fieldDeep.TextFieldR.InternalValue/100, (double) this.fieldDeep.TextFieldA.InternalValue);
			p.E1 = new Polar((double) this.fieldE1.TextFieldR.InternalValue/100, (double) this.fieldE1.TextFieldA.InternalValue);
			p.E2 = new Polar((double) this.fieldE2.TextFieldR.InternalValue/100, (double) this.fieldE2.TextFieldA.InternalValue);
			p.I1 = new Polar((double) this.fieldI1.TextFieldR.InternalValue/100, (double) this.fieldI1.TextFieldA.InternalValue);
			p.I2 = new Polar((double) this.fieldI2.TextFieldR.InternalValue/100, (double) this.fieldI2.TextFieldA.InternalValue);
		}

		protected void EnableWidgets()
		{
			//	Grise les widgets nécessaires.
			bool star = (this.grid.SelectedValue == 1);
			bool flower = (this.grid.SelectedValue == 2);
			this.fieldDeep.Enable = (this.isExtendedSize && (star || flower));
			this.fieldE1.Enable = (this.isExtendedSize && flower);
			this.fieldE2.Enable = (this.isExtendedSize && flower);
			this.fieldI1.Enable = (this.isExtendedSize && flower);
			this.fieldI2.Enable = (this.isExtendedSize && flower);
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

			rect.Top = r.Bottom-10;
			rect.Bottom = rect.Top-20;
			r = rect;
			r.Left = rect.Left;
			r.Right = rect.Right;
			this.fieldI1.SetManualBounds(r);

			rect.Top = r.Bottom+1;
			rect.Bottom = rect.Top-20;
			r = rect;
			r.Left = rect.Left;
			r.Right = rect.Right;
			this.fieldI2.SetManualBounds(r);

			rect.Top = r.Bottom-10;
			rect.Bottom = rect.Top-20;
			r = rect;
			r.Left = rect.Left;
			r.Right = rect.Right;
			this.fieldE1.SetManualBounds(r);

			rect.Top = r.Bottom+1;
			rect.Bottom = rect.Top-20;
			r = rect;
			r.Left = rect.Left;
			r.Right = rect.Right;
			this.fieldE2.SetManualBounds(r);
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
		protected Widgets.TextFieldPolar	fieldDeep;
		protected Widgets.TextFieldPolar	fieldE1;
		protected Widgets.TextFieldPolar	fieldE2;
		protected Widgets.TextFieldPolar	fieldI1;
		protected Widgets.TextFieldPolar	fieldI2;
	}
}
