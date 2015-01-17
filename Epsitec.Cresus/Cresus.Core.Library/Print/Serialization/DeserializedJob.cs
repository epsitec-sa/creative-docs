﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Print.Serialization
{
	public class DeserializedJob
	{
		public DeserializedJob(string jobFullName, string printerPhysicalName)
		{
			this.sections = new List<DeserializedSection> ();

			this.jobFullName = jobFullName;
			this.printerPhysicalName = printerPhysicalName;
		}


		public List<DeserializedSection> Sections
		{
			get
			{
				return this.sections;
			}
		}

		public string JobFullName
		{
			get
			{
				return this.jobFullName;
			}
		}

		public string PrinterPhysicalName
		{
			get
			{
				return this.printerPhysicalName;
			}
		}


		public int PrintablePagesCount
		{
			get
			{
				int count = 0;

				foreach (var section in this.sections)
				{
					count += section.PrintablePagesCount;
				}

				return count;
			}
		}

		public void RemoveUnprintablePages()
		{
			foreach (var section in this.sections)
			{
				section.RemoveUnprintablePages ();
			}
		}


		private readonly List<DeserializedSection>	sections;

		private string								jobFullName;
		private string								printerPhysicalName;
	}
}
