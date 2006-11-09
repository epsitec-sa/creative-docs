//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Cresus.Database
{
	using OpCodes = System.Reflection.Emit.OpCodes;
	using InvariantConverter = Epsitec.Common.Types.InvariantConverter;
	
	/// <summary>
	/// The <c>DbTools</c> class provides a set of useful methods.
	/// </summary>
	public static class DbTools
	{
		/// <summary>
		/// Writes the XML attribute to the XML writer if the value is not empty.
		/// </summary>
		/// <param name="xmlWriter">The XML writer.</param>
		/// <param name="name">The attribute name.</param>
		/// <param name="value">The attribute value.</param>
		public static void WriteAttribute(System.Xml.XmlTextWriter xmlWriter, string name, string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				//	Nothing to do; an empty attribute will never be written to
				//	the XML output.
			}
			else
			{
				xmlWriter.WriteAttributeString (name, value);
			}
		}


		/// <summary>
		/// Makes a composite name based on a list of strings. The individual
		/// names are joined using the "_" character. No trailing "_" will be
		/// generated.
		/// </summary>
		/// <param name="list">The list of strings.</param>
		/// <returns>The composite name.</returns>
		public static string MakeCompositeName(params string[] list)
		{
			int num = list.Length;
			
			while ((num > 0) && (string.IsNullOrEmpty (list[num-1])))
			{
				num--;
			}
			
			return (num == 0) ? "" : string.Join ("_", list, 0, num);
		}
		
		
		public static DbElementCat   ParseElementCategory(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return DbElementCat.Unknown;
			}
			else
			{
				return (DbElementCat) InvariantConverter.ParseInt (text);
			}
		}
		
		public static DbRevisionMode ParseRevisionMode(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return DbRevisionMode.Unknown;
			}
			else
			{
				return (DbRevisionMode) InvariantConverter.ParseInt (text);
			}
		}
		
		public static DbReplicationMode ParseReplicationMode(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return DbReplicationMode.Unknown;
			}
			else
			{
				return (DbReplicationMode) InvariantConverter.ParseInt (text);
			}
		}

		public static DbColumnClass ParseColumnClass(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return DbColumnClass.Data;
			}
			else
			{
				return (DbColumnClass) InvariantConverter.ParseInt (text);
			}
		}

		public static DbColumnLocalization ParseLocalization(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return DbColumnLocalization.None;
			}
			else
			{
				return (DbColumnLocalization) InvariantConverter.ParseInt (text);
			}
		}

		public static DbRawType ParseRawType(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return DbRawType.Unknown;
			}

			int num;
			InvariantConverter.Convert (value, out num);
			return (DbRawType) num;
		}

		public static DbSimpleType ParseSimpleType(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return DbSimpleType.Unknown;
			}

			int num;
			InvariantConverter.Convert (value, out num);
			return (DbSimpleType) num;
		}

		public static string ParseString(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}
			else
			{
				return value;
			}
		}

		public static int ParseInt(string value)
		{
			return InvariantConverter.ParseInt (value);
		}

		public static long ParseLong(string value)
		{
			return InvariantConverter.ParseLong (value);
		}

		public static decimal ParseDecimal(string value)
		{
			return InvariantConverter.ParseDecimal (value);
		}

		public static Druid ParseDruid(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return Druid.Empty;
			}
			else
			{
				return Druid.Parse (value);
			}
		}

		public static bool ParseDefaultingToFalseBool(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return false;
			}
			else
			{
				System.Diagnostics.Debug.Assert (value == "Y");

				return true;
			}
		}

		public static bool ParseDefaultingToTrueBool(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return false;
			}
			else
			{
				System.Diagnostics.Debug.Assert (value == "N");

				return true;
			}
		}

		public static string ElementCategoryToString(DbElementCat cat)
		{
			if (cat == DbElementCat.Unknown)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString ((int) cat);
			}
		}
		
		public static string RevisionModeToString(DbRevisionMode mode)
		{
			if (mode == DbRevisionMode.Unknown)
			{
				return null;
			}
			
			return InvariantConverter.ToString ((int) mode);
		}
		
		public static string ReplicationModeToString(DbReplicationMode mode)
		{
			if (mode == DbReplicationMode.Unknown)
			{
				return null;
			}
			
			return InvariantConverter.ToString ((int) mode);
		}

		public static string ColumnClassToString(DbColumnClass value)
		{
			if (value == DbColumnClass.Data)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString ((int) value);
			}
		}

		public static string ColumnLocalizationToString(DbColumnLocalization value)
		{
			if (value == DbColumnLocalization.None)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString ((int) value);
			}
		}

		public static string DruidToString(Druid value)
		{
			if (value.IsEmpty)
			{
				return null;
			}
			else
			{
				return value.ToString ();
			}
		}

		public static string TypeToString(INamedType value)
		{
			if (value == null)
			{
				return null;
			}
			else
			{
				return value.CaptionId.ToString ();
			}
		}

		public static string RawTypeToString(DbRawType dbRawType)
		{
			if (dbRawType == DbRawType.Unknown)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString ((int) dbRawType);
			}
		}
		
		public static string SimpleTypeToString(DbSimpleType dbSimpleType)
		{
			if (dbSimpleType == DbSimpleType.Unknown)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString ((int) dbSimpleType);
			}
		}


		public static string IntToString(int value)
		{
			if (value == 0)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString (value);
			}
		}

		public static string LongToString(long value)
		{
			if (value == 0)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString (value);
			}
		}

		public static string DecimalToString(decimal value)
		{
			if (value == 0)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString (value);
			}
		}
		
		public static string BoolDefaultingToFalseToString(bool value)
		{
			return value ? "Y" : null;
		}

		public static string BoolDefaultingToTrueToString(bool value)
		{
			return value ? "N" : null;
		}

		public static string StringToString(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}
			else
			{
				return value;
			}
		}

		public static string GetCompactXml(IXmlSerializable value)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			System.IO.StringWriter writer = new System.IO.StringWriter (buffer);
			System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter (writer);

			value.Serialize (xmlWriter);

			xmlWriter.Flush ();
			xmlWriter.Close ();

			return buffer.ToString ();
		}

		public static T DeserializeFromXml<T>(string value)
		{
			using (System.IO.StringReader reader = new System.IO.StringReader (value))
			{
				using (System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader (reader))
				{
					if (xmlReader.Read ())
					{
						return DbTools.DeserializeFromXml<T> (xmlReader);
					}
					else
					{
						throw new System.ArgumentException ("Invalid XML for deserialization", "value");
					}
				}
			}
		}

		public static T DeserializeFromXml<T>(System.Xml.XmlTextReader xmlReader)
		{
			Deserializer<T> deserializer;
			
			lock (DbTools.exclusion)
			{
				object method;
				DbTools.deserializers.TryGetValue (typeof (T), out method);

				if (method == DbTools.noDeserializer)
				{
					throw new System.ArgumentException (string.Format ("No deserializer for type {0}", typeof (T).Name));
				}
				
				deserializer = method as Deserializer<T>;
			}

			if (deserializer == null)
			{
				deserializer = DbTools.CreateDeserializer<T> ();
				
				lock (DbTools.exclusion)
				{
					if (deserializer == null)
					{
						DbTools.deserializers[typeof (T)] = DbTools.noDeserializer;
						
						throw new System.ArgumentException (string.Format ("No deserializer for type {0}", typeof (T).Name));
					}
					else
					{
						DbTools.deserializers[typeof (T)] = deserializer;
					}
				}
			}

			return deserializer (xmlReader);
		}


		private delegate T Deserializer<T>(System.Xml.XmlTextReader xmlReader);

		private static Deserializer<T> CreateDeserializer<T>()
		{
			System.Type type = typeof (T);
			System.Type xmlType   = typeof (System.Xml.XmlTextReader);
			System.Type hostType  = type;
			System.Type[] arguments = new System.Type[] { xmlType };
			
			System.Reflection.MethodInfo method = type.GetMethod ("Deserialize", arguments);

			if ((method == null) ||
				(method.IsStatic == false) ||
				(method.ReturnType != type))
			{
				return null;
			}

			string name = string.Concat ("DynamicCode_XmlDeserializer_", type.Name);

			System.Reflection.Emit.DynamicMethod dynamic;
			dynamic = new System.Reflection.Emit.DynamicMethod (name, type, arguments, type);

			System.Reflection.Emit.ILGenerator generator = dynamic.GetILGenerator ();

			generator.Emit (OpCodes.Ldarg_0);
			generator.EmitCall (OpCodes.Call, method, null);
			generator.Emit (OpCodes.Ret);

			return (Deserializer<T>) dynamic.CreateDelegate (typeof (Deserializer<T>));
		}

		private static object exclusion = new object ();
		private static Dictionary<System.Type, object> deserializers = new Dictionary<System.Type, object> ();
		private static object noDeserializer = new object ();
	}
}
