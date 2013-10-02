/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />

var fabPop;


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function openFabPop(pop) {
	fabPop = pop;
	checkFabPop();
}

/*--------------------------------------------------------------------------------------------*/
function checkFabPop() {
	if ( fabPop.closed ) {
		window.location.reload();
		return;
	}

	setTimeout(checkFabPop, 100);
}