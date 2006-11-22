//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemViewFactoryAttribute</c> attribute is used to tag a class as
	/// compatible with the <see cref="ItemPanel"/> and <see cref="IItemViewFactory"/>
	/// pattern. This attribute is applied at the assembly level.
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple=true)]
	public class ItemViewFactoryAttribute : System.Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ItemViewFactoryAttribute"/> class.
		/// </summary>
		/// <param name="type">The type of the class.</param>
		public ItemViewFactoryAttribute(System.Type type)
		{
			this.type = type;
		}

		/// <summary>
		/// Gets or sets the type of the class which adheres to the
		/// <see cref="ItemPanel"/> and <see cref="IItemViewFactory"/> pattern.
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
		/// <returns>The registered types.</returns>
		public static IEnumerable<System.Type> GetRegisteredTypes(System.Reflection.Assembly assembly)
		{
			System.Type factoryType = typeof (IItemViewFactory);

			foreach (ItemViewFactoryAttribute attribute in assembly.GetCustomAttributes (typeof (ItemViewFactoryAttribute), false))
			{
				//	Return only types which describe classes that implement the
				//	IItemViewFactory interface :

				foreach (System.Type interfaceType in attribute.Type.GetInterfaces ())
				{
					if (interfaceType == factoryType)
					{
						yield return attribute.Type;
						break;
					}
				}
			}
		}

		private System.Type						type;
	}
}
