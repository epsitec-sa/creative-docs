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
		public override IEnumerable<Panel> ProxyPanels
		{
			get
			{
				//	L'ordre n'a aucune importance.
				yield return Panel.FormGeometry;
				yield return Panel.FormBox;
				yield return Panel.FormFont;
				yield return Panel.FormStyle;
			}
		}

		public override IEnumerable<Type> ValueTypes(Panel proxyPanel)
		{
			switch (proxyPanel)
			{
				//	L'ordre n'a aucune importance.
				case Panel.FormGeometry:
					yield return Type.FormColumnsRequired;
					yield return Type.FormRowsRequired;
					yield return Type.FormPreferredWidth;
					yield return Type.FormSeparatorBottom;
					break;

				case Panel.FormBox:
					yield return Type.FormBoxLayout;
					yield return Type.FormBoxPadding;
					yield return Type.FormBoxFrameState;
					yield return Type.FormBoxFrameWidth;
					break;

				case Panel.FormFont:
					yield return Type.FormLabelFontColor;
					yield return Type.FormFieldFontColor;
					yield return Type.FormLabelFontFace;
					yield return Type.FormFieldFontFace;
					yield return Type.FormLabelFontStyle;
					yield return Type.FormFieldFontStyle;
					yield return Type.FormLabelFontSize;
					yield return Type.FormFieldFontSize;
					break;

				case Panel.FormStyle:
					yield return Type.FormBackColor;
					yield return Type.FormButtonClass;
					break;
			}
		}

		public override Widget CreateInterface(Widget parent, Panel proxyPanel, List<AbstractValue> values)
		{
			//	Crée un panneau complet.
			MyWidgets.PropertyPanel panel = new MyWidgets.PropertyPanel(parent);
			panel.Icon = this.GetIcon(proxyPanel);
			panel.Title = this.GetTitle(proxyPanel);
			panel.IsExtendedSize = true;

			List<Type> list = new List<Type>(this.ValueTypes(proxyPanel));
			list.Sort();  // trie les valeurs dans le panneau

			foreach (Type valueType in list)
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

		protected override string GetIcon(Panel proxyPanel)
		{
			switch (proxyPanel)
			{
				case Panel.FormGeometry:
					return "PropertyGeometry";

				case Panel.FormBox:
					return "PropertyLayout";

				case Panel.FormFont:
					return "PropertyPadding";

				case Panel.FormStyle:
					return "PropertyAspect";
			}

			return null;
		}

		protected override string GetTitle(Panel proxyPanel)
		{
			switch (proxyPanel)
			{
				case Panel.FormGeometry:
					return "Géométrie";

				case Panel.FormBox:
					return "Groupe";

				case Panel.FormFont:
					return "Police";

				case Panel.FormStyle:
					return "Style";
			}

			return null;
		}
	}
}
