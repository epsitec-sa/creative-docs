<bundle>
 <data name='class'>Window</data>
 <data name='name'>MainWindow</data>
 <data name='text'>EPSITEC mini Fiche</data>
 <data name='size'>800;600</data>
 <data name='icon'>AppIcon</data>
 <list name='widgets'>
 
  <bundle>									<!-- Définition du menu -->
   <data name='class'>HMenu</data>
   <data name='name'>MainMenu</data>
   <data name='dock'>Top</data>
   <list name='items'>
    <bundle>
     <data name='class'>MenuItem</data>
     <data name='text'>Fichier</data>
     <bundle name='menu'>
      <data name='class'>VMenu</data>
      <list name='items'>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='text'>Ouvrir..</data>
        <data name='icon'>res:BaseIcons#open</data>
        <data name='name'>Open</data>
       </bundle>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='text'>Enregistrer..</data>
        <data name='icon'>res:BaseIcons#save</data>
        <data name='name'>Save</data>
       </bundle>
       <bundle>
        <data name='class'>MenuSeparator</data>
       </bundle>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='text'>Quitter</data>
        <data name='cmd'>Quit</data>
       </bundle>
      </list>
     </bundle>
    </bundle>
    <bundle>
     <data name='class'>MenuItem</data>
     <data name='text'>Edition</data>
     <bundle name='menu'>
      <data name='class'>VMenu</data>
      <list name='items'>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='text'>Couper</data>
        <data name='icon'>res:BaseIcons#cut</data>
        <data name='name'>Open</data>
       </bundle>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='text'>Copier</data>
        <data name='icon'>res:BaseIcons#copy</data>
        <data name='name'>Save</data>
       </bundle>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='text'>Coller</data>
        <data name='icon'>res:BaseIcons#paste</data>
        <data name='name'>Save</data>
       </bundle>
      </list>
     </bundle>
    </bundle>
   </list>
  </bundle>
  
  <bundle>									<!-- Définition de la barre d'icônes -->
   <data name='class'>HToolBar</data>
   <data name='dock'>Top</data>
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
     <data name='text'>Adresses</data>
     <data name='ro'>true</data>
     <data name='size'>80;*</data>
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
   <data name='dock'>Fill</data>
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
       <data name='text'>Panneau 1</data>
       <data name='pos'>50;100</data>
       <data name='size'>80;*</data>
       <data name='anchor'>Bottom, Left</data>
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
       <data name='text'>Panneau 2</data>
       <data name='pos'>50;100</data>
       <data name='size'>80;*</data>
       <data name='anchor'>Bottom, Left</data>
      </bundle>
     </list>
    </bundle>
   </list>
  </bundle>
  
 </list>
</bundle>
