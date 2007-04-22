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

			this.AddRadioIcon(Properties.RegularType.Norm);
			this.AddRadioIcon(Properties.RegularType.Star);
			this.AddRadioIcon(Properties.RegularType.Flower1);
			this.AddRadioIcon(Properties.RegularType.Flower2);

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

			this.fieldSamples = new TextFieldCombo(this);
			this.fieldSamples.IsReadOnly = true;
			foreach (Sample sample in Regular.Samples)
			{
				this.fieldSamples.Items.Add(sample.Text);
			}
			this.fieldSamples.TextChanged += new EventHandler(this.HandleFieldSamplesTextChanged);
			this.fieldSamples.TabIndex = 7;

			this.isNormalAndExtended = true;
		}

		protected void AddRadioIcon(Properties.RegularType type)
		{
			this.grid.AddRadioIcon(Misc.Icon(Properties.Regular.GetIconText(type)), Properties.Regular.GetName(type), (int)type, false);
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
				return ( this.isExtendedSize ? this.LabelHeight+5+25*7 : this.LabelHeight+5+25*1 );
			}
		}

		protected override void PropertyToWidgets()
		{
			//	Propriété -> widgets.
			base.PropertyToWidgets();

			Properties.Regular p = this.property as Properties.Regular;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.grid.SelectedValue = (int) p.RegularType;

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

			this.fieldSamples.Text = "Personnalisé";

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Regular p = this.property as Properties.Regular;
			if ( p == null )  return;

			p.RegularType = (Properties.RegularType) this.grid.SelectedValue;
			p.NbFaces = (int)this.fieldNbFaces.TextFieldReal.InternalValue;
			p.Deep = new Polar((double) this.fieldDeep.TextFieldR.InternalValue/100, (double) this.fieldDeep.TextFieldA.InternalValue);
			p.E1 = new Polar((double) this.fieldE1.TextFieldR.InternalValue/100, (double) this.fieldE1.TextFieldA.InternalValue);
			p.E2 = new Polar((double) this.fieldE2.TextFieldR.InternalValue/100, (double) this.fieldE2.TextFieldA.InternalValue);
			p.I1 = new Polar((double) this.fieldI1.TextFieldR.InternalValue/100, (double) this.fieldI1.TextFieldA.InternalValue);
			p.I2 = new Polar((double) this.fieldI2.TextFieldR.InternalValue/100, (double) this.fieldI2.TextFieldA.InternalValue);

			this.ignoreChanged = true;
			this.fieldSamples.Text = "Personnalisé";
			this.ignoreChanged = false;
		}

		protected void EnableWidgets()
		{
			//	Grise les widgets nécessaires.
			bool star = (this.grid.SelectedValue == 1);
			bool flower1 = (this.grid.SelectedValue == 2);
			bool flower2 = (this.grid.SelectedValue == 3);

			this.fieldDeep.Enable = (this.isExtendedSize && (star || flower1 || flower2));
			this.fieldE1.Enable = (this.isExtendedSize && flower1 || flower2);
			this.fieldE2.Enable = (this.isExtendedSize && flower2);
			this.fieldI1.Enable = (this.isExtendedSize && flower1 || flower2);
			this.fieldI2.Enable = (this.isExtendedSize && flower2);
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
			r.Width = 22*4;
			r.Inflate(1);
			this.grid.SetManualBounds(r);

			r = rect;
			r.Bottom = r.Top-20;
			r.Left = rect.Left+22*4;
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

			rect.Top = r.Bottom-5;
			rect.Bottom = rect.Top-20;
			r = rect;
			r.Left = rect.Left;
			r.Right = rect.Right;
			this.fieldSamples.SetManualBounds(r);
		}
		
		private void HandleFieldChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;

			Properties.Regular p = this.property as Properties.Regular;

			if (p.RegularType == Properties.RegularType.Flower1)
			{
				this.ignoreChanged = true;

				if (sender == this.fieldE1.TextFieldR)
				{
					this.fieldE2.TextFieldR.InternalValue = this.fieldE1.TextFieldR.InternalValue;
				}

				if (sender == this.fieldE1.TextFieldA)
				{
					this.fieldE2.TextFieldA.InternalValue = -this.fieldE1.TextFieldA.InternalValue;
				}

				if (sender == this.fieldI1.TextFieldR)
				{
					this.fieldI2.TextFieldR.InternalValue = this.fieldI1.TextFieldR.InternalValue;
				}

				if (sender == this.fieldI1.TextFieldA)
				{
					this.fieldI2.TextFieldA.InternalValue = -this.fieldI1.TextFieldA.InternalValue;
				}

				this.ignoreChanged = false;
			}

			this.OnChanged();
		}

		private void HandleTypeChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;
			this.EnableWidgets();
			this.OnChanged();
		}

		private void HandleFieldSamplesTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			TextFieldCombo field = sender as TextFieldCombo;
			foreach (Sample sample in Regular.Samples)
			{
				if (field.Text == sample.Text)
				{
					this.grid.SelectedValue = (int) sample.RegularType;

					this.fieldNbFaces.TextFieldReal.InternalValue = (decimal) sample.NbFaces;

					this.fieldDeep.TextFieldR.InternalValue = (decimal) sample.Deep.R*100;
					this.fieldDeep.TextFieldA.InternalValue = (decimal) sample.Deep.A;
					
					this.fieldE1.TextFieldR.InternalValue = (decimal) sample.E1.R*100;
					this.fieldE1.TextFieldA.InternalValue = (decimal) sample.E1.A;
					
					this.fieldE2.TextFieldR.InternalValue = (decimal) sample.E2.R*100;
					this.fieldE2.TextFieldA.InternalValue = (decimal) sample.E2.A;
					
					this.fieldI1.TextFieldR.InternalValue = (decimal) sample.I1.R*100;
					this.fieldI1.TextFieldA.InternalValue = (decimal) sample.I1.A;
					
					this.fieldI2.TextFieldR.InternalValue = (decimal) sample.I2.R*100;
					this.fieldI2.TextFieldA.InternalValue = (decimal) sample.I2.A;
					
					this.OnChanged();

					this.ignoreChanged = true;
					field.Text = sample.Text;
					this.ignoreChanged = false;

					return;
				}
			}
		}


		#region Sample
		protected struct Sample
		{
			public Sample(string text, Properties.RegularType regularType, int nbFaces, Polar deep, Polar i1, Polar i2, Polar e1, Polar e2)
			{
				this.Text = text;
				this.RegularType = regularType;
				this.NbFaces = nbFaces;
				this.Deep = deep;
				this.I1 = i1;
				this.I2 = i2;
				this.E1 = e1;
				this.E2 = e2;
			}

			public string					Text;
			public Properties.RegularType	RegularType;
			public int						NbFaces;
			public Polar					Deep;
			public Polar					I1;
			public Polar					I2;
			public Polar					E1;
			public Polar					E2;
		}

		static protected Sample[] Samples =
		{
			new Sample("Triangle équilatéral", Properties.RegularType.Norm,     3, new Polar(0.50,   0.0), new Polar( 0.60,  10.0), new Polar( 0.60, -10.0), new Polar(-0.05,  20.0), new Polar(-0.05, -20.0)),
			new Sample("Carré",                Properties.RegularType.Norm,     4, new Polar(0.50,   0.0), new Polar( 0.60,  10.0), new Polar( 0.60, -10.0), new Polar(-0.05,  20.0), new Polar(-0.05, -20.0)),
			new Sample("Hexagone",             Properties.RegularType.Norm,     6, new Polar(0.50,   0.0), new Polar( 0.60,  10.0), new Polar( 0.60, -10.0), new Polar(-0.05,  20.0), new Polar(-0.05, -20.0)),
			new Sample("Octogone",             Properties.RegularType.Norm,     8, new Polar(0.50,   0.0), new Polar( 0.60,  10.0), new Polar( 0.60, -10.0), new Polar(-0.05,  20.0), new Polar(-0.05, -20.0)),
			new Sample("Etoile de David",      Properties.RegularType.Star,     6, new Polar(0.42,   0.0), new Polar( 0.60,  10.0), new Polar( 0.60, -10.0), new Polar(-0.05,  20.0), new Polar(-0.05, -20.0)),
			new Sample("Scie circulaire",      Properties.RegularType.Star,    24, new Polar(0.05,  -8.0), new Polar( 0.60,  10.0), new Polar( 0.60, -10.0), new Polar(-0.05,  20.0), new Polar(-0.05, -20.0)),
			new Sample("Soleil droit",         Properties.RegularType.Star,    20, new Polar(0.65,   0.0), new Polar( 0.60,  10.0), new Polar( 0.60, -10.0), new Polar(-0.05,  20.0), new Polar(-0.05, -20.0)),
			new Sample("Soleil tournoyant",    Properties.RegularType.Flower2, 20, new Polar(0.65, -50.0), new Polar( 0.55,  -8.0), new Polar( 0.40,  -8.0), new Polar( 0.40,  -3.0), new Polar( 0.25,   1.0)),
			new Sample("Fleur symétrique 1",   Properties.RegularType.Flower1,  6, new Polar(0.50,   0.0), new Polar( 0.60,  10.0), new Polar( 0.60, -10.0), new Polar(-0.05,  20.0), new Polar(-0.05, -20.0)),
			new Sample("Fleur symétrique 2",   Properties.RegularType.Flower1,  4, new Polar(0.80,   0.0), new Polar( 0.25,  30.0), new Polar( 0.25, -30.0), new Polar(-0.10,  30.0), new Polar(-0.10, -30.0)),
			new Sample("Fleur asymétrique 1",  Properties.RegularType.Flower2,  6, new Polar(0.85,  20.0), new Polar( 0.40, -16.0), new Polar( 0.00, -48.0), new Polar( 0.05,  15.0), new Polar(-0.40, -33.0)),
			new Sample("Fleur asymétrique 2",  Properties.RegularType.Flower2,  8, new Polar(0.45, -10.0), new Polar( 0.50, -10.0), new Polar(-0.10, -44.0), new Polar( 0.30,  16.0), new Polar(-0.65, -38.0)),
			new Sample("Trèfle à 4 feuilles",  Properties.RegularType.Flower1,  4, new Polar(0.80,   0.0), new Polar( 0.70,   0.0), new Polar( 0.70,   0.0), new Polar(-0.75,  75.0), new Polar(-0.75, -75.0)),
			new Sample("Hélice de bateau",     Properties.RegularType.Flower2,  3, new Polar(0.80, -30.0), new Polar( 0.40,  30.0), new Polar( 0.70,   0.0), new Polar(-0.30,  50.0), new Polar(-0.40, -75.0)),
			new Sample("Hélice d'avion",       Properties.RegularType.Flower2,  4, new Polar(0.90,   0.0), new Polar( 0.50, -10.0), new Polar( 0.40, -10.0), new Polar(-0.05,  10.0), new Polar( 0.05,  -5.0)),
		};
		#endregion


		protected RadioIconGrid				grid;
		protected Widgets.TextFieldLabel	fieldNbFaces;
		protected Widgets.TextFieldPolar	fieldDeep;
		protected Widgets.TextFieldPolar	fieldE1;
		protected Widgets.TextFieldPolar	fieldE2;
		protected Widgets.TextFieldPolar	fieldI1;
		protected Widgets.TextFieldPolar	fieldI2;
		protected TextFieldCombo			fieldSamples;
	}
}
