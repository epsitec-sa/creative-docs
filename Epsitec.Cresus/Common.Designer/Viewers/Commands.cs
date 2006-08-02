using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Commands : AbstractCaptions
	{
		public Commands(Module module, PanelsContext context, ResourceAccess access) : base(module, context, access)
		{
			MyWidgets.StackedPanel leftContainer, rightContainer;

			//	Statefull.
			this.CreateBand(out leftContainer, out rightContainer, "Type", 0.1);

			this.primaryStatefull = new CheckButton(leftContainer.Container);
			this.primaryStatefull.Text = "Reflète un état";
			this.primaryStatefull.Dock = DockStyle.StackBegin;
			this.primaryStatefull.TabIndex = this.tabIndex++;
			this.primaryStatefull.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Shortcuts.
			this.CreateBand(out leftContainer, out rightContainer, "Raccourcis clavier", 0.3);

			//	Group.
			this.CreateBand(out leftContainer, out rightContainer, "Groupe", 0.5);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}


		public override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Commands;
			}
		}


		public override void Update()
		{
			//	Met à jour le contenu du Viewer.
			base.Update();
		}

		protected override void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			base.UpdateEdit();
		}


		protected override void TextFieldToIndex(AbstractTextField textField, out int field, out int subfield)
		{
			//	Cherche les index correspondant à un texte éditable.
			base.TextFieldToIndex(textField, out field, out subfield);
		}

		protected override AbstractTextField IndexToTextField(int field, int subfield)
		{
			//	Cherche le TextField permettant d'éditer des index.
			return base.IndexToTextField(field, subfield);
		}


		protected CheckButton					primaryStatefull;
	}
}
