<bundle>
 <field name='class'>Window</field>
 <field name='name'>MainWindow</field>
 <field name='text'>EPSITEC mini Fiche</field>
 <field name='size'>800;600</field>
 <field name='icon'>AppIcon</field>
 <list name='widgets'>
  <bundle>									<!-- Définition du menu -->
   <field name='class'>HMenu</field>
   <field name='name'>MainMenu</field>
   <field name='dock'>Top</field>
   <list name='items'>
    <bundle>
     <field name='class'>MenuItem</field>
     <field name='text'>Fichier</field>
     <bundle name='menu'>
      <field name='class'>VMenu</field>
      <list name='items'>
       <bundle>
        <field name='class'>MenuItem</field>
        <field name='text'>Ouvrir..</field>
        <field name='name'>Open</field>
       </bundle>
       <bundle>
        <field name='class'>MenuItem</field>
        <field name='text'>Enregistrer..</field>
        <field name='name'>Save</field>
       </bundle>
       <bundle>
        <field name='class'>MenuSeparator</field>
       </bundle>
       <bundle>
        <field name='class'>MenuItem</field>
        <field name='text'>Quitter</field>
        <field name='name'>Quit</field>
       </bundle>
      </list>
     </bundle>
    </bundle>
   </list>
  </bundle>
  
  <bundle>
   <field name='class'>Widget</field>
   <field name='dock'>Fill</field>
   <field name='size'>800;570</field>
  </bundle>
 </list>
</bundle>
