function toggleMenuItem(item, selector) {
    if ($(item).parent().next(selector).hasClass('in')) { { $(selector).collapse('hide'); } } else { { $(selector).collapse('show'); } };
}

function refreshView() {
    RaiseXafCallback(globalCallbackControl, "", "XafParentWindowRefresh", "", false);
}