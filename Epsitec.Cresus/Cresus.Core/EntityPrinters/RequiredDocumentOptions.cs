//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Documents.External;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.EntityPrinters
{
	/// <summary>
	/// Cette classe centralise l’obtention des options d’impression pour un type de document donné.
	/// Elle doit se trouver dans Cresus.Core, car elle utilise beaucoup de références éparpillées un
	/// peu partout. Ensuite, elle dispatche les méthodes GetRequiredDocumentOptions* d’une façon un
	/// peu bricolée, mais je n’ai pas trouvé mieux.
	/// </summary>
	public static class RequiredDocumentOptions
	{
		public static void Initialize()
		{
			CresusCore.GetRequiredDocumentOptionsByEntity       = RequiredDocumentOptions.GetRequiredDocumentOptionsByEntity;
			CresusCore.GetRequiredDocumentOptionsByDocumentType = RequiredDocumentOptions.GetRequiredDocumentOptionsByDocumentType;
			CresusCore.GetRequiredPageTypes                     = RequiredDocumentOptions.GetRequiredPageTypes;
		}


		private static IEnumerable<DocumentOption> GetRequiredDocumentOptionsByEntity(AbstractEntity entity)
		{
			//	Retourne la liste des options dont peut avoir besoin une entité lorsqu'elle est imprimée.
			var documentType = RequiredDocumentOptions.GetDocumentType (entity);
			return RequiredDocumentOptions.GetRequiredDocumentOptionsByDocumentType (documentType);
		}

		private static IEnumerable<DocumentOption> GetRequiredDocumentOptionsByDocumentType(DocumentType documentType)
		{
			//	Retourne la liste des options dont peut avoir besoin un type de document lorsqu'il est imprimé.
			switch (documentType)
			{
				case DocumentType.MailContactLabel:
					return MailContactPrinter.RequiredDocumentOptions;

				case DocumentType.RelationSummary:
					return RelationPrinter.RequiredDocumentOptions;

				default:
					return AbstractDocumentMetadataPrinter.GetRequiredDocumentOptions (documentType);
			}
		}

		private static DocumentType GetDocumentType(AbstractEntity entity)
		{
			if (entity is DocumentMetadataEntity)
			{
				var documentMetadata = entity as DocumentMetadataEntity;

				return documentMetadata.DocumentCategory.DocumentType;
			}

			if (entity is MailContactEntity)
			{
				return DocumentType.MailContactLabel;
			}

			if (entity is RelationEntity)
			{
				return DocumentType.RelationSummary;
			}

			return DocumentType.Unknown;
		}


		private static IEnumerable<PageType> GetRequiredPageTypes(DocumentType documentType)
		{
			//	Retourne la liste des types de page dont peut avoir besoin un type de document lorsqu'il est imprimé.
			switch (documentType)
			{
				case DocumentType.MailContactLabel:
					return MailContactPrinter.RequiredPageTypes;

				case DocumentType.RelationSummary:
					return RelationPrinter.RequiredPageTypes;

				default:
					return AbstractDocumentMetadataPrinter.GetRequiredPageTypes (documentType);
			}
		}
	}
}
