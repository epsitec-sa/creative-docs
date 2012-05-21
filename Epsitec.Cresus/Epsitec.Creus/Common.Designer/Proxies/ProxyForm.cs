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
				yield return Panel.FormFont;
				yield return Panel.FormAspect;
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
					yield return Type.FormLineWidth;
					yield return Type.FormPreferredWidth;
					yield return Type.FormSeparatorBottom;
					break;

				case Panel.FormBox:
					yield return Type.FormBoxLayout;
					yield return Type.FormBoxPadding;
					yield return Type.FormBoxFrameEdges;
					yield return Type.FormBoxFrameWidth;
					break;

				case Panel.FormFont:
					yield return Type.FormLabelFontColor;
					yield return Type.FormLabelFontFace;
					yield return Type.FormLabelFontStyle;
					yield return Type.FormLabelFontSize;

					yield return Type.FormFieldFontColor;
					yield return Type.FormFieldFontFace;
					yield return Type.FormFieldFontStyle;
					yield return Type.FormFieldFontSize;
					break;

				case Panel.FormAspect:
					yield return Type.FormBackColor;
					yield return Type.FormButtonClass;
					yield return Type.FormVerbosity;
					yield return Type.FormLabelReplacement;
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

			FrameBox group = null;
			foreach (Type valueType in list)
			{
				AbstractValue value = AbstractProxy.IndexOf(values, valueType);
				if (value != null)
				{
					double space = 3;

					if (valueType == Type.FormColumnsRequired)
					{
						space = -1;  // la valeur suivante sera collée à celle-çi
					}

					if (this.IsBeginOfGroup(valueType))
					{
						group = new FrameBox(panel.Container);
						group.Dock = DockStyle.Top;
						group.Margins = new Margins(0, 0, 0, space);

						StaticText label = new StaticText(group);
						label.Text = this.GetGroupLabel(valueType);
						label.ContentAlignment = ContentAlignment.MiddleRight;
						label.Margins = new Margins(0, 5, 0, 8);
						label.Dock = DockStyle.Fill;

						Widget widget = value.CreateInterface(group);
						widget.Dock = DockStyle.Right;
						widget.Margins = new Margins(2, 0, 0, 0);
					}
					else if (this.IsInsideGroup(valueType))
					{
						Widget widget = value.CreateInterface(group);
						widget.Dock = DockStyle.Right;
						widget.Margins = new Margins(2, 0, 0, 0);
					}
					else
					{
						Widget widget = value.CreateInterface(panel.Container);
						widget.Dock = DockStyle.Top;
						widget.Margins = new Margins(0, 0, 0, space);
					}
				}
			}

			return panel;
		}

		protected string GetGroupLabel(Type type)
		{
			//	Retourne le texte pour l'étiquette du premier élément d'un groupe.
			//	Attention, les groupes sont formés de droite à gauche (Size, Style, Face, Color),
			//	à cause de mode DockStyle.Right !
			switch (type)
			{
				case Type.FormLabelFontSize:
					return "Texte fixe";

				case Type.FormFieldFontSize:
					return "Champ";
			}

			return null;
		}

		protected bool IsBeginOfGroup(Type type)
		{
			//	Indique s'il s'agit du premier élément d'un groupe.
			return type == Type.FormLabelFontSize ||
				   type == Type.FormFieldFontSize;
		}

		protected bool IsInsideGroup(Type type)
		{
			//	Indique s'il s'agit du nième (2..n) élément d'un groupe.
			return type == Type.FormLabelFontStyle || type == Type.FormLabelFontFace || type == Type.FormLabelFontColor ||
				   type == Type.FormFieldFontStyle || type == Type.FormFieldFontFace || type == Type.FormFieldFontColor;
		}


		protected override string GetIcon(Panel proxyPanel)
		{
			switch (proxyPanel)
			{
				case Panel.FormGeometry:
					return "PropertyGeometry";

				case Panel.FormBox:
					return "PropertyPadding";

				case Panel.FormFont:
					return "PropertyTextFont";

				case Panel.FormAspect:
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

				case Panel.FormAspect:
					return "Aspect";
			}

			return null;
		}
	}
}
