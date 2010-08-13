namespace Epsitec.Common.UnitTesting
{
	
	
	public static class ExceptionAssert
	{


		public static void Throw(System.Action action)
		{
			ExceptionAssert.Throw<System.Exception> (action);
		}


		public static void Throw<TException>(System.Action action) where TException : System.Exception
		{
			if (action == null)
			{
				throw new System.ArgumentNullException ("action");
			}

			try
			{
				action ();

				string text = "Exception of type {0} expected but no exception was thrown";
				string message = System.String.Format (text, typeof (TException).FullName);

				throw new Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException (message);
			}
			catch (TException)
			{
			}
			catch (System.Exception e)
			{
				string text = "Exception of type {0} expected but exception of type {1} was thrown";
				string message = System.String.Format (text, typeof (TException).FullName, e.GetType ().FullName);

				throw new Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException (message);
			}
		}


	}


}
