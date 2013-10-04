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

	$("#PhotoViewer .photo").click(null, nextPhoto);
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
}

/*--------------------------------------------------------------------------------------------*/
function prevPhoto() {
	var i = phoData.idMap[phoData.activePhotoId].index;
	
	if ( i <= 0 ) {
		i = phoData.idList.length;
	}

	viewPhoto(phoData.idList[i-1]);
}

/*--------------------------------------------------------------------------------------------*/
function nextPhoto() {
	var i = phoData.idMap[phoData.activePhotoId].index;
	
	if ( i >= phoData.idList.length-1 ) {
		i = -1;
	}

	viewPhoto(phoData.idList[i+1]);
}

/*--------------------------------------------------------------------------------------------*/
function closePhoto() {
	$("#PhotoViewer").hide();
}