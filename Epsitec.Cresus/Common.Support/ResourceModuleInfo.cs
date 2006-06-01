//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ResourceModuleInfo</c> stores the name and the identifier of a
	/// resource module.
	/// </summary>
	public struct ResourceModuleInfo : System.IEquatable<ResourceModuleInfo>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ResourceModuleInfo"/> structure.
		/// </summary>
		/// <param name="name">The name of the module.</param>
		/// <param name="id">The id of the module.</param>
		public ResourceModuleInfo(string name, int id)
		{
			this.name = string.IsNullOrEmpty (name) ? null : name;
			this.id = id+1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ResourceModuleInfo"/> structure.
		/// </summary>
		/// <param name="name">The name of the module.</param>
		public ResourceModuleInfo(string name)
		{
			this.name = string.IsNullOrEmpty (name) ? null : name;
			this.id = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ResourceModuleInfo"/> structure.
		/// </summary>
		/// <param name="id">The id of the module.</param>
		public ResourceModuleInfo(int id)
		{
			this.name = null;
			this.id = id+1;
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
				return this.id-1;
			}
		}

		/// <summary>
		/// Returns the module id as a string representation.
		/// </summary>
		/// <returns>
		/// The module id if it is valid; otherwise, <c>null</c>.
		/// </returns>
		public override string ToString()
		{
			int id = this.Id;

			if (id < 0)
			{
				return null;
			}
			else
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}", id);
			}
		}

		/// <summary>
		/// Parses the specified string, which can be either the module name or a
		/// module id.
		/// </summary>
		/// <param name="moduleName">Name or id of the module.</param>
		/// <returns>The module information structure.</returns>
		public static ResourceModuleInfo Parse(string module)
		{
			int moduleId;

			if (int.TryParse (module, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out moduleId))
			{
				return new ResourceModuleInfo (moduleId);
			}
			else
			{
				return new ResourceModuleInfo (module);
			}
		}

		#region IEquatable<ResourceModuleInfo> Members

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// <c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>.
		/// </returns>
		public bool Equals(ResourceModuleInfo other)
		{
			return (this.id == other.id) && (this.name == other.name);
		}

		#endregion

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>
		/// <c>true</c> if obj and this instance are the same type and represent the same value; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj is ResourceModuleInfo)
			{
				return this.Equals ((ResourceModuleInfo) obj);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		public override int GetHashCode()
		{
			if (this.name == null)
			{
				return this.id;
			}
			else if (this.id == 0)
			{
				return this.name.GetHashCode ();
			}
			else
			{
				return this.id;
			}
		}

		#region Private Fields
		
		private string							name;			//	null or name
		private int								id;				//	0 or module id+1
		
		#endregion
	}
}
