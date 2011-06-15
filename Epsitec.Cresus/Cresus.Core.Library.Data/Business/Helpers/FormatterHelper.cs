//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Resolvers;

namespace Epsitec.Cresus.Core.Business.Helpers
{
	/// <summary>
	/// The <c>FormatterHelper</c> class gets instantiated by the <see cref="FormattedIdGenerator"/>
	/// when it needs to assign a new set of IDs for a given entity.
	/// </summary>
	public sealed class FormatterHelper
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FormatterHelper"/> class.
		/// </summary>
		/// <param name="generator">The ID generator.</param>
		/// <param name="businessContext">The business context.</param>
		/// <param name="entity">The entity (which has <c>IdA</c>, <c>IdB</c> and <c>IdC</c> properties.</param>
		public FormatterHelper(FormattedIdGenerator generator, IBusinessContext businessContext, IReferenceNumber entity)
		{
			this.generator = generator;
			this.businessContext = businessContext;
			this.entity = entity;
			this.date = System.DateTime.Now;
		}


		/// <summary>
		/// Gets the formatting context (see <see cref="FormattingContext"/>)
		/// used internally by the token classes.
		/// </summary>
		public FormattingContext					FormattingContext
		{
			get
			{
				return this.formatContext;
			}
		}


		/// <summary>
		/// Assigns an ID to the attached entity.
		/// </summary>
		/// <param name="def">The generator definition.</param>
		/// <returns>Returns <c>true</c> if the assignment could be processed; otherwise, <c>false</c>.</returns>
		public bool AssignId(GeneratorDefinitionEntity def)
		{
			var assigner = FormattedIdGenerator.GetAssigner (def, this.entity);

			if (assigner == null)
			{
				return false;
			}
			else
			{
				string            name  = this.GetKeyName (def);
				System.Func<long> id    = () => this.generator.GetGeneratorNextId (name, def.InitialValue);
				string            value = this.FormatId (def, id);

				assigner (value);
				
				return true;
			}
		}
		
		
		internal static IEnumerable<FormatToken> GetTokens()
		{
			var simple   = FormatterHelper.GetSimpleTokens ();
			var argument = FormatterHelper.GetArgumentTokens ();

			return Enumerable.Concat (simple, argument);
		}


		private string GetKeyName(GeneratorDefinitionEntity definition)
		{
			return string.Concat (definition.Entity, ">", this.Format (definition.Key, () => 0L));
		}

		private string FormatId(GeneratorDefinitionEntity definition, System.Func<long> idFunc)
		{
			return this.Format (definition.Format, idFunc);
		}


		private string Format(string format, System.Func<long> idFunc)
		{
			if (string.IsNullOrEmpty (format))
			{
				return "";
			}

			System.Diagnostics.Debug.Assert (this.formatContext == null);

			this.formatContext = new FormattingContext (idFunc);

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

			this.formatContext = null;

			return buffer.ToString ();
		}



		private string FormatShortYear()
		{
			return this.date.ToString ("yy");
		}

		private string FormatLongYear()
		{
			return this.date.ToString ("yyyy");
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
		
		
		private readonly FormattedIdGenerator	generator;
		private readonly IBusinessContext		businessContext;
		private readonly IReferenceNumber		entity;
		private readonly System.DateTime		date;

		private FormattingContext				formatContext;
	}
}