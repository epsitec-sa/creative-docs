Ext.define('Epsitec.cresus.webcore.Tile', {
  extend: 'Ext.form.Panel',
  alternateClassName: ['Epsitec.Tile'],

  /* Properties */

  entityId: null,
  entityPanel: null,
  selected: false,
  selectedClass: 'selected-entity',

  /* Additional methods */

  select: function(selected) {
    if (this.selected !== selected) {
      this.selected = selected;
      if (this.selected) {
        this.addCls(this.selectedClass);
      }
      else {
        this.removeCls(this.selectedClass);
      }
    }
  },

  isSelected: function() {
    return this.selected;
  }
});
