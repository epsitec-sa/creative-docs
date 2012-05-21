//	Copyright © 2008-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>CommandParameters</c> class stores the default parameters associated
	/// with a <see cref="Command"/> and is used to configure the look and feel of
	/// the associated widgets.
	/// Well known parameters are "ButtonClass", "ButtonStyle" and "Level".
	/// </summary>
	public sealed class CommandParameters : Dictionary<string, string>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CommandParameters"/> class.
		/// </summary>
		public CommandParameters()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandParameters"/> class.
		/// </summary>
		/// <param name="source">The serialized source for the parameters.</param>
		public CommandParameters(string source)
		{
			if (string.IsNullOrEmpty (source))
			{
				return;
			}

			foreach (string pair in source.Split (';'))
			{
				string[] args = pair.Split ('=');

				System.Diagnostics.Debug.Assert (args.Length == 2);

				string key   = CommandParameters.Unescape (args[0]);
				string value = CommandParameters.Unescape (args[1]);

				this[key] = value;
			}

		}

		
		/// <summary>
		/// Gets or sets the value for the specified key. Setting to <c>null</c>
		/// clears the value. Querying for a missing key returns <c>null</c>.
		/// </summary>
		/// <value>The value.</value>
		public new string this[string key]
		{
			get
			{
				if (this.ContainsKey (key))
				{
					return base[key];
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (value ==  null)
				{
					this.Remove (key);
				}
				else
				{
					base[key] = value;
				}
			}
		}

		
		/// <summary>
		/// Sets the specified value as a command parameter.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
		/// <param name="value">The value.</param>
		public void Set<TEnum>(TEnum value)
			where TEnum : struct
		{
			this[typeof (TEnum).Name] = value.ToString ();
		}

		/// <summary>
		/// Clears the specified value from the command parameter.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
		public void Clear<TEnum>()
			where TEnum : struct
		{
			this[typeof (TEnum).Name] = null;
		}

		/// <summary>
		/// Gets the value of the command parameter, or the default value if
		/// the value is currently undefined.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>The value of the command parameter.</returns>
		public TEnum GetValueOrDefault<TEnum>(TEnum defaultValue = default (TEnum))
			where TEnum : struct
		{
			string value = this[typeof (TEnum).Name];

			if (string.IsNullOrEmpty (value))
			{
				return defaultValue;
			}
			else
			{
				return value.ToEnum<TEnum> ();
			}
		}

		
		/// <summary>
		/// Parses the specified serialized source for the parameters.
		/// </summary>
		/// <param name="source">The serialized source.</param>
		/// <returns>The <see cref="CommandParameters"/> instance.</returns>
		public static CommandParameters Parse(string source)
		{
			return new CommandParameters (source);
		}

		/// <summary>
		/// Returns a serialized source that represents the current parameter
		/// settings.
		/// </summary>
		/// <returns>
		/// The serialized source.
		/// </returns>
		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (var item in this)
			{
				string pair = string.Concat (CommandParameters.Escape (item.Key), "=", CommandParameters.Escape (item.Value));

				if (buffer.Length > 0)
				{
					buffer.Append (";");
				}

				buffer.Append (pair);
			}

			return buffer.ToString ();
		}

		
		private static string Escape(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return "";
			}

			text = text.Replace (@"\", @"\\");
			text = text.Replace (@"=", @"\-");
			text = text.Replace (@";", @"\,");

			return text;
		}

		private static string Unescape(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return "";
			}

			if (text.Contains (@"\"))
			{
				text = text.Replace (@"\,", @";");
				text = text.Replace (@"\-", @"=");
				text = text.Replace (@"\\", @"\");
			}

			return text;
		}
	}
}
