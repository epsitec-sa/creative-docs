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

			DependencyProperty property = null;

			if (field.IndexOf ('.') < 0)
			{
				//	This is a standard, simple field.

				property = obj.ObjectType.GetProperty (field);
			}
			else
			{
				string[] args = field.Split ('.');
				
				string typeTag = args[0];
				string name    = args[1];

				DependencyObjectType type = DependencyObjectType.FromSystemType (this.ObjectMap.GetType (Context.ParseId (typeTag)));

				property = type.GetProperty (name);
			}

			if (property == null)
			{
				throw new System.ArgumentException (string.Format ("Property {0} could not be resolved", field));
			}

			if (MarkupExtension.IsMarkupExtension (value))
			{
				//	This is a markup extension
			}
			else
			{
				object data = property.ConvertFromString (MarkupExtension.Unescape (value), this);
				obj.SetValue (property, data);
			}
		}
	}
}
