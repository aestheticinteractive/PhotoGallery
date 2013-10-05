/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function isTouch() {
	console.log("IS TOUCH: " + !!('ontouchstart' in window));
	return !!('ontouchstart' in window);
}