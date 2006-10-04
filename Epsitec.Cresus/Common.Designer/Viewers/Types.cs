using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Types : AbstractCaptions
	{
		public Types(Module module, PanelsContext context, ResourceAccess access) : base(module, context, access)
		{
			//	Editeur contenant toutes les définitions.
			this.CreateBand(out this.container, "", 0.3);

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
			//	Met à jour le contenu du Viewer.
			base.Update();
		}

		protected override void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
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

			AbstractType type = null;
			ResourceAccess.TypeType typeType = ResourceAccess.TypeType.None;

			if (sel != -1)
			{
				ResourceAccess.Field field = this.access.GetField(sel, null, "AbstractType");
				type = field.AbstractType;
				typeType = ResourceAccess.CaptionType(type);
			}

			this.container.Title = string.Format("Définitions du type <b>{0}</b>", typeType.ToString());

			if (this.typeType != typeType)  // autre type ?
			{
				this.typeType = typeType;

				if (this.editor != null)
				{
					//	Supprime l'éditeur actuel.
					this.container.Container.Children.Clear();

					this.editor.ContentChanged -= new EventHandler(this.HandleEditorContentChanged);
					this.editor = null;
				}

				this.editor = MyWidgets.AbstractTypeEditor.Create(this.typeType);

				if (this.editor != null)
				{
					//	Crée le nouvel éditeur.
					this.editor.SetParent(this.container.Container);
					this.editor.Dock = DockStyle.StackBegin;
					this.editor.ContentChanged += new EventHandler(this.HandleEditorContentChanged);
				}
			}

			if (this.editor != null && type != null)
			{
				this.editor.Type = type;  // donne le type et met à jour l'éditeur
			}

			this.ignoreChange = iic;

			base.UpdateEdit();  // met à jour le reste
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


		private void HandleEditorContentChanged(object sender)
		{
			//	Le contenu de l'éditeur de type a changé.
			int sel = this.access.AccessIndex;
			ResourceAccess.Field field = new ResourceAccess.Field(this.editor.Type);
			this.access.SetField(sel, null, "AbstractType", field);
		}


		protected MyWidgets.StackedPanel		container;
		protected ResourceAccess.TypeType		typeType = ResourceAccess.TypeType.None;
		protected MyWidgets.AbstractTypeEditor	editor;
	}
}
