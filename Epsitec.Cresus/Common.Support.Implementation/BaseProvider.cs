//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Support.Implementation
{
	using System.Globalization;
	using System.Text.RegularExpressions;
	
	/// <summary>
	/// La classe BaseProvider donne accès aux ressources stockées dans une base
	/// de données.
	/// </summary>
	public class BaseProvider : AbstractResourceProvider
	{
		public BaseProvider()
		{
		}
		
		protected string GetRowNameFromId(string id, ResourceLevel level)
		{
			if (this.ValidateId (id))
			{
				switch (level)
				{
					case ResourceLevel.Default:		return id;
					case ResourceLevel.Localised:	return id + this.suffix;
					case ResourceLevel.Customised:	return id + this.custom;
					
					default:
						throw new ResourceException (string.Format ("Invalid resource level {0} for resource '{1}'.", level, id));
				}
			}
			
			return null;
		}
		
		
		public override string			Prefix
		{
			get { return "base"; }
		}
		
		
		public override bool ValidateId(string id)
		{
			return base.ValidateId (id);
		}
		
		public override bool Contains(string id)
		{
			if (this.ValidateId (id))
			{
				//	On valide toujours le nom avant, pour éviter des mauvaises surprises si
				//	l'appelant est malicieux.
				
				//	TODO: vérifie si la ressource existe, en cherchant uniquement le niveau
				//	ResourceLevel.Default.
			}
			
			return false;
		}
		
		public override byte[] GetData(string id, Epsitec.Common.Support.ResourceLevel level)
		{
			string row_name = this.GetRowNameFromId (id, level);
			
			//	TODO: implémenter GetData.
			
			throw new System.NotImplementedException ("GetData not implemented.");
		}
		
		
		public override void Create(string id, Epsitec.Common.Support.ResourceLevel level)
		{
			// TODO:  Add FileProvider.Create implementation
			throw new ResourceException ("Not implemented");
		}
		
		public override void Update(string id, Epsitec.Common.Support.ResourceLevel level, byte[] data)
		{
			// TODO:  Add FileProvider.Update implementation
			throw new ResourceException ("Not implemented");
		}
		
		public override void Remove(string id, Epsitec.Common.Support.ResourceLevel level)
		{
			// TODO:  Add FileProvider.Remove implementation
			throw new ResourceException ("Not implemented");
		}
	}
}
