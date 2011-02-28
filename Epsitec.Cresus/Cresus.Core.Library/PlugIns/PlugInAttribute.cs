//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.PlugIns
{
	/// <summary>
	/// The <c>PlugInAttribute</c> attribute is used to decorated classes
	/// which implement <see cref="ICorePlugIn"/> and which can be loaded
	/// as plug-ins into <see cref="CoreApplication"/>.
	/// </summary>
	[System.AttributeUsage (System.AttributeTargets.Class)]
	public sealed class PlugInAttribute : System.Attribute
	{
		public PlugInAttribute(string name, string version)
		{
			this.name = name;
			this.version = new System.Version (version);
		}


		public string							Name
		{
			get
			{
				return this.name;
			}
		}

		public System.Version					Version
		{
			get
			{
				return this.version;
			}
		}


		private readonly string name;
		private readonly System.Version version;
	}
}
