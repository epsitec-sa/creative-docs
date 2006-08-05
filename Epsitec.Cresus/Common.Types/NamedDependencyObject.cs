//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public abstract class NamedDependencyObject : DependencyObject, IName, ICaption
	{
		public NamedDependencyObject()
		{
		}

		public NamedDependencyObject(string name)
		{
			this.DefineName (name);
		}

		public NamedDependencyObject(Support.Druid captionId)
		{
			this.DefineCaptionId (captionId);
		}
		
		public string Name
		{
			get
			{
				if (this.caption == null)
				{
					return this.name;
				}
				
				return this.caption.Name ?? this.name;
			}
		}

		public Caption Caption
		{
			get
			{
				return this.caption;
			}
		}


		#region IName Members

		string IName.Name
		{
			get
			{
				return this.Name;
			}
		}

		#endregion

		#region INameCaption Members

		public Support.Druid CaptionId
		{
			get
			{
				if (this.caption == null)
				{
					return Support.Druid.Empty;
				}
				else
				{
					return this.caption.Druid;
				}
			}
		}

		#endregion
		
		
		public void DefineName(string name)
		{
			if ((this.caption == null) &&
				(this.name == null))
			{
				this.name = name;
			}
			else
			{
				throw new System.InvalidOperationException ("The name cannot be changed");
			}
		}

		public void DefineCaptionId(Support.Druid druid)
		{
			this.DefineCaption (Support.Resources.DefaultManager.GetCaption (druid));
		}
		
		public void DefineCaption(Caption caption)
		{
			if (this.caption == null)
			{
				this.caption = caption;
			}
			else
			{
				throw new System.InvalidOperationException ("The caption cannot be changed");
			}
		}


		private string name;
		private Caption caption;
	}
}
