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
        <field name='icon'>res:BaseIcons#open</field>
        <field name='name'>Open</field>
       </bundle>
       <bundle>
        <field name='class'>MenuItem</field>
        <field name='text'>Enregistrer..</field>
        <field name='icon'>res:BaseIcons#save</field>
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
    <bundle>
     <field name='class'>MenuItem</field>
     <field name='text'>Edition</field>
     <bundle name='menu'>
      <field name='class'>VMenu</field>
      <list name='items'>
       <bundle>
        <field name='class'>MenuItem</field>
        <field name='text'>Couper</field>
        <field name='icon'>res:BaseIcons#cut</field>
        <field name='name'>Open</field>
       </bundle>
       <bundle>
        <field name='class'>MenuItem</field>
        <field name='text'>Copier</field>
        <field name='icon'>res:BaseIcons#copy</field>
        <field name='name'>Save</field>
       </bundle>
       <bundle>
        <field name='class'>MenuItem</field>
        <field name='text'>Coller</field>
        <field name='icon'>res:BaseIcons#paste</field>
        <field name='name'>Save</field>
       </bundle>
      </list>
     </bundle>
    </bundle>
   </list>
  </bundle>
  
  <bundle>									<!-- Définition de la barre d'icônes -->
   <field name='class'>ToolBar</field>
   <field name='dock'>Top</field>
   <list name='items'>
    <bundle>
     <field name='class'>IconButton</field>
     <field name='icon'>res:BaseIcons#open</field>
    </bundle>
    <bundle>
     <field name='class'>IconButton</field>
     <field name='icon'>res:BaseIcons#save</field>
    </bundle>
    <bundle>
     <field name='class'>IconSeparator</field>
    </bundle>
    <bundle>
     <field name='class'>TextFieldCombo</field>
     <field name='text'>Adresses</field>
     <field name='ro'>true</field>
     <field name='size'>80;*</field>
     <bundle name='items'>
      <field name='00/DB1'>Adresses</field>
      <field name='01/DB2'>Livres</field>
      <field name='02/DB3'>CD</field>
      <field name='03/DB4'>DVD</field>
     </bundle>
    </bundle>
    <bundle>
     <field name='class'>IconSeparator</field>
    </bundle>
    <bundle>
     <field name='class'>IconButton</field>
     <field name='icon'>res:BaseIcons#cut</field>
    </bundle>
    <bundle>
     <field name='class'>IconButton</field>
     <field name='icon'>res:BaseIcons#copy</field>
    </bundle>
    <bundle>
     <field name='class'>IconButton</field>
     <field name='icon'>res:BaseIcons#paste</field>
    </bundle>
   </list>
  </bundle>
  
  <bundle>
   <field name='class'>PaneBook</field>
   <field name='dock'>Fill</field>
   <field name='style'>LeftRight</field>
   <field name='behaviour'>FollowMe</field>
   <list name='items'>
    <bundle>
     <field name='class'>PanePage</field>
     <field name='rs'>10</field>
     <field name='min'>50</field>
     <field name='e'>0</field>
     <list name='widgets'>
      <bundle>
       <field name='class'>Button</field>
       <field name='text'>Panneau 1</field>
       <field name='pos'>50;100</field>
       <field name='size'>80;*</field>
       <field name='anchor'>Bottom, Left</field>
      </bundle>
     </list>
    </bundle>
    <bundle>
     <field name='class'>PanePage</field>
     <field name='rs'>10</field>
     <field name='min'>50</field>
     <field name='e'>1</field>
     <list name='widgets'>
      <bundle>
       <field name='class'>Button</field>
       <field name='text'>Panneau 2</field>
       <field name='pos'>50;100</field>
       <field name='size'>80;*</field>
       <field name='anchor'>Bottom, Left</field>
      </bundle>
     </list>
    </bundle>
   </list>
  </bundle>
  
 </list>
</bundle>
