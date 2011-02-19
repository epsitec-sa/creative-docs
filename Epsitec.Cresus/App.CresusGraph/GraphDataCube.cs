//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph.Data;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;

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

		public string ConverterMeta
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

		public string LoadPath
		{
			get;
			set;
		}

		public System.Text.Encoding LoadEncoding
		{
			get;
			set;
		}



		public Command GetPreferredGraphType()
		{
			var converter = ImportConverters.ImportConverter.FindConverter (this.ConverterName);

			if (converter == null)
			{
				return null;
			}
			else
			{
				return converter.PreferredGraphType;
			}
		}

		protected override IEnumerable<string> GetAnnotations()
		{
			yield return "Guid " + this.Guid.ToString ("D");
			
			if (!string.IsNullOrEmpty (this.SliceDimA))
			{
				yield return "SliceDimA " + this.SliceDimA;
			}

			if (!string.IsNullOrEmpty (this.SliceDimB))
			{
				yield return "SliceDimB " + this.SliceDimB;
			}

			if (!string.IsNullOrEmpty (this.ConverterName))
			{
				yield return "ConverterName " + this.ConverterName;
			}

			if (!string.IsNullOrEmpty (this.ConverterMeta))
            {
				yield return "ConverterMeta " + this.ConverterMeta;
            }

			if (!string.IsNullOrEmpty (this.Title))
            {
				yield return "Title " + this.Title;
			}

			if (!string.IsNullOrEmpty (this.LoadPath))
			{
				yield return "LoadPath " + this.LoadPath;
			}

			if (this.LoadEncoding != null)
			{
				yield return "LoadEncoding " + this.LoadEncoding.WebName;
			}
		}

		protected override void AddAnnotation(string annotation)
		{
			int pos = annotation.IndexOf (' ');
			
			if (pos < 1)
			{
				throw new System.FormatException ("Invalid annotation");
			}

			string key   = annotation.Substring (0, pos);
			string value = annotation.Substring (pos+1);

			switch (key)
			{
				case "Guid":
					this.Guid = new System.Guid (value);
					break;

				case "SliceDimA":
					this.SliceDimA = value;
					break;

				case "SliceDimB":
					this.SliceDimB = value;
					break;

				case "ConverterName":
					this.ConverterName = value;
					break;

				case "ConverterMeta":
					this.ConverterMeta = value;
					break;

				case "Title":
					this.Title = value;
					break;

				case "LoadPath":
					this.LoadPath = value;
					break;

				case "LoadEncoding":
					this.LoadEncoding = System.Text.Encoding.GetEncoding (value);
					break;
			}
		}


		public const string LoadPathClipboard = "clipboard";
	}
}
