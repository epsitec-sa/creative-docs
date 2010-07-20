//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public class DocumentType
	{
		public DocumentType(string name, string shortDescription, string longDescription)
		{
			this.Name = name;
			this.ShortDescription = shortDescription;
			this.LongDescription = longDescription;

			this.options = new List<DocumentOption> ();
		}

		public string Name
		{
			get;
			set;
		}

		public string ShortDescription
		{
			get;
			set;
		}

		public string LongDescription
		{
			get;
			set;
		}

		public List<DocumentOption> DocumentOptions
		{
			get
			{
				return this.options;
			}
		}


		private readonly List<DocumentOption> options;
	}
}
