//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Aider.Entities;

using System.Collections.Generic;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervGroupDefinition : Freezable
	{


		public EervGroupDefinition(string id, string name, Enumerations.GroupNodeType groupNodeType = Enumerations.GroupNodeType.NodeAndLeaf, int groupLevel = 0)
		{
			this.Id = id;
			this.Name = name;
			this.GroupNodeType = groupNodeType;
			this.GroupLevel = groupLevel;

			this.children = new List<EervGroupDefinition> ();
		}

		
		public readonly string					Id;

		public readonly string					Name;
		
		public EervGroupDefinition				Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.ThrowIfReadOnly ();

				if (((this.GroupLevel > 0) && (!this.Id.StartsWith ("01"))) ||
					((this.GroupLevel > 1)))
				{
					//	Not a root node
				}
				else
				{
					value = null;
				}
				
				this.parent = value;

				if (this.parent != null)
				{
					if (this.parent.GroupNodeType == Enumerations.GroupNodeType.Leaf)
					{
						this.parent.GroupNodeType = Enumerations.GroupNodeType.NodeAndLeaf;
					}
				}
				else
				{
					this.GroupNodeType = Enumerations.GroupNodeType.Root;
				}
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

		public Enumerations.GroupNodeType		GroupNodeType
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

		public readonly int						GroupLevel;
		
		public IList<EervGroupDefinition>		Children
		{
			get
			{
				return this.children;
			}
		}

		public Enumerations.GroupClassification GroupClassification
		{
			get
			{
				string prefix = this.Id.SubstringStart (2);

				switch (prefix)
				{
					case "01":
						switch (this.Id.Substring (2, 2))
						{
							case "01":
								return Enumerations.GroupClassification.Function;
							case "02":
								return Enumerations.GroupClassification.Staff;
							case "03":
								return Enumerations.GroupClassification.StaffAssociation;
							default:
								break;
						}
						break;
					
					case "02":
						return Enumerations.GroupClassification.Canton;
					case "03":
						return Enumerations.GroupClassification.Region;
					case "04":
						return Enumerations.GroupClassification.Parish;
					case "05":
						return Enumerations.GroupClassification.Common;
					case "06":
						return Enumerations.GroupClassification.External;
					default:
						break;
				}
				
				return Enumerations.GroupClassification.None;
			}
		}

		public AiderGroupDefEntity				EntityCache;

		public string GetPathTemplate()
		{
			var buffer = new System.Text.StringBuilder ();

			this.BuildPathTemplate (buffer);

			return buffer.ToString ();

		}
		
		protected void BuildPathTemplate(System.Text.StringBuilder buffer)
		{
			if (this.parent != null)
			{
				this.parent.BuildPathTemplate (buffer);
			}

			if (((this.GroupLevel > 0) && (!this.Id.StartsWith ("01"))) ||
				((this.GroupLevel > 1)))
			{
				buffer.Append (this.Id.Substring (this.GroupLevel*2, 2));
				buffer.Append (".");
				return;
			}

			switch (this.GroupClassification)
			{
				case Enumerations.GroupClassification.Canton:
					buffer.Append ("SCC.");
					break;
				case Enumerations.GroupClassification.Common:
					buffer.Append ("MIC.");
					break;
				case Enumerations.GroupClassification.External:
					buffer.Append ("REX.");
					break;
				case Enumerations.GroupClassification.Function:
					buffer.Append ("FNC.");
					break;
				case Enumerations.GroupClassification.Parish:
					buffer.Append ("P__.");
					break;
				case Enumerations.GroupClassification.Region:
					buffer.Append ("R__.");
					break;
				case Enumerations.GroupClassification.Staff:
					buffer.Append ("PRS.");
					break;
				case Enumerations.GroupClassification.StaffAssociation:
					buffer.Append ("ASP.");
					break;

				default:
					break;
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


		private EervGroupDefinition				parent;
		private EervGroupDefinition				function;
		private Enumerations.GroupNodeType			groupType;
		private IList<EervGroupDefinition>		children;


	}


}
