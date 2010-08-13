namespace Epsitec.Common.Debug.UnitTesting
{
	
	
	public static class ExceptionAssert
	{


		public static void Throw(System.Action action)
		{
			ExceptionAssert.Throw<System.Exception> (action);
		}


		public static void Throw<TException>(System.Action action) where TException : System.Exception
		{
			try
			{
				action ();

				Assert.Fail ("Exception of type " + typeof (TException).FullName + " expected but no exception was thrown");
			}
			catch (TException e)
			{
			}
			catch (System.Exception e)
			{
				Assert.Fail ("Exception of type " + typeof (TException).FullName + " expected but exception of type " + e.GetType().FullName + " was thrown");
			}
		}
		

	}


}
