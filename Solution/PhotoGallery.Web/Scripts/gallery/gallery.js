/// <reference path='~/Scripts/jquery-2.0.3-vsdoc.js' />

var ENTER = 13;
var ESCAPE_KEY = 27;
var LEFT_ARROW = 37;
var UP_ARROW = 38;
var RIGHT_ARROW = 39;
var DOWN_ARROW = 40;
var T_KEY = 84;


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

/*--------------------------------------------------------------------------------------------*/
function trackPageview(pUrl) {
	_gaq.push(['_trackPageview', pUrl]);
}
