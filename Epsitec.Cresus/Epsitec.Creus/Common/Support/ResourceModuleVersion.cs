//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ResourceModuleVersion</c> class stores a version information.
	/// </summary>
	public sealed class ResourceModuleVersion : Epsitec.Common.Types.IReadOnly
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceModuleVersion"/> class.
		/// </summary>
		public ResourceModuleVersion()
		{
		}

		/// <summary>
		/// Initializes a new read-only instance of the <see cref="ResourceModuleVersion"/> class.
		/// </summary>
		/// <param name="developerId">The developer id.</param>
		/// <param name="buildNumber">The build number.</param>
		/// <param name="buildDate">The UTC build date.</param>
		public ResourceModuleVersion(int developerId, int buildNumber, System.DateTime buildDate)
		{
			this.DeveloperId = developerId;
			this.BuildNumber = buildNumber;
			this.BuildDate   = buildDate.ToUniversalTime ();
			this.Freeze ();
		}


		/// <summary>
		/// Gets or sets the developer id.
		/// </summary>
		/// <value>The developer id.</value>
		public int DeveloperId
		{
			get
			{
				return this.developerId;
			}
			set
			{
				if (this.developerId != value)
				{
					this.VerifyWritable ("DeveloperId");
					this.developerId = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the build number.
		/// </summary>
		/// <value>The build number.</value>
		public int BuildNumber
		{
			get
			{
				return this.buildNumber;
			}
			set
			{
				if (this.buildNumber != value)
				{
					this.VerifyWritable ("BuildNumber");
					this.buildNumber = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the build date. The build date must always be specified
		/// using a UTC representation.
		/// </summary>
		/// <value>The build date.</value>
		public System.DateTime BuildDate
		{
			get
			{
				return this.buildDate;
			}
			set
			{
				if (this.buildDate != value)
				{
					this.VerifyWritable ("BuildDate");

					if (value.Kind != System.DateTimeKind.Utc)
					{
						throw new System.ArgumentException (string.Format ("{0} uses DateTimeKind.{1}; only UTC allowed", "BuildDate", value.Kind));
					}

					this.buildDate = value;
				}
			}
		}


		/// <summary>
		/// Gets a comparer which can be used to generate a sorted list of
		/// module versions. The sort is based on the developer id, the build
		/// number and finally the build date.
		/// </summary>
		/// <value>The comparer delegate.</value>
		public static System.Comparison<ResourceModuleVersion> Comparer
		{
			get
			{
				return
					delegate (ResourceModuleVersion a, ResourceModuleVersion b)
					{
						if (a.DeveloperId < b.DeveloperId)
						{
							return -1;
						}
						else if (a.DeveloperId > b.DeveloperId)
						{
							return 1;
						}

						if (a.BuildNumber < b.BuildNumber)
						{
							return -1;
						}
						else if (a.BuildNumber > b.BuildNumber)
						{
							return 1;
						}

						return a.BuildDate.CompareTo (b.BuildDate);
					};
			}
		}

		
		#region Interface IReadOnly

		/// <summary>
		/// Gets a value indicating whether this instance is read only.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
		/// </value>
		public bool IsReadOnly
		{
			get
			{
				return this.isFrozen;
			}
		}

		#endregion

		/// <summary>
		/// Freezes this instance. This makes the instance read only. No further
		/// modification will be possible. Any attempt to modify the properties
		/// will throw a <see cref="System.InvalidOperationException"/>.
		/// </summary>
		public void Freeze()
		{
			this.isFrozen = true;
		}

		/// <summary>
		/// Returns a copy of this instance. The copy is modifiable.
		/// </summary>
		/// <returns>The copy of this instance.</returns>
		public ResourceModuleVersion Clone()
		{
			ResourceModuleVersion copy = new ResourceModuleVersion ();

			copy.developerId = this.developerId;
			copy.buildNumber = this.buildNumber;
			copy.buildDate   = this.buildDate;
			
			return copy;
		}

		private void VerifyWritable(string property)
		{
			if (this.isFrozen)
			{
				throw new System.InvalidOperationException (string.Format ("Property {0} is not writable", property));
			}
		}

		bool isFrozen;
		int developerId;
		int buildNumber;
		System.DateTime buildDate;
	}
}
