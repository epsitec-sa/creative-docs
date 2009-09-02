//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph;
using Epsitec.Common.Graph.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.ImportConverters
{
	public static class ImportConverter
	{
		/// <summary>
		/// Converts the raw input data into a cube.
		/// </summary>
		/// <param name="headColumns">The head columns.</param>
		/// <param name="lines">The lines, already split into arrays of values.</param>
		/// <param name="converter">The successful converter.</param>
		/// <param name="cube">The cube.</param>
		/// <returns><c>true</c> if the data could be converted; otherwise, <c>false</c>.</returns>
		public static bool ConvertToCube(IList<string> headColumns, IEnumerable<string[]> lines, out AbstractImportConverter converter, out DataCube cube)
		{
			converter = new ComptaResumePeriodiqueImportConverter ();
			cube      = converter.ToDataCube (headColumns, lines);

			if (cube == null)
			{
				return false;
			}

			return true;
		}
	}
}
