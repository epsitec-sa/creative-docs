//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Aider.Data
{
	public class eCH_Importer
	{
		public eCH_Importer()
		{
			this.xml = XDocument.Load (@"S:\Epsitec.Cresus\App.Aider\Samples\eerv.xml");
		}

		public void ParseAll()
		{
			XNamespace evd2 = "http://evd.vd.ch/xmlns/eVD-0002/1";
			XNamespace evd4 = "http://evd.vd.ch/xmlns/eVD-0004/1";

			var emptyChildren = new XElement[0];

			var root = this.xml.Root;

			foreach (var reportedPerson in root.Elements (evd2 + "reportedPerson"))
			{
				var adults   = reportedPerson.Elements (evd2 + "adult").ToArray ();
				var children = emptyChildren;
				var address  = reportedPerson.Element (evd2 + "address");

				if (reportedPerson.Element (evd2 + "children") != null)
				{
					children = reportedPerson.Element (evd2 + "children").Elements (evd2 + "child").ToArray ();
				}

				System.Diagnostics.Debug.WriteLine (string.Format ("{0} {1} : {2} enfants", adults[0].Element (evd2 + "person").Element (evd4 + "officialName").Value, adults[0].Element (evd2 + "person").Element (evd4 + "firstNames").Value, children.Length));
			}
		}


		private readonly XDocument xml;
	}
}
