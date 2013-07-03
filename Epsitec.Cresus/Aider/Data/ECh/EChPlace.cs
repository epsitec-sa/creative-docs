namespace Epsitec.Aider.Data.ECh
{

    [System.Serializable]
	internal sealed class EChPlace
	{


		public EChPlace(string name, string canton)
		{
			this.Name = name;
			this.Canton = canton;
		}


		public readonly string Name;
		public readonly string Canton;


	}


}
