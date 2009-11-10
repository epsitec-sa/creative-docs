//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	/// <summary>
	/// The <c>GraphDataCube</c> class encapsulates the <see cref="DataCube"/> while adding
	/// some information, such as slicing, unique ID, etc.
	/// </summary>
	public class GraphDataCube : DataCube
	{
		public GraphDataCube()
		{
			this.Guid = System.Guid.NewGuid ();
		}

		public GraphDataCube(DataCube cube)
			: this ()
		{
			var writer = new System.IO.StringWriter ();
			cube.Save (writer);
			var reader = new System.IO.StringReader (writer.ToString ());
			this.Restore (reader);
		}

		public GraphDataCube(GraphDataCube cube)
			: this ((DataCube) cube)
		{
			this.Guid          = cube.Guid;
			this.SliceDimA     = cube.SliceDimA;
			this.SliceDimB     = cube.SliceDimB;
			this.ConverterName = cube.ConverterName;
			this.Title         = cube.Title;
		}

		
		public System.Guid Guid
		{
			get;
			set;
		}
		
		public string SliceDimA
		{
			get;
			set;
		}
		
		public string SliceDimB
		{
			get;
			set;
		}
		
		public string ConverterName
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public GraphDataSettings SavedSettings
		{
			get;
			set;
		}
	}
}
