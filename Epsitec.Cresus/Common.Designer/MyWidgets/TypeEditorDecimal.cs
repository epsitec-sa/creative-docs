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
			this.CreateDecimalLabeled("Valeur minimale", -1000000, 1000000, 0.0001M, 1, this, out group, out this.fieldMin);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 5);
			this.fieldMin.TextChanged += new EventHandler(this.HandleTextFieldRealChanged);

			this.CreateDecimalLabeled("Valeur maximale", -1000000, 1000000, 0.0001M, 1, this, out group, out this.fieldMax);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 5);
			this.fieldMax.TextChanged += new EventHandler(this.HandleTextFieldRealChanged);

			this.CreateDecimalLabeled("Résolution", 0, 1000, 0.0001M, 0.0001M, this, out group, out this.fieldRes);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 15);
			this.fieldRes.TextChanged += new EventHandler(this.HandleTextFieldRealChanged);

			//	PreferredRange.
			this.CreateDecimalLabeled("Valeur minimale préférentielle", -1000000, 1000000, 0.0001M, 1, this, out group, out this.fieldPreferredMin);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 5);
			this.fieldPreferredMin.TextChanged += new EventHandler(this.HandleTextFieldRealChanged);

			this.CreateDecimalLabeled("Valeur maximale préférentielle", -1000000, 1000000, 0.0001M, 1, this, out group, out this.fieldPreferredMax);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 5);
			this.fieldPreferredMax.TextChanged += new EventHandler(this.HandleTextFieldRealChanged);

			this.CreateDecimalLabeled("Résolution préférentielle", 0, 1000, 0.0001M, 0.0001M, this, out group, out this.fieldPreferredRes);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 0);
			this.fieldPreferredRes.TextChanged += new EventHandler(this.HandleTextFieldRealChanged);
		}
		
		public TypeEditorDecimal(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldMin.TextChanged -= new EventHandler(this.HandleTextFieldRealChanged);
				this.fieldMax.TextChanged -= new EventHandler(this.HandleTextFieldRealChanged);
				this.fieldRes.TextChanged -= new EventHandler(this.HandleTextFieldRealChanged);

				this.fieldPreferredMin.TextChanged -= new EventHandler(this.HandleTextFieldRealChanged);
				this.fieldPreferredMax.TextChanged -= new EventHandler(this.HandleTextFieldRealChanged);
				this.fieldPreferredRes.TextChanged -= new EventHandler(this.HandleTextFieldRealChanged);
			}
			
			base.Dispose(disposing);
		}


		protected override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
			DecimalType type = this.type as DecimalType;

			this.ignoreChange = true;

			this.fieldMin.InternalValue = type.Range.Minimum;
			this.fieldMax.InternalValue = type.Range.Maximum;
			this.fieldRes.InternalValue = type.Range.Resolution;

			this.fieldPreferredMin.InternalValue = type.PreferredRange.Minimum;
			this.fieldPreferredMax.InternalValue = type.PreferredRange.Maximum;
			this.fieldPreferredRes.InternalValue = type.PreferredRange.Resolution;

			this.ignoreChange = false;
		}


		void HandleTextFieldRealChanged(object sender)
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
				min = this.fieldMin.InternalValue;
			}

			if (sender == this.fieldMax)
			{
				max = this.fieldMax.InternalValue;
			}

			if (sender == this.fieldRes)
			{
				res = this.fieldRes.InternalValue;
			}

			//	PreferredRange.
			if (sender == this.fieldPreferredMin)
			{
				pmin = this.fieldPreferredMin.InternalValue;
			}

			if (sender == this.fieldPreferredMax)
			{
				pmax = this.fieldPreferredMax.InternalValue;
			}

			if (sender == this.fieldPreferredRes)
			{
				pres = this.fieldPreferredRes.InternalValue;
			}

			type.DefineRange(new DecimalRange(min, max, res));
			type.DefinePreferredRange(new DecimalRange(pmin, pmax, pres));

			this.OnContentChanged();
		}
		

		protected TextFieldReal					fieldMin;
		protected TextFieldReal					fieldMax;
		protected TextFieldReal					fieldRes;
		protected TextFieldReal					fieldPreferredMin;
		protected TextFieldReal					fieldPreferredMax;
		protected TextFieldReal					fieldPreferredRes;
	}
}
