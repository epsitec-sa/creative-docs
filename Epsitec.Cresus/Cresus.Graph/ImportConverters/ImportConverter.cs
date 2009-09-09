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
			foreach (var pair in ImportConverter.GetConverters ())
			{
				converter = pair.Value;
				cube      = converter.ToDataCube (headColumns, lines);

				if (cube != null)
				{
					return true;
				}
			}

			converter = null;
			cube      = null;

			return false;
		}

		/// <summary>
		/// Finds the converter with the specified name.
		/// </summary>
		/// <param name="name">The converter name.</param>
		/// <returns>The converter instance or <c>null</c> if the name does not match any converter.</returns>
		public static AbstractImportConverter FindConverter(string name)
		{
			var dict = ImportConverter.GetConverters ();
			
			AbstractImportConverter converter;

			if (dict.TryGetValue (name, out converter))
			{
				return converter;
			}
			else
			{
				return null;
			}
		}

		
		private static Dictionary<string, AbstractImportConverter> GetConverters()
		{
			if (ImportConverter.converters == null)
			{
				ImportConverter.converters = new Dictionary<string, AbstractImportConverter> ();

				foreach (var pair in ImportConverter.GetConverterTypes (typeof (ImportConverter).Assembly))
				{
					var args = new object[] { pair.Key };
					ImportConverter.converters[pair.Key] = System.Activator.CreateInstance (pair.Value, args) as AbstractImportConverter;
				}
			}

			return ImportConverter.converters;
		}

		private static IEnumerable<KeyValuePair<string, System.Type>> GetConverterTypes(System.Reflection.Assembly assembly)
		{
			return from type in assembly.GetTypes ()
				   from attr in type.GetCustomAttributes (typeof (ImporterAttribute), false).Cast<ImporterAttribute> ()
				   select new KeyValuePair<string, System.Type> (attr.Name, type);
		}


		private static Dictionary<string, AbstractImportConverter> converters;
	}
}
