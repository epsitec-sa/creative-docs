using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System;


namespace Epsitec.Aider.Entities.Helpers
{
	
	
	public static class IDateRangeExtensions
	{


		public static bool IsInRange(this IDateRange entity, Date date)
		{
			return entity != null
				&& date.InRange (entity.StartDate, entity.EndDate);
		}


	}


}
