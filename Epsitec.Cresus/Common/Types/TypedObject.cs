//	Copyright © 2006-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Linq;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>TypedObject</c> structure is used to make any type for which
	/// there is a converter, compatible with the <c>DependencyProperty</c>
	/// serialization.
	/// </summary>
	[System.Serializable]
	[System.ComponentModel.TypeConverter (typeof (TypedObject.Converter))]
	public struct TypedObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TypedObject"/> structure.
		/// </summary>
		/// <param name="value">The value.</param>
		public TypedObject(object value)
		{
			this.value = value;
		}

		/// <summary>
		/// Gets the value of this typed object.
		/// </summary>
		/// <value>The value.</value>
		public object Value
		{
			get
			{
				return this.value;
			}
		}

		/// <summary>
		/// Converts the value to a string representation.
		/// </summary>
		/// <returns>
		/// A string representation of the value.
		/// </returns>
		public override string ToString()
		{
			if (this.value == null)
			{
				return "<null>";
			}
			else
			{
				System.Type type = this.value.GetType ();
				ISerializationConverter converter = InvariantConverter.GetSerializationConverter (type);
				string assemblyTypeName = string.Join (", ", type.AssemblyQualifiedName.Split (new string[] { ", " }, System.StringSplitOptions.None), 0, 2);
				return string.Concat (assemblyTypeName, "/", converter.ConvertToString (this.value, null));
			}
		}

		/// <summary>
		/// Parses the specified value and returns a typed object.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The typed object.</returns>
		public static TypedObject Parse(string value)
		{
			if (value == "<null>")
			{
				return new TypedObject (null);
			}
			else
			{
				int pos = value.IndexOf ('/');
				string typeName = value.Substring (0, pos);
				string strValue = value.Substring (pos+1);
				System.Type type = System.Type.GetType (typeName);

				if (type == null)
				{
					//	This happens if the type belongs to an assembly which was loaded dynamically;
					//	not sure why, however.
					//	http://stackoverflow.com/questions/3758209/why-would-system-type-gettypexyz-return-null-if-typeofxyz-exists

//-					System.Diagnostics.Debug.WriteLine ("Type '" + typeName + "' cannot be resolved by System.Type.GetType...");

					typeName = typeName.Split (',')[0];		//	keep just the type name and drop the assembly name

					var types = from assembly in System.AppDomain.CurrentDomain.GetAssemblies ()
								from assemblyType in assembly.GetTypes ()
								where assemblyType.FullName == typeName
									select assemblyType;

					type = types.FirstOrDefault ();

					System.Diagnostics.Debug.Assert (type != null, string.Format ("The type '{0}' is missing - not found in any loaded assembly.", typeName));
				}

				ISerializationConverter converter = InvariantConverter.GetSerializationConverter (type);
				return new TypedObject (converter.ConvertFromString (strValue, null));
			}
		}
		
		#region Converter Class
		
		public class Converter : AbstractStringConverter
		{
			public override object ParseString(string value, System.Globalization.CultureInfo culture)
			{
				return TypedObject.Parse (value);
			}

			public override string ToString(object value, System.Globalization.CultureInfo culture)
			{
				return ((TypedObject) value).ToString ();
			}
		}
		
		#endregion

		private object value;
	}
}
