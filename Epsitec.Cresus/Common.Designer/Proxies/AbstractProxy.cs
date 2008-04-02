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
		public enum Panel
		{
			None,

			FormGeometry,
			FormBox,
			FormFont,
			FormStyle,
		}

		public enum Type
		{
			None,

			FormColumnsRequired,
			FormRowsRequired,
			FormPreferredWidth,
			FormSeparatorBottom,
			FormBoxLayout,
			FormBoxPadding,
			FormBoxFrameState,
			FormBoxFrameWidth,
			FormBackColor,
			FormLabelFontColor,
			FormFieldFontColor,
			FormLabelFontFace,
			FormFieldFontFace,
			FormLabelFontStyle,
			FormFieldFontStyle,
			FormLabelFontSize,
			FormFieldFontSize,
			FormButtonClass,
		}


		public virtual IEnumerable<Panel> ProxyPanels
		{
			get
			{
				return null;
			}
		}

		public virtual IEnumerable<Type> ValueTypes(Panel proxyPanel)
		{
			return null;
		}

		public virtual Widget CreateInterface(Widget parent, Panel proxyPanel, List<AbstractValue> values)
		{
			return null;
		}

		protected virtual string GetIcon(Panel proxyPanel)
		{
			return null;
		}

		protected virtual string GetTitle(Panel proxyPanel)
		{
			return null;
		}

		static protected AbstractValue IndexOf(List<AbstractValue> values, Type valueType)
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
