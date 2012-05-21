//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Aider.Data.ECh
{
	public class eCH_Importer
	{
		public eCH_Importer()
		{
			this.xml = XDocument.Load (@"S:\Epsitec.Cresus\App.Aider\Samples\eerv-2011-11-29.xml");
			this.personIds = new Dictionary<string, XElement> ();
			this.duplicates = new List<XElement> ();
			this.unidentified = new List<XElement> ();
			this.addressFields = new HashSet<string> ();
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

				foreach (var item in address.Elements ())
				{
					this.addressFields.Add (item.Name.LocalName);
				}

				foreach (var adult in adults)
				{
					this.ProcessAdultOrChild (adult, reportedPerson);
				}
				foreach (var child in children)
				{
					this.ProcessAdultOrChild (child, reportedPerson);
				}

			//	System.Diagnostics.Debug.WriteLine (string.Format ("{0} {1} : {2} enfants", adults[0].Element (evd2 + "person").Element (evd4 + "officialName").Value, adults[0].Element (evd2 + "person").Element (evd4 + "firstNames").Value, children.Length));
			}

			HashSet<string> dedup = new HashSet<string> ();

			foreach (var person in this.duplicates)
			{
				if (dedup.Add (person.Value) == false)
				{
					System.Diagnostics.Debug.WriteLine ("More than duplicate : " + person.Value);
				}
			}
		}

		private void ProcessAdultOrChild(XElement xml, XElement container)
		{
			XNamespace evd2 = "http://evd.vd.ch/xmlns/eVD-0002/1";
			XNamespace evd4 = "http://evd.vd.ch/xmlns/eVD-0004/1";
			
			var person        = xml.Element (evd2 + "person");
			var nationality   = xml.Element (evd2 + "nationality");
			var origin        = xml.Element (evd2 + "origin");
			var maritalStatus = xml.Element (evd2 + "maritalStatus");

			var personId = person.Element (evd4 + "personId");

			if (personId == null)
			{
				System.Diagnostics.Debug.WriteLine ("Person without personId : " + person.Value + " (" + xml.Name.LocalName + ")");
				this.unidentified.Add (person);
				return;
			}

			if (this.personIds.ContainsKey (personId.Value))
			{
				var otherContainer = this.personIds[personId.Value];
				System.Diagnostics.Debug.WriteLine ("Person ID " + personId.Value + " is specified multiple times" + " (" + xml.Name.LocalName + ")");
				this.duplicates.Add (person);
			}
			else
			{
				this.personIds.Add (personId.Value, container);
			}
		}


		private readonly XDocument xml;
		private readonly Dictionary<string, XElement> personIds;
		private readonly List<XElement> duplicates;
		private readonly List<XElement> unidentified;
		private readonly HashSet<string> addressFields;
	}
}
