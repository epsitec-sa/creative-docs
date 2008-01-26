<?xml version="1.0" encoding="utf-8"?>
<bundle name="_KF05" caption="TestAvecTable" type="Panel" culture="fr" rank="8">
  <data name="Panel">
    <xml>
      <panel xmlns:s="http://www.epsitec.ch/XNS/storage-structure-1" xmlns:f="http://www.epsitec.ch/XNS/storage-fields-1">
        <s:storage s:root="_1" s:n_ext="2" s:n_typ="8" s:n_obj="8">
          <s:external s:name="_DataSource" />
          <s:external s:name="_ResourceManager" />
          <s:type id="_1" s:name="Epsitec.Common.UI.Panel" />
          <s:type id="_2" s:name="Epsitec.Common.Support.ResourceManager" />
          <s:type id="_3" s:name="Epsitec.Common.Types.DataObject" />
          <s:type id="_4" s:name="Epsitec.Common.UI.TablePlaceholder" />
          <s:type id="_5" s:name="Epsitec.Common.UI.ItemTableColumn" />
          <s:type id="_6" s:name="Epsitec.Common.UI.DataSourceMetadata" />
          <s:type id="_7" s:name="Epsitec.Common.UI.Panels.EditPanel" />
          <s:type id="_8" s:name="Epsitec.Common.UI.Panels.SearchPanel" />
          <s:object id="_1" s:type="_1" />
          <s:object id="_2" s:type="_4" />
          <s:object id="_3" s:type="_5" />
          <s:object id="_4" s:type="_5" />
          <s:object id="_5" s:type="_5" />
          <s:object id="_6" s:type="_6" />
          <s:object id="_7" s:type="_7" />
          <s:object id="_8" s:type="_8" />
          <s:data id="_1" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="311" f:PreferredHeight="200" f:Anchor="BottomLeft" f:Padding="20;20;20;20" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:_1.BundleId="[KF05]" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:Children="{Collection {ObjRef _2}}" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _6}" f:EditionPanel="{ObjRef _7}" f:SearchPanel="{ObjRef _8}" f:Text="" />
          <s:data id="_2" f:Items="{Binding Path=*.Employees, Mode=TwoWay}" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:Margins="5;5;5;5" f:Text="&lt;i&gt;Table&lt;/i&gt;" f:Dock="StackFill" f:Anchor="None" f:Columns="{Collection {ObjRef _3}, {ObjRef _4}, {ObjRef _5}}" f:SourceType="[KF0E]" />
          <s:data id="_3" f:FieldId="FirstName" />
          <s:data id="_4" f:FieldId="LastName" />
          <s:data id="_5" f:TemplateId="[KF]" f:CaptionId="[KF05]" />
          <s:data id="_6" f:Fields="{Collection {Text '*;[KF0M]'}}" />
          <s:data id="_7" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="456" f:PreferredHeight="200" f:Anchor="BottomLeft" f:Dock="None" f:Margins="0;0;0;0" f:Padding="20;20;20;20" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _6}" f:EditionPanel="{ObjRef _7}" f:Owner="{ObjRef _1}" f:SearchPanel="{ObjRef _8}" f:Text="" />
          <s:data id="_8" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="456" f:PreferredHeight="200" f:Anchor="BottomLeft" f:Dock="None" f:Margins="0;0;0;0" f:Padding="20;20;20;20" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _6}" f:EditionPanel="{ObjRef _7}" f:Owner="{ObjRef _1}" f:SearchPanel="{ObjRef _8}" f:Text="" />
        </s:storage>
      </panel>
    </xml>
  </data>
  <data name="DefaultSize">311;200</data>
  <data name="RootEntityId"></data>
</bundle>