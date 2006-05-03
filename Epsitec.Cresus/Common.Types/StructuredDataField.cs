//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The StructuredDataField class represents a data field in a <see cref="T:StructuredDataRecord"/>.
	/// </summary>
	public class StructuredDataField
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:StructuredDataField"/> class.
		/// </summary>
		/// <param name="name">The name of the field.</param>
		/// <param name="type">The type of the field.</param>
		public StructuredDataField(string name, INamedType type)
		{
			this.name = name;
			this.type = type;
		}

		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		public INamedType						Type
		{
			get
			{
				return this.type;
			}
		}

		
		private string name;
		private INamedType type;
	}
}
