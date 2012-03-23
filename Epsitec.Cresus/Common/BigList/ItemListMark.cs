//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemListMark
	{
		public ItemListMark()
		{
		}


		public ItemListMarkAttachment			Attachment
		{
			get;
			set;
		}

		public int								Index
		{
			get;
			set;
		}

		public int								Breadth
		{
			get;
			set;
		}

		
		public int GetInsertionIndex()
		{
			switch (this.Attachment)
			{
				case ItemListMarkAttachment.Before:
					return this.Index;
				case ItemListMarkAttachment.After:
					return this.Index+1;
			}

			throw new System.NotSupportedException (string.Format ("Attachment {0} not supported", this.Attachment.GetQualifiedName ()));
		}

		public void SetInsertionIndex(int index)
		{
			switch (this.Attachment)
			{
				case ItemListMarkAttachment.Before:
					this.Index = index;
					return;

				case ItemListMarkAttachment.After:
					this.Index = index-1;
					return;
			}

			throw new System.NotSupportedException (string.Format ("Attachment {0} not supported", this.Attachment.GetQualifiedName ()));
		}
	}
}