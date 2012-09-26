using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervGroupDefinition : Freezable
	{


		public EervGroupDefinition(string id, string name, Enumerations.GroupType groupType = Enumerations.GroupType.NodeAndLeaf)
		{
			this.Id = id;
			this.Name = name;
			this.GroupType = groupType;

			this.children = new List<EervGroupDefinition> ();
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
				
				if (this.parent != null)
				{
					if (this.parent.GroupType == Enumerations.GroupType.Leaf)
					{
						this.parent.GroupType = Enumerations.GroupType.NodeAndLeaf;
					}
				}
			}
		}

		public EervGroupDefinition Function
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

		public Enumerations.GroupType GroupType
		{
			get
			{
				return this.groupType;
			}
			set
			{
				this.ThrowIfReadOnly ();

				this.groupType = value;
			}
		}

		public IList<EervGroupDefinition> Children
		{
			get
			{
				return this.children;
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


		public readonly string					Id;
		public readonly string					Name;

		private EervGroupDefinition				parent;
		private EervGroupDefinition				function;
		private Enumerations.GroupType			groupType;
		private IList<EervGroupDefinition>		children;


	}


}
