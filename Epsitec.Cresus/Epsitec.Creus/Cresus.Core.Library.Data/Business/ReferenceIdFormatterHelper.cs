//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library.Formatters;
using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// The <c>FormatterHelper</c> class gets instantiated by the <see cref="FormattedIdGenerator"/>
	/// when it needs to assign a new set of IDs for a given entity.
	/// </summary>
	internal sealed class ReferenceIdFormatterHelper : Epsitec.Cresus.Core.Library.Formatters.FormatterHelper
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FormatterHelper"/> class.
		/// </summary>
		/// <param name="generator">The ID generator.</param>
		/// <param name="businessContext">The business context.</param>
		/// <param name="entity">The entity (which has <c>IdA</c>, <c>IdB</c> and <c>IdC</c> properties.</param>
		public ReferenceIdFormatterHelper(FormattedIdGenerator generator, IBusinessContext businessContext, IReferenceNumber entity)
		{
			this.generator = generator;
			this.businessContext = businessContext;
			this.entity = entity;
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


		public override T GetComponent<T>()
		{
			var type = typeof (T);

			if (type == typeof (IBusinessContext))
			{
				return this.businessContext as T;
			}
			if (type == typeof (RefIdGeneratorPool))
			{
				return this.generator.RefIdGeneratorPool as T;
			}

			return base.GetComponent<T> ();
		}


		private string Format(string format, System.Func<long> idFunc)
		{
			if (string.IsNullOrEmpty (format))
			{
				return "";
			}

			System.Diagnostics.Debug.Assert (this.FormattingContext == null);

			this.FormattingContext = new FormattingContext (idFunc);

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
		
		private string GetKeyName(GeneratorDefinitionEntity definition)
		{
			return string.Concat (definition.Entity, ">", this.Format (definition.Key, () => 0L));
		}

		private string FormatId(GeneratorDefinitionEntity definition, System.Func<long> idFunc)
		{
			return this.Format (definition.Format, idFunc);
		}
		
		private readonly FormattedIdGenerator	generator;
		private readonly IBusinessContext		businessContext;
		private readonly IReferenceNumber		entity;
	}
}