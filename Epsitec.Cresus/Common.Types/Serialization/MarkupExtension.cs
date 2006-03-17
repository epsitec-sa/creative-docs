using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Types.Serialization
{
	public static class MarkupExtension
	{
		public static bool IsMarkupExtension(string value)
		{
			//	Return true is the value is a markup extension. This does not
			//	check for a valid syntax; it only analyses the value to see if
			//	the "{" and "}" markers are found.
			//
			//	Escaped values are recognized as such and won't be considered
			//	to be markup extensions.

			if ((value != null) &&
				(value.StartsWith ("{")) &&
				(value.StartsWith ("{}") == false) &&
				(value.EndsWith ("}")))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static string Escape(string value)
		{
			//	If needed, inserts a special escape sequence to make the value
			//	valid and easily recognizable as escaped by the markup extension
			//	parser.
			//
			//	NB: A value string may not contain { and } curly braces, since
			//		these are used to define the markup extensions.

			if ((value == null) ||
				(value.IndexOfAny (new char[] { '{', '}' }) < 0))
			{
				return value;
			}
			else
			{
				return string.Concat ("{}", value);
			}
		}
		public static string Unescape(string value)
		{
			//	If the string was escaped, remove the escape sequence and
			//	return the original string (see Escape).

			if ((value != null) &&
				(value.StartsWith ("{}")))
			{
				return value.Substring (2);
			}
			else
			{
				return value;
			}
		}

		public static string NullToString()
		{
			return "{Null}";
		}
		public static string BindingToString(Binding binding, IContextResolver resolver)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			buffer.Append ("{");
			buffer.Append ("Binding");

			string space = " ";

			DependencyObject source = binding.Source as DependencyObject;
			BindingMode mode = binding.Mode;
			DependencyPropertyPath path = binding.Path;

			if (source != null)
			{
				string markup = resolver.ResolveToMarkup (source);

				if (markup == null)
				{
					//	TODO: handle unknown sources
				}
				else
				{
					buffer.Append (space);
					space = ", ";

					buffer.Append ("Source=");
					buffer.Append (markup);
				}
			}

			if (path != null)
			{
				string value = path.GetFullPath ();

				if (value.Length > 0)
				{
					buffer.Append (space);
					space = ", ";

					buffer.Append ("Path=");
					buffer.Append (value);
				}
			}

			if (mode != BindingMode.None)
			{
				string value = mode.ToString ();

				buffer.Append (space);
				space = ", ";

				buffer.Append ("Mode=");
				buffer.Append (value);
			}

			buffer.Append ("}");
			return buffer.ToString ();
		}
		public static string ExtRefToString(object value, Context context)
		{
			return string.Concat ("{ExtRef ", context.ExternalMap.GetTag (value), "}");
		}
		public static string ObjRefToString(DependencyObject value, Context context)
		{
			return string.Concat ("{ObjRef ", Context.IdToString (context.ObjectMap.GetId (value)), "}");
		}
		public static string CollectionToString(ICollection<DependencyObject> collection, SerializerContext context)
		{
			System.Text.StringBuilder buffer = new StringBuilder ();

			buffer.Append ("{Collection");
			
			int i = -1;
			
			foreach (DependencyObject node in collection)
			{
				if (++i == 0)
				{
					buffer.Append (" ");
				}
				else
				{
					buffer.Append (", ");
				}
				
				if (node == null)
				{
					buffer.Append (MarkupExtension.NullToString ());
					continue;
				}
				
				if (context.ObjectMap.IsValueDefined (node))
				{
					buffer.Append (MarkupExtension.ObjRefToString (node, context));
					continue;
				}

				if (context.ExternalMap.IsValueDefined (node))
				{
					buffer.Append (MarkupExtension.ExtRefToString (node, context));
					continue;
				}
				
				throw new System.ArgumentException (string.Format ("Element {0} in collection cannot be resolved", i));
			}

			buffer.Append ("}");
			return buffer.ToString ();
		}
	}
}
