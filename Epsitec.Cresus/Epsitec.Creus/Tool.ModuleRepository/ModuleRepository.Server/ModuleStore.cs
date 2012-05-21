//	Copyright © 2008-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Epsitec.Common.Types;

namespace Epsitec.ModuleRepository
{
	/// <summary>
	/// The <c>ModuleStore</c> class provides read and write support for the
	/// module store, which contains a collection of <see cref="ModuleRecord"/>
	/// items.
	/// </summary>
	public static class ModuleStore
	{
		/// <summary>
		/// Reads the specified XML file and returns the collection of
		/// <see cref="ModuleRecord"/> items.
		/// </summary>
		/// <param name="path">The path to the XML file.</param>
		/// <returns>The collection of <see cref="ModuleRecord"/> items.</returns>
		public static IEnumerable<ModuleRecord> Read(string path)
		{
			XDocument doc = XDocument.Load (path);

			var records = from c in doc.Descendants ("module")
						  select new ModuleRecord ()
						  {
							  ModuleId = (int) c.Attribute ("id"),
							  ModuleName = (string) c.Attribute ("name"),
							  ModuleState = ((string) c.Attribute ("state")).ToEnum<ModuleState> (),
							  DeveloperName = (string) c.Attribute ("owner")
						  };

			foreach (var rec in records)
			{
				System.Console.WriteLine ("{0}: {1}, {2}, {3}", rec.ModuleId, rec.ModuleName, rec.ModuleState, rec.DeveloperName);
			}

			return records;
		}

		/// <summary>
		/// Writes the collection of <see cref="ModuleRecord"/> items to the XML
		/// file at the specified path.
		/// </summary>
		/// <param name="path">The path to the XML file.</param>
		/// <param name="records">The <see cref="ModuleRecord"/> items.</param>
		public static void Write(string path, IEnumerable<ModuleRecord> records)
		{
			System.DateTime now = System.DateTime.UtcNow;
			string timeStamp = string.Concat (now.ToShortDateString (), " ", now.ToShortTimeString (), " UTC");

			XDocument doc = new XDocument (
				new XDeclaration ("1.0", "utf-8", "yes"),
				new XComment ("ModuleStore saved on " + timeStamp),
				new XElement ("store",
					new XAttribute ("version", "1.0"),
					new XElement ("modules",
						from module in records
						orderby module.ModuleId
						select new XElement ("module",
							new XAttribute ("id", module.ModuleId),
							new XAttribute ("name", module.ModuleName),
							new XAttribute ("state", module.ModuleState.ToString ()),
							new XAttribute ("owner", module.DeveloperName)))));

			doc.Save (path);
		}
	}
}
