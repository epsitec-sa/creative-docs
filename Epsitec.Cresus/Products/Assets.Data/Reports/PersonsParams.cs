//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Assets.Data.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data.Reports
{
	public class PersonsParams : AbstractReportParams
	{
		public PersonsParams()
			: base ()
		{
		}

		public PersonsParams(string customTitle)
			: base (customTitle)
		{
		}

		public PersonsParams(System.Xml.XmlReader reader)
			: base (reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}


		public override string					Title
		{
			get
			{
				return Res.Strings.Reports.Persons.DefaultTitle.ToString ();
			}
		}

		public override bool					HasParams
		{
			get
			{
				return false;
			}
		}


		public override void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement (X.Report_Persons);
			base.Serialize (writer);
			writer.WriteEndElement ();
		}
	}
}
