//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>INamedType</c> interface is implemented by all type description
	/// classes. It is also the base for interfaces such as <see cref="T:INumericType" />,
	/// <see cref="T:IEnumType" /> and <see cref="T:IStringType" />.
	/// </summary>
	public interface INamedType : ICaption, IName, ISystemType
	{
		/// <summary>
		/// Gets the default controller used to represent data of this type.
		/// </summary>
		/// <value>The default controller.</value>
		string DefaultController
		{
			get;
		}

		/// <summary>
		/// Gets the parameter used with the default controller.
		/// </summary>
		/// <value>The default controller parameter.</value>
		string DefaultControllerParameter
		{
			get;
		}
	}
}
