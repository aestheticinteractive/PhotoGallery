/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
$(document).ready(function() {
});

/*--------------------------------------------------------------------------------------------* /
$(window).resize(function() {
});*/


////////////////////////////////////////////////////////////////////////////////////////////////
var createAlbumObj;

/*--------------------------------------------------------------------------------------------*/
function submitCreateAlbum(albumUrl) {
	var form = $("#CreateAlbum");
			
	form.validate();

	if ( !form.valid() ) {
		return;
	}

	form.hide();

	createAlbumObj = {};
	createAlbumObj.title = $('#Title').val();
	createAlbumObj.imagesOrig = $('#Files').get(0).files;
	createAlbumObj.imageCount = createAlbumObj.imagesOrig.length;
	createAlbumObj.uploadCount = 0;
	createAlbumObj.failCount = 0;

	var prog = $("#CreateAlbumProgress").show();
	prog.find("#Title").html('Creating album "'+createAlbumObj.title+'":');
	updateProgress();

	var data = {
		Title: createAlbumObj.title
	};

	var jqxhr = $.post(albumUrl, data, onAlbumTitleSuccess);
	jqxhr.fail(onAlbumTitleFail);
}

/*--------------------------------------------------------------------------------------------*/
function onAlbumTitleSuccess(data, textStatus, jqXhr) {
	//alert("SUCCESS: "+data+" // "+textStatus+" // "+jqXhr);
}

/*--------------------------------------------------------------------------------------------*/
function onAlbumTitleFail(data, textStatus, jqXhr) {
	alert("FAIL: "+data+" // "+textStatus+" // "+jqXhr);
}

/*--------------------------------------------------------------------------------------------*/
function updateProgress() {
	var n = createAlbumObj.imageCount;
	var u = createAlbumObj.uploadCount;
	var f = createAlbumObj.failCount;

	var s = 'Uploading '+n+' image(s)...<br/>'+
		' &bull; Complete: '+u+' ('+Math.round(u/n*100)+'%)<br/>'+
		' &bull; Failures: '+f+' ('+Math.round(f/n*100)+'%)<br/>';

	$("#CreateAlbumProgress").find("#Info").html(s);
}