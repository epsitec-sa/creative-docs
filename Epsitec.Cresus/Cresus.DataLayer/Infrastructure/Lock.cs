//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{


	/// <summary>
	/// The <c>Lock</c> class provides information about who owns a given lock in the database.
	/// </summary>
	public sealed class Lock
	{

		
		internal Lock(Connection owner , string name, System.DateTime creationTime)
		{
			owner.ThrowIfNull ("owner");
			name.ThrowIfNullOrEmpty ("name");

			this.owner = owner;
			this.name = name;
			this.creationTime = creationTime;
		}


		/// <summary>
		/// Gets the low level connection identity.
		/// </summary>
		/// <value>The connection identity.</value>
		public Connection Owner
		{
			get
			{
				return this.owner;
			}
		}


		/// <summary>
		/// Gets the low level name of the lock.
		/// </summary>
		/// <value>The name of the lock.</value>
		public string Name
		{
			get
			{
				return this.name;
			}
		}


		/// <summary>
		/// Gets the date and time when the lock was acquired in the database.
		/// </summary>
		/// <value>The lock date and time.</value>
		public System.DateTime CreationTime
		{
			get
			{
				return this.creationTime;
			}
		}


		private readonly Connection owner;


		private readonly string name;


		private readonly System.DateTime creationTime;


	}


}
