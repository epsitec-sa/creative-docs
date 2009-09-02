//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.ImportConverters
{
	public abstract class AbstractImportConverter
	{
		public abstract DataCube ToDataCube(IList<string> header, IEnumerable<IList<string>> lines);

		public DataCube ToDataCube(IList<string> header, IEnumerable<string[]> lines)
		{
			return this.ToDataCube (header, lines.Cast<IList<string>> ());
		}
	}
}
