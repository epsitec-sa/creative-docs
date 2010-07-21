namespace Epsitec.Common.Support.Extensions
{


	public static class ExceptionThrower
	{


		public static void ThrowIfNull<T>(this T element, string elementName) where T : class
		{
			if (element == null)
			{
				throw new System.ArgumentNullException (elementName);
			}
		}


		public static void ThrowIfNullOrEmpty(this string element, string elementName)
		{
			if (string.IsNullOrEmpty (element))
			{
				throw new System.ArgumentException (elementName);
			}
		}


	}



}
