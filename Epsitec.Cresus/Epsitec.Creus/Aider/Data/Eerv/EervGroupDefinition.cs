using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervGroupDefinition : Freezable
	{


		public EervGroupDefinition(string id, string name)
		{
			this.Id = id;
			this.Name = name;

			this.children = new List<EervGroupDefinition> ();
		}

		public EervGroupDefinition Parent
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


		public readonly string Id;
		public readonly string Name;


		private EervGroupDefinition parent;
		private IList<EervGroupDefinition> children;


	}


}
