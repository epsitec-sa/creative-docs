using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	/// <summary>
	/// Cette classe permet de représenter la valeur d'une propriété d'un objet reflété.
	/// </summary>
	public abstract class AbstractValue : Types.DependencyObject
	{
		public string Name
		{
			//	Nom interne de la valeur. Chaque valeur doit avoir un nom différent.
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
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


		public virtual Widget CreateInterface(Widget parent)
		{
			//	Crée les widgets permettant d'éditer la valeur.
			return null;
		}

		protected virtual void UpdateInterface()
		{
			//	Met à jour la valeur dans l'interface.
		}


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


		protected string name;
		protected string label;
		protected object value;
		protected bool ignoreChange;
	}
}
