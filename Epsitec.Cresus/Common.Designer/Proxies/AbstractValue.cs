using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	/// <summary>
	/// Cette classe permet de représenter la valeur d'une propriété d'un objet reflété.
	/// </summary>
	public abstract class AbstractValue : Types.DependencyObject, System.IComparable
	{
		public AbstractValue()
		{
			this.selectedObjects = new List<Widget>();
		}

		public AbstractProxy.Type Type
		{
			//	Type interne de la valeur.
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public string Label
		{
			//	Nom visible de la valeur.
			get
			{
				return this.label;
			}
			set
			{
				this.label = value;
			}
		}

		public Types.Caption Caption
		{
			//	Nom visible de la valeur.
			get
			{
				return this.caption;
			}
			set
			{
				this.caption = value;
			}
		}

		public object Value
		{
			//	Valeur de n'importe quel type contenue dans l'instance de la classe.
			//	Si la valeur est changée, un événement 'ValueChanged' est généré.
			get
			{
				return this.value;
			}
			set
			{
				if (this.value != value)
				{
					this.value = value;
					this.UpdateInterface();
					this.OnValueChanged();
				}
			}
		}


		public List<Widget> SelectedObjects
		{
			get
			{
				return this.selectedObjects;
			}
		}

		public Widget WidgetInterface
		{
			//	Retourne le widget parent de l'interface permettant d'éditer la valeur.
			get
			{
				return this.widgetInterface;
			}
		}

		public virtual Widget CreateInterface(Widget parent)
		{
			//	Crée les widgets permettant d'éditer la valeur.
			return null;
		}

		protected virtual void UpdateInterface()
		{
			//	Met à jour la valeur dans l'interface.
		}


		#region IComparable Members
		public int CompareTo(object obj)
		{
			AbstractValue that = obj as AbstractValue;
			return this.type.CompareTo(that.type);
		}
		#endregion


		#region Events handler
		protected virtual void OnValueChanged()
		{
			//	Génère un événement pour dire que la valeur a été changée.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("ValueChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Support.EventHandler ValueChanged
		{
			add
			{
				this.AddUserEventHandler("ValueChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ValueChanged", value);
			}
		}
		#endregion


		protected AbstractProxy.Type type;
		protected Widget widgetInterface;
		protected string label;
		protected Types.Caption caption;
		protected object value;
		protected List<Widget> selectedObjects;
		protected bool ignoreChange;
	}
}
