﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.IO;

namespace Epsitec.Common.Debug
{

	/// <summary>
	/// The ErrorLogger class provides the tools to log errors and Exceptions.
	/// </summary>
	public static class ErrorLogger
	{
		/// <summary>
		/// Builds a new Exception with message as message, logs it to the default error
		/// log file and throws it. The format used to log the Exception is the same as
		/// LogException(Exception, string).
		/// </summary>
		/// <param name="message">The message of the error.</param>
		/// <exception cref="System.Exception">Always.</exception>
		public static void LogAndThrowException(string message)
		{
			ErrorLogger.LogAndThrowException (new System.Exception (message), ErrorLogger.defaultErrorFile);
		}

		/// <summary>
		/// Builds a new Exception with message as message, logs it to file
		/// and throws it. The format used to log the Exception is the same as
		/// LogException(Exception, string).
		/// </summary>
		/// <param name="message">The message of the error.</param>
		/// <param name="file">The file where to log the error.</param>
		/// <exception cref="System.Exception">Always.</exception>
		public static void LogAndThrowException(string message, string file)
		{
			ErrorLogger.LogAndThrowException (new System.Exception (message), file);
		}

		/// <summary>
		/// Logs exception to the default error log file and throws it. The format used to
		/// log exception is the same as LogException(Exception, string).
		/// </summary>
		/// <param name="exception">The Exception to log and throw.</param>
		/// <exception cref="System.Exception">Always.</exception>
		public static void LogAndThrowException(System.Exception exception)
		{
			ErrorLogger.LogAndThrowException (exception, ErrorLogger.defaultErrorFile);
		}

		/// <summary>
		/// Logs exception to file and throws it. The format used to log exception
		/// is the same as LogException(Exception, string).
		/// </summary>
		/// <param name="exception">The Exception to log and throw.</param>
		/// <param name="file">The file where to log the error.</param>
		public static void LogAndThrowException(System.Exception exception, string file)
		{
			ErrorLogger.LogException (exception, file);
			throw exception;
		}
		
		/// <summary>
		/// Logs exception to the default error log file. The format used to log
		/// exception is the same as LogException(Exception, string).
		/// </summary>
		/// <param name="exception">The Exception to log.</param>
		public static void LogException(System.Exception exception)
		{
			ErrorLogger.LogException (exception, ErrorLogger.defaultErrorFile);
		}

		/// <summary>
		/// Logs exception to file. The type of exception, its message, its source and
		/// its stack trace are written to the log file, recursively for the inner
		/// exception.
		/// </summary>
		/// <param name="exception">The Exception to log.</param>
		/// <param name="file">The file where to log exception.</param>
		public static void LogException(System.Exception exception, string file)
		{
			string error = "========= New Exception =========";

			for (System.Exception e = exception; e != null; e = e.InnerException)
			{
				error = string.Format ("{0}\n\nType: {1}\nMessage: {2}\nSource: {3}\nStack trace: {4}",
					error,
					e.GetType (),
					e.Message,
					e.Source,
					e.StackTrace
				);
			}

			ErrorLogger.LogErrorMessage (error, file);
		}

		/// <summary>
		/// Logs message to the default error log file.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public static void LogErrorMessage(string message)
		{
			ErrorLogger.LogErrorMessage (message, ErrorLogger.defaultErrorFile);
		}

		/// <summary>
		/// Logs message to file.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="file">The file where to log message.</param>
		public static void LogErrorMessage(string message, string file)
		{
			Logger.Log (message, file);
		}


		/// <summary>
		/// The path of the default error file (UserAppData\Logs\logs.txt).
		/// </summary>
		/// <remarks>
		/// Cannot use Epsitec.Common.Support.Globals.Directories.UserAppData to build the path
		/// because Visual Studio cannot handle circular project references.
		/// </remarks>
		public static readonly string defaultErrorFile = string.Format (@"{0}\Logs\errors.txt", System.IO.Path.GetDirectoryName (System.Windows.Forms.Application.UserAppDataPath));

	}

}
