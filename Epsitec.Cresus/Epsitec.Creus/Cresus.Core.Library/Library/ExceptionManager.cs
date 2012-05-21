//	Copyright © 2008-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>ExceptionManager</c> class manages expressions which might throw a
	/// <see cref="System.NullReferenceException"/> exception.
	/// </summary>
	public sealed class ExceptionManager : CoreAppComponent, IExceptionManager, System.IDisposable
	{
		public ExceptionManager(CoreApp app)
			: base (app)
		{
		}


		#region IExceptionManager

		public TResult Execute<TResult>(System.Func<TResult> action, System.Func<string> logSourceInfoGetter)
		{
			TResult result;

			try
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Evaluating function; source: {0}", logSourceInfoGetter ()));
				result = action ();
			}
			catch (System.NullReferenceException)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Function threw a null reference exception; source: {0}", logSourceInfoGetter ()));

				result = default (TResult);
			}

			return result;
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion

		#region Factory Class

		class Factory : Epsitec.Cresus.Core.Factories.DefaultCoreAppComponentFactory<ExceptionManager>
		{
		}

		#endregion
	}
}
