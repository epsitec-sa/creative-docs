//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class AccessData
	{
		public EntityViewController			ViewController;
		public BusinessContext				BusinessContext;
		public DataContext					DataContext;
		public CoreData						CoreData;
		public DocumentMetadataEntity		DocumentMetadataEntity;
		public BusinessDocumentEntity		BusinessDocumentEntity;
	}
}
