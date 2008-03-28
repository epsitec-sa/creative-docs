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
		public virtual IEnumerable<string> ProxyNames
		{
			get
			{
				return null;
			}
		}

		public virtual IEnumerable<string> ValueNames(string proxyName)
		{
			return null;
		}

		public virtual Widget CreateInterface(Widget parent, string proxyName, List<AbstractValue> values)
		{
			return null;
		}

		protected virtual string GetIcon(string proxyName)
		{
			return null;
		}

		protected virtual string GetTitle(string proxyName)
		{
			return null;
		}

		static protected AbstractValue IndexOf(List<AbstractValue> values, string valueName)
		{
			foreach (AbstractValue value in values)
			{
				if (value.Name == valueName)
				{
					return value;
				}
			}

			return null;
		}
	}
}
