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
		public ProxyForm(Viewers.Abstract viewer) : base(viewer)
		{
		}

		public override IEnumerable<Panel> ProxyPanels
		{
			get
			{
				//	L'ordre n'a aucune importance.
				yield return Panel.FormGeometry;
				yield return Panel.FormBox;
				yield return Panel.FormFontLabel;
				yield return Panel.FormFontField;
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

				case Panel.FormFontLabel:
					yield return Type.FormLabelFontColor;
					yield return Type.FormLabelFontFace;
					yield return Type.FormLabelFontStyle;
					yield return Type.FormLabelFontSize;
					break;

				case Panel.FormFontField:
					yield return Type.FormFieldFontColor;
					yield return Type.FormFieldFontFace;
					yield return Type.FormFieldFontStyle;
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
			panel.Name = proxyPanel.ToString();
			panel.Icon = this.GetIcon(proxyPanel);
			panel.Title = this.GetTitle(proxyPanel);
			panel.IsExtendedSize = this.viewer.PanelsContext.IsExtendedProxies(panel.Name);
			panel.ExtendedSize += new EventHandler(this.HandlePanelExtendedSize);

			List<Type> list = new List<Type>(this.ValueTypes(proxyPanel));
			list.Sort();  // trie les valeurs dans le panneau

			foreach (Type valueType in list)
			{
				AbstractValue value = AbstractProxy.IndexOf(values, valueType);
				if (value != null)
				{
					double space = 3;

					if (valueType == Type.FormColumnsRequired)
					{
						space = -1;  // la valeur suivante sera collé à celle-çi
					}

					Widget widget = value.CreateInterface(panel.Container);
					widget.Dock = DockStyle.Top;
					widget.Margins = new Margins(0, 0, 0, space);
				}
			}

			return panel;
		}

		private void HandlePanelExtendedSize(object sender)
		{
			MyWidgets.PropertyPanel panel = sender as MyWidgets.PropertyPanel;
			System.Diagnostics.Debug.Assert(panel != null);

			this.viewer.PanelsContext.SetExtendedProxies(panel.Name, panel.IsExtendedSize);
		}


		protected override string GetIcon(Panel proxyPanel)
		{
			switch (proxyPanel)
			{
				case Panel.FormGeometry:
					return "PropertyGeometry";

				case Panel.FormBox:
					return "PropertyPadding";

				case Panel.FormFontLabel:
				case Panel.FormFontField:
					return "PropertyTextFont";

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

				case Panel.FormFontLabel:
					return "Police étiquette";

				case Panel.FormFontField:
					return "Police champ";

				case Panel.FormStyle:
					return "Style";
			}

			return null;
		}
	}
}
