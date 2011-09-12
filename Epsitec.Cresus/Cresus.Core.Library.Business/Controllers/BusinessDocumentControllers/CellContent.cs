//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library.Business.ContentAccessors;

using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class CellContent
	{
		public CellContent(FormattedText text)
		{
			this.Text  = text;
			this.Error = DocumentItemAccessorError.None;
		}

		public CellContent(FormattedText text, DocumentItemAccessorError error)
		{
			this.Text  = text;
			this.Error = error;
		}

		public FormattedText Text
		{
			get;
			internal set;
		}

		public DocumentItemAccessorError Error
		{
			get;
			internal set;
		}
	}
}
