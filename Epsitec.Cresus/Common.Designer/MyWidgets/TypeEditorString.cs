using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'éditer un Caption.Type.
	/// </summary>
	public class TypeEditorString : AbstractTypeEditor
	{
		public TypeEditorString()
		{
			Widget group;

			this.CreateDecimalLabeled("Longueur minimale", this, out group, out this.fieldMin);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldMin.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled("Longueur maximale", this, out group, out this.fieldMax);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 0);
			this.fieldMax.TextChanged += new EventHandler(this.HandleTextFieldChanged);
		}

		public TypeEditorString(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldMin.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldMax.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
			}
			
			base.Dispose(disposing);
		}


		protected override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
			StringType type = this.type as StringType;

			this.ignoreChange = true;
			this.SetDecimal(this.fieldMin, type.MinimumLength);
			this.SetDecimal(this.fieldMax, type.MaximumLength);
			this.ignoreChange = false;
		}


		private void HandleTextFieldChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

			StringType type = this.type as StringType;

			if (sender == this.fieldMin)
			{
				type.DefineMinimumLength((int) this.GetDecimal(this.fieldMin));
			}

			if (sender == this.fieldMax)
			{
				type.DefineMaximumLength((int) this.GetDecimal(this.fieldMax));
			}

			this.OnContentChanged();
		}
		

		protected TextField						fieldMin;
		protected TextField						fieldMax;
	}
}
