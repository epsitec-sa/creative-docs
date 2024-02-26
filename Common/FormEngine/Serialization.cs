namespace Epsitec.Common.FormEngine
{
    /// <summary>
    /// Procédures de sérialisation et désérialisation de masques de saisie.
    /// </summary>
    public static class Serialization
    {
        public static string SerializeForm(FormDescription form)
        {
            //	Retourne la chaîne qui contient la sérialisation d'un masque de saisie.
            return form.Serialize();
        }

        public static FormDescription DeserializeForm(string xml)
        {
            //	Retourne le masque de saisie désérialisé à partir d'une chaîne.
            FormDescription form = new FormDescription();
            form.Deserialize(xml);
            return form;
        }
    }
}
