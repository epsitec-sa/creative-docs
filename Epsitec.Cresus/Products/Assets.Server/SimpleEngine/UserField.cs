//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public struct UserField
	{
		public UserField(Guid guid, string name, ObjectField field, FieldType type, int maxLength = 100)
		{
			this.Guid      = guid;
			this.Name      = name;
			this.Field     = field;
			this.Type      = type;
			this.MaxLength = maxLength;
		}

		public UserField(string name, ObjectField field, FieldType type, int maxLength = 100)
		{
			this.Guid      = Guid.NewGuid ();
			this.Name      = name;
			this.Field     = field;
			this.Type      = type;
			this.MaxLength = maxLength;
		}

		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty (this.Name)
					&& this.Field == ObjectField.Unknown
					&& this.Type  == FieldType.Unknown;
			}
		}


		public static UserField Empty = new UserField (null, ObjectField.Unknown, FieldType.Unknown, 0);


		public readonly Guid					Guid;
		public readonly string					Name;
		public readonly ObjectField				Field;
		public readonly FieldType				Type;
		public readonly int						MaxLength;
	}
}