/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function isTouch() {
	return !!('ontouchstart' in window);
}

/*--------------------------------------------------------------------------------------------*/
function pagingLoad(url) {
	var onSuccess = function (data) {
		$('#DataPage .dataHold').stop().fadeIn(200);
		$('#DataPage').html(data);
	};

	jQuery.get(url, onSuccess);
	$('#DataPage .dataHold').stop().fadeTo(200, 0.333);
	$('#DataPage .wait').fadeIn(200);
}