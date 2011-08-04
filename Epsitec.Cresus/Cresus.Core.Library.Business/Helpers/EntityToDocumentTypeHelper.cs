//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Helpers
{
	public static class EntityToDocumentTypeHelper
	{
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

	}
}
