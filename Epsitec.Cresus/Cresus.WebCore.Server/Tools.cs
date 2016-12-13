//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Debug;

using Epsitec.Common.IO;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;

using System.Diagnostics;

using System.IO;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server
{
	/// <summary>
	/// This class contains some general purpose methods that are used throughout the project.
	/// </summary>
	internal static class Tools
	{
		public static System.IDisposable Bind(this BusinessContext businessContext, params AbstractEntity[] entities)
		{
			return businessContext.Bind ((IEnumerable<AbstractEntity>) entities);
		}

		public static System.IDisposable Bind(this BusinessContext businessContext, IEnumerable<AbstractEntity> entities)
		{
			var entitiesToDispose = new List<AbstractEntity> ();

			try
			{
				// We filter the dummy entities that might be in the list and we check for null
				// because DataContext.Contains() doesn't like them.
				var validEntities = entities
					.Where (e => e != null)
					.Where (e => businessContext.DataContext.Contains (e));

				foreach (var entity in validEntities)
				{
					businessContext.Register (entity);

					entitiesToDispose.Add (entity);
				}
			}
			catch
			{
				foreach (var entity in entitiesToDispose)
				{
					businessContext.Unregister (entity);
				}

				throw;
			}

			System.Action action = () =>
			{
				foreach (var entity in entities)
				{
					businessContext.Unregister (entity);
				}
			};

			return DisposableWrapper.CreateDisposable (action);
		}


		public static string GetOptionalParameter(dynamic parameter)
		{
			return parameter.HasValue
				? parameter.Value
				: null;
		}


//		[Conditional ("DEBUG")]
		public static void LogMessage(string message)
		{
			Logger.LogToConsole (message);
		}

		public static void LogError(string message)
		{
			Tools.LogMessage (message);

			var path = Tools.GetErrorFilePath ();
			ErrorLogger.LogErrorMessage (message, path);
		}


		private static string GetErrorFilePath()
		{
			var now    = System.DateTime.Now;
			var format = "crash {1:0000}-{2:00}-{3:00} {4:00}-{5:00}-{6:00} {0}.log";
			var name   = string.Format (format, System.Guid.NewGuid (), now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

			return Path.Combine
			(
				System.Environment.GetFolderPath (System.Environment.SpecialFolder.CommonApplicationData),
				"Epsitec",
				"WebCore",
				"Logs",
				name
			);
		}
	}
}
