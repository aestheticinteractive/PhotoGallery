/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />

var phoData = {
	idList: [],
	idMap: {},
	activePhotoId: 0
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function initPhotoView() {
	$(document).keyup(function(e) {
		console.log(e.which);

		switch ( e.which ) {
			case 27: //Escape key
				closePhoto();
				break;

			case 37: //Left Arrow key
				prevPhoto();
				break;

			case 39: //Right Arrow key
				nextPhoto();
				break;
		}
	});
}

/*--------------------------------------------------------------------------------------------*/
function registerPhoto(photoId, name, url, created, albumId) {
	phoData.idMap[photoId] = {
		index: phoData.idList.length,
		name: name,
		url: url,
		created: created,
		albumId: albumId
	};

	phoData.idList.push(photoId);
}

/*--------------------------------------------------------------------------------------------*/
function viewPhoto(photoId) {
	phoData.activePhotoId = photoId;
	var pho = phoData.idMap[photoId];
	var pv = $("#PhotoViewer").show();

	var img = pv.find('.photo');
	img.css('background-image', 'url('+pho.url+')');

	var det = pv.find('.details');
	det.html(pho.name+"<br/>"+pho.created);

	var prev = pv.find('#prev');
	prev.attr('disabled', (pho.index <= 0));
	
	var next = pv.find('#next');
	next.attr('disabled', (pho.index >= phoData.idList.length-1));
}

/*--------------------------------------------------------------------------------------------*/
function prevPhoto() {
	var i = phoData.idMap[phoData.activePhotoId].index;
	
	if ( i > 0 ) {
		viewPhoto(phoData.idList[i-1]);
	}
}

/*--------------------------------------------------------------------------------------------*/
function nextPhoto() {
	var i = phoData.idMap[phoData.activePhotoId].index;
	
	if ( i < phoData.idList.length-1 ) {
		viewPhoto(phoData.idList[i+1]);
	}
}

/*--------------------------------------------------------------------------------------------*/
function closePhoto() {
	$("#PhotoViewer").hide();
}