//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Data;

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
			var accounts = this.items.Select (item => item.SerializeToXml (CresusChartOfAccounts.XmlAccount));

			return new XElement (xmlNodeName,
				new XAttribute (CresusChartOfAccounts.XmlTitle, this.Title.ToSimpleText ()),
				new XAttribute (CresusChartOfAccounts.XmlPath, this.Path.ToString ()),
				new XAttribute (CresusChartOfAccounts.XmlBeginDate, CresusChartOfAccounts.dateConverter.ConvertToString (this.BeginDate)),
				new XAttribute (CresusChartOfAccounts.XmlEndDate, CresusChartOfAccounts.dateConverter.ConvertToString (this.EndDate)),
				new XAttribute (CresusChartOfAccounts.XmlId, ItemCodeGenerator.FromGuid (this.Id)),
				accounts);
		}

		public static CresusChartOfAccounts DeserializeFromXml(XElement xml)
		{
			var chartOfAccounts = new CresusChartOfAccounts ();

			string title      = (string) xml.Attribute (CresusChartOfAccounts.XmlTitle);
			string path       = (string) xml.Attribute (CresusChartOfAccounts.XmlPath);
			string beginDate  = (string) xml.Attribute (CresusChartOfAccounts.XmlBeginDate);
			string endDate    = (string) xml.Attribute (CresusChartOfAccounts.XmlEndDate);
			string id         = (string) xml.Attribute (CresusChartOfAccounts.XmlId);
			
			var accounts = xml.Elements (CresusChartOfAccounts.XmlAccount).Select (element => BookAccountDefinition.DeserializeFromXml (element));

			chartOfAccounts.Title     = FormattedText.FromSimpleText (title);
			chartOfAccounts.Path      = MachineFilePath.Parse (path);
			chartOfAccounts.BeginDate = CresusChartOfAccounts.dateConverter.ConvertFromString (beginDate).Value;
			chartOfAccounts.EndDate   = CresusChartOfAccounts.dateConverter.ConvertFromString (endDate).Value;
			chartOfAccounts.Id        = ItemCodeGenerator.ToGuid (id);
			chartOfAccounts.Items.AddRange (accounts);

			return chartOfAccounts;
		}

		private const string XmlTitle		= "title";
		private const string XmlPath		= "path";
		private const string XmlBeginDate	= "beginDate";
		private const string XmlEndDate		= "endDate";
		private const string XmlId			= "id";
		private const string XmlAccount		= "account";
		
		private static readonly DateConverter dateConverter = new DateConverter (System.Globalization.CultureInfo.InvariantCulture);

		private readonly List<BookAccountDefinition> items;
	}
}
