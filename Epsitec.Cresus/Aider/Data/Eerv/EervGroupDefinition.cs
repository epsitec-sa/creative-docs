//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System;

using System.Collections.Generic;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervGroupDefinition : Freezable
	{


		public EervGroupDefinition(string id, string name, bool isLeaf)
		{
			this.Id = id;
			this.Name = name;
			this.isLeaf = isLeaf;

			this.children = new List<EervGroupDefinition> ();
		}


		public readonly string					Id;

		public readonly string					Name;


		public bool								MembersAllowed
		{
			get
			{
				return this.isLeaf || !this.SubgroupsAllowed;
			}
		}


		public bool								SubgroupsAllowed
		{
			get
			{
				return this.children.Count > 0 || this.FunctionGroup != null;
			}
		}


		public int GroupLevel
		{
			get
			{
				return this.groupLevel;
			}
			set
			{
				this.ThrowIfReadOnly ();

				this.groupLevel = value;
			}
		}


		public int? DefinitionNumber
		{
			get
			{
				return this.definitionNumber;
			}
			set
			{
				this.ThrowIfReadOnly ();

				this.definitionNumber = value;
			}
		}


		public EervGroupDefinition				Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.ThrowIfReadOnly ();

				this.parent = value;
			}
		}

		public EervGroupDefinition				FunctionGroup
		{
			get
			{
				return this.function;
			}
			set
			{
				this.ThrowIfReadOnly ();

				this.function = value;
			}
		}

		public IList<EervGroupDefinition>		Children
		{
			get
			{
				return this.children;
			}
		}

		public GroupClassification GroupClassification
		{
			get
			{
				if (this.Id.StartsWith ("0101"))
				{
					return GroupClassification.Function;
				}

				switch (this.Id.SubstringStart (2))
				{
					case "03":
						return GroupClassification.Region;
					case "04":
						return GroupClassification.Parish;
					case "07":
						return GroupClassification.NoParish;
					default:
						return GroupClassification.None;
				}
			}
		}


		public bool IsFunctionDefinition()
		{
			return this.GroupClassification == GroupClassification.Function
				&& this.GroupLevel == 1;
		}


		public string GetPathTemplate()
		{
			if (this.Parent == null)
			{
				// If the path template is special, like for region or function, we use that prefix,
				// otherwise we use the number of the definition to setup the path template, in
				// order to ensure that it is unique.
				return AiderGroupIds.CreateTopLevelPathTemplate (this.GroupClassification)
					?? AiderGroupIds.CreateTopLevelPathTemplate (this.DefinitionNumber.Value);
			}

			var parentPathTemplate = this.Parent.GetPathTemplate ();
			var number = parent.Children.IndexOf (this);

			if (this.GroupClassification == GroupClassification.Parish && this.GroupLevel == 1)
			{
				return AiderGroupIds.CreateParishSubgroupPath (parentPathTemplate);
			}
			else if (this.Parent.IsFunctionDefinition ())
			{
				return AiderGroupIds.CreateFunctionSubgroupPath (parentPathTemplate, number);
			}
			else
			{
				return AiderGroupIds.CreateDefinitionSubgroupPath (parentPathTemplate, number);
			}
		}


		protected override void HandleFreeze()
		{
			base.HandleFreeze ();

			this.children = this.children.AsReadOnlyCollection ();
		}


		public static int GetLevel(string id)
		{
			int level = 0;

			for (int i = 0; i < id.Length && id.Substring (i, 2) != "00"; i += 2)
			{
				level++;
			}

			return level;
		}


		public static string GetParentId(string id)
		{
			var level = EervGroupDefinition.GetLevel (id);

			return id.Substring (0, (level - 1) * 2).PadRight (10, '0');
		}


		private readonly bool					isLeaf;
		private int								groupLevel;
		private int?							definitionNumber;
		private EervGroupDefinition				parent;
		private EervGroupDefinition				function;
		private IList<EervGroupDefinition>		children;


	}

}
