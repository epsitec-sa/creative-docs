//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph.Data;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Graph.ImportConverters
{
	/// <summary>
	/// The <c>AbstractImportConverter</c> class provides the methods used to convert
	/// raw tabular data into a cube.
	/// </summary>
	public abstract class AbstractImportConverter
	{
		protected AbstractImportConverter(string name)
		{
			this.name = name;
		}

		public abstract string DataTitle
		{
			get;
		}

		public abstract Command PreferredGraphType
		{
			get;
		}

		public IDictionary<string, string> Meta
		{
			get;
			protected set;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string FlatMeta
		{
			get
			{
				var dict = this.Meta;

				if (dict == null)
                {
					return "";
                }

				var buffer = new System.Text.StringBuilder ();

				foreach (var pair in dict)
				{
					if (buffer.Length > 0)
					{
						buffer.Append ("\n");
					}

					buffer.Append (pair.Key);
					buffer.Append ("\t");
					buffer.Append (pair.Value);
				}

				return buffer.ToString ();
			}
		}

		public int Priority
		{
			get;
			internal set;
		}

		public abstract AbstractImportConverter CreateSpecificConverter(IDictionary<string, string> meta);

		public AbstractImportConverter CreateSpecificConverter(string metaText)
		{
			Dictionary<string, string> dict = new Dictionary<string, string> ();

			if (!string.IsNullOrEmpty (metaText))
			{
				string[] lines = metaText.Split ('\n');

				foreach (var line in lines)
				{
					string[] args = line.Split ('\t');

					string key   = args[0];
					string value = args[1];

					dict[key] = value;
				}
			}

			return this.CreateSpecificConverter (dict);
		}
		
		public abstract bool CheckCompatibleMeta(IDictionary<string, string> meta);

		public abstract GraphDataCube ToDataCube(IList<string> header, IEnumerable<IEnumerable<string>> lines, string sourcePath, IDictionary<string, string> meta);

		public GraphDataCube ToDataCube(IList<string> header, IEnumerable<string[]> lines, string sourcePath, IDictionary<string, string> meta)
		{
			return this.ToDataCube (header, lines.Cast<IEnumerable<string>> (), sourcePath, meta);
		}

		public virtual GraphDataCategory GetCategory(ChartSeries series)
		{
			return GraphDataCategory.Generic;
		}


		private readonly string name;

	}
}
