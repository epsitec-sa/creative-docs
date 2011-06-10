//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Business.Helpers
{
	/// <summary>
	/// The <c>FormattingContext</c> class is tightly coupled with the <see cref="FormatterHelper"/>.
	/// It stores temporary information about the current formatting operation.
	/// </summary>
	public sealed class FormattingContext
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FormattingContext"/> class.
		/// </summary>
		/// <param name="idFunc">The function which will return the ID, if property ID is accessed.</param>
		public FormattingContext(System.Func<long> idFunc)
		{
			this.idFunc = idFunc;
		}


		/// <summary>
		/// Gets the id which will have to be pretty printed.
		/// </summary>
		[System.Diagnostics.DebuggerBrowsable (System.Diagnostics.DebuggerBrowsableState.Never)]
		public long								Id
		{
			get
			{
				if (this.id == null)
				{
					this.id = this.idFunc ();
				}

				return this.id.Value;
			}
		}

		/// <summary>
		/// Gets or sets the arguments for the formatting function, if any.
		/// </summary>
		/// <value>
		/// The arguments.
		/// </value>
		public string Args
		{
			get;
			set;
		}


		private readonly System.Func<long> idFunc;
		private long? id;
	}
}
