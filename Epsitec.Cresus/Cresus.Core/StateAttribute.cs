//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.PlugIns;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>StateAttribute</c> class is used to map a state to a state class.
	/// </summary>
	[System.AttributeUsage (System.AttributeTargets.Assembly)]
	public sealed class StateAttribute : System.Attribute, IPlugInAttribute<string>
	{
		public StateAttribute(System.Type type)
		{
			this.Type = type;
		}

		
		public System.Type Type
		{
			get;
			set;
		}

		
		#region IPlugInAttribute<string> Members

		string IPlugInAttribute<string>.Id
		{
			get
			{
				return this.Type.Name;
			}
		}

		System.Type IPlugInAttribute<string>.Type
		{
			get
			{
				return this.Type;
			}
		}

		#endregion
	}
}
