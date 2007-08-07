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
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceModuleInfo"/> class.
		/// </summary>
		public ResourceModuleInfo()
		{
		}

		/// <summary>
		/// Gets or sets the resource module id for this module.
		/// </summary>
		/// <value>The resource module id.</value>
		public ResourceModuleId FullId
		{
			get
			{
				return this.fullId;
			}
			set
			{
				this.VerifyWritable ("FullId");
				this.fullId = value;
			}
		}

		/// <summary>
		/// Gets or sets the path of the reference module.
		/// </summary>
		/// <value>The path of the reference module.</value>
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

		/// <summary>
		/// Gets or sets the source namespace used when generating associated
		/// code.
		/// </summary>
		/// <value>The source namespace.</value>
		public string SourceNamespace
		{
			get
			{
				return this.sourceNamespace;
			}
			set
			{
				this.VerifyWritable ("SourceNamespace");
				this.sourceNamespace = value;
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
		public ResourceModuleInfo Clone()
		{
			ResourceModuleInfo copy = new ResourceModuleInfo ();

			copy.fullId = this.fullId;
			copy.referenceModulePath = this.referenceModulePath;
			copy.sourceNamespace = this.sourceNamespace;

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
		ResourceModuleId fullId;
		string referenceModulePath;
		string sourceNamespace;
	}
}
