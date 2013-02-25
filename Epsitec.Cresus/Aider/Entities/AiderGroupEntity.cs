//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Data;
using Epsitec.Aider.Enumerations;

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


		public AiderGroupEntity Parent
		{
			get
			{
				return this.Parents.LastOrDefault ();
			}
		}


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


		public string GetHierarchicalName(AiderPersonEntity person)
		{
			return this
				.GetHierarchicalParents (person.Parish.Group.Path)
				.Append (this)
				.Select (g => g.Name)
				.Join (", ");
		}


		private IEnumerable<AiderGroupEntity> GetHierarchicalParents(string parishPath)
		{
			var currentPath = this.Path;
			
			var skip = 0;

			if (AiderGroupIds.IsWithinParish (currentPath))
			{
				skip += 1;

				if (AiderGroupIds.IsWithinSameParish (currentPath, parishPath))
				{
					skip += 1;
				}
			}
			else if (AiderGroupIds.IsWithinRegion (currentPath))
			{
				if (AiderGroupIds.IsWithinSameRegion (currentPath, parishPath))
				{
					skip += 1;
				}
			}

			return this.GetParents ().Skip (skip);
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


		public bool CanHaveSubgroups()
		{
			var definition = this.GroupDef;

			return this.GroupLevel < AiderGroupIds.MaxGroupLevel
				&& (definition.IsNull () || definition.SubgroupsAllowed);
		}


		public bool CanSubgroupsBeEdited()
		{
			var definition = this.GroupDef;

			return this.CanHaveSubgroups ()
				&& (definition.IsNull () || definition.Mutability == Mutability.Customizable);
		}


		public bool CanBeEdited()
		{
			return this.GroupDef.IsNull ();
		}


		public bool CanHaveMembers()
		{
			var definition = this.GroupDef;

			return definition.IsNull () || definition.MembersAllowed;
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
			if (!this.CanHaveSubgroups ())
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


		private void AddSubgroupInternal(AiderGroupEntity group)
		{
			this.GetSubgroups ().Add (group);
		}


		private void RemoveSubgroupInternal(AiderGroupEntity group)
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


		private void SetParentInternal(AiderGroupEntity newParent)
		{
			if (this.parents == null)
			{
				this.parents = new List<AiderGroupEntity> ();
			}
			else
			{
				this.parents.Clear ();
			}

			this.parents.AddRange (newParent.GetParents ().Append (newParent));
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
			if (!this.CanHaveSubgroups ())
			{
				throw new InvalidOperationException ("This group cannot have subgroups");
			}

			var subgroup = businessContext.CreateAndRegisterEntity<AiderGroupEntity> ();

			subgroup.Name = name;
			subgroup.GroupLevel = this.GroupLevel + 1;
			subgroup.Path = this.GetNextSubgroupPath ();

			this.AddSubgroupInternal (subgroup);
			subgroup.SetParentInternal (this);

			return subgroup;
		}


		private string GetNextSubgroupPath()
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

			return AiderGroupIds.CreateSubGroupPath (this.Path, number.Value);
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


		public void Move(AiderGroupEntity newParent)
		{
			if (!newParent.CanHaveSubgroups ())
			{
				throw new InvalidOperationException ("This group cannot have subgroups");
			}
			
			// We start by removing this group from its current parent.

			var currentParent = this.Parent;

			if (currentParent.IsNotNull ())
			{
				currentParent.RemoveSubgroupInternal (this);
			}

			// Here we must move the current group and all its subgroups recursively. Since the
			// subgroups of a group are determined by their path only, we must update the path of
			// the current group and of its subgroups. We must also update their group level. To do
			// this, we make a single SQL request to fetch all the direct and the indirect children
			// of the current group. Then we update all their paths and their group level

			var oldPathSize = this.Path.Length;
			var newPath = newParent.GetNextSubgroupPath ();

			var deltaGroupLevel = newParent.GroupLevel + 1 - this.GroupLevel;

			var allSubgroups = this.GetAllSubgroups ();
			var allGroups = allSubgroups.Append (this);

			foreach (var group in allGroups)
			{
				var path = newPath;

				if (group.Path.Length > oldPathSize)
				{
					path += group.Path.Substring (oldPathSize);
				}

				group.Path = path;
				group.GroupLevel += deltaGroupLevel;
			}

			// Finaly, we update the in memory cache of the parents of this group and of all its
			// subgroups. Note that we sort the groups based on their path before looping on them.
			// This ensures that when we update the cache of the parents of a group, the cache of
			// the parents of its parent has already been updated.

			var pathToGroups = allGroups.ToDictionary (g => g.Path);
			
			this.SetParentInternal (newParent);

			foreach (var group in this.GetAllSubgroups ().OrderBy (g => g.Path))
			{
				var parentPath = AiderGroupIds.GetParentPath (group.Path);
				var parent = pathToGroups[parentPath];

				group.SetParentInternal (parent);
			}
		}


		private IList<AiderGroupEntity> GetAllSubgroups()
		{
			return this.ExecuteWithDataContext
			(
				d => this.FindAllSubgroups (d),
				() => this.FindAllSubgroups ()
			);
		}


		private IList<AiderGroupEntity> FindAllSubgroups(DataContext dataContext)
		{
			if (!this.CanHaveSubgroups ())
			{
				return new List<AiderGroupEntity> ();
			}

			var example = new AiderGroupEntity ();
			var request = Request.Create (example);
			
			request.AddCondition (dataContext, example, x => x.GroupLevel > this.GroupLevel);
			request.AddCondition (dataContext, example, x => SqlMethods.Like (x.Path, this.Path + "%"));

			return dataContext.GetByRequest (request);
		}


		private IList<AiderGroupEntity> FindAllSubgroups()
		{
			var groups = new List<AiderGroupEntity> ();

			this.FindAllSubgroups (groups);

			return groups;
		}


		private void FindAllSubgroups(IList<AiderGroupEntity> groups)
		{
			foreach (var subgroup in this.GetSubgroups ())
			{
				groups.Add (subgroup);

				subgroup.FindAllSubgroups (groups);
			}
		}


		public int GetDepth()
		{
			var totalDepth = this.ExecuteWithDataContext
			(
				d => this.FindDepth (d),
				() => this.FindDepth ()
			);

			return totalDepth - this.GroupLevel + 1;
		}


		private int FindDepth(DataContext dataContext)
		{
			// Here it would be cool to make a request that simply returns the maximum, but it can't
			// be done now. This is something that the DataLayer does not implement right now.

			var subgroups = this.FindAllSubgroups (dataContext);

			if (subgroups.Count == 0)
			{
				return this.GroupLevel;
			}

			return subgroups.Max (g => g.GroupLevel);
		}


		private int FindDepth()
		{
			var subgroups = this.GetSubgroups ();

			if (subgroups.Count == 0)
			{
				return this.GroupLevel;
			}

			return this.GetSubgroups ().Max (g => g.FindDepth ());
		}


		public bool IsChild(AiderGroupEntity group)
		{
			return AiderGroupIds.IsWithinGroup (this.Path, group.Path);
		}


		private IList<AiderGroupEntity> subgroups;


		private IList<AiderGroupEntity> parents;


	}


}
