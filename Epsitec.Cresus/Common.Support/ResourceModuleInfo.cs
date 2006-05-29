//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ResourceModuleInfo</c> stores the name and the identifier of a
	/// resource module.
	/// </summary>
	public struct ResourceModuleInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ResourceModuleInfo"/> structure.
		/// </summary>
		/// <param name="name">The name of the module.</param>
		/// <param name="id">The id of the module.</param>
		public ResourceModuleInfo(string name, int id)
		{
			this.name = name;
			this.id = id;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ResourceModuleInfo"/> structure.
		/// </summary>
		/// <param name="name">The name of the module.</param>
		public ResourceModuleInfo(string name)
		{
			this.name = name;
			this.id = -1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ResourceModuleInfo"/> structure.
		/// </summary>
		/// <param name="id">The id of the module.</param>
		public ResourceModuleInfo(int id)
		{
			this.name = null;
			this.id = id;
		}

		/// <summary>
		/// Gets the name of the module.
		/// </summary>
		/// <value>The name of the module.</value>
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// Gets the id of the module.
		/// </summary>
		/// <value>The id of the module.</value>
		public int Id
		{
			get
			{
				return this.id;
			}
		}

		#region Private Fields
		private string name;
		private int id;
		#endregion
	}
}
