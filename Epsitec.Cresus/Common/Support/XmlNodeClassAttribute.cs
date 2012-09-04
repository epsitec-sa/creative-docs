//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.PlugIns;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Xml.Linq;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>XmlNodeClassAttribute</c> attribute is used by the <see cref="XmlNodeClassFactory"/>
	/// to associate an XML node name with a class, which should implement a static <c>Restore</c>
	/// method for deserialization based on XML.
	/// </summary>
	[System.AttributeUsage (System.AttributeTargets.Assembly,
		/* */				AllowMultiple=true)]
	public sealed class XmlNodeClassAttribute : System.Attribute, IPlugInAttribute<string>
	{
		public XmlNodeClassAttribute(string id, System.Type type)
		{
			this.id = id;
			this.type = type;
		}

		#region IPlugInAttribute<string> Members

		public string							Id
		{
			get
			{
				return this.id;
			}
		}

		public System.Type						Type
		{
			get
			{
				return this.type;
			}
		}

		#endregion


		private readonly string					id;
		private readonly System.Type			type;
	}
}
