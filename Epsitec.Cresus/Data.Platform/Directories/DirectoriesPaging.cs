using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Epsitec.Data.Platform.Directories
{
	public class DirectoriesPaging
	{

		public int StartAtIndex
		{
			get
			{
				if (this.startAtIndex==null)
				{
					this.startAtIndex = 0;
				}
				return this.startAtIndex;
			}
			set
			{
				this.startAtIndex = value;
			}
		}

		public int FinishAtIndex
		{
			get
			{
				if (this.finishAtIndex==null)
				{
					this.finishAtIndex = 1;
				}
				return this.finishAtIndex;
			}

			set
			{
				this.finishAtIndex = value;
			}
		}

		private int startAtIndex;
		private int finishAtIndex;

		public XElement GetPagingElement()
		{
			XElement Paging = new XElement ("Paging");

			Paging.SetAttributeValue ("StartAtIndex", this.StartAtIndex);
			Paging.SetAttributeValue ("FinishAtIndex", this.FinishAtIndex);

			return Paging;
		}
	}
}
