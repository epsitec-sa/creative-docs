//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TextStyle définit un style de texte de haut niveau. Il s'agit
	/// d'une collection de propriétés.
	/// </summary>
	public sealed class TextStyle : Styles.BasePropertyContainer, IContentsComparer
	{
		internal TextStyle(string name)
		{
			this.name = name;
		}
		
		internal TextStyle(string name, System.Collections.ICollection properties) : base (properties)
		{
			this.name = name;
		}
		
		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		
		public override void UpdateContentsSignature(Epsitec.Common.IO.IChecksum checksum)
		{
			//	Ignore le nom dans le calcul de la signature. C'est voulu !
			
			base.UpdateContentsSignature (checksum);
		}
		
		
		#region IContentsComparer Members
		public bool CompareEqualContents(object value)
		{
			return TextStyle.CompareEqualContents (this, value as TextStyle);
		}
		#endregion
		
		public static bool CompareEqualContents(TextStyle a, TextStyle b)
		{
			if (Styles.BasePropertyContainer.CompareEqualContents (a, b))
			{
				//	Ignore le nom dans la comparaison du contenu. C'est voulu !
				
				//	TODO: compléter
				
				return true;
			}
			else
			{
				return false;
			}
		}
		
		
		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			foreach (Properties.BaseProperty property in this)
			{
				buffer.Append (property.GetType ().Name);
				buffer.Append (":");
				property.SerializeToText (buffer);
				buffer.Append ("\n");
			}
			
			return buffer.ToString ();
		}
		
		private string							name;
	}
}
