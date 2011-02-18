namespace Epsitec.Common.Support.Extensions
{


	/// <summary>
	/// The <c>ExceptionThrower</c> class contains extension methods that can be used to check
	/// variables and throw <see cref="Exception"/> if some condition is not met.
	/// </summary>
	public static class ExceptionThrower
	{


		/// <summary>
		/// Checks that <paramref name="element"/> is not null.
		/// </summary>
		/// <typeparam name="T">The type of <paramref name="element"/>.</typeparam>
		/// <param name="element">The element to ensure that it is not null.</param>
		/// <param name="elementName">The name of <paramref name="element"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="element"/> is null.</exception>
		public static void ThrowIfNull<T>(this T element, string elementName) where T : class
		{
			if (element == null)
			{
				throw new System.ArgumentNullException (elementName);
			}
		}


		/// <summary>
		/// Checks that <paramref name="element"/> has a value.
		/// </summary>
		/// <typeparam name="T">The type of <paramref name="element"/>.</typeparam>
		/// <param name="element">The element to ensure that it has a value.</param>
		/// <param name="elementName">The name of <paramref name="element"/>.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="element"/> has no value.</exception>
		public static void ThrowIfWithoutValue<T>(this T? element, string elementName) where T : struct
		{
			if (!element.HasValue)
			{
				throw new System.ArgumentException (elementName);
			}
		}


		/// <summary>
		/// Checks that <paramref name="element"/> is not null and not empty.
		/// </summary>
		/// <typeparam name="T">The type of <paramref name="element"/>.</typeparam>
		/// <param name="element">The element to ensure that it is not null and not empty.</param>
		/// <param name="elementName">The name of <paramref name="element"/>.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="element"/> is null or empty.</exception>
		public static void ThrowIfNullOrEmpty(this string element, string elementName)
		{
			if (string.IsNullOrEmpty (element))
			{
				throw new System.ArgumentException (elementName);
			}
		}


		/// <summary>
		/// Checks that <paramref name="element"/> satisfies <paramref name="condition"/>.
		/// </summary>
		/// <typeparam name="T">The type of <paramref name="element"/>.</typeparam>
		/// <param name="element">The element on which to ensure the condition.</param>
		/// <param name="condition">The condition to check on <paramref name="element"/>.</param>
		/// <param name="message">The message that will be put in the <see cref="System.ArgumentException"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="condition"/> is null.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="condition"/> is not met by <paramref name="element"/>.</exception>
		public static void ThrowIf<T>(this T element, System.Predicate<T> condition, string message)
		{
			condition.ThrowIfNull ("condition");

			if (condition (element))
			{
				throw new System.ArgumentException (message);
			}
		}


	}



}
