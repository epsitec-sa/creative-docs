Ext.require([
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.DropZone', {
    extend: 'Ext.view.View',
    alternateClassName: ['Epsitec.DropZone'],

    /* Properties */
    dropZoneId: null,
    dropZoneStore: null,
    label: null,
    onDropFunction: null,
    scope: null,
    /* Constructor */

    constructor: function(dropZoneId,label,onDropFunction,scope) {
      var config;
      this.dropZoneId = dropZoneId;
      this.onDropFunction = onDropFunction;
      this.scope = scope;
      this.label = label;
      this.dropZoneStore = Ext.create('Ext.data.Store', {
          model: 'Bag',
          data: {},
      });

      config = {
          dock: 'bottom',
          cls: 'entity-view',
          tpl: '<tpl for=".">' +
                  '<div class="entitybag-target">' + this.label + '</div>' +
               '</tpl>',
          itemSelector: 'div.entitybag-target',
          overItemCls: 'entitybag-target-over',
          selectedItemClass: 'entitybag-selected',
          singleSelect: true,
          width: '100%',
          executer: this,
          store: this.dropZoneStore, 
          listeners: {
              render: this.initializeEntityDropZone,
              destroy: this.unregEntityDragZone,
              scope: this
          }
        };

      this.callParent([config]);
      this.hide();
      return this;
    },

    /* Methods */

    executeOnDrop: function (data) {
      this.onDropFunction.apply(this.scope,[data.entityData]);
      this.hide();
    },

    unregEntityDragZone: function (v) {
      if(v.dragZone)
      {
        v.dragZone.unreg();
      }
    },
    
    initializeEntityDropZone: function(v) {
        v.dropZone = Ext.create('Ext.dd.DropZone', v.el, {

//      If the mouse is over a target node, return that node. This is
//      provided as the "target" parameter in all "onNodeXXXX" node event handling functions
        getTargetFromEvent: function(e) {
            return e.getTarget('.entitybag-target');
        },

//      On entry into a target node, highlight that node.
        onNodeEnter : function(target, dd, e, data){
          if(target)
          {
             Ext.fly(target).addCls('entitybag-target-hover');
          }          
        },

//      On exit from a target node, unhighlight that node.
        onNodeOut : function(target, dd, e, data){
          if(target)
          {
            Ext.fly(target).removeCls('entitybag-target-hover');
          }        
        },

//      While over a target node, return the default drop allowed class which
//      places a "tick" icon into the drag proxy.
        onNodeOver : function(target, dd, e, data){
            return Ext.dd.DropZone.prototype.dropAllowed;
        },

        onNodeDrop : function(target, dd, e, data){  
            v.executer.executeOnDrop(data);
            return true;
        }
      });
    }
  });
});
