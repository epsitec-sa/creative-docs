<bundle>
 <data name='class'>Window</data>
 <data name='Name'>MainWindow</data>
 <data name='Text'>EPSITEC mini Fiche</data>
 <data name='Size'>800;600</data>
 <data name='icon'>AppIcon</data>
 <list name='widgets'>
 
  <bundle>									<!-- Définition du menu -->
   <data name='class'>HMenu</data>
   <data name='Name'>MainMenu</data>
   <data name='Dock'>Top</data>
   <list name='items'>
    <bundle>
     <data name='class'>MenuItem</data>
     <data name='Text'>Fichier</data>
     <bundle name='menu'>
      <data name='class'>VMenu</data>
      <list name='items'>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='Text'>Ouvrir..</data>
        <data name='icon'>res:BaseIcons#open</data>
        <data name='Name'>Open</data>
       </bundle>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='Text'>Enregistrer..</data>
        <data name='icon'>res:BaseIcons#save</data>
        <data name='Name'>Save</data>
       </bundle>
       <bundle>
        <data name='class'>MenuSeparator</data>
       </bundle>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='Text'>Quitter</data>
        <data name='Command'>Quit</data>
       </bundle>
      </list>
     </bundle>
    </bundle>
    <bundle>
     <data name='class'>MenuItem</data>
     <data name='Text'>Edition</data>
     <bundle name='menu'>
      <data name='class'>VMenu</data>
      <list name='items'>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='Text'>Couper</data>
        <data name='icon'>res:BaseIcons#cut</data>
        <data name='Name'>Open</data>
       </bundle>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='Text'>Copier</data>
        <data name='icon'>res:BaseIcons#copy</data>
        <data name='Name'>Save</data>
       </bundle>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='Text'>Coller</data>
        <data name='icon'>res:BaseIcons#paste</data>
        <data name='Name'>Save</data>
       </bundle>
      </list>
     </bundle>
    </bundle>
   </list>
  </bundle>
  
  <bundle>									<!-- Définition de la barre d'icônes -->
   <data name='class'>HToolBar</data>
   <data name='Dock'>Top</data>
   <list name='items'>
    <bundle>
     <data name='class'>IconButton</data>
     <data name='icon'>res:BaseIcons#open</data>
    </bundle>
    <bundle>
     <data name='class'>IconButton</data>
     <data name='icon'>res:BaseIcons#save</data>
    </bundle>
    <bundle>
     <data name='class'>IconSeparator</data>
    </bundle>
    <bundle>
     <data name='class'>TextFieldCombo</data>
     <data name='Text'>Adresses</data>
     <data name='ro'>true</data>
     <data name='Size'>80;*</data>
     <bundle name='items'>
      <data name='00/DB1'>Adresses</data>
      <data name='01/DB2'>Livres</data>
      <data name='02/DB3'>CD</data>
      <data name='03/DB4'>DVD</data>
     </bundle>
    </bundle>
    <bundle>
     <data name='class'>IconSeparator</data>
    </bundle>
    <bundle>
     <data name='class'>IconButton</data>
     <data name='icon'>res:BaseIcons#cut</data>
    </bundle>
    <bundle>
     <data name='class'>IconButton</data>
     <data name='icon'>res:BaseIcons#copy</data>
    </bundle>
    <bundle>
     <data name='class'>IconButton</data>
     <data name='icon'>res:BaseIcons#paste</data>
    </bundle>
   </list>
  </bundle>
  
  <bundle>
   <data name='class'>PaneBook</data>
   <data name='Dock'>Fill</data>
   <data name='style'>LeftRight</data>
   <data name='behaviour'>FollowMe</data>
   <list name='items'>
    <bundle>
     <data name='class'>PanePage</data>
     <data name='rs'>10</data>
     <data name='min'>50</data>
     <data name='e'>0</data>
     <list name='widgets'>
      <bundle>
       <data name='class'>Button</data>
       <data name='Text'>Panneau 1</data>
       <data name='Location'>50;100</data>
       <data name='Size'>80;*</data>
       <data name='Anchor'>Bottom, Left</data>
      </bundle>
     </list>
    </bundle>
    <bundle>
     <data name='class'>PanePage</data>
     <data name='rs'>10</data>
     <data name='min'>50</data>
     <data name='e'>1</data>
     <list name='widgets'>
      <bundle>
       <data name='class'>Button</data>
       <data name='Text'>Panneau 2</data>
       <data name='Location'>50;100</data>
       <data name='Size'>80;*</data>
       <data name='Anchor'>Bottom, Left</data>
      </bundle>
     </list>
    </bundle>
   </list>
  </bundle>
  
 </list>
</bundle>
