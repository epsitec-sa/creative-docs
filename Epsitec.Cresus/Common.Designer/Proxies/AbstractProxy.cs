using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	/// <summary>
	/// Cette classe détermine l'ensemble des valeurs représentées par un proxy, qui sera matérialisé par un panneau.
	/// </summary>
	public abstract class AbstractProxy
	{
		public virtual IEnumerable<AbstractObjectManager.Type> ProxyTypes
		{
			get
			{
				return null;
			}
		}

		public virtual IEnumerable<AbstractObjectManager.Type> ValueTypes(AbstractObjectManager.Type proxyType)
		{
			return null;
		}

		public virtual Widget CreateInterface(Widget parent, AbstractObjectManager.Type proxyType, List<AbstractValue> values)
		{
			return null;
		}

		protected virtual string GetIcon(AbstractObjectManager.Type proxyType)
		{
			return null;
		}

		protected virtual string GetTitle(AbstractObjectManager.Type proxyType)
		{
			return null;
		}

		static protected AbstractValue IndexOf(List<AbstractValue> values, AbstractObjectManager.Type valueType)
		{
			foreach (AbstractValue value in values)
			{
				if (value.Type == valueType)
				{
					return value;
				}
			}

			return null;
		}
	}
}
