//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.ModuleRepository
{
	static class ModuleStore
	{
		public static IEnumerable<ModuleRecord> Read(string path)
		{
			XDocument doc = XDocument.Load (path);

			var q = from c in doc.Descendants ("module")
					select new ModuleRecord ()
					{
						ModuleId = (int) c.Attribute ("id"),
						ModuleName = (string) c.Attribute ("name"),
						ModuleState = (ModuleState) System.Enum.Parse (typeof (ModuleState), (string) c.Attribute ("state")),
						DeveloperName = (string) c.Attribute ("owner")
					};

			return q;
		}

		public static void Write(string path, IEnumerable<ModuleRecord> records)
		{
			System.DateTime now = System.DateTime.Now;
			string timeStamp = string.Concat (now.ToShortDateString (), " ", System.DateTime.Now.ToShortTimeString ());

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
