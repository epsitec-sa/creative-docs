//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// Summary description for BaseProperty.
	/// </summary>
	public abstract class BaseProperty : IContentsSignature, IContentsSignatureUpdater, IContentsComparer
	{
		protected BaseProperty()
		{
		}
		
		
		public long 							Version
		{
			get
			{
				return this.version;
			}
		}
		
		public abstract PropertyType			PropertyType
		{
			get;
		}
		
		
		public void Invalidate()
		{
			this.contents_signature = 0;
			this.version            = 0;
		}
		
		public void UpdateVersion()
		{
			this.version = StyleVersion.Current;
		}
		
		
		#region IContentsSignatureUpdater Members
		public abstract void UpdateContentsSignature(IO.IChecksum checksum);
		#endregion
		
		#region IContentsComparer Members
		public abstract bool CompareEqualContents(object value);
		#endregion
		
		#region IContentsSignature Members
		public int GetContentsSignature()
		{
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
		
		
		public abstract Properties.BaseProperty GetCombination(Properties.BaseProperty property);
		
		
		private int								contents_signature;
		private long							version;
	}
}
