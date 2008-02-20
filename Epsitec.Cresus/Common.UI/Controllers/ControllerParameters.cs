//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// The <c>ControllerParameters</c> class stores parameters associated with
	/// a controller implementing the <see cref="IController"/> interface.
	/// </summary>
	public class ControllerParameters : System.IEquatable<ControllerParameters>, IReadOnlyLock
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerParameters"/> class.
		/// </summary>
		/// <param name="source">The source string used to define the parameters.</param>
		public ControllerParameters(string source)
		{
			this.source = string.IsNullOrEmpty (source) ? null : source;
		}


		/// <summary>
		/// Gets the parameter value.
		/// </summary>
		/// <param name="key">The parameter key.</param>
		/// <returns>The parameter value or <c>null</c> if none is defined.</returns>
		public string GetParameterValue(string key)
		{
			if (source == null)
			{
				return null;
			}

			if (this.dictionary == null)
			{
				this.AllocateDictionary ();
			}

			string value;

			if (this.dictionary.TryGetValue (key, out value))
			{
				return value;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Sets the parameter value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public void SetParameterValue(string key, string value)
		{
			if (this.IsReadOnly)
			{
				throw new System.InvalidOperationException ("ControllerParameters is read only");
			}

			if (this.dictionary == null)
			{
				this.AllocateDictionary ();
			}

			if (value == null)
			{
				this.dictionary.Remove (key);
			}
			else
			{
				this.dictionary[key] = value;
			}
		}

		/// <summary>
		/// Merges the parameters. This will produce a string concatenation of
		/// the provided sources.
		/// </summary>
		/// <param name="sources">The parameter sources.</param>
		/// <returns>The merged parameters.</returns>
		public static string MergeParameters(params string[] sources)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (string source in sources)
			{
				if (string.IsNullOrEmpty (source))
				{
					continue;
				}
				
				if (buffer.Length > 0)
				{
					buffer.Append (" ");
				}

				buffer.Append (source);
			}

			return buffer.ToString ();
		}

		#region IReadOnlyLock Members

		public void Lock()
		{
			this.isReadOnly = true;
		}

		public void Unlock()
		{
			throw new System.NotImplementedException ();
		}

		#endregion

		#region IReadOnly Members

		public bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
		}

		#endregion

		#region IEquatable<ControllerParameters> Members

		public bool Equals(ControllerParameters other)
		{
			return this.ToString () == other.ToString ();
		}

		#endregion

		public override bool Equals(object obj)
		{
			return this.Equals (obj as ControllerParameters);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			if ((this.source != null) &&
				(this.dictionary == null))
			{
				this.AllocateDictionary ();
			}

			if (this.dictionary != null)
			{
				List<string> keys = new List<string> (this.dictionary.Keys);
				keys.Sort ();

				foreach (string key in keys)
				{
					string value = this.dictionary[key];

					System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (key));
					System.Diagnostics.Debug.Assert (value != null);

					if (buffer.Length > 0)
					{
						buffer.Append (" ");
					}

					System.Diagnostics.Debug.Assert (key.IndexOfAny (new char[] { ' ', '=' }) == -1);
					System.Diagnostics.Debug.Assert (value.IndexOfAny (new char[] { ' ' }) == -1);

					buffer.Append (key);

					if (value.Length > 0)
					{
						buffer.Append ("=");
						buffer.Append (value);
					}
				}
			}

			return buffer.ToString ();
		}

		private void AllocateDictionary()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string> ();

			if (this.source != null)
			{
				string[] tokens = this.source.Split (new char[] { ' ', '\t', '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

				foreach (string token in tokens)
				{
					string[] args = token.Split ('=');

					string key;
					string value;

					if (args.Length > 2)
					{
						key   = args[0];
						value = string.Join ("=", args, 1, args.Length-1);
					}
					else if (args.Length == 1)
					{
						key   = args[0];
						value = "";
					}
					else
					{
						key   = args[0];
						value = args[1];
					}

					string simpleKey = key.TrimStart ('+', '-');
					string prefix = key.Substring (0, key.Length - simpleKey.Length);

					if (dictionary.ContainsKey (simpleKey))
					{
						switch (prefix)
						{
							case "-":
								dictionary.Remove (simpleKey);
								break;
							
							default:
								break;
						}
					}

					if (prefix != "-")
					{
						dictionary[simpleKey] = value;
					}
				}
			}

			this.dictionary = dictionary;
		}


		private readonly string source;
		private Dictionary<string, string> dictionary;
		private bool isReadOnly;
	}
}
