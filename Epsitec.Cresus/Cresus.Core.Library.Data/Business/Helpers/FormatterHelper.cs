//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Helpers
{
	internal class FormatterHelper
	{
		public FormatterHelper(FormattedIdGenerator generator, IBusinessContext businessContext, IReferenceNumber entity)
		{
			this.generator = generator;
			this.businessContext = businessContext;
			this.entity = entity;
			this.date = System.DateTime.Now;
		}


		public FormatContext					FormatContext
		{
			get
			{
				return this.formatContext;
			}
		}

		public bool AssignId(GeneratorDefinitionEntity def)
		{
			var assigner = FormattedIdGenerator.GetAssigner (def, this.entity);

			if (assigner == null)
			{
				return false;
			}
			else
			{
				string name  = this.GetKeyName (def);
				long   id    = this.generator.GetGeneratorNextId (name);
				string value = this.FormatId (def, id);

				assigner (value);
				
				return true;
			}
		}
		
		
		private string GetKeyName(GeneratorDefinitionEntity definition)
		{
			return string.Concat (definition.Entity, ">", this.Format (definition.Key));
		}

		private string FormatId(GeneratorDefinitionEntity definition, long id)
		{
			return this.Format (definition.Format, id);
		}


		private string Format(string format, long id = 0)
		{
			if (string.IsNullOrEmpty (format))
			{
				return "";
			}

			System.Diagnostics.Debug.Assert (this.formatContext == null);

			this.formatContext = new FormatContext (id);

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

		private string FormatReference(string arg)
		{
			return "";
		}

		public static IEnumerable<FormatToken> GetTokens()
		{
			yield return new SimpleFormatToken ("yy",     x => x.FormatShortYear ());
			yield return new SimpleFormatToken ("yyyy",   x => x.FormatLongYear ());
			yield return new SimpleFormatToken ("##",     x => "#");
			yield return new SimpleFormatToken ("n",      x => x.FormatId ("{0:0}"));
			yield return new SimpleFormatToken ("nn",     x => x.FormatId ("{0:00}"));
			yield return new SimpleFormatToken ("nnn",    x => x.FormatId ("{0:000}"));
			yield return new SimpleFormatToken ("nnnn",   x => x.FormatId ("{0:0000}"));
			yield return new SimpleFormatToken ("nnnnn",  x => x.FormatId ("{0:00000}"));
			yield return new SimpleFormatToken ("nnnnnn", x => x.FormatId ("{0:000000}"));
			
			//	Tokens such as '#ref(x)' are processed by class ArgumentFormatToken, which takes
			//	apart the provided format string in order to extract the argument (here, 'x').
			
			yield return new ArgumentFormatToken ("#ref", (x, arg) => x.FormatReference (arg));
		}


		
		
		private readonly FormattedIdGenerator	generator;
		private readonly IBusinessContext		businessContext;
		private readonly IReferenceNumber		entity;
		private readonly System.DateTime		date;

		private FormatContext					formatContext;
	}
}