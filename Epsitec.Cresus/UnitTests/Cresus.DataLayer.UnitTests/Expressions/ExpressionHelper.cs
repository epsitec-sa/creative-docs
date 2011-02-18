using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.UnitTests.Expressions
{


	internal static class ExpressionHelper
	{


		public static IEnumerable<Field> GetSampleFields()
		{
			for (int i = 0; i < 5; i++)
			{
				Druid id = Druid.FromLong (i);

				yield return new Field (id);
			}
		}


	}


}
