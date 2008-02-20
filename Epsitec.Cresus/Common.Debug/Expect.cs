//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Debug
{
	/// <summary>
	/// Summary description for Expect.
	/// </summary>
	public sealed class Expect
	{
		[System.Diagnostics.Conditional ("DEBUG")]
		public static void Exception(Method method, System.Type ex_type)
		{
			try
			{
				method ();
			}
			catch (System.Exception ex)
			{
				if (ex.GetType () == ex_type)
				{
					return;
				}
				
				throw new AssertFailedException ("Exception raised; wrong type: " + ex.GetType ());
			}
			
			throw new AssertFailedException ("Expected exception not raised.");
		}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void Exception(Method method, string ex_message)
		{
			try
			{
				method ();
			}
			catch (System.Exception ex)
			{
				if (ex.Message == ex_message)
				{
					return;
				}
				
				throw new AssertFailedException ("Expected exception raised; wrong message received: " + ex.Message);
			}
			
			throw new AssertFailedException ("Expected exception not raised.");
		}
	}
	
	public delegate void Method();
}
