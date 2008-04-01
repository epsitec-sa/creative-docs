using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	/// <summary>
	/// Cette classe détermine l'ensemble des valeurs représentées par un proxy, qui sera matérialisé par un panneau.
	/// </summary>
	public class ProxyForm : AbstractProxy
	{
		public override IEnumerable<AbstractObjectManager.Type> ProxyTypes
		{
			get
			{
				yield return AbstractObjectManager.Type.FormGeometry;
				yield return AbstractObjectManager.Type.FormStyle;
			}
		}

		public override IEnumerable<AbstractObjectManager.Type> ValueTypes(AbstractObjectManager.Type proxyType)
		{
			switch (proxyType)
			{
				case AbstractObjectManager.Type.FormGeometry:
					yield return AbstractObjectManager.Type.FormColumnsRequired;
					yield return AbstractObjectManager.Type.FormRowsRequired;
					yield return AbstractObjectManager.Type.FormPreferredWidth;
					yield return AbstractObjectManager.Type.FormSeparatorBottom;
					break;

				case AbstractObjectManager.Type.FormStyle:
					yield return AbstractObjectManager.Type.FormBackColor;
					yield return AbstractObjectManager.Type.FormLabelFontColor;
					yield return AbstractObjectManager.Type.FormFieldFontColor;
					break;
			}
		}

		public override Widget CreateInterface(Widget parent, AbstractObjectManager.Type proxyType, List<AbstractValue> values)
		{
			//	Crée un panneau complet.
			MyWidgets.PropertyPanel panel = new MyWidgets.PropertyPanel(parent);
			panel.Icon = this.GetIcon(proxyType);
			panel.Title = this.GetTitle(proxyType);
			panel.IsExtendedSize = true;

			foreach (AbstractObjectManager.Type valueType in this.ValueTypes(proxyType))
			{
				AbstractValue value = AbstractProxy.IndexOf(values, valueType);
				if (value != null)
				{
					Widget widget = value.CreateInterface(panel.Container);
					widget.Dock = DockStyle.Top;
					widget.Margins = new Margins(0, 0, 0, 1);
				}
			}

			return panel;
		}

		protected override string GetIcon(AbstractObjectManager.Type proxyType)
		{
			switch (proxyType)
			{
				case AbstractObjectManager.Type.FormGeometry:
					return "PropertyGeometry";

				case AbstractObjectManager.Type.FormStyle:
					return "PropertyAspect";
			}

			return null;
		}

		protected override string GetTitle(AbstractObjectManager.Type proxyType)
		{
			switch (proxyType)
			{
				case AbstractObjectManager.Type.FormGeometry:
					return "Géométrie";

				case AbstractObjectManager.Type.FormStyle:
					return "Style";
			}

			return null;
		}
	}
}
