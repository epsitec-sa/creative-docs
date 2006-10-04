using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'�diter un Caption.Type.
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
			//	Cr�e le bon widget AbstractTypeEditor pour �diter un type.
			switch (typeType)
			{
				case ResourceAccess.TypeType.Decimal:  return new TypeEditorDecimal();
				case ResourceAccess.TypeType.String:   return new TypeEditorString();
				case ResourceAccess.TypeType.Enum:     return new TypeEditorEnum();
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
			//	Met � jour le contenu de l'�diteur.
		}


		#region Super widgets
		protected void CreateDecimalLabeled(string label, Widget parent, out Widget group, out TextField field)
		{
			//	Cr�e un super-widget permettant d'�diter une valeur d�cimale, avec une �tiquette � gauche.
			group = new Widget(parent);
			group.TabIndex = this.tabIndex++;
			group.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			StaticText text = new StaticText(group);
			text.Text = label;
			text.ContentAlignment = ContentAlignment.MiddleRight;
			text.PreferredWidth = 200;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 8, 0, 0);

			field = new TextField(group);
			field.PreferredWidth = 80;
			field.Dock = DockStyle.Left;
			field.TabIndex = 0;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
		}

		protected decimal GetDecimal(TextField field)
		{
			decimal value;
			if (!decimal.TryParse(field.Text, out value))
			{
				return 0;
			}
			return value;
		}

		protected void SetDecimal(TextField field, decimal value)
		{
			field.Text = value.ToString();
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
		protected int							tabIndex = 0;
		protected bool							ignoreChange = false;
	}
}
