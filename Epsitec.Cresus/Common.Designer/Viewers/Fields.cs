using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de repr�senter les ressources d'un module.
	/// </summary>
	public class Fields : AbstractCaptions
	{
		public Fields(Module module, PanelsContext context, ResourceAccess access, DesignerApplication mainWindow) : base(module, context, access, mainWindow)
		{
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
				return ResourceAccess.Type.Fields;
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

		protected override bool HasDeleteOrDuplicate
		{
			get
			{
				//	Il n'est pas possible de cr�er ou de supprimer une ressource, puisque cela
				//	se fait depuis l'�diteur d'entit�s.
				return false;
			}
		}


	}
}
