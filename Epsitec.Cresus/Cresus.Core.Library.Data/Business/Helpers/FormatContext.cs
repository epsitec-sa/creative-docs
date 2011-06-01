//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Business.Helpers
{
	internal class FormatContext
	{
		public FormatContext(long id)
		{
			this.id = id;
		}
		
		public long								Id
		{
			get
			{
				return this.id;
			}
		}

		public string Args
		{
			get;
			set;
		}

		private readonly long id;
	}
}
