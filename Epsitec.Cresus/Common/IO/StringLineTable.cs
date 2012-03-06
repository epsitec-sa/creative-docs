//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.IO
{
	/// <summary>
	/// The <c>StringLineTable</c> class can be used to transform a stream of lines into a
	/// table consisting of a header followed by a stream of rows. This simplifies the logic
	/// to extract the first line and the following lines and does not require multiple
	/// enumerations of the input collection, unless <see cref="Rows"/> gets enumerated
	/// multiple times.
	/// </summary>
	public sealed class StringLineTable
	{
		public StringLineTable(IEnumerable<string> lines)
		{
			this.enumerator = lines.GetEnumerator ();

			if (this.enumerator.MoveNext ())
			{
				this.header = this.enumerator.Current;
				this.state  = State.Ready;
			}
			else
			{
				this.state = State.Empty;
			}
		}

		
		public string							Header
		{
			get
			{
				return this.header;
			}
		}

		public IEnumerable<string>				Rows
		{
			get
			{
				switch (this.state)
				{
					case State.Enumerating:
						throw new System.InvalidOperationException ("Cannot enumerate several times concurrently");

					case State.Empty:
						yield break;

					case State.Done:
						this.enumerator.Reset ();
						this.enumerator.MoveNext ();
						break;
				}

				this.state = State.Enumerating;

				while (true)
				{
					if (this.enumerator.MoveNext ())
					{
						yield return this.enumerator.Current;
					}
					else
					{
						this.state = State.Done;
						break;
					}
				}
			}
		}


		private enum State
		{
			Ready,
			Empty,
			Enumerating,
			Done,
		}

		private readonly IEnumerator<string> enumerator;
		private readonly string header;
		private State state;
	}
}
