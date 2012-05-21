//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.ImportConverters
{
	/// <summary>
	/// The <c>ImporterAttribute</c> class is used to decorate importer classes
	/// derived from <see cref="AbstractImportConverter"/>.
	/// </summary>
	[System.AttributeUsage (System.AttributeTargets.Class, AllowMultiple=true)]
	public class ImporterAttribute : System.Attribute
	{
		public ImporterAttribute(string name)
		{
			this.name = name;
			this.Priority = 100;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public int Priority
		{
			get;
			set;
		}

		private readonly string name;
	}
}
