using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'éditer un Caption.Type.
	/// </summary>
	public abstract class AbstractTypeEditor : AbstractGroup
	{
		public AbstractTypeEditor()
		{
		}
		
		public AbstractTypeEditor(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		public static AbstractTypeEditor Create(ResourceAccess.TypeType typeType)
		{
			//	Crée le bon widget AbstractTypeEditor pour éditer un type.
			switch (typeType)
			{
				case ResourceAccess.TypeType.Decimal:
					return new TypeEditorDecimal();
				case ResourceAccess.TypeType.String:
					return new TypeEditorString();
			}

			return null;
		}


		public AbstractType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				if (this.type != value)
				{
					this.type = value;
					this.UpdateContent();
				}
			}
		}

		protected virtual void UpdateContent()
		{
		}


		#region Super widgets
		protected void CreateDoubleLabeled(string label, double min, double max, double def, double step, double resol, Widget parent, out Widget group, out TextFieldReal field)
		{
			group = new Widget(parent);

			StaticText text = new StaticText(group);
			text.Text = label;
			text.PreferredWidth = 120;
			text.Dock = DockStyle.Left;

			field = new TextFieldReal(group);
			field.InternalMinValue     = (decimal) min;
			field.InternalMaxValue     = (decimal) max;
			field.InternalDefaultValue = (decimal) def;
			field.Step                 = (decimal) step;
			field.Resolution           = (decimal) resol;
			field.PreferredWidth = 60;
			field.Dock = DockStyle.Left;
		}
		#endregion


		#region Events
		protected virtual void OnContentChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("ContentChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler ContentChanged
		{
			add
			{
				this.AddUserEventHandler("ContentChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ContentChanged", value);
			}
		}
		#endregion


		protected AbstractType					type;
		protected bool							ignoreChange = false;
	}
}
