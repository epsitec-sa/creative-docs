//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ResourceModuleInfo</c> class stores the module identity and
	/// associated properties.
	/// </summary>
	public sealed class ResourceModuleInfo : Epsitec.Common.Types.IReadOnly
	{
		public ResourceModuleInfo()
		{
		}

		public ResourceModuleId FullId
		{
			get
			{
				return this.id;
			}
			set
			{
				this.VerifyWritable ("FullId");
				this.id = value;
			}
		}

		public string ReferenceModulePath
		{
			get
			{
				return this.referenceModulePath;
			}
			set
			{
				this.VerifyWritable ("ReferenceModulePath");
				this.referenceModulePath = value;
			}
		}

		#region Interface IReadOnly

		public bool IsReadOnly
		{
			get
			{
				return this.readOnly;
			}
		}

		#endregion

		public void Freeze()
		{
			this.readOnly = true;
		}

		public ResourceModuleInfo Clone()
		{
			ResourceModuleInfo copy = new ResourceModuleInfo ();

			copy.id = this.id;
			copy.referenceModulePath = this.referenceModulePath;

			return copy;
		}

		private void VerifyWritable(string property)
		{
			if (this.readOnly)
			{
				throw new System.InvalidOperationException (string.Format ("Property {0} is not writable", property));
			}
		}

		bool readOnly;
		ResourceModuleId id;
		string referenceModulePath;
	}
}
