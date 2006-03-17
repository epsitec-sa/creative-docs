//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types.Serialization.Generic;

namespace Epsitec.Common.Types.Serialization
{
	public class DeserializerContext : Context
	{
		public DeserializerContext(IO.AbstractReader reader)
		{
			this.reader = reader;
		}

		public override void RestoreObjectData(int id, DependencyObject obj)
		{
			this.AssertReadable ();

			this.reader.BeginObject (id, obj);

			string field;
			string value;

			while (this.reader.ReadObjectFieldValue (obj, out field, out value))
			{
				this.RestoreObjectField (obj, field, value);
			}
			
			this.reader.EndObject (id, obj);
		}

		private void RestoreObjectField(DependencyObject obj, string field, string value)
		{
			System.Console.Out.WriteLine ("{0}: {1}='{2}'", this.ObjectMap.GetId (obj), field, value);
		}
	}
}
