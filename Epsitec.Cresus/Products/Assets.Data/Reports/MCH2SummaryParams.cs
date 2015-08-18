//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Data.Serialization;

namespace Epsitec.Cresus.Assets.Data.Reports
{
	public class MCH2SummaryParams : AbstractReportParams
	{
		public MCH2SummaryParams(string customTitle, DateRange dateRange, Guid rootGuid, int? level, Guid filterGuid, MCH2SummaryType summaryType, bool skipHiddenRows)
			: base (customTitle)
		{
			this.DateRange      = dateRange;
			this.RootGuid       = rootGuid;
			this.Level          = level;
			this.FilterGuid     = filterGuid;
			this.SummaryType    = summaryType;
			this.SkipHiddenRows = skipHiddenRows;
		}

		public MCH2SummaryParams(System.Xml.XmlReader reader)
			: base (reader)
		{
			while (reader.Read ())
			{
				string s;

				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.DateRange:
							this.DateRange = new DateRange (reader);
							break;

						case X.RootGuid:
							s = reader.ReadElementContentAsString ();
							this.RootGuid = s.ParseGuid ();
							break;

						case X.Level:
							s = reader.ReadElementContentAsString ();
							this.Level = s.ParseInt ();
							break;

						case X.FilterGuid:
							s = reader.ReadElementContentAsString ();
							this.FilterGuid = s.ParseGuid ();
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
				return Res.Strings.Reports.MCH2Summary.DefaultTitle.ToString ();
			}
		}


		public override bool Equals(AbstractReportParams other)
		{
			//	Il ne faut surtout pas comparer les Guid !
			if (!base.Equals (other))
			{
				return false;
			}

			var o = other as MCH2SummaryParams;

			return !object.ReferenceEquals (o, null)
				&& this.DateRange      == o.DateRange
				&& this.RootGuid       == o.RootGuid
				&& this.Level          == o.Level
				&& this.FilterGuid     == o.FilterGuid
				&& this.SummaryType    == o.SummaryType
				&& this.SkipHiddenRows == o.SkipHiddenRows;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ()
				^  this.CustomTitle.GetHashCode ()
				^  this.DateRange.GetHashCode ()
				^  this.RootGuid.GetHashCode ()
				^  this.Level.GetHashCode ()
				^  this.FilterGuid.GetHashCode ()
				^  this.SummaryType.GetHashCode ()
				^  this.SkipHiddenRows.GetHashCode ();
		}


		public override AbstractReportParams ChangePeriod(int direction)
		{
			return new MCH2SummaryParams (this.CustomTitle, this.DateRange.ChangePeriod (direction), this.RootGuid, this.Level, this.FilterGuid, this.SummaryType, this.SkipHiddenRows);
		}

		public override AbstractReportParams ChangeCustomTitle(string customTitle)
		{
			return new MCH2SummaryParams (customTitle, this.DateRange, this.RootGuid, this.Level, this.FilterGuid, this.SummaryType, this.SkipHiddenRows);
		}


		public override void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement (X.Report_MCH2Summary);
			base.Serialize (writer);

			this.DateRange.Serialize (writer, X.DateRange);
			writer.WriteElementString (X.RootGuid, this.RootGuid.ToStringIO ());

			if (this.Level.HasValue)
			{
				writer.WriteElementString (X.Level, this.Level.Value.ToStringIO ());
			}

			writer.WriteElementString (X.FilterGuid, this.FilterGuid.ToStringIO ());

			writer.WriteEndElement ();
		}


		public readonly DateRange				DateRange;
		public readonly Guid					RootGuid;
		public readonly int?					Level;
		public readonly Guid					FilterGuid;
		public readonly MCH2SummaryType			SummaryType;
		public readonly bool					SkipHiddenRows;
	}
}
