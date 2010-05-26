//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>TaskletRunMode</c> defines how <see cref="TaskletJob"/> jobs have to
	/// be executed within a <see cref="Tasklet"/> batch (before/within/after).
	/// </summary>
	public enum TaskletRunMode
	{
		/// <summary>
		/// Synchronous execution -- reserved for internal use only.
		/// </summary>
		Sync,

		/// <summary>
		/// Asynchronous execution; the job is part of the main body of the
		/// batch.
		/// </summary>
		Async,

		/// <summary>
		/// Synchronous execution before the main body of the batch starts.
		/// </summary>
		Before,

		/// <summary>
		/// Synchronous execution before the main body of the batch starts;
		/// and then also asynchronous execution after the main body of the
		/// batch ends.
		/// </summary>
		BeforeAndAfter,

		/// <summary>
		/// Asynchronous execution after the main body of the batch ends.
		/// </summary>
		After,
	}
}
