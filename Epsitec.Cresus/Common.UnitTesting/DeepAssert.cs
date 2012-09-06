using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Reflection;
using System.Collections.Generic;


namespace Epsitec.Common.UnitTesting
{


	public sealed class DeepAssert
	{


		public static void AreEqual(object expected, object actual)
		{
			if (!DeepAssert.AreObjectEqualDispatcher (expected, actual))
			{
				throw new AssertFailedException ("Objects are not equal");
			}
		}


		private static bool AreObjectEqualDispatcher(object a, object b)
		{
			// Because of the cast to dynamic, the compiler does not know which overload to call
			// and the overload will be chosen at runtime based on the real type of a and b. If we
			// implement a specific method, say for 2 int, and that a and b are both int, this
			// overload will be called instead of the one that takes 2 objects as arguments. So we
			// will only fall back on the method tat takes 2 objects if we have not implemented a
			// more specific one that is compatible with the arguments.

			dynamic dynamicA = a;
			dynamic dynamicB = b;

			return (bool) DeepAssert.AreObjectEqual (dynamicA, dynamicB);
		}


		// Add some special cases here if you need them, exactly like the one below.


		private static bool AreObjectEqual(int a, int b)
		{
			return a == b;
		}


		private static bool AreObjectEqual(bool a, bool b)
		{
			return a == b;
		}


		private static bool AreObjectEqual(decimal a, decimal b)
		{
			return a == b;
		}


		private static bool AreObjectEqual(string a, string b)
		{
			return a == b;
		}


		private static bool AreObjectEqual(object a, object b)
		{
			if (object.ReferenceEquals (a, b))
			{
				return true;
			}

			if (object.Equals (a, b))
			{
				return true;
			}

			if (a.GetType () != b.GetType ())
			{
				return false;
			}

			var flags = BindingFlags.Public | BindingFlags.Instance;

			foreach (var field in a.GetType ().GetFields (flags))
			{
				var val1 = field.GetValue (a);
				var val2 = field.GetValue (b);

				if (!DeepAssert.AreObjectEqualDispatcher (val1, val2))
				{
					return false;
				}
			}

			foreach (var property in a.GetType ().GetProperties (flags))
			{
				if (property.GetIndexParameters ().Length > 0)
				{
					// Sorry, it's impossible to implement this case...

					throw new NotImplementedException ();
				}

				if (property.CanRead)
				{
					var getter = property.GetGetMethod ();

					var val1 = getter.Invoke (a, null);
					var val2 = getter.Invoke (b, null);

					if (!DeepAssert.AreObjectEqualDispatcher (val1, val2))
					{
						return false;
					}
				}
			}

			return true;
		}


	}


}
