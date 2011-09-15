//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library.Business.ContentAccessors;

using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public struct CellContent
	{
		public CellContent(FormattedText text)
		{
			this.text  = text;
			this.error = DocumentItemAccessorError.None;
		}

		public CellContent(FormattedText text, DocumentItemAccessorError error)
		{
			this.text  = text;
			this.error = error;
		}


		public FormattedText					Text
		{
			get
			{
				return this.text;
			}
		}

		public DocumentItemAccessorError		Error
		{
			get
			{
				return this.error;
			}
		}


		public static readonly CellContent		Empty = new CellContent ();

		
		private readonly DocumentItemAccessorError	error;
		private readonly FormattedText				text;
	}
}
