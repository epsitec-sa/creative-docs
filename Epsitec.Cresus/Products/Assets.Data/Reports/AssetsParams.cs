//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Data.Serialization;

namespace Epsitec.Cresus.Assets.Data.Reports
{
	public class AssetsParams : AbstractReportParams
	{
		public AssetsParams(string customTitle, Timestamp timestamp, Guid rootGuid, int? level)
			: base (customTitle)
		{
			this.Timestamp = timestamp;
			this.RootGuid  = rootGuid;
			this.Level     = level;
		}

		public AssetsParams(System.Xml.XmlReader reader)
			: base (reader)
		{
			while (reader.Read ())
			{
				string s;

				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.Timestamp:
							this.Timestamp = new Timestamp (reader);
							break;

						case X.RootGuid:
							s = reader.ReadElementContentAsString ();
							this.RootGuid = s.ParseGuid ();
							break;

						case X.Level:
							s = reader.ReadElementContentAsString ();
							this.Level = s.ParseInt ();
							break;
					}
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
				return Res.Strings.Reports.Assets.DefaultTitle.ToString ();
			}
		}


		public override bool Equals(AbstractReportParams other)
		{
			//	Il ne faut surtout pas comparer les Guid !
			if (!base.Equals (other))
			{
				return false;
			}

			var o = other as AssetsParams;

			return !object.ReferenceEquals (o, null)
				&& this.Timestamp == o.Timestamp
				&& this.RootGuid  == o.RootGuid
				&& this.Level     == o.Level;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ()
				^  this.CustomTitle.GetHashCode ()
				^  this.Timestamp.GetHashCode ()
				^  this.RootGuid.GetHashCode ()
				^  this.Level.GetHashCode ();
		}


		public override AbstractReportParams ChangePeriod(int direction)
		{
			var timestamp = new Timestamp (this.Timestamp.Date.AddYears (direction), 0);
			return new AssetsParams (this.CustomTitle, timestamp, this.RootGuid, this.Level);
		}

		public override AbstractReportParams ChangeCustomTitle(string customTitle)
		{
			return new AssetsParams (customTitle, this.Timestamp, this.RootGuid, this.Level);
		}


		public override void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement (X.Report_Assets);
			base.Serialize (writer);

			this.Timestamp.Serialize (writer, X.Timestamp);
			writer.WriteElementString (X.RootGuid, this.RootGuid.ToStringIO ());

			if (this.Level.HasValue)
			{
				writer.WriteElementString (X.Level, this.Level.Value.ToStringIO ());
			}

			writer.WriteEndElement ();
		}


		public readonly Timestamp				Timestamp;
		public readonly Guid					RootGuid;
		public readonly int?					Level;
	}
}
