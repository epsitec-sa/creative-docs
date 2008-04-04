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
				yield return Panel.PanelPadding;
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
					yield return Type.PanelLeftMargin;
					yield return Type.PanelRightMargin;
					yield return Type.PanelTopMargin;
					yield return Type.PanelBottomMargin;
					yield return Type.PanelOriginX;
					yield return Type.PanelOriginY;
					yield return Type.PanelWidth;
					yield return Type.PanelHeight;
					break;

				case Panel.PanelPadding:
					yield return Type.PanelLeftPadding;
					yield return Type.PanelRightPadding;
					yield return Type.PanelTopPadding;
					yield return Type.PanelBottomPadding;
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
					yield return Type.PanelGridColumnValue;
					yield return Type.PanelGridMinWidth;
					yield return Type.PanelGridMaxWidth;
					yield return Type.PanelGridLeftBorder;
					yield return Type.PanelGridRightBorder;
					yield return Type.PanelGridRowMode;
					yield return Type.PanelGridRowValue;
					yield return Type.PanelGridMinHeight;
					yield return Type.PanelGridMaxHeight;
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
			panel.ExtendedSize += new EventHandler(this.HandlePanelExtendedSize);

			List<Type> list = new List<Type>(this.ValueTypes(proxyPanel));
			list.Sort();  // trie les valeurs dans le panneau

			foreach (Type valueType in list)
			{
				AbstractValue value = AbstractProxy.IndexOf(values, valueType);
				if (value != null)
				{
					double space = 3;

					if (valueType == Type.PanelLeftMargin ||
						valueType == Type.PanelRightMargin ||
						valueType == Type.PanelTopMargin ||
						valueType == Type.PanelOriginX ||
						valueType == Type.PanelWidth)
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

				case Panel.PanelPadding:
					return "PropertyPadding";

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

				case Panel.PanelPadding:
					return "Marges";

				case Panel.PanelLayout:
					return "Mise en page";

				case Panel.PanelGrid:
					return "Grille";
			}

			return null;
		}
	}
}
