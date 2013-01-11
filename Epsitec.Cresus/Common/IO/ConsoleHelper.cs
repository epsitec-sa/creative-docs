using System;

using System.Runtime.InteropServices;


namespace Epsitec.Common.IO
{


	/// <summary>
	/// Little helper class that allows windows application to create a console that can be used
	/// for output and input.
	/// </summary>
	/// <remarks>
	/// Note that a process can have at most one console, so you can't create a console twice and
	/// trying to do so might be problematic.
	/// </remarks>
	public static class ConsoleCreator
	{


		/// <summary>
		/// Creates a console, executes the given action and deletes the console afterwards. If the
		/// new console can't be created, an exception will be thrown and the action won't be
		/// executed.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		/// <param name="windowWidth">Width of the window (or zero).</param>
		public static void RunWithConsole(Action action, int windowWidth = 0)
		{
			bool success = false;

			try
			{
				success = ConsoleCreator.CreateConsole ();

				if (!success)
				{
					throw new Exception ("The console could not be created.");
				}

				if (windowWidth > 0)
				{
					System.Console.SetWindowSize (windowWidth, System.Console.WindowHeight);
					System.Console.SetBufferSize (windowWidth, System.Console.BufferHeight);
				}

				action ();
			}
			finally
			{
				if (success)
				{
					ConsoleCreator.DeleteConsole ();
				}
			}
		}


		/// <summary>
		/// Creates a console.
		/// </summary>
		/// <returns>return true for success and false for failure.</returns>
		private static bool CreateConsole()
		{
			return ConsoleCreator.AllocConsole ();
		}


		/// <summary>
		/// Deletes a console.
		/// </summary>
		/// <returns>return true for success and false for failure.</returns>
		private static bool DeleteConsole()
		{
			return ConsoleCreator.FreeConsole ();
		}


		[DllImport ("kernel32.dll", SetLastError=true)]
		[return: MarshalAs (UnmanagedType.Bool)]
		private static extern bool AllocConsole();


		[DllImport ("kernel32.dll", SetLastError=true)]
		[return: MarshalAs (UnmanagedType.Bool)]
		private static extern bool FreeConsole();


	}


}
