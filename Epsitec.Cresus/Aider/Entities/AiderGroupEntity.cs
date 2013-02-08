//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Data;
using Epsitec.Aider.Data.Eerv;
using Epsitec.Aider.Entities.Helpers;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderGroupEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			yield return this.Name;
		}

		public static AiderGroupEntity Create(BusinessContext businessContext, AiderGroupDefEntity groupDefinition,
			/**/							  GroupPathInfo info)
		{
			var name = info.Name;
			var level = info.Level;
			var path = info.MapPath (groupDefinition);
			
			return AiderGroupEntity.Create (businessContext, groupDefinition, name, level, path);
		}

		public static AiderGroupEntity Create(BusinessContext businessContext, AiderGroupDefEntity groupDefinition, string name, int level, string path)
		{
			var group = businessContext.CreateAndRegisterEntity<AiderGroupEntity> ();

			group.Name = name;
			group.GroupLevel = level;
			group.Path = path;
			group.GroupDef = groupDefinition;

			return group;
		}


		public AiderGroupEntity CreateSubGroup(BusinessContext businessContext, string name, int subGroupNumber)
		{
			if (subGroupNumber > AiderGroupIds.MaxSubGroupNumber)
			{
				throw new Exception ("Group number too high");
			}

			var subGroup = businessContext.CreateAndRegisterEntity<AiderGroupEntity> ();

			subGroup.Name = name;

			this.SetupSubGroup (subGroup, subGroupNumber);

			return subGroup;
		}


		public void SetupSubGroup(AiderGroupEntity subGroup, int subGroupNumber)
		{
			subGroup.GroupLevel = this.GroupLevel + 1;
			subGroup.Path = AiderGroupIds.CreateSubGroupPath (this.Path, subGroupNumber);
		}


		public AiderGroupEntity CreateSubGroup(BusinessContext businessContext, string name)
		{
			var nextSubGroupNumber = this.GetNextSubGroupNumber ();

			return this.CreateSubGroup (businessContext, name, nextSubGroupNumber);
		}


		public int GetNextSubGroupNumber()
		{
			// We look for a number that is not used yet in the subgroups.

			var usedNumbers = this.Subgroups
				// We check for that in case the group we want to compute the number has already
				// been added to the list, like in the AiderGroupSubGroupList class.
				.Where (g => !string.IsNullOrEmpty (g.Path))
				.Select (g => AiderGroupIds.GetGroupNumber (g.Path))
				.ToSet ();

			int? number = null;

			for (int i = AiderGroupIds.MinSubGroupNumber; i < AiderGroupIds.MaxSubGroupNumber + 1; i++)
			{
				if (!usedNumbers.Contains (i))
				{
					number = i;
					break;
				}
			}

			if (!number.HasValue)
			{
				throw new Exception ("Too many subgroups.");
			}

			return number.Value;
		}


		public IEnumerable<AiderGroupEntity> FindSubgroups(BusinessContext businessContext)
		{
			// If we are at the maximum group level, there's no point in looking for sub groups in
			// the database. We won't find any. So we return directly an empty sequence. Note that
			// this optimization avoids to make a request with a group path longer than the maximum
			// group path which Firebird don't like at all. It considers such requests invalid.

			if (this.GroupLevel >= AiderGroupEntity.maxGroupLevel)
			{
				return Enumerable.Empty<AiderGroupEntity> ();
			}
			
			var dataContext = businessContext.DataContext;

			var example = new AiderGroupEntity ();
			var request = Request.Create (example);

			var path  = this.Path + AiderGroupIds.SubgroupSqlWildcard;
			var level = this.GroupLevel + 1;

			request.AddCondition (dataContext, example, x => x.GroupLevel == level && SqlMethods.Like (x.Path, path));
			request.AddSortClause (ValueField.Create (example, x => x.Name));

			return dataContext.GetByRequest (request);
		}

		public IEnumerable<AiderGroupParticipantEntity> FindParticipants(BusinessContext businessContext)
		{
			var example = new AiderGroupParticipantEntity ();
			var request = Request.Create (example);

			example.Group = this;

			return businessContext.DataContext.GetByRequest (request);
		}


		public static IList<AiderGroupEntity> FindRootGroups(BusinessContext businessContext)
		{
			var example = new AiderGroupEntity ()
			{
				GroupLevel = 0,
			};

			var request = Request.Create (example);
			request.AddSortClause (ValueField.Create (example, x => x.Name));

			return businessContext.DataContext.GetByRequest (request);
		}


		public IEnumerable<AiderGroupEntity> GetGroupChain(BusinessContext businessContext)
		{
			foreach (var path in AiderGroupIds.GetGroupChainPaths (this.Path))
			{
				var example = new AiderGroupEntity ()
				{
					Path = path
				};

				yield return businessContext.DataContext.GetByExample (example).Single ();
			}
		}


		partial void GetSubgroups(ref IList<AiderGroupEntity> value)
		{
			if (this.subgroupsList == null)
			{
				this.subgroupsList = new AiderGroupSubGroupList (this);;
			}

			value = this.subgroupsList;
		}

		partial void GetParticipants(ref IList<AiderPersonEntity> value)
		{
			throw new NotImplementedException ();
		}


		public FormattedText GetParticipantsTitle()
		{
			var nbParticipants = this.GetNbParticipants ();

			return TextFormatter.FormatText ("Particpants (", nbParticipants, ")");
		}


		public FormattedText GetParticipantsSummary()
		{
			int count = 10;

			var groups = this.GetParticipants (count + 1)
				.Select (g => g.GetCompactSummary ())
				.CreateSummarySequence (count, "...");

			return FormattedText.Join (FormattedText.FromSimpleText ("\n"), groups);
		}


		private int GetNbParticipants()
		{
			var dataContext = DataContextPool.GetDataContext (this);

			if ((dataContext != null) &&
				(dataContext.IsPersistent (this)))
			{
				var request = AiderGroupParticipantEntity.CreateParticipantRequest (dataContext, this, false, true, true);

				return dataContext.GetCount (request);
			}

			return 0;
		}


		private IEnumerable<AiderPersonEntity> GetParticipants(int count)
		{
			var dataContext = DataContextPool.GetDataContext (this);

			if ((dataContext != null) &&
				(dataContext.IsPersistent (this)))
			{
				var request = AiderGroupParticipantEntity.CreateParticipantRequest (dataContext, this, true, true, true);

				request.Skip = 0;
				request.Take = count;

				return dataContext.GetByRequest<AiderPersonEntity> (request);
			}

			return Enumerable.Empty<AiderPersonEntity> ();
		}


		private AiderGroupSubGroupList subgroupsList;

		private static readonly int maxGroupLevel = 6;

	}
}
