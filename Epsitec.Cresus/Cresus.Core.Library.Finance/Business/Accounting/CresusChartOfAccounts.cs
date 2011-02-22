//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Business.Accounting
{
	/// <summary>
	/// The <c>CresusChartOfAccounts</c> class describes the chart of accounts of a
	/// given Crésus Comptabilité file (in French: "plan comptable").
	/// </summary>
	public sealed class CresusChartOfAccounts
	{
		public CresusChartOfAccounts()
		{
			this.items = new List<BookAccountDefinition> ();
		}

		
		public FormattedText					Title
		{
			get;
			set;
		}

		public MachineFilePath					Path
		{
			get;
			set;
		}

		public IList<BookAccountDefinition>		Items
		{
			get
			{
				return this.items;
			}
		}

		public Date								BeginDate
		{
			get;
			set;
		}

		public Date								EndDate
		{
			get;
			set;
		}

		public System.Guid						Id
		{
			get;
			set;
		}


		public XElement SerializeToXml(string xmlNodeName)
		{
			var xml = new XElement (xmlNodeName);

			xml.Add (new XAttribute ("title",     this.Title.ToSimpleText ()));
			xml.Add (new XAttribute ("path",      this.Path.ToString ()));
			xml.Add (new XAttribute ("beginDate", CresusChartOfAccounts.dateConverter.ConvertToString (this.BeginDate)));
			xml.Add (new XAttribute ("endDate",   CresusChartOfAccounts.dateConverter.ConvertToString (this.EndDate)));
			xml.Add (new XAttribute ("id",        this.Id.ToString ()));

			foreach (var item in this.items)
			{
				xml.Add (item.SerializeToXml ("account"));
			}

			return xml;
		}

		public static CresusChartOfAccounts DeserializeFromXml(XElement xml)
		{
			var chartOfAccounts = new CresusChartOfAccounts ();

			string title      = (string) xml.Attribute ("title");
			string path       = (string) xml.Attribute ("path");
			string beginDate  = (string) xml.Attribute ("beginDate");
			string endDate    = (string) xml.Attribute ("endDate");
			string id         = (string) xml.Attribute ("id");

			chartOfAccounts.Title     = FormattedText.FromSimpleText (title);
			chartOfAccounts.Path      = MachineFilePath.Parse (path);
			chartOfAccounts.BeginDate = CresusChartOfAccounts.dateConverter.ConvertFromString (beginDate).Value;
			chartOfAccounts.EndDate   = CresusChartOfAccounts.dateConverter.ConvertFromString (endDate).Value;
			chartOfAccounts.Id        = new System.Guid (id);

			foreach (XElement element in xml.Elements ("account"))
			{
				chartOfAccounts.items.Add (BookAccountDefinition.DeserializeFromXml (element));
			}

			return chartOfAccounts;
		}

		
		private static readonly DateConverter dateConverter = new DateConverter ();

		private readonly List<BookAccountDefinition> items;
	}
}
