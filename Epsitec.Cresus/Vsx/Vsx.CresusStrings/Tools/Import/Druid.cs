using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.Tools
{
	/// <summary>
	/// This is an exerpt from Epsitec.Common.Support.Druid class that implements
	/// only the necessary methods to get the string representation of a DRUID
	/// </summary>
	/// <remarks>
	/// The copy-paste is to avoid a reference to Common.dll
	/// </remarks>
	public class Druid
	{
		public enum DruidType
		{
			Invalid,
			ModuleRelative,
			Full
		}

		public Druid(int module, int dev, int local)
		{
			this.module    = module+1;
			this.developer = dev+1;
			this.local     = local+1;
		}

		public static implicit operator Druid(long value)
		{
			return Druid.FromLong (value);
		}

		public static Druid FromLong(long value)
		{
			return new Druid (Druid.GetModuleId (value), Druid.GetDevId (value), Druid.GetLocalId (value));
		}

		public static int GetModuleId(long druid)
		{
			return (int) (druid >> 44) & 0xfffff;
		}

		public static int GetDevId(long druid)
		{
			return (int) (druid >> 24) & 0xfffff;
		}

		public static int GetLocalId(long druid)
		{
			return (int) (druid >> 0) & 0xffffff;
		}

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




		public int Module
		{
			get
			{
				return this.module-1;
			}
		}
		public int DeveloperAndPatchLevel
		{
			get
			{
				return this.developer-1;
			}
		}
		public int Local
		{
			get
			{
				return this.local-1;
			}
		}

		public DruidType Type
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

		public override string ToString()
		{
			switch (this.Type)
			{
				case DruidType.Invalid:
					return "<invalid>";

				case DruidType.ModuleRelative:
					return Druid.ToPrefixedModuleString ('$', this.DeveloperAndPatchLevel, this.Local);

				case DruidType.Full:
					return string.Concat ("[", Druid.ToFullString (this.Module, this.DeveloperAndPatchLevel, this.Local), "]");

				default:
					return "<not supported>";
			}
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

			return digit < 10 ? (char) ('0'+digit) : (char) ('A'+digit-10);
		}

		private int	module;			//	0 or module id + 1
		private int	developer;		//	0 or developer id + 1
		private int	local;			//	0 or local id + 1
	}
}
