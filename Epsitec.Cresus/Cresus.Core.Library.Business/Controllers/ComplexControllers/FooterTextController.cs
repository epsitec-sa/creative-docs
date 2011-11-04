//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Accounting;
using Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComplexControllers
{
	/// <summary>
	/// Permet de choisir les salutations de pied de page.
	/// </summary>
	public class FooterTextController : System.IDisposable
	{
		public FooterTextController(BusinessContext businessContext, DocumentMetadataEntity documentMetadata, BusinessDocumentEntity businessDocument, bool isReadOnly)
		{
			this.businessContext  = businessContext;
			this.documentMetadata = documentMetadata;
			this.businessDocument = businessDocument;
			this.isReadOnly       = isReadOnly;

			this.InitializeFooterTexts ();
		}


		public void CreateUI(Widget parent)
		{
			int tabIndex = 0;

			new StaticText
			{
				Parent   = parent,
				Text     = "Salutations du pied de page :",
				Dock     = DockStyle.Top,
				Margins  = new Margins (0, 0, 0, Library.UI.Constants.MarginUnderLabel),
			};

			this.textFieldCombo = new TextFieldCombo
			{
				Parent          = parent,
				IsReadOnly      = true,
				Enable          = !this.isReadOnly,
				PreferredHeight = 20,
				MenuButtonWidth = Library.UI.Constants.ComboButtonWidth,
				Dock            = DockStyle.Top,
				TabIndex        = tabIndex++,
			};

			if (!this.isReadOnly)
			{
				this.textFieldCombo.SelectedItemChanged += delegate
				{
					if (this.textFieldCombo.SelectedItemIndex == -1)
					{
						this.businessDocument.FooterText = null;
					}
					else
					{
						var footer = documentFooterTextEntities.ElementAt (this.textFieldCombo.SelectedItemIndex);
						this.businessDocument.FooterText = footer.Description;
					}
				};
			}
		}

		public void UpdateUI()
		{
			this.textFieldCombo.Items.Clear ();

			var currentText = this.businessDocument.FooterText;
			var currentName = FormattedText.Empty;

			if (this.documentFooterTextEntities != null)
			{
				foreach (var footer in this.documentFooterTextEntities)
				{
					this.textFieldCombo.Items.Add (footer.Name);

					if (footer.Description == currentText)
					{
						currentName = footer.Name;
					}
				}
			}

			if (currentName.IsNullOrEmpty)
			{
				if (this.businessDocument.FooterText.IsNullOrEmpty)
				{
					currentName = TextFormatter.FormatText ("Aucun texte").ApplyItalic ();
				}
				else
				{
					currentName = TextFormatter.FormatText ("Texte sur mesure").ApplyItalic ();
				}
			}

			this.textFieldCombo.FormattedText = currentName;
		}


		private void InitializeFooterTexts()
		{
			var example = new DocumentCategoryEntity ();
			example.DocumentType = this.documentMetadata.DocumentCategory.DocumentType;

			var documentCategory = this.businessContext.DataContext.GetByExample<DocumentCategoryEntity> (example).FirstOrDefault ();

			if (documentCategory != null)
			{
				this.documentFooterTextEntities = documentCategory.DocumentFooterTexts.Where (x => !x.Description.IsNullOrEmpty);
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion


		private readonly BusinessContext				businessContext;
		private readonly DocumentMetadataEntity			documentMetadata;
		private readonly BusinessDocumentEntity			businessDocument;
		private readonly bool							isReadOnly;

		private TextFieldCombo							textFieldCombo;

		private IEnumerable<DocumentFooterTextEntity>	documentFooterTextEntities;
	}
}
