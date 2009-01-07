//	Copyright © 2008-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>KernelDatabaseInfo</c> structure stores a database unique id and
	/// its user friendly name.
	/// </summary>
	[System.Serializable]
	public struct KernelDatabaseInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="KernelDatabaseInfo"/> struct.
		/// </summary>
		/// <param name="id">The unique id.</param>
		/// <param name="name">The user friendly name.</param>
		public KernelDatabaseInfo(System.Guid id, string name)
		{
			this.id = id;
			this.name = name;
		}

		/// <summary>
		/// Gets the unique id.
		/// </summary>
		/// <value>The unique id.</value>
		public System.Guid Id
		{
			get
			{
				return this.id;
			}
		}

		/// <summary>
		/// Gets the user friendly name.
		/// </summary>
		/// <value>The user friendly name.</value>
		public string Name
		{
			get
			{
				return this.name;
			}
		}


		private readonly System.Guid			id;
		private readonly string					name;
	}
}
