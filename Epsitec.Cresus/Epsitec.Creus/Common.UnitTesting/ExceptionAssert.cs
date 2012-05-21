using Epsitec.Common.Support.Extensions;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Common.UnitTesting
{
	
	
	/// <summary>
	/// The <c>ExceptionAssert</c> class extends the Visual Studio 2010 unit test framework by
	/// providing a better way of testing exceptions.
	/// </summary>
	public static class ExceptionAssert
	{


		/// <summary>
		/// Asserts thats <paramref name="action"/> throws an <see cref="System.Exception"/>.
		/// </summary>
		/// <param name="action">The <see cref="System.Action"/> to check.</param>
		/// <exception cref="AssertFailedException">If <paramref name="action"/> does not throws an <see cref="System.Exception"/>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="action"/> is <c>null</c>.</exception>
		public static void Throw(System.Action action)
		{
			ExceptionAssert.Throw<System.Exception> (action);
		}


		/// <summary>
		/// Asserts thats <paramref name="action"/> throws an <see cref="System.Exception"/> of type
		/// <typeparamref name="TException"/>.
		/// </summary>
		/// <typeparam name="TException">The type of the <see cref="System.Exception"/> to be thrown.</typeparam>
		/// <param name="action">The <see cref="System.Action"/> to check.</param>
		/// <exception cref="AssertFailedException">If <paramref name="action"/> does not throws an <see cref="System.Exception"/> of type <typeparamref name="TException"/>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="action"/> is <c>null</c>.</exception>
		public static void Throw<TException>(System.Action action) where TException : System.Exception
		{
			action.ThrowIfNull ("action");

			Result result;
			System.Exception exception;

			try
			{
				action ();

				result = Result.NoExceptionThrown;
				exception = null;
			}
			catch (TException e)
			{
				result = Result.ExpectedExceptionThrown;
				exception = e;
			}
			catch (System.Exception e)
			{
				result = Result.UnexpectedExceptionThrown;
				exception = e;
			}

			switch (result)
			{
				case Result.NoExceptionThrown:
				{

					string text = "Exception of type {0} expected but no exception was thrown";
					string message = System.String.Format (text, typeof (TException).FullName);

					throw new AssertFailedException (message);
				}

				case Result.ExpectedExceptionThrown:
					break;

				case Result.UnexpectedExceptionThrown:
				{
					string text = "Exception of type {0} expected but exception of type {1} was thrown";
					string message = System.String.Format (text, typeof (TException).FullName, exception.GetType ().FullName);

					throw new AssertFailedException (message);
				}
			}
		}


		/// <summary>
		/// The <c>Result</c> enum is used to store the "exception result" of a statement, that is,
		/// if an <see cref="System.Exeption"/> has been thrown or not.
		/// </summary>
		private enum Result
		{
			
			
			/// <summary>
			/// The execution of the statement has not thrown any <see cref="System.Exeption"/>.
			/// </summary>
			NoExceptionThrown,


			/// <summary>
			/// The execution of the statement has thrown an <see cref="System.Exeption"/> of the
			/// expected type.
			/// </summary>
			ExpectedExceptionThrown,


			/// <summary>
			/// The execution of the statement has an <see cref="System.Exeption"/> of an unexpected
			/// type.
			/// </summary>
			UnexpectedExceptionThrown,


		}


	}


}
