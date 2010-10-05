//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;
using Epsitec.Common.Types;
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


		public XElement SerializeToXml(string nodeName)
		{
			throw new System.NotImplementedException ();
		}

		public static CresusChartOfAccounts DeserializeFromXml(XElement xml)
		{
			throw new System.NotImplementedException ();
		}

		
		private readonly List<BookAccountDefinition> items;
	}
}
