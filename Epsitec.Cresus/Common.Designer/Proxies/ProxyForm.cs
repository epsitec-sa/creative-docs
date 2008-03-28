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
		public override IEnumerable<string> ProxyNames
		{
			get
			{
				yield return "Geometry";
				yield return "Style";
			}
		}

		public override IEnumerable<string> ValueNames(string proxyName)
		{
			switch (proxyName)
			{
				case "Geometry":
					yield return "ColumnsRequired";
					yield return "RowsRequired";
					yield return "PreferredWidth";
					yield return "SeparatorBottom";
					break;

				case "Style":
					yield return "BackColor";
					yield return "LabelFontColor";
					yield return "FieldFontColor";
					break;
			}
		}

		public override Widget CreateInterface(Widget parent, string proxyName, List<AbstractValue> values)
		{
			//	Crée un panneau complet.
			MyWidgets.PropertyPanel panel = new MyWidgets.PropertyPanel(parent);
			panel.Icon = this.GetIcon(proxyName);
			panel.Title = this.GetTitle(proxyName);
			panel.IsExtendedSize = true;

			foreach (string valueName in this.ValueNames(proxyName))
			{
				AbstractValue value = AbstractProxy.IndexOf(values, valueName);
				if (value != null)
				{
					Widget widget = value.CreateInterface(panel.Container);
					widget.Dock = DockStyle.Top;
					widget.Margins = new Margins(0, 0, 0, 1);
				}
			}

			return panel;
		}

		protected override string GetIcon(string proxyName)
		{
			switch (proxyName)
			{
				case "Geometry":
					return "PropertyGeometry";

				case "Style":
					return "PropertyAspect";
			}

			return null;
		}

		protected override string GetTitle(string proxyName)
		{
			switch (proxyName)
			{
				case "Geometry":
					return "Géométrie";

				case "Style":
					return "Style";
			}

			return null;
		}
	}
}
