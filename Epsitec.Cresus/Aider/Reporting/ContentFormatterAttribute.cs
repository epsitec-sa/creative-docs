//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.PlugIns;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Reporting
{
	[System.AttributeUsage (System.AttributeTargets.Assembly, AllowMultiple=true)]
	public sealed class ContentFormatterAttribute : System.Attribute, IPlugInAttribute<string>
	{
		public ContentFormatterAttribute()
		{
		}

		public ContentFormatterAttribute(System.Type type)
		{
			this.id   = type.Name;
			this.type = type;
		}

		#region IPlugInAttribute<string> Members

		public string							Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		public System.Type						Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		#endregion


		private string							id;
		private System.Type						type;
	}
}

