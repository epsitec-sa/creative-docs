//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.PlugIns;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.DocumentOptionsEditor
{
	public sealed class DocumentCategoryController : System.IDisposable
	{
		public DocumentCategoryController(IBusinessContext businessContext, DocumentCategoryEntity documentCategoryEntity)
		{
			System.Diagnostics.Debug.Assert (businessContext != null);
			System.Diagnostics.Debug.Assert (documentCategoryEntity.IsNotNull ());

			this.businessContext        = businessContext;
			this.documentCategoryEntity = documentCategoryEntity;

			this.businessContext.SavingChanges += this.HandleBusinessContextSavingChanges;
		}



		public void CreateUI(Widget parent)
		{
			var box = new FrameBox
			{
				Parent = parent,
				PreferredHeight = 400,
				Dock = DockStyle.Top,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, 0),
			};

			this.CreateDocumentType (box);

			this.documentCategoryOptionsSubController = new DocumentCategoryOptionsSubController (this.businessContext, this.documentCategoryEntity);
			this.documentCategoryOptionsSubController.CreateUI (box);
		}

		private void CreateDocumentType(Widget parent)
		{
			new StaticText
			{
				Parent = parent,
				FormattedText = "Type du document :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 2),
			};

			var combo = new TextFieldCombo
			{
				Parent = parent,
				IsReadOnly = true,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, -3, 0),
			};

			var types = EnumKeyValues.FromEnum<DocumentType> ();
			foreach (var type in types)
			{
				if (type.Key != DocumentType.None   &&
					type.Key != DocumentType.Unknown)
				{
					combo.Items.Add (type.Key.ToString (), type.Values[0]);

					if (type.Key == this.documentCategoryEntity.DocumentType)
					{
						combo.SelectedItemIndex = combo.Items.Count-1;
					}
				}
			}

			combo.SelectedItemChanged += delegate
			{
				string key = combo.Items.GetKey (combo.SelectedItemIndex);
				this.documentCategoryEntity.DocumentType = (DocumentType) System.Enum.Parse (typeof (DocumentType), key);

				this.documentCategoryOptionsSubController.UpdateAfterDocumentTypeChanged ();
			};
		}



		private void HandleBusinessContextSavingChanges(object sender, CancelEventArgs e)
		{
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.businessContext.SavingChanges -= this.HandleBusinessContextSavingChanges;
		}

		#endregion


		private readonly IBusinessContext					businessContext;
		private readonly DocumentCategoryEntity				documentCategoryEntity;

		private DocumentCategoryOptionsSubController		documentCategoryOptionsSubController;
	}
}
