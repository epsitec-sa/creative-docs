//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// Summary description for BasePropertyContainer.
	/// </summary>
	public abstract class BasePropertyContainer : IContentsSignature, IContentsSignatureUpdater
	{
		public BasePropertyContainer()
		{
		}
		
		public BasePropertyContainer(System.Collections.ICollection properties)
		{
			this.Initialise (properties);
		}
		
		
		
		public long 							Version
		{
			get
			{
				if (this.version == 0)
				{
					this.Update ();
				}
				
				return this.version;
			}
		}
		
		public Properties.BaseProperty[]		Properties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new Properties.BaseProperty[0];
				}
				
				return this.properties;
			}
		}
		
		public int								CountUsers
		{
			get
			{
				return this.user_count;
			}
		}
		
		
		public void IncrementUserCount()
		{
			Debug.Assert.IsInBounds (this.user_count+1, 1, BaseStyle.MaxUserCount-1);
			this.user_count++;
		}
		
		public void DecrementUserCount()
		{
			Debug.Assert.IsInBounds (this.user_count, 1, BaseStyle.MaxUserCount-1);
			this.user_count--;
		}
		
		
		public virtual void Initialise(System.Collections.ICollection properties)
		{
			this.properties = new Properties.BaseProperty[properties.Count];
			properties.CopyTo (this.properties, 0);
			
			this.Invalidate ();
		}
		
		public void Invalidate()
		{
			this.version = 0;
			this.ClearContentsSignature ();
		}
		
		public bool Update()
		{
			//	Recalcule le numéro de version correspondant à ce style
			//	en se basant sur les versions des propriétés.
			
			bool changed = false;
			
			//	Retourne true si une modification a eu lieu.
			
			if ((this.properties != null) &&
				(this.properties.Length > 0))
			{
				long version = 0;
				
				for (int i = 0; i < this.properties.Length; i++)
				{
					version = System.Math.Max (version, this.properties[i].Version);
				}
				
				if (this.version != version)
				{
					this.version = version;
					this.ClearContentsSignature ();
					
					changed = true;
				}
			}
			else if (this.version > 0)
			{
				this.version = 0;
				this.ClearContentsSignature ();
				
				changed = true;
			}
			
			return changed;
		}
		
		
		
		#region IContentsSignatureUpdater Members
		public abstract void UpdateContentsSignature(IO.IChecksum checksum);
		#endregion
		
		#region IContentsSignature Members
		public int GetContentsSignature()
		{
			//	Retourne la signature (CRC) correspondant au contenu du style.
			//	La signature exclut les réglages et l'index.
			
			//	Si la signature n'existe pas, il faut la calculer; on ne fait
			//	cela qu'à la demande, car le calcul de la signature peut être
			//	relativement onéreux :
			
			if (this.contents_signature == 0)
			{
				IO.IChecksum checksum = IO.Checksum.CreateAdler32 ();
				
				this.UpdateContentsSignature (checksum);
				
				int signature = (int) checksum.Value;
				
				//	La signature calculée pourrait être nulle; dans ce cas, on
				//	l'ajuste pour éviter d'interpréter cela comme une absence
				//	de signature :
				
				this.contents_signature = (signature == 0) ? 1 : signature;
			}
			
			return this.contents_signature;
		}
		#endregion
		
		protected void ClearContentsSignature()
		{
			this.contents_signature = 0;
		}
		
		
		private long							version;
		private int								contents_signature;
		
		private Properties.BaseProperty[]		properties;
		private int								user_count;
	}
}
