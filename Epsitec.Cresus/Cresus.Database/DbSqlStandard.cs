//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	using System.Text.RegularExpressions;
	
	/// <summary>
	/// The <c>DbSqlStandard</c> class implements SQL related validation functions.
	/// </summary>
	public static class DbSqlStandard
	{
		static DbSqlStandard()
		{
			//	A valid SQL name is defined as :
			//
			//	- An alphabetic character followed by 0-n alphanumeric characters, including
			//	  the "_" underscore character.
			//
			//	- A quoted text which can contain all possible characters; quotes must be
			//	  doubled if they are part of the SQL name.

			DbSqlStandard.regexName = new Regex (@"^([a-zA-Z][a-zA-Z0-9_]*|""([^""]|"""")*"")$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
			
			//	A valid SQL string is enclosed within single quotes; single quotes must be
			//	doubled if they are part of the SQL string.
			
			DbSqlStandard.regexString = new Regex (@"^'([^']|'')*'$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
			
			//	A valid SQL number is defined as an optional minus sign followed by 1-n digits
			//	and then an optional decimal point and more digits.
			
			DbSqlStandard.regexNumber = new Regex (@"^-?[0-9]+(\.[0-9]+)?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		}


		/// <summary>
		/// Concatenates two SQL names.
		/// </summary>
		/// <param name="a">First part of the name.</param>
		/// <param name="b">Second part of the name.</param>
		/// <returns>The SQL name.</returns>
		public static string ConcatNames(string a, string b)
		{
			if (DbSqlStandard.ValidateName (a) &&
				DbSqlStandard.ValidateName (b))
			{
				bool forceQuotes = false;
				
				if (a.StartsWith (@""""))
				{
					forceQuotes = true;
					a = a.Substring (1, a.Length-2);
				}
				if (b.StartsWith (@""""))
				{
					forceQuotes = true;
					b = b.Substring (1, b.Length-2);
				}

				if (forceQuotes)
				{
					return string.Concat (@"""", a, b, @"""");
				}
				else
				{
					return a + b;
				}
			}
			
			throw new Exceptions.FormatException (string.Format ("Expected two valid names: {0} and {1}", a, b));
		}

		/// <summary>
		/// Concatenates two SQL strings.
		/// </summary>
		/// <param name="a">The first SQL string.</param>
		/// <param name="b">The second SQL string.</param>
		/// <returns>The SQL string.</returns>
		public static string ConcatStrings(string a, string b)
		{
			if (DbSqlStandard.ValidateString (a) &&
				DbSqlStandard.ValidateString (b))
			{
				return string.Concat ("'", a.Substring (1, a.Length-2), b.Substring (1, b.Length-2), "'");
			}
			
			throw new Exceptions.FormatException (string.Format ("Expected two valid strings: {0} and {1}", a, b));
		}


		/// <summary>
		/// Quotes the text to make it a valid SQL string. This will double
		/// single quotes found in the text.
		/// </summary>
		/// <param name="value">The text.</param>
		/// <returns>The SQL String.</returns>
		public static string QuoteString(string value)
		{
			return string.Concat ("'", value.Replace ("'", "''"), "'");
		}


		/// <summary>
		/// Creates a qualified name from a simple name and a qualifier.
		/// </summary>
		/// <param name="qualifier">The qualifier.</param>
		/// <param name="name">The name.</param>
		/// <returns>The qualified SQL name.</returns>
		public static string QualifyName(string qualifier, string name)
		{
			if (DbSqlStandard.ValidateName (qualifier) &&
				DbSqlStandard.ValidateName (name))
			{
				return string.Concat (qualifier, ".", name);
			}
			
			throw new Exceptions.FormatException (string.Format ("Cannot make qualified name from {0} and {1}", qualifier, name));
		}

		/// <summary>
		/// Splits the qualified name.
		/// </summary>
		/// <param name="value">The qualified name.</param>
		/// <param name="qualifier">The qualifier.</param>
		/// <param name="name">The name.</param>
		public static void SplitQualifiedName(string value, out string qualifier, out string name)
		{
			if (DbSqlStandard.ValidateQualifiedName (value))
			{
				//	Caution: a qualified name can have dots embedded between quotes, such
				//	as "A.B".C which maps to: qualifier="A.B", name="C".

				string[] tokens;
				int count = Common.Support.Utilities.StringToTokens (value, '.', out tokens);

				if (count != 2)
				{
					throw new Exceptions.FormatException (string.Format ("{0} is not a qualified name", value));
				}

				qualifier = tokens[0];
				name      = tokens[1];
				
				return;
			}
			
			throw new Exceptions.FormatException (string.Format ("{0} is not a qualified name", value));
		}


		/// <summary>
		/// Validates the SQL name.
		/// </summary>
		/// <param name="value">The SQL name.</param>
		/// <returns><c>true</c> if the name is valid; otherwise, <c>false</c>.</returns>
		public static bool ValidateName(string value)
		{
			if (value != null)
			{
				int len = value.Length;
				if ((len > 0) && (len < 128))
				{
					return DbSqlStandard.regexName.IsMatch (value);
				}
			}
			
			return false;
		}

		/// <summary>
		/// Validates the SQL string.
		/// </summary>
		/// <param name="value">The SQL string.</param>
		/// <returns><c>true</c> if the string is valid; otherwise, <c>false</c>.</returns>
		public static bool ValidateString(string value)
		{
			if (value != null)
			{
				int len = value.Length;

				if ((len > 1) &&
					(len < 10000))
				{
					return DbSqlStandard.regexString.IsMatch (value);
				}
			}
			
			return false;
		}

		/// <summary>
		/// Validates the SQL number.
		/// </summary>
		/// <param name="value">The SQL number.</param>
		/// <returns><c>true</c> if the number is valid; otherwise, <c>false</c>.</returns>
		public static bool ValidateNumber(string value)
		{
			if (value != null)
			{
				int len = value.Length;
				
				if ((len > 0) &&
					(len < 40))
				{
					return DbSqlStandard.regexNumber.IsMatch (value);
				}
			}
			
			return false;
		}

		///	<summary>
		/// Validates the qualified SQL name.
		/// </summary>
		/// <param name="value">The qualified SQL name.</param>
		/// <returns><c>true</c> if the qualified name is valid; otherwise, <c>false</c>.</returns>
		public static bool ValidateQualifiedName(string value)
		{
			if (value != null)
			{
				int len = value.Length;
				
				if ((len > 0) &&
					(len < 256))
				{
					string[] tokens;
					int count;

					//	Validate a qualified name N1.N2 where both N1 and N2 can
					//	be quoted names.

					try
					{
						count = Common.Support.Utilities.StringToTokens (value, '.', out tokens);
					}
					catch
					{
						return false;
					}

					if (count != 2)
					{
						return false;
					}
					else
					{
						return ValidateName (tokens[0])
							&& ValidateName (tokens[1]);
					}
				}
			}
			
			return false;
		}


		/// <summary>
		/// Create an SQL table name based on a high level name and an element
		/// category. This will prefix the name with "U_" if the table is a user
		/// data table and suffix it with the key id. This is required to allow
		/// several tables with the same name in the life of the database.
		/// </summary>
		/// <param name="name">The high level name.</param>
		/// <param name="category">The table category.</param>
		/// <param name="key">The key.</param>
		/// <returns>The SQL table name.</returns>
		public static string MakeSqlTableName(string name, DbElementCat category, DbKey key)
		{
			System.Text.StringBuilder buffer;
			
			switch (category)
			{
				case DbElementCat.Internal:
					if ((DbSqlStandard.ValidateName (name)) &&
						(name.StartsWith ("CR_")))
					{
						return name;
					}
					
					throw new Exceptions.GenericException (DbAccess.Empty, string.Format ("'{0}' is not an internal table name", name));
				
				case DbElementCat.ManagedUserData:
					buffer = new System.Text.StringBuilder ();
					buffer.Append ("U_");
					break;

				case DbElementCat.RevisionHistory:
					buffer = new System.Text.StringBuilder ();
					buffer.Append ("R_");
					break;
				
				default:
					throw new System.NotImplementedException (string.Format ("Support for category {0} not implemented", category));
			}
			
			DbSqlStandard.CreateSimpleSqlName (name, buffer);
			
			//	Limit table name to at most 30 characters. As we add a maximum of 18 digits
			//	long suffix, only 12 characters are usable for the name itself :
			
			if (buffer.Length > 30-18)
			{
				buffer.Length = 30-18;
			}
			
			buffer.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "_{0}", key.Id.Value);
			
			return buffer.ToString ();
		}

		/// <summary>
		/// Creates a simple SQL name by stripping or replacing invalid characters
		/// found in the high level name.
		/// </summary>
		/// <param name="name">The high level name.</param>
		/// <returns>The simple SQL name.</returns>
		public static string MakeSimpleSqlName(string name)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			DbSqlStandard.CreateSimpleSqlName (name, buffer);
			return buffer.ToString ();
		}

		/// <summary>
		/// Creates a simple SQL name by stripping or replacing invalid characters
		/// found in the high level name. The high level name is the concatenation
		/// of the prefix, the name and the suffix.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="prefix">The prefix.</param>
		/// <param name="suffix">The suffix.</param>
		/// <returns>The simple concatenaed SQL name.</returns>
		public static string MakeSimpleSqlName(string name, string prefix, string suffix)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append (prefix);
			buffer.Append ('_');
			DbSqlStandard.CreateSimpleSqlName (name, buffer);
			buffer.Append ('_');
			buffer.Append (suffix);
			
			return buffer.ToString ();
		}

		/// <summary>
		/// Creates a simple SQL name by stripping or replacing invalid characters
		/// found in the high level name, prefixing it with "U_" if the element
		/// belongs to the user.
		/// </summary>
		/// <param name="name">The high level name.</param>
		/// <param name="category">The category for the named element.</param>
		/// <returns>The simple SQL name.</returns>
		public static string MakeSimpleSqlName(string name, DbElementCat category)
		{
			System.Text.StringBuilder buffer;
			
			switch (category)
			{
				case DbElementCat.Internal:
					if ((DbSqlStandard.ValidateName (name)) &&
						((name.StartsWith ("CR_") || name.StartsWith ("CREF_"))))
					{
						return name;
					}
					
					throw new Exceptions.GenericException (DbAccess.Empty, string.Format ("'{0}' is an invalid internal name", name));
				
				case DbElementCat.ManagedUserData:
					buffer = new System.Text.StringBuilder ();
					buffer.Append ("U_");
					break;
				
				default:
					throw new System.NotImplementedException (string.Format ("Support for category {0} not implemented", category));
			}
			
			DbSqlStandard.CreateSimpleSqlName (name, buffer);
			return buffer.ToString ();
		}


		/// <summary>
		/// Add quotes to the name, if needed; existing quotes will be doubled.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>An SQL name.</returns>
		public static string MakeDelimitedIdentifier(string name)
		{
			bool ok  = true;
			
			for (int i = 0; i < name.Length; i++)
			{
				char c = name[i];
				
				if (((c >= 'a') && (c <= 'z')) ||
					((c >= 'A') && (c <= 'Z')) ||
					((c == '_')))
				{
					continue;
				}
				if ((i > 0) &&
					(c >= '0') && (c <= '9'))
				{
					continue;
				}
				
				ok = false;
			}

			if (ok)
			{
				return name;
			}
			else
			{
				return string.Concat (@"""", name.Replace (@"""", @""""""), @"""");
			}
		}


		/// <summary>
		/// Creates a simple SQL name by stripping or replacing invalid characters
		/// found in the high level name. The result will be in upper case ASCII.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="buffer">The buffer.</param>
		private static void CreateSimpleSqlName(string name, System.Text.StringBuilder buffer)
		{
			for (int i = 0; i < name.Length; i++)
			{
				char c = name[i];
				
				if ((c >= 'a') && (c <= 'z'))
				{
					buffer.Append ((char)(c + 'A' - 'a'));
				}
				else if ((c >= 'A') && (c <= 'Z'))
				{
					buffer.Append (c);
				}
				else if ((c >= '0') && (c <= '9'))
				{
					if (buffer.Length > 0)
					{
						buffer.Append (c);
					}
				}
				else if (buffer.Length > 0)
				{
					buffer.Append ('_');
				}
			}
		}
		
		
		private static Regex					regexName;
		private static Regex					regexString;
		private static Regex					regexNumber;
	}
}
