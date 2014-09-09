//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data.Reports
{
	public class PersonsParams : AbstractReportParams
	{
		public PersonsParams(string customTitle)
			: base (customTitle)
		{
		}


		public override string					Title
		{
			get
			{
				return Res.Strings.Reports.Persons.DefaultTitle.ToString ();
			}
		}

		public override bool					HasParams
		{
			get
			{
				return false;
			}
		}

		//?public override bool StrictlyEquals(AbstractReportParams other)
		//?{
		//?	if (other is PersonsParams)
		//?	{
		//?		return true;
		//?	}
		//?
		//?	return false;
		//?}
	}
}
