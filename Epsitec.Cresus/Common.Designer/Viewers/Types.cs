using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de repr�senter les ressources d'un module.
	/// </summary>
	public class Types : AbstractCaptions
	{
		public Types(Module module, PanelsContext context, ResourceAccess access) : base(module, context, access)
		{
			MyWidgets.StackedPanel leftContainer;

			//	Editeur contenant toutes les d�finitions.
			this.CreateBand(out leftContainer, "D�finitions", 0.3);
			this.container = leftContainer.Container;

			this.UpdateEdit();
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
				return ResourceAccess.Type.Types;
			}
		}


		public override void Update()
		{
			//	Met � jour le contenu du Viewer.
			base.Update();
		}

		protected override void UpdateEdit()
		{
			//	Met � jour les lignes �ditables en fonction de la s�lection dans le tableau.
			if (this.container == null)
			{
				return;
			}

			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			int sel = this.access.AccessIndex;

			if (sel >= this.access.AccessCount)
			{
				sel = -1;
			}

			this.container.Children.Clear();

			if (sel != -1)
			{
				ResourceAccess.Field field = this.access.GetField(sel, null, "AbstractType");
				AbstractType type = field.AbstractType;

				this.editor = new MyWidgets.TypeEditorDecimal(this.container);
				this.editor.Type = type;
				this.editor.Dock = DockStyle.StackBegin;
			}

			this.ignoreChange = iic;

			base.UpdateEdit();
		}


		protected override void TextFieldToIndex(AbstractTextField textField, out int field, out int subfield)
		{
			//	Cherche les index correspondant � un texte �ditable.
			base.TextFieldToIndex(textField, out field, out subfield);
		}

		protected override AbstractTextField IndexToTextField(int field, int subfield)
		{
			//	Cherche le TextField permettant d'�diter des index.
			return base.IndexToTextField(field, subfield);
		}


		protected Widget						container;
		protected MyWidgets.AbstractTypeEditor	editor;
	}
}
