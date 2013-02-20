//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Data;

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

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


		public IList<AiderGroupEntity> Parents
		{
			get
			{
				return this.GetParents ().AsReadOnlyCollection ();
			}
		}


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


		// This stuff is not cached in the entity, therefore updates in memory won't modify the
		// value returned by this method. If the entity is not persisted, it will always be 0.
		private int GetNbParticipants()
		{
			return this.ExecuteWithDataContext
			(
				d => this.GetNbParticipants (d),
				() => 0
			);
		}


		private int GetNbParticipants(DataContext dataContext)
		{
			var request = AiderGroupParticipantEntity.CreateParticipantRequest (dataContext, this, false, true, true);

			return dataContext.GetCount (request);
		}


		// This stuff is not cached in the entity, therefore updates in memory won't modify the
		// value returned by this method. It the entity is not persisted, it will always be an empty
		// list.
		private IList<AiderPersonEntity> GetParticipants(int count)
		{
			return this.ExecuteWithDataContext
			(
				d => this.GetParticipants (d, count),
				() => new List<AiderPersonEntity> ()
			);
		}


		private IList<AiderPersonEntity> GetParticipants(DataContext dataContext, int count)
		{
			var request = AiderGroupParticipantEntity.CreateParticipantRequest (dataContext, this, true, true, true);

			request.Skip = 0;
			request.Take = count;

			return dataContext.GetByRequest<AiderPersonEntity> (request);
		}


		partial void GetSubgroups(ref IList<AiderGroupEntity> value)
		{
			value = this.GetSubgroups ().AsReadOnlyCollection ();
		}


		private IList<AiderGroupEntity> GetSubgroups()
		{
			if (this.subgroups == null)
			{
				this.subgroups = this.ExecuteWithDataContext
				(
					d => this.FindSubgroups (d),
					() => new List<AiderGroupEntity> ()
				);
			}

			return this.subgroups;
		}


		private IList<AiderGroupEntity> FindSubgroups(DataContext dataContext)
		{
			// If we are at the maximum group level, there's no point in looking for sub groups in
			// the database. We won't find any. So we return directly an empty sequence. Note that
			// this optimization avoids to make a request with a group path longer than the maximum
			// group path which Firebird don't like at all. It considers such requests invalid.

			if (this.GroupLevel >= AiderGroupIds.maxGroupLevel)
			{
				return new List<AiderGroupEntity> ();
			}

			var example = new AiderGroupEntity ();
			var request = Request.Create (example);

			var path  = this.Path + AiderGroupIds.SubgroupSqlWildcard;
			var level = this.GroupLevel + 1;

			request.AddCondition (dataContext, example, x => x.GroupLevel == level && SqlMethods.Like (x.Path, path));
			request.AddSortClause (ValueField.Create (example, x => x.Name));

			return dataContext.GetByRequest (request);
		}


		public void AddSubgroupInternal(AiderGroupEntity group)
		{
			this.GetSubgroups ().Add (group);
		}


		public void RemoveSubgroupInternal(AiderGroupEntity group)
		{
			this.GetSubgroups ().Remove (group);
		}


		private IList<AiderGroupEntity> GetParents()
		{
			if (this.parents == null)
			{
				this.parents = this.ExecuteWithDataContext
				(
					d => this.FindParents (d),
					() => new List<AiderGroupEntity> ()
				);
			}

			return this.parents;
		}


		private IList<AiderGroupEntity> FindParents(DataContext dataContext)
		{
			var groupPaths = AiderGroupIds.GetParentPaths (this.Path).ToList ();

			if (groupPaths.Count == 0)
			{
				return new List<AiderGroupEntity> ();
			}

			var example = new AiderGroupEntity ();

			var request = Request.Create (example);
			request.AddCondition (dataContext, example, g => SqlMethods.IsInSet (g.Path, groupPaths));
			request.AddSortClause (ValueField.Create (example, g => g.GroupLevel), SortOrder.Ascending);

			return dataContext.GetByRequest<AiderGroupEntity> (request);
		}


		public void SetParentsInternal(IEnumerable<AiderGroupEntity> newParents)
		{
			var currentParents = this.GetParents ();

			currentParents.Clear ();
			currentParents.AddRange (newParents);
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


		public static AiderGroupEntity Create(BusinessContext businessContext, AiderGroupEntity parent, AiderGroupDefEntity groupDefinition, string name, int level, string path)
		{
			var group = AiderGroupEntity.Create (businessContext, groupDefinition, name, level, path);

			if (parent != null)
			{
				parent.AddSubgroupInternal (group);
			}

			return group;
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


		public AiderGroupEntity CreateSubgroup(BusinessContext businessContext, string name)
		{
			var subgroup = businessContext.CreateAndRegisterEntity<AiderGroupEntity> ();
			var subgroupNumber = this.GetNextSubgroupNumber ();

			subgroup.Name = name;
			subgroup.GroupLevel = this.GroupLevel + 1;
			subgroup.Path = AiderGroupIds.CreateSubGroupPath (this.Path, subgroupNumber);

			this.AddSubgroupInternal (subgroup);
			this.SetParentsInternal (this.GetParents ().Append (this));

			return subgroup;
		}


		private int GetNextSubgroupNumber()
		{
			// We look for a number that is not used yet in the subgroups.

			var usedNumbers = this.Subgroups
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


		public void DeleteSubgroup(BusinessContext businessContext, AiderGroupEntity subgroup)
		{
			// Recursively delete the children of the subgroup that we want to delete.

			foreach (var child in subgroup.GetSubgroups ().ToList ())
			{
				subgroup.DeleteSubgroup (businessContext, child);
			}

			this.RemoveSubgroupInternal (subgroup);
			
			// This might be very costly for groups which have a lot of participations.

			var participations = this.FindParticipations (businessContext);

			foreach (var participation in participations)
			{
				businessContext.DeleteEntity (participation);
			}

			if (subgroup.Comment.IsNotNull ())
			{
				businessContext.DeleteEntity (subgroup.Comment);
			}

			businessContext.DeleteEntity (subgroup);
		}


		private IEnumerable<AiderGroupParticipantEntity> FindParticipations(BusinessContext businessContext)
		{
			var example = new AiderGroupParticipantEntity ()
			{
				Group = this
			};

			return businessContext.DataContext.GetByExample (example);
		}


		private IList<AiderGroupEntity> subgroups;


		private IList<AiderGroupEntity> parents;


	}


}
