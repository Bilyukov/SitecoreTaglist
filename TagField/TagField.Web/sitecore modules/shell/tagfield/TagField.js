scContentEditor.prototype.onTagFieldUpdated = function () {

    var valueId = '#' + jQuery(this).attr('id').replace("_Select", "_Value");

    var selectedValues = jQuery(this).val();

    var valueStr = selectedValues ? selectedValues.join('|') : "";

    jQuery(valueId).val(valueStr);

    event.stopPropagation();
}

scContentEditor.prototype.onDropDown = function () {
    jQuery(this).next().find('.chosen-drop').css('position', 'relative');
}

scContentEditor.prototype.onHideDropDown = function () {
    jQuery(this).next().find('.chosen-drop').css('position', 'absolute');
}