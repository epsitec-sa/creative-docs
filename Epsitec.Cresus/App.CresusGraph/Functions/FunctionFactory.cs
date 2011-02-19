//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Graph.Widgets;

namespace Epsitec.Cresus.Graph.Functions
{
	public static class FunctionFactory
	{
		public static System.Func<IList<double>, double> GetFunction(string name)
		{
			switch (name)
			{
				case FunctionFactory.FunctionSum:
					return x => x.Aggregate (0.0, (sum, value) => sum + value);

				case FunctionFactory.FunctionMean:
					return x => x.Count == 0 ? 0 : x.Aggregate (0.0, (sum, value) => sum + value) / x.Count;

				case FunctionFactory.FunctionMin:
					return x => x.Aggregate (double.PositiveInfinity, (min, value) => System.Math.Min (min, value));

				case FunctionFactory.FunctionMax:
					return x => x.Aggregate (double.NegativeInfinity, (max, value) => System.Math.Max (max, value));
			}
			
			return null;
		}

		public static string GetFunctionCaption(string name)
		{
			switch (name)
			{
				case FunctionFactory.FunctionSum:
					return "Somme";
				case FunctionFactory.FunctionMean:
					return "Moyenne";
				case FunctionFactory.FunctionMin:
					return "Minimum";
				case FunctionFactory.FunctionMax:
					return "Maximum";
			}

			return null;
		}

		public static IEnumerable<string> GetFunctionNames()
		{
			yield return FunctionFactory.FunctionSum;
			yield return FunctionFactory.FunctionMean;
			yield return FunctionFactory.FunctionMin;
			yield return FunctionFactory.FunctionMax;
		}

		public const string FunctionSum  = "sum";
		public const string FunctionMean = "mean";
		public const string FunctionMin  = "min";
		public const string FunctionMax  = "max";
	}
}
