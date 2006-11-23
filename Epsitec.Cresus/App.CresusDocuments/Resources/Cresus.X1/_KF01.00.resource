<?xml version="1.0" encoding="utf-8"?>
<bundle name="_KF01" caption="Form.Invoice" type="Panel" culture="fr" rank="0">
  <data name="Panel">
    <xml>
      <panel xmlns:s="http://www.epsitec.ch/XNS/storage-structure-1" xmlns:f="http://www.epsitec.ch/XNS/storage-fields-1">
  <s:storage s:root="_1" s:n_ext="2" s:n_typ="13" s:n_obj="18">
    <s:external s:name="_DataSource" />
    <s:external s:name="_ResourceManager" />
    <s:type id="_1" s:name="Epsitec.Common.UI.Panel" />
    <s:type id="_2" s:name="Epsitec.Common.Support.ResourceManager" />
    <s:type id="_3" s:name="Epsitec.Common.Types.DataObject" />
    <s:type id="_4" s:name="Epsitec.Common.Widgets.GroupBox" />
    <s:type id="_5" s:name="Epsitec.Common.UI.PanelPlaceholder" />
    <s:type id="_6" s:name="Epsitec.Common.Widgets.FrameBox" />
    <s:type id="_7" s:name="Epsitec.Common.Widgets.Layouts.LayoutEngine" />
    <s:type id="_8" s:name="Epsitec.Common.Widgets.Layouts.GridLayoutEngine" />
    <s:type id="_9" s:name="Epsitec.Common.Widgets.Layouts.RowDefinition" />
    <s:type id="_10" s:name="Epsitec.Common.Widgets.Layouts.ColumnDefinition" />
    <s:type id="_11" s:name="Epsitec.Common.UI.Placeholder" />
    <s:type id="_12" s:name="Epsitec.Common.UI.DataSourceMetadata" />
    <s:type id="_13" s:name="Epsitec.Common.UI.Panels.EditPanel" />
    <s:object id="_1" s:type="_1" />
    <s:object id="_2" s:type="_4" />
    <s:object id="_3" s:type="_5" />
    <s:object id="_4" s:type="_4" />
    <s:object id="_5" s:type="_5" />
    <s:object id="_6" s:type="_6" />
    <s:object id="_7" s:type="_8" />
    <s:object id="_8" s:type="_9" />
    <s:object id="_9" s:type="_9" />
    <s:object id="_10" s:type="_9" />
    <s:object id="_11" s:type="_9" />
    <s:object id="_12" s:type="_10" />
    <s:object id="_13" s:type="_10" />
    <s:object id="_14" s:type="_10" />
    <s:object id="_15" s:type="_11" />
    <s:object id="_16" s:type="_11" />
    <s:object id="_17" s:type="_12" />
    <s:object id="_18" s:type="_13" />
    <s:data id="_1" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Anchored" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="547" f:PreferredHeight="189" f:Anchor="BottomLeft" f:Padding="20;20;20;20" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:_1.BundleId="[KF01]" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:Children="{Collection {ObjRef _2}, {ObjRef _4}, {ObjRef _6}}" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _17}" f:EditionPanel="{ObjRef _18}" f:SearchPanel="{Null}" f:Text="" />
    <s:data id="_2" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:Anchor="BottomLeft" f:PreferredWidth="160" f:PreferredHeight="60" f:MinWidth="50" f:MinHeight="50" f:Dock="None" f:Margins="347.5;0;0;83" f:CaptionId="[KF0I]" f:Children="{Collection {ObjRef _3}}" />
    <s:data id="_3" f:Value="{Binding Path=*.BillingAddress, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Anchored" f:ContainerLayoutMode="HorizontalFlow" f:Anchor="None" f:Text="&lt;i&gt;Panel&lt;/i&gt;" f:PreferredWidth="137" f:PreferredHeight="56" f:MinWidth="50" f:MinHeight="50" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:Dock="StackEnd" f:Margins="0;0;0;0" f:PanelId="[KF]" />
    <s:data id="_4" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:Anchor="BottomLeft" f:PreferredWidth="160" f:PreferredHeight="60" f:MinWidth="50" f:MinHeight="50" f:Dock="None" f:Margins="347.5;0;0;6" f:CaptionId="[KF0J]" f:Children="{Collection {ObjRef _5}}" />
    <s:data id="_5" f:Value="{Binding Path=*.ShippingAddress, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Anchored" f:ContainerLayoutMode="HorizontalFlow" f:Anchor="None" f:Text="&lt;i&gt;Panel&lt;/i&gt;" f:PreferredWidth="137" f:PreferredHeight="56" f:MinWidth="50" f:MinHeight="50" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:Dock="StackEnd" f:Margins="0;0;0;0" f:PanelId="[KF]" f:TabIndex="1" />
    <s:data id="_6" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:ChildrenLayoutMode="Grid" f:ContainerLayoutMode="VerticalFlow" f:Anchor="All" f:PreferredWidth="332.5" f:PreferredHeight="95" f:MinWidth="50" f:MinHeight="50" f:Padding="0;0;0;0" f:Dock="None" f:Margins="0;175;12.5;28.5" f:_7.LayoutEngine="{ObjRef _7}" f:Children="{Collection {ObjRef _15}, {ObjRef _16}}" f:Text="" />
    <s:data id="_7" f:ColumnDefinitions="{Collection {ObjRef _8}, {ObjRef _9}, {ObjRef _10}, {ObjRef _11}}" f:RowDefinitions="{Collection {ObjRef _12}, {ObjRef _13}, {ObjRef _14}}" />
    <s:data id="_8" f:Height="Auto" f:MinHeight="20" />
    <s:data id="_9" f:Height="Auto" f:MinHeight="20" />
    <s:data id="_10" f:Height="Auto" f:MinHeight="20" />
    <s:data id="_11" f:Height="Auto" f:MinHeight="20" />
    <s:data id="_12" f:Width="Auto" f:MinWidth="20" />
    <s:data id="_13" f:Width="Auto" f:MinWidth="60" />
    <s:data id="_14" f:Width="*100" f:MinWidth="20" />
    <s:data id="_15" f:Value="{Binding Path=*.Title, Mode=TwoWay}" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:MinWidth="20" f:Anchor="None" f:MinHeight="20" f:Dock="None" f:PreferredHeight="20" f:Margins="0;0;0;0" f:PreferredWidth="146" f:ContainerLayoutMode="HorizontalFlow" f:_8.ColumnSpan="3" f:_8.Column="0" f:_8.Row="0" f:VerticalAlignment="Center" />
    <s:data id="_16" f:Value="{Binding Path=*.TotalAmount, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:MinWidth="20" f:MinHeight="20" f:Anchor="None" f:Dock="None" f:_8.Column="0" f:_8.Row="2" f:ContainerLayoutMode="HorizontalFlow" />
    <s:data id="_17" f:Fields="{Collection {Text '*;[KF0G]'}}" />
    <s:data id="_18" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Anchored" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="389" f:PreferredHeight="332" f:Anchor="BottomLeft" f:Dock="None" f:Margins="0;0;0;0" f:Padding="20;20;20;20" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _17}" f:EditionPanel="{ObjRef _18}" f:Owner="{ObjRef _1}" f:SearchPanel="{Null}" f:Text="" />
  </s:storage>
</panel>
    </xml>
  </data>
</bundle>