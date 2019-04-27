//	Copyright © 2006-2013, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ControllerAttribute</c> attribute is used to tag a class as
	/// compatible with the <see cref="Placeholder"/> and <see cref="IController"/>
	/// pattern. This attribute is applied at the assembly level.
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple=true)]
	public class ControllerAttribute : System.Attribute, Epsitec.Common.Support.PlugIns.IPlugInAttribute<string>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerAttribute"/> class.
		/// </summary>
		/// <param name="type">The type of the class.</param>
		public ControllerAttribute(System.Type type)
		{
			this.type = type;
		}

		/// <summary>
		/// Gets or sets the type of the class which adheres to the
		/// <see cref="Placeholder"/> and <see cref="IController"/> pattern.
		/// </summary>
		/// <value>The type.</value>
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


		/// <summary>
		/// Gets the registered types for a specified assembly.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <returns>The first registered type (as a collection).</returns>
		public static IEnumerable<System.Type> GetRegisteredTypes(System.Reflection.Assembly assembly)
		{
			foreach (var attribute in assembly.GetCustomAttributes<ControllerAttribute> ())
			{
				if (attribute.Type.ContainsInterface<IController> ())
				{
					yield return attribute.Type;
					break;
				}
			}
		}

		#region IPlugInAttribute<string> Members

		string Epsitec.Common.Support.PlugIns.IPlugInAttribute<string>.Id
		{
			get
			{
				return this.type.Name;
			}
		}

		System.Type Epsitec.Common.Support.PlugIns.IPlugInAttribute<string>.Type
		{
			get
			{
				return this.type;
			}
		}

		#endregion

		private System.Type						type;
	}
}
