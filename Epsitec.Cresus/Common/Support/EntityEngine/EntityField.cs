//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	public sealed class EntityField : System.IEquatable<EntityField>
	{
		public EntityField()
			: this (Druid.Empty, null)
		{

		}

		public EntityField(Druid entityId, string fieldId)
		{
			this.entityId = entityId;
			this.fieldId  = fieldId ?? "";
		}

		public EntityField(PropertyInfo propertyInfo)
			: this (Druid.Empty, null)
		{
			if (propertyInfo == null)
			{
				return;
			}

			var structuredType = EntityInfo.GetStructuredType (propertyInfo.DeclaringType);
			var fieldAttribute = propertyInfo.GetCustomAttributes<EntityFieldAttribute> (true).FirstOrDefault ();

			if ((structuredType == null) ||
				(fieldAttribute == null) ||
				(fieldAttribute.FieldId == null))
			{
				return;
			}

			this.entityId = structuredType.CaptionId;
			this.fieldId  = fieldAttribute.FieldId;
		}

		
		public Druid							EntityId
		{
			get
			{
				return entityId;
			}
		}

		public string							FieldId
		{
			get
			{
				return fieldId;
			}
		}


		public static explicit operator PropertyInfo(EntityField field)
		{
			return EntityFieldConverter.ConvertToProperty (field);
		}

		public static explicit operator EntityField(PropertyInfo propertyInfo)
		{
			return EntityFieldConverter.ConvertToEntityInfo (propertyInfo);
		}


		public static EntityField Parse(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				throw new System.ArgumentNullException ("vaue");
			}

			int pos = value.IndexOf (':');

			if (pos < 1)
			{
				throw new System.FormatException ("EntityField is not properly formatted");
			}

			var druid = value.Substring (0, pos);
			var field = value.Substring (pos+1);

			return new EntityField (Druid.Parse (druid), field);
		}


		public override string ToString()
		{
			return string.Concat (this.entityId.ToString (), ":", this.fieldId);
		}
		
		public override int GetHashCode()
		{
			return this.entityId.GetHashCode () ^ this.fieldId.GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			return this.Equals (obj as EntityField);
		}

		#region IEquatable<EntityField> Members

		public bool Equals(EntityField other)
		{
			if (other == null)
			{
				return false;
			}

			return this.entityId == other.entityId
				&& this.fieldId == other.fieldId;
		}

		#endregion


		private readonly Druid					entityId;
		private readonly string					fieldId;
	}
}
