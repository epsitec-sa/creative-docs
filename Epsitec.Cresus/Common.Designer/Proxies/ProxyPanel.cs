using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	/// <summary>
	/// Cette classe détermine l'ensemble des valeurs représentées par un proxy, qui sera matérialisé par un panneau.
	/// </summary>
	public class ProxyPanel : AbstractProxy
	{
		public ProxyPanel(Viewers.Abstract viewer) : base(viewer)
		{
		}

		public override IEnumerable<Panel> ProxyPanels
		{
			get
			{
				//	L'ordre n'a aucune importance.
				yield return Panel.PanelContent;
				yield return Panel.PanelAspect;
				yield return Panel.PanelGeometry;
				yield return Panel.PanelLayout;
				yield return Panel.PanelGrid;
			}
		}

		public override IEnumerable<Type> ValueTypes(Panel proxyPanel)
		{
			switch (proxyPanel)
			{
				//	L'ordre n'a aucune importance.
				case Panel.PanelContent:
					yield return Type.PanelDruidCaption;
					yield return Type.PanelDruidPanel;
					yield return Type.PanelBinding;
					yield return Type.PanelTableColumns;
					yield return Type.PanelStructuredType;
					break;

				case Panel.PanelAspect:
					yield return Type.PanelButtonAspect;
					break;

				case Panel.PanelGeometry:
					yield return Type.PanelMargins;
					yield return Type.PanelOriginX;
					yield return Type.PanelOriginY;
					yield return Type.PanelWidth;
					yield return Type.PanelHeight;
					yield return Type.PanelPadding;
					break;

				case Panel.PanelLayout:
					yield return Type.PanelChildrenPlacement;
					yield return Type.PanelAnchoredHorizontalAttachment;
					yield return Type.PanelAnchoredVerticalAttachment;
					yield return Type.PanelStackedHorizontalAttachment;
					yield return Type.PanelStackedVerticalAttachment;
					yield return Type.PanelStackedHorizontalAlignment;
					yield return Type.PanelStackedVerticalAlignment;
					break;

				case Panel.PanelGrid:
					yield return Type.PanelGridColumnsCount;
					yield return Type.PanelGridRowsCount;
					yield return Type.PanelGridColumnSpan;
					yield return Type.PanelGridRowSpan;
					yield return Type.PanelGridColumnMode;
					yield return Type.PanelGridColumnWidth;
					yield return Type.PanelGridColumnMinWidth;
					yield return Type.PanelGridColumnMaxWidth;
					yield return Type.PanelGridLeftBorder;
					yield return Type.PanelGridRightBorder;
					yield return Type.PanelGridRowMode;
					yield return Type.PanelGridRowHeight;
					yield return Type.PanelGridRowMinHeight;
					yield return Type.PanelGridRowMaxHeight;
					yield return Type.PanelGridTopBorder;
					yield return Type.PanelGridBottomBorder;
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
			panel.ExtendedSize += this.HandlePanelExtendedSize;

			List<Type> list = new List<Type>(this.ValueTypes(proxyPanel));
			list.Sort();  // trie les valeurs dans le panneau

			foreach (Type valueType in list)
			{
				AbstractValue value = AbstractProxy.IndexOf(values, valueType);
				if (value != null)
				{
					double space = 3;

					if (valueType == Type.PanelOriginX ||
						valueType == Type.PanelWidth ||
						valueType == Type.PanelGridColumnsCount ||
						valueType == Type.PanelGridColumnMinWidth ||
						valueType == Type.PanelGridLeftBorder ||
						valueType == Type.PanelGridRowMinHeight ||
						valueType == Type.PanelGridTopBorder)
					{
						space = -1;  // la valeur suivante sera collée à celle-çi
					}

					Widget widget = value.CreateInterface(panel.Container);
					widget.Dock = DockStyle.Top;
					widget.Margins = new Margins(0, 0, 0, space);
				}
			}

			return panel;
		}


		protected override string GetIcon(Panel proxyPanel)
		{
			switch (proxyPanel)
			{
				case Panel.PanelContent:
					return "PropertyContent";

				case Panel.PanelAspect:
					return "PropertyAspect";

				case Panel.PanelGeometry:
					return "PropertyGeometry";

				case Panel.PanelLayout:
					return "PropertyLayout";

				case Panel.PanelGrid:
					return "PropertyGrid";
			}

			return null;
		}

		protected override string GetTitle(Panel proxyPanel)
		{
			switch (proxyPanel)
			{
				case Panel.PanelContent:
					return "Contenu";

				case Panel.PanelAspect:
					return "Aspect";

				case Panel.PanelGeometry:
					return "Géométrie";

				case Panel.PanelLayout:
					return "Mise en page";

				case Panel.PanelGrid:
					return "Grille";
			}

			return null;
		}
	}
}
