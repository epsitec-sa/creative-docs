//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business.Finance;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	/// <summary>
	/// The <c>AccessData</c> class provides the working context for the
	/// controllers operating on business documents.
	/// </summary>
	public sealed class AccessData
	{
		public AccessData(EntityViewController controller, DocumentLogic logic)
		{
			this.ViewController   = controller;
			this.BusinessContext  = controller.BusinessContext;
			this.DataContext      = controller.DataContext;
			this.CoreData         = controller.Data;
			this.DocumentLogic    = logic;
			this.DocumentMetadata = logic.DocumentMetadata;
			this.BusinessDocument = logic.BusinessDocument;
		}

		public bool IsExcludingTax
		{
			get
			{
				return this.BillingMode == Business.Finance.BillingMode.ExcludingTax;
			}
		}

		public BillingMode BillingMode
		{
			get
			{
				if (this.BusinessDocument.IsNotNull ())
				{
					return this.BusinessDocument.BillingMode;
				}
				else
				{
					return Business.Finance.BillingMode.None;
				}
			}
		}
		
		public readonly EntityViewController	ViewController;
		public readonly BusinessContext			BusinessContext;
		public readonly DataContext				DataContext;
		public readonly CoreData				CoreData;
		public readonly DocumentMetadataEntity	DocumentMetadata;
		public readonly BusinessDocumentEntity	BusinessDocument;
		public readonly DocumentLogic			DocumentLogic;
	}
}
