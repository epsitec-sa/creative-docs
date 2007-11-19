//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	
	[System.AttributeUsage (System.AttributeTargets.Property, AllowMultiple=false)]
	public sealed class EntityFieldAttribute : System.Attribute
	{
		public EntityFieldAttribute(string fieldId)
		{
			this.fieldId = fieldId;
		}

		
		/// <summary>
		/// Gets the field id.
		/// </summary>
		/// <value>The field id.</value>
		public string FieldId
		{
			get
			{
				return this.fieldId;
			}
		}
		
		private string fieldId;
	}
}
