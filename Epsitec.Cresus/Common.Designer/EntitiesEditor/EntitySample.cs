using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	public class EntitySample
	{
		public EntitySample(string name, StructuredTypeFlags flags, int boxCount)
		{
			this.Name = name;
			this.Flags = flags;
			this.BoxCount = boxCount;
		}


		public string Name
		{
			get;
			private set;
		}

		public StructuredTypeFlags Flags
		{
			get;
			private set;
		}

		public int BoxCount
		{
			get;
			private set;
		}


		public string NameDescription
		{
			get
			{
				if (this.IsMajor)
				{
					return Misc.Bold (this.Name);
				}
				else
				{
					return this.Name;
				}
			}
		}

		public string FlagsDescription
		{
			get
			{
				if ((this.Flags & Types.StructuredTypeFlags.GenerateSchema) != 0)
				{
					return "Schéma";
				}
				else
				{
					return "";
				}
			}
		}

		public string BoxCountDescription
		{
			get
			{
				if (this.BoxCount > 1)
				{
					return string.Concat (this.BoxCount.ToString (), " boîtes");
				}
				else
				{
					return "";
				}
			}
		}

		public bool IsMajor
		{
			get
			{
				return (this.Flags & Types.StructuredTypeFlags.GenerateSchema) != 0 || this.BoxCount > 1;
			}
		}
	}
}
