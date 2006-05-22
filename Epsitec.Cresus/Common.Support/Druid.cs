//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>Druid</c> class manages Description Resource Unique ID related
	/// conversions. These IDs are 64-bit identifiers which encode the module
	/// identity, the developer identity and a locally unique value.
	/// </summary>
	public static class Druid
	{
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
	}
}
