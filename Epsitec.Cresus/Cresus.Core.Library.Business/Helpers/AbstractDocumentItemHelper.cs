//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Helpers
{
	public static class AbstractDocumentItemHelper
	{
		public static void UpdateDialogs(AbstractDocumentItemEntity documentItem)
		{
#if false
			//	Met à jour le ou les dialogues d'aperçu avant impression ouverts.
			foreach (var dialog in CoreProgram.Application.AttachedDialogs)
			{
				foreach (var entity in dialog.Entities)
				{
					if (entity is InvoiceDocumentEntity)
					{
						var invoiceDocument = entity as InvoiceDocumentEntity;
						if (invoiceDocument.Lines.Contains (documentItem))
						{
							dialog.Update ();
							break;
						}
					}
				}
			}
#endif
		}
	}
}
