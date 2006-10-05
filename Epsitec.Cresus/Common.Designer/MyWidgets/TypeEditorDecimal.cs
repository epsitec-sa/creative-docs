using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'�diter un Caption.Type.
	/// </summary>
	public class TypeEditorDecimal : AbstractTypeEditor
	{
		public TypeEditorDecimal()
		{
			Widget group;

			Widget band = new Widget(this);
			band.TabIndex = this.tabIndex++;
			band.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			band.Dock = DockStyle.StackBegin;
			band.Margins = new Margins(0, 0, 0, 15);
			band.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			Widget left = new Widget(band);
			left.TabIndex = this.tabIndex++;
			left.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			left.Dock = DockStyle.Fill;

			Widget right = new Widget(band);
			right.TabIndex = this.tabIndex++;
			right.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			right.Dock = DockStyle.Fill;

			//	Range.
			this.CreateDecimalLabeled("Valeur minimale", left, out group, out this.fieldMin);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldMin.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled("Valeur maximale", left, out group, out this.fieldMax);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldMax.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled("R�solution", left, out group, out this.fieldRes);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 0);
			this.fieldRes.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			//	PreferredRange.
			this.CreateDecimalLabeled("Valeur minimale pr�f�rentielle", right, out group, out this.fieldPreferredMin);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldPreferredMin.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled("Valeur maximale pr�f�rentielle", right, out group, out this.fieldPreferredMax);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldPreferredMax.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled("R�solution pr�f�rentielle", right, out group, out this.fieldPreferredRes);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 0);
			this.fieldPreferredRes.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			//	Steps.
			this.CreateDecimalLabeled("Petit pas", this, out group, out this.fieldSmallStep);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldSmallStep.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled("Grand pas", this, out group, out this.fieldLargeStep);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 0);
			this.fieldLargeStep.TextChanged += new EventHandler(this.HandleTextFieldChanged);
		}
		
		public TypeEditorDecimal(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldMin.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldMax.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldRes.TextChanged -= new EventHandler(this.HandleTextFieldChanged);

				this.fieldPreferredMin.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldPreferredMax.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldPreferredRes.TextChanged -= new EventHandler(this.HandleTextFieldChanged);

				this.fieldSmallStep.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldLargeStep.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
			}
			
			base.Dispose(disposing);
		}


		protected override void UpdateContent()
		{
			//	Met � jour le contenu de l'�diteur.
			DecimalType type = this.type as DecimalType;

			this.ignoreChange = true;

			if (type.Range.IsEmpty)
			{
				this.fieldMin.Text = "";
				this.fieldMax.Text = "";
				this.fieldRes.Text = "";
			}
			else
			{
				this.SetDecimal(this.fieldMin, type.Range.Minimum);
				this.SetDecimal(this.fieldMax, type.Range.Maximum);
				this.SetDecimal(this.fieldRes, type.Range.Resolution);
			}

			if (type.PreferredRange.IsEmpty)
			{
				this.fieldPreferredMin.Text = "";
				this.fieldPreferredMax.Text = "";
				this.fieldPreferredRes.Text = "";
			}
			else
			{
				this.SetDecimal(this.fieldPreferredMin, type.PreferredRange.Minimum);
				this.SetDecimal(this.fieldPreferredMax, type.PreferredRange.Maximum);
				this.SetDecimal(this.fieldPreferredRes, type.PreferredRange.Resolution);
			}

			this.SetDecimal(this.fieldSmallStep, type.SmallStep);
			this.SetDecimal(this.fieldLargeStep, type.LargeStep);

			this.ignoreChange = false;
		}


		private void HandleTextFieldChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

			DecimalType type = this.type as DecimalType;

			decimal min = type.Range.Minimum;
			decimal max = type.Range.Maximum;
			decimal res = type.Range.Resolution;

			decimal pmin = type.PreferredRange.Minimum;
			decimal pmax = type.PreferredRange.Maximum;
			decimal pres = type.PreferredRange.Resolution;

			//	Range.
			if (sender == this.fieldMin)
			{
				min = this.GetDecimal(this.fieldMin);
			}

			if (sender == this.fieldMax)
			{
				max = this.GetDecimal(this.fieldMax);
			}

			if (sender == this.fieldRes)
			{
				res = this.GetDecimal(this.fieldRes);
			}

			//	PreferredRange.
			if (sender == this.fieldPreferredMin)
			{
				pmin = this.GetDecimal(this.fieldPreferredMin);
			}

			if (sender == this.fieldPreferredMax)
			{
				pmax = this.GetDecimal(this.fieldPreferredMax);
			}

			if (sender == this.fieldPreferredRes)
			{
				pres = this.GetDecimal(this.fieldPreferredRes);
			}

			//	Steps.
			if (sender == this.fieldSmallStep)
			{
				type.DefineSmallStep(this.GetDecimal(this.fieldSmallStep));
			}

			if (sender == this.fieldLargeStep)
			{
				type.DefineLargeStep(this.GetDecimal(this.fieldLargeStep));
			}

			if (min == 0 && max == 0 && res == 0)
			{
				type.DefineRange(new DecimalRange());
			}
			else
			{
				type.DefineRange(new DecimalRange(min, max, res));
			}

			if (pmin == 0 && pmax == 0 && pres == 0)
			{
				type.DefinePreferredRange(new DecimalRange());
			}
			else
			{
				type.DefinePreferredRange(new DecimalRange(pmin, pmax, pres));
			}

			this.OnContentChanged();
		}
		

		protected TextField						fieldMin;
		protected TextField						fieldMax;
		protected TextField						fieldRes;
		protected TextField						fieldPreferredMin;
		protected TextField						fieldPreferredMax;
		protected TextField						fieldPreferredRes;
		protected TextField						fieldSmallStep;
		protected TextField						fieldLargeStep;
	}
}
