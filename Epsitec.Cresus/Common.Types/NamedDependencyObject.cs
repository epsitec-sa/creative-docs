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
				if (this.name != null)
				{
					return this.name;
				}

				if ((this.caption == null) &&
					(this.captionId.IsEmpty))
				{
					return null;
				}
				
				return this.Caption.Name;
			}
		}

		public Caption Caption
		{
			get
			{
				if (this.caption == null)
				{
					if (this.captionId.IsEmpty)
					{
						this.DefineCaption (new Caption ());
					}
					else
					{
						this.DefineCaption (Support.Resources.DefaultManager.GetCaption (this.captionId));
					}
				}
				
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
				return this.captionId;
			}
		}

		#endregion


		internal void LockName()
		{
			this.lockedName = true;
		}
		
		public void DefineName(string name)
		{
			if (this.lockedName)
			{
				throw new System.InvalidOperationException ("The name is locked and cannot be changed");
			}
			
			if (this.caption == null)
			{
				this.name = name;
			}
			else
			{
				this.caption.Name = name;
			}
		}

		public void DefineCaptionId(Support.Druid druid)
		{
			if (this.captionId.IsEmpty)
			{
				this.captionId = druid;
			}
			else
			{
				throw new System.InvalidOperationException ("The caption DRUID cannot be changed");
			}
		}
		
		public void DefineCaption(Caption caption)
		{
			if ((this.caption == null) &&
				(caption != null) &&
				(this.captionId.IsEmpty || (this.captionId == caption.Druid)))
			{
				this.caption = caption;
				this.captionId = caption.Druid;
			}
			else
			{
				throw new System.InvalidOperationException ("The caption cannot be changed");
			}
		}


		private string name;
		private bool lockedName;
		private Support.Druid captionId;
		private Caption caption;
	}
}
