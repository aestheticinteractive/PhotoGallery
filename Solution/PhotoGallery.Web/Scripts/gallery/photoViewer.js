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
	list: [],
	spotRelX: 0,
	spotRelY: 0,
	showSearch: true,
	cancelClick: false
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function initPhotoView() {
	$(document).keyup(function(e) {
		switch ( e.which ) {
			case 27: //Escape key
				onEscapeKey();
				break;

			case 37: //Left Arrow key
				prevPhoto();
				break;

			case 39: //Right Arrow key
				nextPhoto();
				break;
		}
	});

	$(window).resize(resizePhoto);
	$(window).resize(resizeTagLayer);

	$('body').prepend($('#PhotoViewerPadding')).prepend($('#PhotoViewer'));
	phoData.origBg = $('body').css('background-color');
	phoData.origOy = $('body').css('overflow-y');

	$('#PhotoViewer .photo').click(null, nextPhoto);
	closePhoto();
}

/*--------------------------------------------------------------------------------------------*/
function onEscapeKey() {
	if ( !tagData.tagMode ) {
		closePhoto();
		return;
	}

	if ( tagData.showSearch ) {
		cancelTagSearch(true);
		return;
	}

	toggleTagMode();
}


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function registerPhoto(photoId, url, created, ratio) {
	phoData.idMap[photoId] = {
		index: phoData.idList.length,
		url: url,
		created: created,
		ratio: ratio
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

	$('#PhotoViewer .photo').attr('src', pho.url);
	resizePhoto();

	$('#PhotoViewer .details').html(pho.created.replace(' ', '<br/>'));
	$('body').css('background-color', '#000').css('overflow-y', 'hidden');

	_gaq.push(['_trackPageview', '/Photos/'+photoId]);

	// preload next image

	photoId = phoData.idList[nextPhotoIndex()];
	pho = phoData.idMap[photoId];
	var img = new Image();
	img.src = pho.url;
}

/*--------------------------------------------------------------------------------------------*/
function resizePhoto() {
	var pho = phoData.idMap[phoData.activePhotoId];
	var img = $('#PhotoViewer .photo');
	var pv = $("#PhotoViewer");
	var pvW = pv.width();
	var pvH = pv.height();
	var pvRatio = pvW/pvH;

	if ( pvRatio > pho.ratio ) {
		var imgW = pho.ratio*pvH;
		img.css('height', '100%').css('width', 'auto').css('margin', '0 0 0 '+(pvW-imgW)/2+'px');
	}
	else {
		var imgH = pvW/pho.ratio;
		img.css('width', '100%').css('height', 'auto').css('margin', (pvH-imgH)/2+'px 0 0 0');
	}
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


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function initTagSearch(localUrl, fabUrl) {
	tagData.localUrl = localUrl;
	tagData.fabUrl = fabUrl;
	
	$('#TagSearch').keyup(function () {
		clearTimeout(tagData.timer);
		tagData.timer = setTimeout(onSearchKeyup, 250);
	});

	$('#TagLayer').click(null, onTagLayerClick);
	$('#TagSearchCancel').click(null, cancelTagSearch);
	$('#TagModeBar').hide();
}

/*--------------------------------------------------------------------------------------------*/
function toggleTagMode() {
	tagData.tagMode = !tagData.tagMode;
	$('#TagLayer').toggle();
	$('#PhotoViewBar').toggle();
	$('#TagModeBar').toggle();
	$('#TagSearch').val('');
	$('#TagLayer .spot').hide();
	$('#TagLayer .inputPanel').hide();
	tagData.showSearch = false;
}

/*--------------------------------------------------------------------------------------------*/
function cancelTagSearch(fromEscKey) {
	tagData.cancelClick = (fromEscKey != true);
	tagData.showSearch = false;
	$('#TagLayer .spot').hide();
	$('#TagLayer .inputPanel').hide();
}

/*--------------------------------------------------------------------------------------------*/
function onTagLayerClick(event) {
	if ( tagData.showSearch || tagData.cancelClick ) {
		tagData.cancelClick = false;
		return;
	}

	$('#TagLayer .spot').show();
	$('#TagLayer .inputPanel').show();
	$('#TagSearch').focus();
	tagData.showSearch = true;

	var pho = $('#PhotoViewer .photo');
	var pos = pho.offset();
	var x = Math.max(pos.left, Math.min(pos.left+pho.width(), event.pageX));
	var y = Math.max(pos.top, Math.min(pos.top+pho.height(), event.pageY));
	tagData.spotRelX = (x-pos.left)/pho.width();
	tagData.spotRelY = (y-pos.top)/pho.height();
	//console.log(tagData.spotRelX+" / "+tagData.spotRelY);

	resizeTagLayer();
}

/*--------------------------------------------------------------------------------------------*/
function resizeTagLayer() {
	if ( !tagData.showSearch ) {
		return;
	}

	var spot = $('#TagLayer .spot');
	var panel = $('#TagLayer .inputPanel');
	var pho = $('#PhotoViewer .photo');
	var pos = pho.offset();
	var winW = $(window).width();
	var winH = $(window).height();
	var x = pos.left+pho.width()*tagData.spotRelX;
	var y = pos.top+pho.height()*tagData.spotRelY;
	var boxW = 300;
	var boxH = 300;
	var boxX = x;
	var boxY = y+40;
	var pad = 10;

	if ( boxX-boxW/2-pad < 0 ) {
		boxX = boxW/2+pad;
	}
	else if ( boxX+boxW/2+pad > winW ) {
		boxX = winW-boxW/2-pad;
	}

	if ( boxY+boxH+pad > winH ) {
		boxY = winH-boxH-pad;
	}

	spot.css('left', x+'px').css('top', y+'px');
	panel.css('left', boxX+'px').css('top', boxY+'px');
}

/*--------------------------------------------------------------------------------------------*/
function onSearchKeyup() {
	var asyncName = tagData.name = $('#TagSearch').val();
	tagData.list = [];

	if ( asyncName == "" ) {
		$('#TagSearchRows').html('');
		return;
	}

	$('#TagSearchRows').html('<tr><td><span class="disamb">Loading...</span></tr></td>');

	/*jQuery.post(tagData.localUrl+asyncName, null, function(localData) {
		if (tagData.name != asyncName) {
			return;
		}

		tagData.list = tagData.list.concat(localData);
		onSearchData();
	});*/

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

	$('#TagSearchRows').html(rows);
}