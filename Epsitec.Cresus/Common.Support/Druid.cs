//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>Druid</c> class manages Description Resource Unique ID related
	/// conversions. These IDs are 64-bit identifiers which encode the module
	/// identity, the developer identity and a locally unique value.
	/// The string encoding is very compact for small IDs: we rely on an
	/// interleaved encoding such as MMDLLDLLDMLDM, with trailing zeroes
	/// omitted. M=module ID, D=dev. ID, L=local ID. Each is encoded using
	/// 5-bit digits (0..9, A..V) and the first digit of each category
	/// encodes the lowest bits of each ID. For instance "1023" can be used
	/// to represent module=1 ('1'*32^0 + '0'*32^1), dev=2, local=3.
	/// </summary>
	public struct Druid
	{
		public Druid(Druid druid)
		{
			this.module = druid.module;
			this.dev = druid.dev;
			this.local = druid.local;
		}

		public Druid(int dev, int local)
		{
			this.module = 0;
			this.dev = dev+1;
			this.local = local+1;
		}

		public Druid(int module, int dev, int local)
		{
			this.module = module+1;
			this.dev = dev+1;
			this.local = local+1;
		}

		public DruidType Type
		{
			get
			{
				int module = this.Module;
				int dev = this.Developer;
				int local = this.Local;

				if ((dev < 0) || (dev > 0x000fffff))
				{
					return DruidType.Invalid;
				}
				if ((local < 0) || (local > 0x00ffffff))
				{
					return DruidType.Invalid;
				}
				
				if (module == -1)
				{
					return DruidType.ModuleRelative;
				}

				if ((module < 0) || (module > 0x000fffff))
				{
					return DruidType.Invalid;
				}
				
				return DruidType.Full;
			}
		}
		
		public int Module
		{
			get
			{
				return this.module-1;
			}
		}

		public int Developer
		{
			get
			{
				return this.dev-1;
			}
		}

		public int Local
		{
			get
			{
				return this.local-1;
			}
		}

		public string ToResourceId()
		{
			DruidType type = this.Type;

			if (type != DruidType.Full)
			{
				throw new System.InvalidOperationException (string.Format ("Cannot convert {0} DRUID to a resource id", type));
			}

			return string.Concat ("[", Druid.ToFullString (Druid.FromIds (this.Module, this.Developer, this.Local)), "]");
		}

		public string ToFieldIdName()
		{
			DruidType type = this.Type;
			
			if ((type != DruidType.Full) &&
				(type != DruidType.ModuleRelative))
			{
				throw new System.InvalidOperationException (string.Format ("Cannot convert {0} DRUID to a field id name", type));
			}

			return string.Concat (ResourceBundle.FieldIdPrefix, Druid.ToModuleString (Druid.FromIds (this.Developer, this.Local)));
		}

		public long ToLong()
		{
			DruidType type = this.Type;

			switch (type)
			{
				case DruidType.Full:
					return Druid.FromIds (this.Module, this.Developer, this.Local);
				
				case DruidType.ModuleRelative:
					return Druid.FromIds (this.Developer, this.Local);
				
				default:
					throw new System.InvalidOperationException (string.Format ("Cannot convert {0} DRUID to a long", type));
			}
		}
		
		public long ToFieldId()
		{
			DruidType type = this.Type;

			if ((type != DruidType.Full) &&
				(type != DruidType.ModuleRelative))
			{
				throw new System.InvalidOperationException (string.Format ("Cannot convert {0} DRUID to a field id", type));
			}

			return Druid.FromIds (this.Developer, this.Local);
		}

		public static Druid Parse(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return new Druid ();
			}

			if (value[0] == '$')
			{
				long druid = Druid.FromModuleString (value.Substring (1));
				return new Druid (Druid.GetDevId (druid), Druid.GetLocalId (druid));
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
		/// Converts the DRUID to a full string encoded as MMDLLDLLDMLDM, less
		/// significant digits first, trailing zeroes omitted.
		/// </summary>
		/// <param name="druid">The 64-bit DRUID to convert.</param>
		/// <returns>The string representing the full DRUID.</returns>
		public static string ToFullString(long druid)
		{
			int module = (int) (druid >> 44) & 0xfffff;
			int dev = (int) (druid >> 24) & 0xfffff;
			int local = (int) (druid >> 0) & 0xffffff;

			char[] buffer = new char[13];

			buffer[0]  = Druid.OutputDigit (ref module);
			buffer[1]  = Druid.OutputDigit (ref module);
			buffer[2]  = Druid.OutputDigit (ref dev);
			buffer[3]  = Druid.OutputDigit (ref local);
			buffer[4]  = Druid.OutputDigit (ref local);
			buffer[5]  = Druid.OutputDigit (ref dev);
			buffer[6]  = Druid.OutputDigit (ref local);
			buffer[7]  = Druid.OutputDigit (ref local);
			buffer[8]  = Druid.OutputDigit (ref dev);
			buffer[9]  = Druid.OutputDigit (ref module);
			buffer[10] = Druid.OutputDigit (ref local);
			buffer[11] = Druid.OutputDigit (ref dev);
			buffer[12] = Druid.OutputDigit (ref module);

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

			Druid.InputDigit (ref module, buffer[12]);
			Druid.InputDigit (ref dev, buffer[11]);
			Druid.InputDigit (ref local, buffer[10]);
			Druid.InputDigit (ref module, buffer[9]);
			Druid.InputDigit (ref dev, buffer[8]);
			Druid.InputDigit (ref local, buffer[7]);
			Druid.InputDigit (ref local, buffer[6]);
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
				/**/ (digit <= 'A'-10+31))
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

		public static readonly string BundleName = "DruidData";

		private int module;
		private int dev;
		private int local;
	}
}
