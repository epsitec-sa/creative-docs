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
using Epsitec.Cresus.Core.Widgets.Tiles;

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
				PreferredWidth = 250,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 2, 0, 10),
			};

			var centerFrame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 47, 10),
			};

			var rightFrame = new FrameBox
			{
				Parent = parent,
				PreferredWidth = 240,
				Dock = DockStyle.Right,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 47, 10),
			};

			this.CreateDocumentType (leftFrame);

			this.documentOptionsController = new DocumentOptionsController (this);
			this.documentOptionsController.CreateUI (leftFrame);

			this.summaryController = new SummaryController (this);
			this.summaryController.CreateUI (centerFrame);

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
				Margins = new Margins (0, TileArrow.Breadth, 0, 0),
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
				this.summaryController.UpdateAfterDocumentTypeChanged ();
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

		public DocumentOptionsController DocumentOptionsController
		{
			get
			{
				return this.documentOptionsController;
			}
		}

		public void UpdateAfterOptionChanged()
		{
			this.summaryController.UpdateAfterOptionChanged ();
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


		public static readonly int		lineHeight       = 15;
		public static readonly int		errorHeight      = 20;
		public static readonly int		errorBulletWidth = 15;

		public static readonly Color	acceptedColor  = Color.FromBrightness (1.0);
		public static readonly Color	toleratedColor = Color.FromBrightness (0.9);
		public static readonly Color	rejectedColor  = Color.FromBrightness (0.8);

		public static readonly Color	errorColor     = Color.FromRgb (230.0/255.0,   0.0/255.0,   0.0/255.0);  // rouge
		public static readonly Color	uselessColor   = Color.FromRgb (153.0/255.0, 153.0/255.0, 153.0/255.0);  // gris
		public static readonly Color	missingColor   = Color.FromRgb (  0.0/255.0,   0.0/255.0, 216.0/255.0);  // bleu

		public static readonly Color	backColor      = Color.FromHexa ("f3f8ff");  // bleu très clair

		public static FormattedText	normalBullet  = Misc.GetResourceIconImageTag ("DocumentOptions.Normal",  -2, new Size (13, 13));
		public static FormattedText	errorBullet   = Misc.GetResourceIconImageTag ("DocumentOptions.Error",   -2, new Size (13, 13));
		public static FormattedText	uselessBullet = Misc.GetResourceIconImageTag ("DocumentOptions.Useless", -2, new Size (13, 13));
		public static FormattedText	missingBullet = Misc.GetResourceIconImageTag ("DocumentOptions.Missing", -2, new Size (13, 13));

		private readonly IBusinessContext					businessContext;
		private readonly DocumentCategoryEntity				documentCategoryEntity;

		private DocumentOptionsController					documentOptionsController;
		private PageTypesController							pageTypesController;
		private SummaryController							summaryController;
	}
}
