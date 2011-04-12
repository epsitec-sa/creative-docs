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
		public AbstractValue(DesignerApplication application)
		{
			this.application = application;
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

		public bool HasHiddenLabel
		{
			//	Permet d'occuper une place minimale, sans étiquette.
			get
			{
				return this.hasHiddenLabel;
			}
			set
			{
				this.hasHiddenLabel = value;
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
				//	'this.value' et 'value' sont de type 'object'. Ils peuvent contenir des 'double', des 'int'
				//	ou des énumérations, donc des types par valeur. Mais lorsqu'un type par valeur est vu à travers
				//	un 'object', il est vu comme un type par référence. Donc, 'this.value != value' compare les
				//	références, ce qui n'est pas l'effet escompté ici. D'où l'utilisation de Equals, qui compare
				//	bit à bit les valeurs.
				if (!object.Equals(this.value, value))
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
			var handler = this.GetUserEventHandler("ValueChanged");
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


		protected DesignerApplication application;
		protected AbstractProxy.Type type;
		protected Widget widgetInterface;
		protected string label;
		protected bool hasHiddenLabel;
		protected Types.Caption caption;
		protected object value;
		protected List<Widget> selectedObjects;
		protected bool ignoreChange;
	}
}
