//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library.Business.ContentAccessors
{
	/// <summary>
	/// The <c>DocumentAccessorContentLine</c> structure stores a document item and its
	/// group index.
	/// </summary>
	public struct DocumentAccessorContentLine
	{
		public DocumentAccessorContentLine(AbstractDocumentItemEntity line)
		{
			this.line = line;
			this.groupIndex = line.GroupIndex;
		}

		public DocumentAccessorContentLine(AbstractDocumentItemEntity line, int groupIndex)
		{
			this.line = line;
			this.groupIndex = groupIndex;
		}


		public AbstractDocumentItemEntity		Line
		{
			get
			{
				return this.line;
			}
		}

		public int								GroupIndex
		{
			get
			{
				return this.groupIndex;
			}
		}

		
		public static readonly DocumentAccessorContentLine	Empty = new DocumentAccessorContentLine ();

		
		private readonly AbstractDocumentItemEntity	line;
		private readonly int						groupIndex;
	}
}
