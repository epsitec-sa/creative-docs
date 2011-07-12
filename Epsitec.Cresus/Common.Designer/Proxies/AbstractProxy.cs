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
			//	L'ordre détermine l'ordre d'apparition des panneaux, de haut en bas.
			None,

			FormGeometry,
			FormBox,
			FormFont,
			FormAspect,

			PanelContent,
			PanelGeometry,
			PanelLayout,
			PanelGrid,
			PanelAspect,
		}

		public enum Type
		{
			//	L'ordre détermine l'ordre d'apparition dans un panneau.
			None,

			//	FormGeometry
			FormSeparatorBottom,
			FormColumnsRequired,
			FormRowsRequired,
			FormLineWidth,
			FormPreferredWidth,

			//	FormBox
			FormBoxLayout,
			FormBoxPadding,
			FormBoxFrameEdges,
			FormBoxFrameWidth,

			//	FormFontLabel
			FormLabelFontSize,		// de droite à gauche !
			FormLabelFontStyle,
			FormLabelFontFace,
			FormLabelFontColor,

			//	FormFontField
			FormFieldFontSize,		// de droite à gauche !
			FormFieldFontStyle,
			FormFieldFontFace,
			FormFieldFontColor,

			//	FormStyle
			FormBackColor,
			FormButtonClass,
			FormVerbosity,
			FormLabelReplacement,

			//	PanelContent
			PanelDruidCaption,
			PanelDruidPanel,
			PanelBinding,
			PanelTableColumns,
			PanelStructuredType,

			//	PanelGeometry
			PanelMargins,
			PanelOriginX,
			PanelOriginY,
			PanelWidth,
			PanelHeight,
			PanelPadding,
			
			//	PanelLayout
			PanelChildrenPlacement,
			PanelAnchoredHorizontalAttachment,
			PanelAnchoredVerticalAttachment,
			PanelStackedHorizontalAttachment,
			PanelStackedVerticalAttachment,
			PanelStackedHorizontalAlignment,
			PanelStackedVerticalAlignment,
			
			//	PanelGrid
			PanelGridColumnsCount,
			PanelGridRowsCount,
			PanelGridColumnSpan,
			PanelGridRowSpan,
			PanelGridColumnMode,
			PanelGridColumnWidth,
			PanelGridColumnMinWidth,
			PanelGridColumnMaxWidth,
			PanelGridLeftBorder,
			PanelGridRightBorder,
			PanelGridRowMode,
			PanelGridRowHeight,
			PanelGridRowMinHeight,
			PanelGridRowMaxHeight,
			PanelGridTopBorder,
			PanelGridBottomBorder,

			//	PanelAspect
			PanelButtonAspect,
		}


		public AbstractProxy(Viewers.Abstract viewer)
		{
			this.viewer = viewer;
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


		protected void HandlePanelExtendedSize(object sender)
		{
			MyWidgets.PropertyPanel panel = sender as MyWidgets.PropertyPanel;
			System.Diagnostics.Debug.Assert(panel != null);

			this.viewer.PanelsContext.SetExtendedProxies(panel.Name, panel.IsExtendedSize);
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


		protected Viewers.Abstract viewer;
	}
}
