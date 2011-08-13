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

namespace Epsitec.Cresus.Core.DocumentCategoryController
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
			parent.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			var leftFrame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 10, 0, 10),
			};

			var rightFrame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 37, 10),
			};

			this.CreateDocumentType (leftFrame);

			this.documentOptionsController = new DocumentOptionsController (this);
			this.documentOptionsController.CreateUI (leftFrame);

			this.pageTypesController = new PageTypesController (this);
			this.pageTypesController.CreateUI (rightFrame);
		}

		private void CreateDocumentType(Widget parent)
		{
			new StaticText
			{
				Parent = parent,
				FormattedText = "Type du document :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, Library.UI.Constants.MarginUnderLabel),
			};

			var combo = new TextFieldCombo
			{
				Parent = parent,
				IsReadOnly = true,
				Dock = DockStyle.Top,
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

				this.documentOptionsController.UpdateAfterDocumentTypeChanged ();
				this.pageTypesController.UpdateAfterDocumentTypeChanged ();
			};
		}


		public IBusinessContext BusinessContext
		{
			get
			{
				return this.businessContext;
			}
		}

		public DocumentCategoryEntity DocumentCategoryEntity
		{
			get
			{
				return this.documentCategoryEntity;
			}
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


		public readonly int		lineHeight  = 15;
		public readonly int		errorHeight = 20;

		public readonly Color	acceptedColor  = Color.FromBrightness (1.0);
		public readonly Color	toleratedColor = Color.FromBrightness (0.9);
		public readonly Color	rejectedColor  = Color.FromBrightness (0.8);

		private readonly IBusinessContext					businessContext;
		private readonly DocumentCategoryEntity				documentCategoryEntity;

		private DocumentOptionsController					documentOptionsController;
		private PageTypesController							pageTypesController;
	}
}
