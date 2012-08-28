//	Copyright © 2006-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>Druid</c> structure manages Data Resource Unique IDs and their
	/// related conversions.
	/// These DRUIDs are 64-bit identifiers which encode the module identity,
	/// the developer identity and a locally unique value.
	/// The string encoding is very compact for small DRUIDs: we rely on an
	/// interleaved encoding such as MMDLLDMLDMLDL, with trailing zeroes
	/// omitted. M=module ID, D=dev. ID, L=local ID. Each is encoded using
	/// 5-bit digits (0..9, A..V) and the first digit of each category
	/// encodes the lowest bits of each ID. For instance "1023" can be used
	/// to represent module=1 ('1'*32^0 + '0'*32^1), dev=2, local=3.
	/// A compact, module relative DRUID exists; it uses only 44-bit for
	/// the encoding of the data.
	/// </summary>

	[Types.SerializationConverter (typeof (Druid.SerializationConverter))]
	[System.Diagnostics.DebuggerStepThrough]

	public struct Druid : System.IEquatable<Druid>, System.IComparable<Druid>, INullable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Druid"/> structure.
		/// </summary>
		/// <param name="druid">The druid to copy from.</param>
		public Druid(Druid druid)
		{
			this.module    = druid.module;
			this.developer = druid.developer;
			this.local     = druid.local;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Druid"/> structure.
		/// </summary>
		/// <param name="druid">The druid to copy from.</param>
		/// <param name="module">The module id to use.</param>
		public Druid(Druid druid, int module)
		{
			this.module    = module+1;
			this.developer = druid.developer;
			this.local     = druid.local;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Druid"/> structure.
		/// </summary>
		/// <param name="dev">The developer and patch level id.</param>
		/// <param name="local">The local id.</param>
		public Druid(int dev, int local)
		{
			this.module    = 0;
			this.developer = dev+1;
			this.local     = local+1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Druid"/> structure.
		/// </summary>
		/// <param name="module">The module id.</param>
		/// <param name="dev">The developer and patch level id.</param>
		/// <param name="local">The local id.</param>
		public Druid(int module, int dev, int local)
		{
			this.module    = module+1;
			this.developer = dev+1;
			this.local     = local+1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Druid"/> structure.
		/// This constructor can throw a <see cref="T:System.FormatException"/>
		/// exception if the value has an unrecognized or invalid format.
		/// </summary>
		/// <param name="value">The value (it will get parsed by <c>Druid.Parse</c>).</param>
		public Druid(string value)
			: this (Druid.Parse (value))
		{
		}


		/// <summary>
		/// Performs an implicit conversion from <c>long</c> to <see cref="Druid"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The result of the conversion.</returns>
		public static implicit operator Druid(long value)
		{
			return Druid.FromLong (value);
		}


		/// <summary>
		/// Gets a value indicating whether this instance is empty.
		/// </summary>
		/// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
		public bool								IsEmpty
		{
			get
			{
				return (this.Type == DruidType.Invalid);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is valid.
		/// </summary>
		/// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
		public bool								IsValid
		{
			get
			{
				return this.Type != DruidType.Invalid;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this DRUID is temporary.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this is a temporary DRUID; otherwise, <c>false</c>.
		/// </value>
		public bool								IsTemporary
		{
			get
			{
				return this.Module == 0xfffff;
			}
		}
		
		/// <summary>
		/// Gets the type of the DRUID.
		/// </summary>
		/// <value>The type of the DRUID.</value>
		public DruidType						Type
		{
			get
			{
				int module = this.Module;
				int dev = this.DeveloperAndPatchLevel;
				int local = this.Local;

				if (!Druid.IsValidDeveloperAndPatchId (dev))
				{
					return DruidType.Invalid;
				}
				
				if (!Druid.IsValidLocal (local))
				{
					return DruidType.Invalid;
				}
				
				if (module == -1)
				{
					return DruidType.ModuleRelative;
				}

				if (!Druid.IsValidModule (module))
				{
					return DruidType.Invalid;
				}
				
				return DruidType.Full;
			}
		}

		/// <summary>
		/// Gets the module id.
		/// </summary>
		/// <value>The module id.</value>
		public int								Module
		{
			get
			{
				return this.module-1;
			}
		}

		/// <summary>
		/// Gets the encoded developer and patch level id.
		/// </summary>
		/// <value>The encoded developer and patch level id.</value>
		public int								DeveloperAndPatchLevel
		{
			get
			{
				return this.developer-1;
			}
		}

		/// <summary>
		/// Gets the decoded developer id.
		/// </summary>
		/// <value>The decoded developer id.</value>
		public int								Developer
		{
			get
			{
				int id = this.DeveloperAndPatchLevel;

				return id < Druid.DeveloperIdMultiplier ? id : id / Druid.DeveloperIdMultiplier;
			}
		}

		/// <summary>
		/// Gets the decoded patch level.
		/// </summary>
		/// <value>The decoded patch level.</value>
		public int								PatchLevel
		{
			get
			{
				return this.DeveloperAndPatchLevel % Druid.DeveloperIdMultiplier;
			}
		}

		/// <summary>
		/// Gets the local id.
		/// </summary>
		/// <value>The local id.</value>
		public int								Local
		{
			get
			{
				return this.local-1;
			}
		}

		/// <summary>
		/// Returns the DRUID encoded as a resource id (e.g. "[1023]").
		/// </summary>
		/// <returns>The resource id.</returns>
		public string ToResourceId()
		{
			DruidType type = this.Type;

			if (type != DruidType.Full)
			{
				throw new System.InvalidOperationException (string.Format ("Cannot convert {0} DRUID to a resource id", type));
			}

			return string.Concat ("[", Druid.ToFullString (this.Module, this.DeveloperAndPatchLevel, this.Local), "]");
		}

		/// <summary>
		/// Returns the DRUID encoded as a bundle id (e.g. "_1023").
		/// </summary>
		/// <returns>The bundle id.</returns>
		public string ToBundleId()
		{
			DruidType type = this.Type;

			if (type != DruidType.Full)
			{
				throw new System.InvalidOperationException (string.Format ("Cannot convert {0} DRUID to a resource id", type));
			}

			return string.Concat ("_", Druid.ToFullString (this.Module, this.DeveloperAndPatchLevel, this.Local));
		}

		/// <summary>
		/// Returns the DRUID encoded as a resource field name (e.g. "$23").
		/// </summary>
		/// <returns>The resource field name.</returns>
		public string ToFieldName()
		{
			DruidType type = this.Type;
			
			if ((type != DruidType.Full) &&
				(type != DruidType.ModuleRelative))
			{
				throw new System.InvalidOperationException (string.Format ("Cannot convert {0} DRUID to a field id name", type));
			}

			int dev   = this.DeveloperAndPatchLevel;
			int local = this.Local;
			
			return Druid.ToPrefixedModuleString (Resources.FieldIdPrefix, dev, local);
		}

		/// <summary>
		/// Returns the DRUID encoded as a resource field id (e.g. 0x2000003L).
		/// </summary>
		/// <returns>The resource field id.</returns>
		public long ToFieldId()
		{
			DruidType type = this.Type;

			if ((type != DruidType.Full) &&
				(type != DruidType.ModuleRelative))
			{
				throw new System.InvalidOperationException (string.Format ("Cannot convert {0} DRUID to a field id", type));
			}

			return Druid.FromIds (this.DeveloperAndPatchLevel, this.Local);
		}

		/// <summary>
		/// Returns the DRUID encoded as a raw 64-bit DRUID value.
		/// </summary>
		/// <returns>The raw 64-bit DRUID value.</returns>
		public long ToLong()
		{
			DruidType type = this.Type;

			switch (type)
			{
				case DruidType.Full:
					return Druid.FromIds (this.Module, this.DeveloperAndPatchLevel, this.Local);

				case DruidType.ModuleRelative:
					return Druid.FromIds (this.DeveloperAndPatchLevel, this.Local);

				default:
					throw new System.InvalidOperationException (string.Format ("Cannot convert {0} DRUID to a long", type));
			}
		}
		
		public static bool operator==(Druid a, Druid b)
		{
			return (a.module == b.module) && (a.developer == b.developer) && (a.local == b.local);
		}

		public static bool operator!=(Druid a, Druid b)
		{
			return (a.module != b.module) || (a.developer != b.developer) || (a.local != b.local);
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns><c>true</c> if obj and this instance are the same type and
		/// represent the same value; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			if (obj is Druid)
			{
				return this.Equals ((Druid) obj);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer that is the hash code for this
		/// instance.</returns>
		public override int GetHashCode()
		{
			return this.module ^ this.developer ^ this.local;
		}

		/// <summary>
		/// Returns the string representation of this DRUID.
		/// </summary>
		/// <returns>The string representation of this DRUID.</returns>
		public override string ToString()
		{
			switch (this.Type)
			{
				case DruidType.Invalid:
					return Strings.Invalid;
				
				case DruidType.ModuleRelative:
					return Druid.ToPrefixedModuleString (Resources.FieldIdPrefix, this.DeveloperAndPatchLevel, this.Local);

				case DruidType.Full:
					return string.Concat ("[", Druid.ToFullString (this.Module, this.DeveloperAndPatchLevel, this.Local), "]");
				
				default:
					return Strings.NotSupported;
			}
		}

		/// <summary>
		/// Same as <see cref="ToString()"/>, with the difference that instead of returning
		/// <c>"&lt;invalid&gt;"</c>, it returns an empty string.
		/// </summary>
		/// <returns>The string representation of this DRUID.</returns>
		public string ToCompactString()
		{
			if (this.IsEmpty)
			{
				return "";
			}
			else
			{
				return this.ToString ();
			}
		}


		/// <summary>
		/// Parses the specified value; this recognizes DRUIDs in the resource
		/// id "[1023]" format, resource field name "$23" format, bundle id
		/// "_1023" format and XML field id "23" format. This method throws a
		/// <see cref="T:System.FormatException"/> exception if the format is
		/// not recognized. An empty attribute maps to the <c>Empty</c> DRUID.
		/// </summary>
		/// <param name="attribute">The attribute containing the value to parse.</param>
		/// <returns>The DRUID.</returns>
		public static Druid Parse(System.Xml.Linq.XAttribute attribute)
		{
			if (attribute == null)
			{
				return Druid.Empty;
			}

			return Druid.Parse (attribute.Value);
		}

		/// <summary>
		/// Parses the specified value; this recognizes DRUIDs in the resource
		/// id "[1023]" format, resource field name "$23" format, bundle id
		/// "_1023" format and XML field id "23" format. This method throws a
		/// <see cref="T:System.FormatException"/> exception if the format is
		/// not recognized. An empty string maps to the <c>Empty</c> DRUID.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		/// <returns>The DRUID.</returns>
		public static Druid Parse(string value)
		{
			if ((string.IsNullOrEmpty (value)) ||
				(value == Strings.Invalid))
			{
				return new Druid ();
			}

			if (value[0] == '$')
			{
				long druid = Druid.FromModuleString (value.Substring (1));
				return new Druid (Druid.GetDevId (druid), Druid.GetLocalId (druid));
			}
			
			if (value[0] == '_')
			{
				long druid = Druid.FromFullString (value.Substring (1));
				return new Druid (Druid.GetModuleId (druid), Druid.GetDevId (druid), Druid.GetLocalId (druid));
			}

			if ((value.Length > 2) &&
				(value[0] == '[') &&
				(value[value.Length-1] == ']'))
			{
				long druid = Druid.FromFullString (value.Substring (1, value.Length-2));
				return new Druid (Druid.GetModuleId (druid), Druid.GetDevId (druid), Druid.GetLocalId (druid));
			}

			if (Druid.IsValidModuleString (value))
			{
				long druid = Druid.FromModuleString (value);
				return new Druid (Druid.GetDevId (druid), Druid.GetLocalId (druid));
			}

			throw new System.FormatException (string.Format ("Value '{0}' is not a valid DRUID encoding", value));
		}

		/// <summary>
		/// Parses the specified value; this recognizes DRUIDs in the resource
		/// id "[1023]" format, resource field name "$23" formant and bundle id
		/// "_1023" format. The number of recognized formats is restricted with
		/// respect to what the <c>Parse</c> method supports.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		/// <param name="druid">The parsed DRUID.</param>
		/// <returns><c>true</c> if the conversion was successful; otherwise,
		/// <c>false</c>.</returns>
		public static bool TryParse(string value, out Druid druid)
		{
			if (string.IsNullOrEmpty (value))
			{
				druid = Druid.Empty;
				return false;
			}

			if ((value[0] == '[') &&
				(value.Length > 2) &&
				(value[value.Length-1] == ']') &&
				(Druid.IsValidFullString (value.Substring (1, value.Length-2))))
			{
				long id = Druid.FromFullString (value.Substring (1, value.Length-2));
				druid = new Druid (Druid.GetModuleId (id), Druid.GetDevId (id), Druid.GetLocalId (id));
				return true;
			}

			if ((value[0] == '_') &&
				(Druid.IsValidFullString (value.Substring (1))))
			{
				long id = Druid.FromFullString (value.Substring (1));
				druid = new Druid (Druid.GetModuleId (id), Druid.GetDevId (id), Druid.GetLocalId (id));
				return true;
			}
			
			if ((value[0] == '$') &&
				(Druid.IsValidModuleString (value.Substring (1))))
			{
				long id = Druid.FromModuleString (value.Substring (1));
				druid = new Druid (Druid.GetDevId (id), Druid.GetLocalId (id));
				return true;
			}

			druid = Druid.Empty;
			return false;
		}

		/// <summary>
		/// Escapes the specified value. This ensures that the value won't be
		/// confused with a valid DRUID (recognized as such by <c>TryParse</c>).
		/// This method simply adds a special character in front of the value if
		/// it could be mistaken with a DRUID.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The escaped value.</returns>
		public static string Escape(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return value;
			}

			switch (value[0])
			{
				case '$':
				case '_':
				case '[':
				case ']':
					value = string.Concat ("]", value);
					break;
				
				default:
					break;
			}
			
			return value;
		}

		/// <summary>
		/// Removes the escape code from the specified value, if any.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The value, as it was provided to the <c>Escape</c> method.</returns>
		public static string Unescape(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return value;
			}
			else if (value[0] == ']')
			{
				return value.Substring (1);
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Determines whether the specified value is a valid resource id (it
		/// must be a DRUID enclosed within brackets).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> if the specified value is a valid resource id;
		/// otherwise, <c>false</c>.</returns>
		public static bool IsValidResourceId(string value)
		{
			if ((value != null) &&
				(value.Length > 2) &&
				(value[0] == '[') &&
				(value[value.Length-1] == ']'))
			{
				return Druid.IsValidFullString (value.Substring (1, value.Length-2));
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Determines whether the specified value is a valid bundle id (it
		/// must be a DRUID prefixed with an underscore).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> if the specified value is a valid bundle id;
		/// otherwise, <c>false</c>.</returns>
		public static bool IsValidBundleId(string value)
		{
			if ((value != null) &&
				(value.Length > 1) &&
				(value[0] == '_'))
			{
				return Druid.IsValidFullString (value.Substring (1, value.Length-1));
			}
			else
			{
				return false;
			}
		}

		public static bool IsValidDeveloperAndPatchId(int dev)
		{
			if ((dev < 0) || (dev > 0x000fffff))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool IsValidLocal(int local)
		{
			if ((local < 0) || (local > 0x00ffffff))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool IsValidModule(int module)
		{
			if ((module < 0) || (module > 0x000fffff))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Converts the raw resource field id into a DRUID.
		/// </summary>
		/// <param name="value">The resource raw field id value.</param>
		/// <returns>The DRUID.</returns>
		public static Druid FromFieldId(long value)
		{
			if (value < 0)
			{
				return new Druid ();
			}
			else
			{
				int dev = Druid.GetDevId (value);
				int local = Druid.GetLocalId (value);

				return new Druid (dev, local);
			}
		}

		/// <summary>
		/// Converts the raw 64-bit id value into a DRUID.
		/// </summary>
		/// <param name="value">The raw 64-bit id value.</param>
		/// <returns>The DRUID.</returns>
		public static Druid FromLong(long value)
		{
			return new Druid (Druid.GetModuleId (value), Druid.GetDevId (value), Druid.GetLocalId (value));
		}

		/// <summary>
		/// Converts the raw 64-bit id value into a DRUID.
		/// </summary>
		/// <param name="moduleId">The module id.</param>
		/// <param name="value">The raw 64-bit id value.</param>
		/// <returns>The DRUID.</returns>
		public static Druid FromLong(int moduleId, long value)
		{
			return new Druid (moduleId, Druid.GetDevId (value), Druid.GetLocalId (value));
		}

		/// <summary>
		/// Converts the DRUID to a full string encoded as MMDLLDMLDMLDL, less
		/// significant digits first, trailing zeroes omitted.
		/// </summary>
		/// <param name="druid">The 64-bit DRUID to convert.</param>
		/// <returns>The string representing the full DRUID.</returns>
		public static string ToFullString(long druid)
		{
			int module = (int) (druid >> 44) & 0xfffff;
			int dev = (int) (druid >> 24) & 0xfffff;
			int local = (int) (druid >> 0) & 0xffffff;

			return Druid.ToFullString (module, dev, local);
		}

		private static string ToFullString(int module, int dev, int local)
		{
			char[] buffer = new char[13];

			buffer[0]  = Druid.OutputDigit (ref module);
			buffer[1]  = Druid.OutputDigit (ref module);
			buffer[2]  = Druid.OutputDigit (ref dev);
			buffer[3]  = Druid.OutputDigit (ref local);
			buffer[4]  = Druid.OutputDigit (ref local);
			buffer[5]  = Druid.OutputDigit (ref dev);
			buffer[6]  = Druid.OutputDigit (ref module);
			buffer[7]  = Druid.OutputDigit (ref local);
			buffer[8]  = Druid.OutputDigit (ref dev);
			buffer[9]  = Druid.OutputDigit (ref module);
			buffer[10] = Druid.OutputDigit (ref local);
			buffer[11] = Druid.OutputDigit (ref dev);
			buffer[12] = Druid.OutputDigit (ref local);

			return Druid.StripEndZeroes (buffer);
		}

		/// <summary>
		/// Converts the DRUID to a module relative string encoded as DLLDLLDLD,
		/// less significant digits first, trailing zeroes omitted.
		/// </summary>
		/// <param name="druid">The 64-bit DRUID to convert.</param>
		/// <returns>The string representing the module relative DRUID.</returns>
		public static string ToModuleString(long druid)
		{
			int dev = (int) (druid >> 24) & 0xfffff;
			int local = (int) (druid >> 0) & 0xffffff;

			char[] buffer = new char[9];

			buffer[0] = Druid.OutputDigit (ref dev);
			buffer[1] = Druid.OutputDigit (ref local);
			buffer[2] = Druid.OutputDigit (ref local);
			buffer[3] = Druid.OutputDigit (ref dev);
			buffer[4] = Druid.OutputDigit (ref local);
			buffer[5] = Druid.OutputDigit (ref local);
			buffer[6] = Druid.OutputDigit (ref dev);
			buffer[7] = Druid.OutputDigit (ref local);
			buffer[8] = Druid.OutputDigit (ref dev);

			return Druid.StripEndZeroes (buffer);
		}

		private static string ToPrefixedModuleString(char prefix, int dev, int local)
		{
			char[] buffer = new char[10];

			buffer[0] = prefix;
			buffer[1] = Druid.OutputDigit (ref dev);
			buffer[2] = Druid.OutputDigit (ref local);
			buffer[3] = Druid.OutputDigit (ref local);
			buffer[4] = Druid.OutputDigit (ref dev);
			buffer[5] = Druid.OutputDigit (ref local);
			buffer[6] = Druid.OutputDigit (ref local);
			buffer[7] = Druid.OutputDigit (ref dev);
			buffer[8] = Druid.OutputDigit (ref local);
			buffer[9] = Druid.OutputDigit (ref dev);

			return Druid.StripEndZeroes (buffer);
		}

		/// <summary>
		/// Converts the DRUID encoded as a full string into the native 64-bit
		/// DRUID value. See <see cref="M:ToFullString"/>.
		/// </summary>
		/// <param name="value">The encoded string value.</param>
		/// <returns>The resulting native 64-bit DRUID value.</returns>
		public static long FromFullString(string value)
		{
			int module = 0;
			int dev = 0;
			int local = 0;

			char[] buffer = new char[13];

			if (value.Length > buffer.Length)
			{
				throw new System.FormatException ();
			}

			int i;
			
			for (i = 0; i < value.Length; i++)
			{
				buffer[i] = value[i];
			}
			for (; i < buffer.Length; i++)
			{
				buffer[i] = '0';
			}

			Druid.InputDigit (ref local, buffer[12]);
			Druid.InputDigit (ref dev, buffer[11]);
			Druid.InputDigit (ref local, buffer[10]);
			Druid.InputDigit (ref module, buffer[9]);
			Druid.InputDigit (ref dev, buffer[8]);
			Druid.InputDigit (ref local, buffer[7]);
			Druid.InputDigit (ref module, buffer[6]);
			Druid.InputDigit (ref dev, buffer[5]);
			Druid.InputDigit (ref local, buffer[4]);
			Druid.InputDigit (ref local, buffer[3]);
			Druid.InputDigit (ref dev, buffer[2]);
			Druid.InputDigit (ref module, buffer[1]);
			Druid.InputDigit (ref module, buffer[0]);

			long druid = 0;

			druid |= (uint) module;
			druid <<= 20;
			druid |= (uint) dev;
			druid <<= 24;
			druid |= (uint) local;

			return druid;
		}

		/// <summary>
		/// Converts the DRUID encoded as a module relative string into the native
		/// 44-bit DRUID value. See <see cref="M:ToModuleString"/>.
		/// </summary>
		/// <param name="value">The module relative encoded string value.</param>
		/// <returns>The resulting native 44-bit DRUID value.</returns>
		public static long FromModuleString(string value)
		{
			return Druid.FromModuleString (value, 0);
		}
		
		/// <summary>
		/// Converts the DRUID encoded as a module relative string into the native
		/// full 64-bit DRUID value. See <see cref="M:ToModuleString"/>.
		/// </summary>
		/// <param name="value">The module relative encoded string value.</param>
		/// <param name="module">The module ID.</param>
		/// <returns>The resulting native full 64-bit DRUID value.</returns>
		public static long FromModuleString(string value, int module)
		{
			int dev = 0;
			int local = 0;

			char[] buffer = new char[9];

			if (value.Length > buffer.Length)
			{
				throw new System.FormatException ();
			}

			int i;

			for (i = 0; i < value.Length; i++)
			{
				buffer[i] = value[i];
			}
			for (; i < buffer.Length; i++)
			{
				buffer[i] = '0';
			}

			Druid.InputDigit (ref dev, buffer[8]);
			Druid.InputDigit (ref local, buffer[7]);
			Druid.InputDigit (ref dev, buffer[6]);
			Druid.InputDigit (ref local, buffer[5]);
			Druid.InputDigit (ref local, buffer[4]);
			Druid.InputDigit (ref dev, buffer[3]);
			Druid.InputDigit (ref local, buffer[2]);
			Druid.InputDigit (ref local, buffer[1]);
			Druid.InputDigit (ref dev, buffer[0]);

			long druid = 0;

			druid |= (uint) module;
			druid <<= 20;
			druid |= (uint) dev;
			druid <<= 24;
			druid |= (uint) local;

			return druid;
		}

		/// <summary>
		/// Converts the full 64-bit DRUID value to a 44-bit module relative
		/// DRUID value.
		/// </summary>
		/// <param name="druid">The full 64-bit DRUID value.</param>
		/// <returns>The module relative 44-bit DRUID value.</returns>
		public static long ToModuleDruid(long druid)
		{
			int dev   = Druid.GetDevId (druid);
			int local = Druid.GetLocalId (druid);

			return Druid.FromIds (dev, local);
		}

		/// <summary>
		/// Converts the module id and the 44-bit module relative DRUID value
		/// into a full 64-bit DRUID value.
		/// </summary>
		/// <param name="module">The module id.</param>
		/// <param name="druid">The 44-bit module relative DRUID value.</param>
		/// <returns>The full 64-bit DRUID value.</returns>
		public static long FromModuleDruid(int module, long druid)
		{
			int dev   = Druid.GetDevId (druid);
			int local = Druid.GetLocalId (druid);
			
			return Druid.FromIds (module, dev, local);
		}

		/// <summary>
		/// Converts the module id, developer id and local id combination into
		/// a full 64-bit DRUID value.
		/// </summary>
		/// <param name="module">The module id.</param>
		/// <param name="dev">The developer id.</param>
		/// <param name="local">The local id.</param>
		/// <returns>The full 64-bit DRUID value.</returns>
		public static long FromIds(int module, int dev, int local)
		{
			long druid = 0;

			druid |= (uint) module;
			druid <<= 20;
			druid |= (uint) dev;
			druid <<= 24;
			druid |= (uint) local;

			return druid;
		}

		/// <summary>
		/// Converts the developer id and the local id combination into a module
		/// relative 44-bit DRUID value.
		/// </summary>
		/// <param name="dev">The developer id.</param>
		/// <param name="local">The local id.</param>
		/// <returns>The module relative 44-bit DRUID value.</returns>
		public static long FromIds(int dev, int local)
		{
			long druid = 0;

			druid |= (uint) dev;
			druid <<= 24;
			druid |= (uint) local;

			return druid;
		}

		/// <summary>
		/// Gets the module id from the full 64-bit DRUID.
		/// </summary>
		/// <param name="druid">The full 64-bit DRUID value.</param>
		/// <returns>The module id.</returns>
		public static int GetModuleId(long druid)
		{
			return (int) (druid >> 44) & 0xfffff;
		}

		/// <summary>
		/// Gets the developer id from the DRUID value.
		/// </summary>
		/// <param name="druid">The full 64-bit DRUID value or module
		/// relative 44-bit DRUID value.</param>
		/// <returns>The developer id.</returns>
		public static int GetDevId(long druid)
		{
			return (int) (druid >> 24) & 0xfffff;
		}

		/// <summary>
		/// Gets the local id from the DRUID value.
		/// </summary>
		/// <param name="druid">The full 64-bit DRUID value or module
		/// relative 44-bit DRUID value.</param>
		/// <returns>The developer id.</returns>
		public static int GetLocalId(long druid)
		{
			return (int) (druid >> 0) & 0xffffff;
		}

		/// <summary>
		/// Determines whether the specified value is a string representing a valid
		/// full DRUID.
		/// </summary>
		/// <param name="value">The DRUID value.</param>
		/// <returns><c>true</c> if the specified value is valid; otherwise,
		/// <c>false</c>.</returns>
		public static bool IsValidFullString(string value)
		{
			if (value == null)
			{
				return false;
			}

			int length = value.Length;

			if ((length < 1) ||
				(length > 13))
			{
				return false;
			}

			return Druid.IsValidBase32Number (value);
		}

		/// <summary>
		/// Determines whether the specified value is a string representing a valid
		/// module relative DRUID.
		/// </summary>
		/// <param name="value">The DRUID value.</param>
		/// <returns><c>true</c> if the specified value is valid; otherwise,
		/// <c>false</c>.</returns>
		public static bool IsValidModuleString(string value)
		{
			if (value == null)
			{
				return false;
			}

			int length = value.Length;

			if ((length < 1) ||
				(length > 9))
			{
				return false;
			}

			return Druid.IsValidBase32Number (value);
		}

		/// <summary>
		/// Creates a temporary druid which is unique during a given session.
		/// </summary>
		/// <returns>A temporary DRUID.</returns>
		public static Druid CreateTemporaryDruid()
		{
			long value = System.Threading.Interlocked.Increment (ref Druid.uniqueId);

			int module    = 0xfffff;
			int local     = (int) ((value >> 0) & 0xfffff);
			int developer = (int) ((value >> 20) & 0xfffff);

			return new Druid (module, developer, local);
		}

		#region INullable Members

		bool INullable.IsNull
		{
			get
			{
				return this.IsEmpty;
			}
		}

		#endregion

		#region IEquatable<Druid> Members

		/// <summary>
		/// Indicates whether the current object is equal to another object of
		/// the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns><c>true</c> if the current object is equal to the other
		/// parameter; otherwise, <c>false</c>.</returns>
		public bool Equals(Druid other)
		{
			return this == other;
		}

		#endregion

		#region IComparable<Druid> Members

		public int CompareTo(Druid other)
		{
			if (this.module < other.module)
			{
				return -1;
			}
			else if (this.module > other.module)
			{
				return 1;
			}
			else if (this.developer < other.developer)
			{
				return -1;
			}
			else if (this.developer > other.developer)
			{
				return 1;
			}
			else if (this.local < other.local)
			{
				return -1;
			}
			else if (this.local > other.local)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		#endregion

		#region SerializationConverter Class

		public class SerializationConverter : Types.ISerializationConverter
		{
			#region ISerializationConverter Members

			public string ConvertToString(object value, Types.IContextResolver context)
			{
				Druid druid = (Druid) value;
				return druid.ToString ();
			}

			public object ConvertFromString(string value, Types.IContextResolver context)
			{
				return Druid.Parse (value);
			}

			#endregion
		}

		#endregion

		#region Private Methods
		
		private static bool IsValidBase32Number(string value)
		{
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];

				if ((c >= '0') && (c <= '9'))
				{
					continue;
				}
				if ((c >= 'A') && (c <= 'A'-10+31))
				{
					continue;
				}

				return false;
			}

			return true;
		}

		private static string StripEndZeroes(char[] buffer)
		{
			int length = buffer.Length;

			while (length > 1)
			{
				if (buffer[length-1] != '0')
				{
					break;
				}

				length--;
			}

			return new string (buffer, 0, length);
		}

		private static char OutputDigit(ref int value)
		{
			int digit = value & 0x1f;
			value = value >> 5;

			return digit < 10 ? (char)('0'+digit) : (char)('A'+digit-10);
		}

		private static void InputDigit(ref int value, char digit)
		{
			if ((digit >= '0') &&
				(digit <= '9'))
			{
				value <<= 5;
				value |= digit-'0';
			}
			else if ((digit >= 'A') &&
				     (digit <= 'A'-10+31))
			{
				value <<= 5;
				value |= digit-'A'+10;
			}
			else
			{
				throw new System.FormatException ();
			}
		}
		
		#endregion


		private static class Strings
		{
			public static readonly string		Invalid = "<invalid>";
			public static readonly string		NotSupported = "<not supported>";
		}


		public static readonly Druid			Empty = new Druid ();
		public const int						DeveloperIdMultiplier	= 10;

		private static long						uniqueId;

		private int								module;			//	0 or module id + 1
		private int								developer;		//	0 or developer id + 1
		private int								local;			//	0 or local id + 1
	}
}
