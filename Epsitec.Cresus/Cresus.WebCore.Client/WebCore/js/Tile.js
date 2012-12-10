Ext.require([
  'Epsitec.cresus.webcore.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.Tile', {
    extend: 'Ext.form.Panel',
    alternateClassName: ['Epsitec.Tile'],

    /* Config */

    minHeight: 50,
    border: false,
    style: {
      borderRight: '1px solid #99BCE8',
      borderBottom: '1px solid #99BCE8',
      borderLeft: '1px solid #99BCE8'
    },
    bodyCls: 'tile',

    /* Properties */

    column: null,
    selected: false,

    /* Constructor */

    constructor: function(options) {
      this.callParent([options]);
      return this;
    },

    /* Additional methods */

    setSelected: function(selected) {
      this.selected = selected;
    },

    isSelected: function() {
      return this.selected;
    }
  });
});
