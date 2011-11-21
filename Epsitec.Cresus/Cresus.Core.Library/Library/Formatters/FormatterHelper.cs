//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library.Formatters
{
	/// <summary>
	/// The <c>FormatterHelper</c> class gets instantiated by the <see cref="FormattedIdGenerator"/>
	/// when it needs to assign a new set of IDs for a given entity.
	/// </summary>
	public class FormatterHelper
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FormatterHelper"/> class.
		/// </summary>
		public FormatterHelper()
		{
			this.localDate = System.DateTime.Now;
		}


		/// <summary>
		/// Gets the formatting context (see <see cref="FormattingContext"/>)
		/// used internally by the token classes.
		/// </summary>
		public FormattingContext				FormattingContext
		{
			get
			{
				return this.formatContext;
			}
			protected set
			{
				this.formatContext = value;
			}
		}


		/// <summary>
		/// Gets the specified component. This method is overridden by the specialized
		/// formatter helpers, in order to provide access to <c>IBusinessContext</c>,
		/// for instance.
		/// </summary>
		/// <typeparam name="T">The type of the component.</typeparam>
		/// <returns>The specified component, or <c>null</c>.</returns>
		public virtual T GetComponent<T>()
			where T : class, ICoreComponent
		{
			return null;
		}

		
		internal static IEnumerable<FormatToken> GetTokens()
		{
			var simple   = FormatterHelper.GetSimpleTokens ();
			var argument = FormatterHelper.GetArgumentTokens ();

			return Enumerable.Concat (simple, argument);
		}

		internal string Format(string format, object value)
		{
			if (string.IsNullOrEmpty (format))
			{
				return "";
			}

			System.Diagnostics.Debug.Assert (this.FormattingContext == null);

			this.FormattingContext = new FormattingContext (value);

			var buffer = new System.Text.StringBuilder ();
			int pos    = 0;

			while (pos < format.Length)
			{
				foreach (var token in FormatToken.Items)
				{
					if (token.Matches (this, format, pos))
					{
						pos += token.Format (this, buffer);
						goto next;
					}
				}

				buffer.Append (format[pos++]);
			next:
				;
			}

			this.FormattingContext = null;

			return buffer.ToString ();
		}
		

		private string FormatShortYear()
		{
			return this.localDate.ToString ("yy");
		}

		private string FormatLongYear()
		{
			return this.localDate.ToString ("yyyy");
		}

		private string FormatId(string numberFormat)
		{
			return string.Format (numberFormat, this.formatContext.Id);
		}

		private static IEnumerable<FormatToken> GetSimpleTokens()
		{
			//	Tokens such as '#ref(x)' are processed by class ArgumentFormatToken, which takes
			//	apart the provided format string in order to extract the argument (here, 'x').
			
			yield return new SimpleFormatToken ("yy",     x => x.FormatShortYear ());
			yield return new SimpleFormatToken ("yyyy",   x => x.FormatLongYear ());
			yield return new SimpleFormatToken ("##",     x => "#");
			yield return new SimpleFormatToken ("n",      x => x.FormatId ("{0:0}"));
			yield return new SimpleFormatToken ("nn",     x => x.FormatId ("{0:00}"));
			yield return new SimpleFormatToken ("nnn",    x => x.FormatId ("{0:000}"));
			yield return new SimpleFormatToken ("nnnn",   x => x.FormatId ("{0:0000}"));
			yield return new SimpleFormatToken ("nnnnn",  x => x.FormatId ("{0:00000}"));
			yield return new SimpleFormatToken ("nnnnnn", x => x.FormatId ("{0:000000}"));
		}

		private static IEnumerable<FormatToken> GetArgumentTokens()
		{
			return FormatTokenFormatterResolver.GetFormatTokens ();
		}
		
		
		private readonly System.DateTime		localDate;

		private FormattingContext				formatContext;
	}
}