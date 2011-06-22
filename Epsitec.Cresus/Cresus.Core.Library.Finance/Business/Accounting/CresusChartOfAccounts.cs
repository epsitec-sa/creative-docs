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
			var accounts = this.items.Select (item => item.SerializeToXml (Xml.Account));

			return new XElement (xmlNodeName,
				new XAttribute (Xml.Title, this.Title.ToSimpleText ()),
				new XAttribute (Xml.Path, this.Path.ToString ()),
				new XAttribute (Xml.BeginDate, DateConverter.Invariant.ConvertToString (this.BeginDate)),
				new XAttribute (Xml.EndDate, DateConverter.Invariant.ConvertToString (this.EndDate)),
				new XAttribute (Xml.Id, ItemCodeGenerator.FromGuid (this.Id)),
				accounts);
		}

		public static CresusChartOfAccounts DeserializeFromXml(XElement xml)
		{
			var chartOfAccounts = new CresusChartOfAccounts ();

			string title      = (string) xml.Attribute (Xml.Title);
			string path       = (string) xml.Attribute (Xml.Path);
			string beginDate  = (string) xml.Attribute (Xml.BeginDate);
			string endDate    = (string) xml.Attribute (Xml.EndDate);
			string id         = (string) xml.Attribute (Xml.Id);
			
			var accounts = xml.Elements (Xml.Account).Select (element => BookAccountDefinition.DeserializeFromXml (element));

			chartOfAccounts.Title     = FormattedText.FromSimpleText (title);
			chartOfAccounts.Path      = MachineFilePath.Parse (path);
			chartOfAccounts.BeginDate = DateConverter.Invariant.ConvertFromString (beginDate).Value;
			chartOfAccounts.EndDate   = DateConverter.Invariant.ConvertFromString (endDate).Value;
			chartOfAccounts.Id        = ItemCodeGenerator.ToGuid (id);
			chartOfAccounts.Items.AddRange (accounts);

			return chartOfAccounts;
		}


		private static class Xml
		{
			public const string Title		= "title";
			public const string Path		= "path";
			public const string BeginDate	= "beginDate";
			public const string EndDate		= "endDate";
			public const string Id			= "id";
			public const string Account		= "account";
		}
		

		private readonly List<BookAccountDefinition> items;
	}
}
