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

			this.CreateDoubleLabeled("Longueur minimale", 0, 1000, 1, 1, this, out group, out this.fieldMin);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 5);
			this.fieldMin.TextChanged += new EventHandler(this.HandleTextFieldRealChanged);

			this.CreateDoubleLabeled("Longueur maximale", 0, 1000, 1, 1, this, out group, out this.fieldMax);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 0);
			this.fieldMax.TextChanged += new EventHandler(this.HandleTextFieldRealChanged);
		}

		public TypeEditorString(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldMin.TextChanged -= new EventHandler(this.HandleTextFieldRealChanged);
				this.fieldMax.TextChanged -= new EventHandler(this.HandleTextFieldRealChanged);
			}
			
			base.Dispose(disposing);
		}


		protected override void UpdateContent()
		{
			StringType type = this.type as StringType;

			this.ignoreChange = true;
			this.fieldMin.InternalValue = (decimal) type.MinimumLength;
			this.fieldMax.InternalValue = (decimal) type.MaximumLength;
			this.ignoreChange = false;
		}


		void HandleTextFieldRealChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

			StringType type = this.type as StringType;

			if (sender == this.fieldMin)
			{
				type.DefineMinimumLength((int) this.fieldMin.InternalValue);
			}

			if (sender == this.fieldMax)
			{
				type.DefineMaximumLength((int) this.fieldMax.InternalValue);
			}

			this.OnContentChanged();
		}
		

		protected TextFieldReal					fieldMin;
		protected TextFieldReal					fieldMax;
	}
}
