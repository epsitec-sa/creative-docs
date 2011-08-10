//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Core.Documents.External
{
	public static class CresusCore
	{
		public static System.Func<AbstractEntity, IEnumerable<DocumentOption>>	GetRequiredDocumentOptionsByEntity;
		public static System.Func<DocumentType, IEnumerable<DocumentOption>>	GetRequiredDocumentOptionsByDocumentType;
		public static System.Func<DocumentType, IEnumerable<PageType>>			GetRequiredPageTypes;
	}
}
