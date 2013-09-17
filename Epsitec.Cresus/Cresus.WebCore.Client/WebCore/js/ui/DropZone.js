Ext.require([
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.DropZone', {
    extend: 'Ext.view.View',
    alternateClassName: ['Epsitec.DropZone'],

    /* Properties */
    dropZoneStore: null,
    label: null,
    onDropFunction: null,
    /* Constructor */

    constructor: function(label,onDropFunction) {
      var config;
      this.onDropFunction = onDropFunction;
      this.label = label
      this.dropZoneStore = Ext.create('Ext.data.Store', {
          model: 'Bag',
          data: [{
          id: 1,
          summary: "---",
          entityType: "---",
          data: "---"
        }]
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
          store: this.dropZoneStore, 
          listeners: {
              render: this.initializeEntityDropZone
          }
        };

      this.callParent([config]);

      return this;
    },

    /* Methods */

    executeOnDrop: function (data) {
      this.onDropFunction(data.entityData);
    },

    initializeEntityDropZone: function(v) {
      var dropzone = this;
        v.dropZone = Ext.create('Ext.dd.DropZone', v.el, {

//      If the mouse is over a target node, return that node. This is
//      provided as the "target" parameter in all "onNodeXXXX" node event handling functions
        getTargetFromEvent: function(e) {
            return e.getTarget('.entitybag-target');
        },

//      On entry into a target node, highlight that node.
        onNodeEnter : function(target, dd, e, data){
            Ext.fly(target).addCls('entitybag-target-hover');
        },

//      On exit from a target node, unhighlight that node.
        onNodeOut : function(target, dd, e, data){
            Ext.fly(target).removeCls('entitybag-target-hover');
        },

//      While over a target node, return the default drop allowed class which
//      places a "tick" icon into the drag proxy.
        onNodeOver : function(target, dd, e, data){
            return Ext.dd.DropZone.prototype.dropAllowed;
        },

        onNodeDrop : function(target, dd, e, data){  
            dropzone.executeOnDrop(data);
            return true;
        }
      });
    }
  });
});
