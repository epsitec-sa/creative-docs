//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	public class GraphDataCube : DataCube
	{
		public GraphDataCube()
		{
		}

		public GraphDataCube(DataCube cube)
		{
			var writer = new System.IO.StringWriter ();
			cube.Save (writer);
			var reader = new System.IO.StringReader (writer.ToString ());
			this.Restore (reader);
		}

		public GraphDataCube(GraphDataCube cube)
			: this ((DataCube) cube)
		{
			this.Guid = cube.Guid;
			this.SliceDim1 = cube.SliceDim1;
			this.SliceDim2 = cube.SliceDim2;
			this.ConverterName = cube.ConverterName;
		}

		public System.Guid Guid
		{
			get;
			set;
		}
		
		public string SliceDim1
		{
			get;
			set;
		}
		
		public string SliceDim2
		{
			get;
			set;
		}
		
		public string ConverterName
		{
			get;
			set;
		}
	}
}
