﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Print.Serialization
{
	public class DeserializedSection
	{
		public DeserializedSection(DeserializedJob parentJob, string documentPrintingUnitCode, string documentPrintingUnitName, string printerPhysicalTray, Size pageSize)
		{
			this.pages = new List<DeserializedPage> ();

			this.parentJob                = parentJob;
			this.documentPrintingUnitCode = documentPrintingUnitCode;
			this.documentPrintingUnitName = documentPrintingUnitName;
			this.printerPhysicalTray      = printerPhysicalTray;
			this.pageSize                 = pageSize;
		}

		public DeserializedSection(FormattedText error)
		{
			this.pages = new List<DeserializedPage> ();

			this.error = error;
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

		public string DocumentPrintingUnitCode
		{
			get
			{
				return this.documentPrintingUnitCode;
			}
		}

		public string DocumentPrintingUnitName
		{
			get
			{
				return this.documentPrintingUnitName;
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


		public bool IsOK
		{
			get
			{
				return this.error.IsNullOrEmpty ();
			}
		}

		public bool IsError
		{
			get
			{
				return !this.error.IsNullOrEmpty ();
			}
		}

		public FormattedText Error
		{
			get
			{
				return this.error;
			}
		}


		public int PrintablePagesCount
		{
			get
			{
				return this.pages.Where (x => x.IsPrintable).Count ();
			}
		}

		public void RemoveUnprintablePages()
		{
			int i = 0;
			while (i < this.pages.Count)
			{
				if (this.pages[i].IsPrintable)
				{
					i++;
				}
				else
				{
					this.pages.RemoveAt (i);
				}
			}
		}


		private readonly DeserializedJob		parentJob;
		private readonly List<DeserializedPage> pages;

		private string							documentPrintingUnitCode;
		private string							documentPrintingUnitName;
		private readonly string					printerPhysicalTray;
		private readonly Size					pageSize;
		private readonly FormattedText			error;
	}
}
