/// <reference path='~/Scripts/jquery-2.0.3-vsdoc.js' />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function isTouch() {
	return !!('ontouchstart' in window);
}

/*--------------------------------------------------------------------------------------------*/
function pagingLoad(pUrl) {
	var onSuccess = function(pData) {
		$('#DataPage .dataHold').stop().fadeIn(200);
		$('#DataPage').html(pData);
	};

	jQuery.get(pUrl, onSuccess);
	$('#DataPage .dataHold').stop().fadeTo(200, 0.333);
	$('#DataPage .wait').fadeIn(200);
}
