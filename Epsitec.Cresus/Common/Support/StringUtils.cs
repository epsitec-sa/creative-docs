using System.Linq;

namespace Epsitec.Common.Support
{


	public static class StringUtils
	{


		public static string Join(string separator, params string[] strings)
		{
			return string.Join (separator, strings.Where (s => !string.IsNullOrEmpty (s)));
		}


		public static string Join(string separator, params object[] objects)
		{
			var strings = objects
				.Where (o => o != null)
				.Select (o => o.ToString ())
				.Where (s => !string.IsNullOrEmpty (s));

			return string.Join (separator, strings);
		}


	}


}
