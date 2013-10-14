/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />

var phoData = {
	idList: [],
	idMap: {},
	activePhotoId: 0
};

var tagData = {
	tagMode: false,
	localUrl: null,
	fabUrl: null,
	timer: 0,
	name: null,
	list: []
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function initPhotoView() {
	$(document).keyup(function(e) {
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

	$('body').prepend($('#PhotoViewerPadding')).prepend($('#PhotoViewer'));
	phoData.origBg = $('body').css('background-color');
	phoData.origOy = $('body').css('overflow-y');

	$('#PhotoViewer .photo').click(null, onPhotoClick);
	closePhoto();
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
	
	$("#PhotoViewer").show();
	$('#PhotoViewerPadding').show();
	$("#Site").hide();

	$('#PhotoViewer .photo').css('background-image', 'url('+pho.url+')');
	$('#PhotoViewer .details').html(pho.name + "<br/>" + pho.created);
	$('body').css('background-color', '#000').css('overflow-y', 'hidden');

	_gaq.push(['_trackPageview', '/Photos/'+photoId]);

	// preload next image

	photoId = phoData.idList[nextPhotoIndex()];
	pho = phoData.idMap[photoId];
	var img = new Image();
	img.src = pho.url;
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
	viewPhoto(phoData.idList[nextPhotoIndex()]);
}

/*--------------------------------------------------------------------------------------------*/
function nextPhotoIndex() {
	var i = phoData.idMap[phoData.activePhotoId].index;

	if ( i >= phoData.idList.length-1 ) {
		i = -1;
	}

	return i+1;
}

/*--------------------------------------------------------------------------------------------*/
function closePhoto() {
	$('#PhotoViewer').hide();
	$('#PhotoViewerPadding').hide();
	$('#Site').show();
	$('body').css('background-color', phoData.origBg).css('overflow-y', phoData.origOy);
}

/*--------------------------------------------------------------------------------------------*/
function onPhotoClick() {
	if ( !tagData.tagMode ) {
		nextPhoto();
		return;
	}

	showTagDialog();
}


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function initTagSearch(localUrl, fabUrl) {
	tagData.localUrl = localUrl;
	tagData.fabUrl = fabUrl;
	
	$('#TagSearch').keyup(function () {
		clearTimeout(tagData.timer);
		tagData.timer = setTimeout(onSearchKeyup, 250);
	});
}

/*--------------------------------------------------------------------------------------------*/
function toggleTagMode() {
	tagData.tagMode = !tagData.tagMode;
}

/*--------------------------------------------------------------------------------------------*/
function showTagDialog() {

}

/*--------------------------------------------------------------------------------------------*/
function onSearchKeyup() {
	$('#TagSearchRows').hide();

	var asyncName = tagData.name = $('#TagSearch').val();
	tagData.list = [];

	jQuery.post(tagData.localUrl+asyncName, null, function(localData) {
		if (tagData.name != asyncName) {
			return;
		}

		tagData.list = tagData.list.concat(localData);
		onSearchData();
	});

	jQuery.post(tagData.fabUrl+asyncName, null, function(fabData) {
		if ( tagData.name != asyncName ) {
			return;
		}

		tagData.list = tagData.list.concat(fabData);
		onSearchData();
	});
}

/*--------------------------------------------------------------------------------------------*/
function onSearchData() {
	tagData.list.sort(function(a, b) {
		var name = tagData.name.toLowerCase();
		var an = a.Name.toLowerCase();
		var bn = b.Name.toLowerCase();

		if ( an == name ) {
			return -1;
		}

		if ( bn == name ) {
			return 1;
		}

		if ( an == bn ) {
			var ad = a.Disamb.toLowerCase();
			var bd = b.Disamb.toLowerCase();
			return (ad > bd);
		}

		return (an > bn);
	});

	var rows = "";
	var n = tagData.list.length;
	var dup = {};

	for ( var i = 0 ; i < n ; ++i ) {
		var item = tagData.list[i];

		if ( dup[item.ArtifactId] ) {
			continue;
		}

		dup[item.ArtifactId] = true;
		rows += '<tr><td title="['+item.ArtifactId+']'+(item.Note ? ' '+item.Note : '')+'">' +
			item.Name +(item.Disamb ? '<br/><span class="disamb">'+item.Disamb+'</span>' : '')+
			'</td></tr>';
	}

	$('#TagSearchRows').html(rows).show();
}