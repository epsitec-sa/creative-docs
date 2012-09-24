//	Copyright © 2007-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>EntityFieldAttribute</c> class defines an <c>EntityField</c>
	/// attribute, which is used by <see cref="EntityContext"/> to map fields
	/// to .NET class properties.
	/// </summary>
	[System.AttributeUsage (System.AttributeTargets.Property, AllowMultiple=false)]
	public sealed class EntityFieldAttribute : System.Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityFieldAttribute"/> class.
		/// </summary>
		/// <param name="fieldId">The field id.</param>
		public EntityFieldAttribute(string fieldId)
		{
			this.fieldId = fieldId;
		}

		/// <summary>
		/// Gets the field id.
		/// </summary>
		/// <value>The field id.</value>
		public string							FieldId
		{
			get
			{
				return this.fieldId;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this field is virtual.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this field is virtual; otherwise, <c>false</c>.
		/// </value>
		public bool								IsVirtual
		{
			get
			{
				return this.isVirtual;
			}
			set
			{
				this.isVirtual = value;
			}
		}
		
		private string							fieldId;
		private bool							isVirtual;
	}
}
