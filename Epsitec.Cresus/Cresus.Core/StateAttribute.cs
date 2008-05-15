//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.PlugIns;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>StateAttribute</c> class is used to identify classes which implement
	/// a state compatible with the <see cref="StateManager"/> class.
	/// </summary>
	[System.AttributeUsage (System.AttributeTargets.Assembly)]
	public sealed class StateAttribute : System.Attribute, IPlugInAttribute<string>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StateAttribute"/> class.
		/// </summary>
		public StateAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StateAttribute"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		public StateAttribute(System.Type type)
		{
			this.Type = type;
			this.Name = type == null ? null : type.Name;
		}

		/// <summary>
		/// Gets the type of the class described by this attribute.
		/// </summary>
		/// <value>The type.</value>
		public System.Type Type
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the class, used for the serialization.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get;
			set;
		}
		
		#region IPlugInAttribute<string> Members

		string IPlugInAttribute<string>.Id
		{
			get
			{
				return this.Name;
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
