//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Print.Serialization
{
	public class DeserializedSection
	{
		public DeserializedSection(DeserializedJob parentJob, string printerLogicalName, string printerPhysicalTray, Size pageSize)
		{
			this.pages = new List<DeserializedPage> ();

			this.parentJob           = parentJob;
			this.printerLogicalName  = printerLogicalName;
			this.printerPhysicalTray = printerPhysicalTray;
			this.pageSize            = pageSize;
		}


		public List<DeserializedPage> Pages
		{
			get
			{
				return this.pages;
			}
		}

		public DeserializedJob ParentJob
		{
			get
			{
				return this.parentJob;
			}
		}

		public string PrinterLogicalName
		{
			get
			{
				return this.printerLogicalName;
			}
		}

		public string PrinterPhysicalTray
		{
			get
			{
				return this.printerPhysicalTray;
			}
		}

		public Size PageSize
		{
			get
			{
				return this.pageSize;
			}
		}


		private readonly DeserializedJob		parentJob;
		private readonly List<DeserializedPage> pages;

		private string							printerLogicalName;
		private readonly string					printerPhysicalTray;
		private readonly Size					pageSize;
	}
}
