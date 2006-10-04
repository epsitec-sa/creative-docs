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
			//	Met � jour le contenu de l'�diteur.
		}


		#region Super widgets
		protected void CreateDecimalLabeled(string label, decimal min, decimal max, decimal resol, decimal step, Widget parent, out Widget group, out TextFieldReal field)
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

			field = new TextFieldReal(group);
			field.InternalMinValue = min;
			field.InternalMaxValue = max;
			field.Resolution       = resol;
			field.Step             = step;
			field.PreferredWidth = 100;
			field.Dock = DockStyle.Left;
			field.TabIndex = 0;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
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
