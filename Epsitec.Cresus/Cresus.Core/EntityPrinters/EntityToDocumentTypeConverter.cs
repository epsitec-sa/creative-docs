//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Print.Bands;
using Epsitec.Cresus.Core.Print.Containers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Epsitec.Cresus.Core.Resolvers;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Print.EntityPrinters;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.EntityPrinters
{
	public static class EntityToDocumentTypeConverter
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
