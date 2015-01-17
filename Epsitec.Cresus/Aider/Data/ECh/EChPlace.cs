﻿namespace Epsitec.Aider.Data.ECh
{


	internal sealed class EChPlace
	{


		public EChPlace(string name, string canton)
		{
			this.Name = name;
			this.Canton = canton;
		}

		
		public string Display()
		{
			return this.Name + " (" + this.Canton + ")";
		}


		public readonly string Name;
		public readonly string Canton;


	}


}
