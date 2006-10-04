using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'éditer un Caption.Type.
	/// </summary>
	public class TypeEditorDecimal : AbstractTypeEditor
	{
		public TypeEditorDecimal()
		{
			Widget group;

			//	Range.
			this.CreateDecimalLabeled("Valeur minimale", this, out group, out this.fieldMin);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 5);
			this.fieldMin.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled("Valeur maximale", this, out group, out this.fieldMax);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 5);
			this.fieldMax.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled("Résolution", this, out group, out this.fieldRes);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 15);
			this.fieldRes.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			//	PreferredRange.
			this.CreateDecimalLabeled("Valeur minimale préférentielle", this, out group, out this.fieldPreferredMin);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 5);
			this.fieldPreferredMin.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled("Valeur maximale préférentielle", this, out group, out this.fieldPreferredMax);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 5);
			this.fieldPreferredMax.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled("Résolution préférentielle", this, out group, out this.fieldPreferredRes);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 0);
			this.fieldPreferredRes.TextChanged += new EventHandler(this.HandleTextFieldChanged);
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
			}
			
			base.Dispose(disposing);
		}


		protected override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
			DecimalType type = this.type as DecimalType;

			this.ignoreChange = true;

			this.SetDecimal(this.fieldMin, type.Range.Minimum);
			this.SetDecimal(this.fieldMax, type.Range.Maximum);
			this.SetDecimal(this.fieldRes, type.Range.Resolution);

			this.SetDecimal(this.fieldPreferredMin, type.PreferredRange.Minimum);
			this.SetDecimal(this.fieldPreferredMax, type.PreferredRange.Maximum);
			this.SetDecimal(this.fieldPreferredRes, type.PreferredRange.Resolution);

			this.ignoreChange = false;
		}


		void HandleTextFieldChanged(object sender)
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

			type.DefineRange(new DecimalRange(min, max, res));
			type.DefinePreferredRange(new DecimalRange(pmin, pmax, pres));

			this.OnContentChanged();
		}
		

		protected TextField						fieldMin;
		protected TextField						fieldMax;
		protected TextField						fieldRes;
		protected TextField						fieldPreferredMin;
		protected TextField						fieldPreferredMax;
		protected TextField						fieldPreferredRes;
	}
}
