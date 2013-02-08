//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities.Helpers;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderPersonWarningEntity
	{
		public override FormattedText GetTitle()
		{
			return AiderWarningImplementation.GetTitle (this);
		}

		public override FormattedText GetSummary()
		{
			return AiderWarningImplementation.GetSummary (this);
		}

		public override FormattedText GetCompactSummary()
		{
			return AiderWarningImplementation.GetCompactSummary (this);
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				AiderWarningImplementation.Accumulate (this, a);

				a.Accumulate (this.Person.IsNull () ? EntityStatus.Empty : EntityStatus.Valid);

				return a.EntityStatus;
			}
		}
	}

	public interface IAiderWarningExampleFactoryGetter
	{
		AiderWarningExampleFactory GetWarningExampleFactory();
	}

	public abstract class AiderWarningExampleFactory
	{
		public abstract IEnumerable<T> GetWarnings<T>(BusinessContext context, AbstractEntity source);
	}

	public sealed class AiderWarningExampleFactory<TSource, TWarning> : AiderWarningExampleFactory
		where TSource : AbstractEntity, new ()
		where TWarning : AbstractEntity, IAiderWarning, new ()
	{
		public AiderWarningExampleFactory(System.Action<TWarning, TSource> exampleSetter)
		{
			this.exampleSetter = exampleSetter;
		}

		public override IEnumerable<T> GetWarnings<T>(BusinessContext context, AbstractEntity source)
		{
			if (typeof (T) == typeof (TWarning))
			{
				return this.GetWarnings (context, source as TSource) as IEnumerable<T>;
			}
			else
			{
				throw new System.ArgumentException ("Type mismatch");
			}
		}

		private IEnumerable<TWarning> GetWarnings(BusinessContext context, TSource source)
		{
			var repository = context.GetRepository<TWarning> ();
			var example    = repository.CreateExample ();

			this.exampleSetter (example, source as TSource);

			return repository.GetByExample (example);
		}

		private readonly System.Action<TWarning, TSource> exampleSetter;
	}
}
