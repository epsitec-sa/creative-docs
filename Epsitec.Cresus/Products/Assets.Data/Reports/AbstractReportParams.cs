//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data.Reports
{
	public abstract class AbstractReportParams : IGuid
	{
		public AbstractReportParams()
		{
			this.guid = Guid.NewGuid ();
		}


		#region IGuid Members
		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}
		#endregion


		public virtual bool StrictlyEquals(AbstractReportParams other)
		{
			return false;
		}

		public virtual AbstractReportParams ChangePeriod(int direction)
		{
			return null;
		}


		private readonly Guid					guid;
	}
}
