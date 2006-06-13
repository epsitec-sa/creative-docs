//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization
{
	/// <summary>
	/// The MarkupExtension class provides the basic conversion methods needed
	/// to convert DependencyObject references/bindings/external object references
	/// to strings and back to the original objects.
	/// </summary>
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

		public static string BindingToString(IContextResolver resolver, Binding binding)
		{
			ResourceBinding resBinding = binding as ResourceBinding;
			
			//	If the binding object really is a resource binding, use the specialized
			//	ResourceBindingToString method :
			
			if (resBinding != null)
			{
				return MarkupExtension.ResourceBindingToString (resolver, resBinding);
			}
			
			//	Convert the binding description to a string representation :
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			buffer.Append ("{");
			buffer.Append ("Binding");

			string space = " ";

			DependencyObject source = binding.Source as DependencyObject;
			BindingMode mode = binding.Mode;
			string path = binding.Path;

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

			if ((path != null) &&
				(path.Length > 0))
			{
				buffer.Append (space);
				space = ", ";

				buffer.Append ("Path=");
				buffer.Append (path);
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

		public static string ResourceBindingToString(IContextResolver resolver, ResourceBinding binding)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			buffer.Append ("{");
			buffer.Append ("ResBinding");
			buffer.Append (" ");
			buffer.Append ("Id=");
			buffer.Append (binding.ResourceId);
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
		
		public static string CollectionToString(IEnumerable<DependencyObject> collection, SerializerContext context)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

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
			return (i < 0) ? null : buffer.ToString ();
		}

		
		public static object Resolve(Context context, string markup)
		{
			string[] args = MarkupExtension.Explode (markup);

			if (args.Length > 0)
			{
				string tag = args[0];

				switch (tag)
				{
					case "Null":
						return MarkupExtension.NullFromString (context, args);

					case "Binding":
						return MarkupExtension.BindingFromString (context, args);
					
					case "ResBinding":
						return MarkupExtension.ResourceBindingFromString (context, args);

					case "ObjRef":
						return MarkupExtension.ObjRefFromString (context, args);

					case "ExtRef":
						return MarkupExtension.ExtRefFromString (context, args);

					case "Collection":
						return MarkupExtension.CollectionFromString (context, args);
				}
			}

			throw new System.NotImplementedException (string.Format ("Cannot resolve '{0}'", markup));
		}

		
		public static Binding BindingFromString(IContextResolver context, string value)
		{
			if (value.StartsWith ("ResBinding"))
			{
				return MarkupExtension.ResourceBindingFromString (context, value);
			}
			
			string[] args = MarkupExtension.Explode (value);

			if ((args.Length == 0) ||
				(args[0] != "Binding"))
			{
				throw new System.FormatException (string.Format ("String '{0}' is not a valid Binding expression", value));
			}

			return MarkupExtension.BindingFromString (context, args);
		}
		
		public static ResourceBinding ResourceBindingFromString(IContextResolver context, string value)
		{
			string[] args = MarkupExtension.Explode (value);

			if ((args.Length == 0) ||
				(args[0] != "ResBinding"))
			{
				throw new System.FormatException (string.Format ("String '{0}' is not a valid ResBinding expression", value));
			}

			return MarkupExtension.ResourceBindingFromString (context, args);
		}
		
		public static string[] Explode(string source)
		{
			if ((source.StartsWith ("{")) &&
				(source.EndsWith ("}")))
			{
				int start = 1;
				int end = source.Length-1;
				int num = 0;
				
				bool onSpace = true;
				bool hasComma = false;
				bool isEmpty = true;
				int skipBraces = 0;

				for (int i = start; i < end; i++)
				{
					bool wasEmpty = isEmpty;
					
					char c = source[i];

					if (skipBraces > 0)
					{
						switch (c)
						{
							case '{':
								skipBraces++;
								break;
							case '}':
								skipBraces--;
								break;
						}
						continue;
					}
					
					if (char.IsWhiteSpace (c))
					{
						if (onSpace)
						{
							continue;
						}
						
						onSpace = true;
						continue;
					}

					isEmpty = false;
					
					if (c == ',')
					{
						if (hasComma || wasEmpty)
						{
							hasComma = true;
							num++;
							continue;
						}

						hasComma = true;
						onSpace = true;
						continue;
					}
					if (onSpace)
					{
						hasComma = false;
						onSpace = false;
						num++;
					}

					if (c == '{')
					{
						skipBraces++;
					}
				}
				
				if (hasComma)
				{
					num++;
				}

				string[] args = new string[num];

				onSpace = true;
				hasComma = false;
				isEmpty = true;
				skipBraces = 0;

				int index = 0;

				for (int i = start; i < end; i++)
				{
					bool wasEmpty = isEmpty;
					
					char c = source[i];

					if (skipBraces > 0)
					{
						switch (c)
						{
							case '{':
								skipBraces++;
								break;
							case '}':
								skipBraces--;
								break;
						}
						continue;
					}
					
					if (char.IsWhiteSpace (c))
					{
						if (onSpace)
						{
							continue;
						}

						args[index++] = source.Substring (start, i-start);
						
						onSpace = true;
						continue;
					}
					
					isEmpty = false;
					
					if (c == ',')
					{
						if (hasComma || wasEmpty)
						{
							hasComma = true;
							args[index++] = "";
							start = i+1;
							continue;
						}

						hasComma = true;

						if (onSpace)
						{
							continue;
						}
						
						args[index++] = source.Substring (start, i-start);
						
						onSpace = true;
						continue;
					}
					if (onSpace)
					{
						hasComma = false;
						onSpace = false;
						start = i;
					}
					
					if (c == '{')
					{
						skipBraces++;
					}
				}

				if (hasComma)
				{
					args[index++] = "";
				}
				else if (onSpace == false)
				{
					args[index++] = source.Substring (start, end-start);
				}

				System.Diagnostics.Debug.Assert (index == num);
				
				return args;
			}
			else
			{
				throw new System.FormatException (string.Format ("String '{0}' is not a valid markup extension", source));
			}
		}

		
		private static object NullFromString(Context context, string[] args)
		{
			if ((args.Length != 1) ||
				(args[0] != "Null"))
			{
				throw new System.FormatException ("Null format error");
			}

			return null;
		}
		
		private static Binding BindingFromString(IContextResolver context, string[] args)
		{
			if ((args.Length > 0) &&
				(args[0] == "ResBinding"))
			{
				return MarkupExtension.ResourceBindingFromString (context, args);
			}
			
			if ((args.Length < 1) ||
				(args[0] != "Binding"))
			{
				throw new System.FormatException ("Binding format error");
			}

			Binding binding = new Binding ();

			for (int i = 1; i < args.Length; i++)
			{
				string element = args[i];

				if (element.Length > 0)
				{
					string[] elems = element.Split ('=');

					if (elems.Length != 2)
					{
						throw new System.FormatException (string.Format ("Element '{0}' not valid in Binding expression", element));
					}

					System.Enum mode;

					switch (elems[0])
					{
						case "Path":
							binding.Path = elems[1];
							break;
						case "Source":
							binding.Source = context.ResolveFromMarkup (elems[1]);
							break;
						case "Mode":
							InvariantConverter.Convert (elems[1], typeof (BindingMode), out mode);
							binding.Mode = (BindingMode) mode;
							break;

						default:
							throw new System.FormatException (string.Format ("Element '{0}' not valid in Binding expression", element));
					}
				}
			}

			return binding;
		}

		private static ResourceBinding ResourceBindingFromString(IContextResolver context, string[] args)
		{
			if ((args.Length < 1) ||
				(args[0] != "ResBinding"))
			{
				throw new System.FormatException ("ResBinding format error");
			}

			ResourceBinding binding = new ResourceBinding ();

			for (int i = 1; i < args.Length; i++)
			{
				string element = args[i];

				if (element.Length > 0)
				{
					string[] elems = element.Split ('=');

					if (elems.Length != 2)
					{
						throw new System.FormatException (string.Format ("Element '{0}' not valid in Binding expression", element));
					}

					switch (elems[0])
					{
						case "Id":
							binding.ResourceId = elems[1];
							break;

						default:
							throw new System.FormatException (string.Format ("Element '{0}' not valid in ResBinding expression", element));
					}
				}
			}

			object manager = context.ResolveFromMarkup (string.Concat ("{ExtRef ", Context.WellKnownTagResourceManager, "}"));
			
			ResourceBinding.RebindCallback (manager, binding);

			return binding;
		}
		
		private static object ExtRefFromString(Context context, string[] args)
		{
			if ((args.Length != 2) ||
				(args[0] != "ExtRef"))
			{
				throw new System.FormatException ("ExtRef format error");
			}

			return context.ExternalMap.GetValue (args[1]);
		}
		
		private static object ObjRefFromString(Context context, string[] args)
		{
			if ((args.Length != 2) ||
				(args[0] != "ObjRef"))
			{
				throw new System.FormatException ("ObjRef format error");
			}

			return context.ObjectMap.GetValue (Context.ParseId (args[1]));
		}
		
		private static ICollection<DependencyObject> CollectionFromString(Context context, string[] args)
		{
			DependencyObject[] items = new DependencyObject[args.Length-1];
			
			for (int i = 1; i < args.Length; i++)
			{
				items[i-1] = context.ResolveFromMarkup (args[i]) as DependencyObject;
			}
			
			return items;
		}
	}
}
