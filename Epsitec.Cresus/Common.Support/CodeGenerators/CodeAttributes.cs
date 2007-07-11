//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.CodeGenerators
{
	struct CodeAttributes
	{
		public CodeAttributes(CodeAttributes attributes)
		{
			this.access = attributes.access;
			this.visibility = attributes.visibility;
			this.readOnly = attributes.readOnly;
			this.newDefinition = false;
		}

		public CodeAttributes(CodeAccess access)
		{
			this.access = access;
			this.visibility = CodeVisibility.Public;
			this.readOnly = false;
			this.newDefinition = false;
		}

		public CodeAttributes(CodeVisibility visibility)
		{
			this.access = CodeAccess.Final;
			this.visibility = visibility;
			this.readOnly = false;
			this.newDefinition = false;
		}

		public CodeAttributes(CodeAccess access, CodeVisibility visibility)
		{
			this.access = access;
			this.visibility = visibility;
			this.readOnly = false;
			this.newDefinition = false;
		}

		public CodeAttributes(CodeAccess access, CodeVisibility visibility, params object[] attributes)
		{
			this.access = access;
			this.visibility = visibility;
			this.readOnly = false;
			this.newDefinition = false;

			foreach (object attribute in attributes)
			{
				if (attribute == CodeAttributes.ReadOnlyAttribute)
				{
					this.readOnly = true;
				}
				else if (attribute == CodeAttributes.NewAttribute)
				{
					this.newDefinition = false;
				}
			}
		}

		public CodeAccess Access
		{
			get
			{
				return this.access;
			}
		}

		public CodeVisibility Visibility
		{
			get
			{
				return this.visibility;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return this.readOnly;
			}
		}

		public bool New
		{
			get
			{
				return this.newDefinition;
			}
		}

		public static readonly CodeAttributes Default = new CodeAttributes ();

		public static implicit operator CodeAccess(CodeAttributes attributes)
		{
			return attributes.Access;
		}

		public static implicit operator CodeVisibility(CodeAttributes attributes)
		{
			return attributes.Visibility;
		}

		public override string ToString()
		{
			List<string> tokens = new List<string> ();

			switch (this.visibility)
			{
				case CodeVisibility.Internal:
					tokens.Add ("internal");
					break;
				
				case CodeVisibility.Private:
					tokens.Add ("private");
					break;

				case CodeVisibility.Protected:
					tokens.Add ("protected");
					break;
				
				case CodeVisibility.Public:
					tokens.Add ("public");
					break;

				default:
					throw new System.NotSupportedException (string.Format ("CodeVisibility.{0} not supported here", this.visibility));
			}
			
			switch (this.access)
			{
				case CodeAccess.Abstract:
					tokens.Add ("abstract");
					break;
				
				case CodeAccess.Constant:
					tokens.Add ("const");
					break;

				case CodeAccess.Final:
					break;
				
				case CodeAccess.Override:
					tokens.Add ("override");
					break;

				case CodeAccess.Static:
					tokens.Add ("static");
					break;
				
				case CodeAccess.Virtual:
					tokens.Add ("virtual");
					break;

				default:
					throw new System.NotSupportedException (string.Format ("CodeAccess.{0} not supported here", this.access));
			}

			if (this.readOnly)
			{
				tokens.Add ("readonly");
			}

			if (this.newDefinition)
			{
				tokens.Add ("new");
			}

			return string.Join (" ", tokens.ToArray ());
		}

		public static readonly object ReadOnlyAttribute = new object ();
		public static readonly object NewAttribute = new object ();
		
		CodeAccess access;
		CodeVisibility visibility;
		bool readOnly;
		bool newDefinition;
	}
}
