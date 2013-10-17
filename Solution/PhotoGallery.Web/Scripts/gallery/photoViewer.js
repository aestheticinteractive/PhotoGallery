/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />

var ENTER = 13;
var ESCAPE_KEY = 27;
var LEFT_ARROW = 37;
var UP_ARROW = 38;
var RIGHT_ARROW = 39;
var DOWN_ARROW = 40;
var T_KEY = 84;

var phoData = {
	idList: [],
	idMap: {},
	activePhotoId: 0
};

var tagData = {
	tagMode: false,
	tagUrl: null,
	addUrl: null,
	timer: 0,
	name: null,
	list: [],
	idToIndex: {},
	activeId: null,
	spotRelX: 0,
	spotRelY: 0,
	showSearch: true,
	cancelClick: false
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function initPhotoView() {
	$(document).keyup(onKeyUp);
	$(window).resize(resizePhoto);
	$(window).resize(resizeTagLayer);

	$('body').prepend($('#PhotoViewerPadding')).prepend($('#PhotoViewer'));
	phoData.origBg = $('body').css('background-color');
	phoData.origOy = $('body').css('overflow-y');

	$('#PhotoViewer .photo').click(null, nextPhoto);
	closePhoto();
}

/*--------------------------------------------------------------------------------------------*/
function onKeyUp(event) {
	if ( !$('#PhotoViewer').is(':visible') ) {
		return;
	}

	var key = event.which;

	if ( !tagData.tagMode ) {
		switch ( key ) {
			case T_KEY: toggleTagMode(); break;
			case ESCAPE_KEY: closePhoto(); break;
			case LEFT_ARROW: prevPhoto(); break;
			case RIGHT_ARROW: nextPhoto(); break;
		}

		return;
	}

	if ( tagData.showSearch ) {
		switch ( key ) {
			case ENTER: onTagClick(tagData.activeId, true); break;
			case ESCAPE_KEY: cancelTagSearch(true); break;
			case UP_ARROW: onTagSearchHighlightKey(-1); break;
			case DOWN_ARROW: onTagSearchHighlightKey(1); break;
		}

		return;
	}

	switch ( key ) {
		case ESCAPE_KEY: toggleTagMode(); break;
	}
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
function initTagSearch(tagUrl, addUrl) {
	tagData.tagUrl = tagUrl;
	tagData.addUrl = addUrl;
	
	$('#TagSearch').keyup(function() {
		clearTimeout(tagData.timer);
		tagData.timer = setTimeout(onSearchKeyup, 500);
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
	cancelTagSearch(true);
}

/*--------------------------------------------------------------------------------------------*/
function cancelTagSearch(fromEscKey) {
	tagData.cancelClick = (fromEscKey != true);
	tagData.showSearch = false;
	tagData.list = [];
	tagData.idToIndex = {};
	tagData.activeId = null;

	$('#TagLayer .spot').hide();
	$('#TagLayer .inputPanel').hide();
	$('#TagSearch').val('');
	onSearchKeyup();
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
	var asyncName = $('#TagSearch').val();

	if ( asyncName == tagData.name ) {
		return;
	}

	tagData.name = asyncName;
	tagData.list = [];
	tagData.activeId = null;

	if ( asyncName == "" ) {
		$('#TagSearchRows').html('');
		return;
	}

	$('#TagSearchRows').html('<tr><td><span class="disamb">Loading...</span></tr></td>');
	findTagsAsync(asyncName, true);
}

/*--------------------------------------------------------------------------------------------*/
function findTagsAsync(asyncName, first) {
	var url = tagData.tagUrl+asyncName+(first ? '?first=true' : '');

	jQuery.post(url, null, function(data) {
		if ( tagData.name != asyncName ) {
			return;
		}

		tagData.list = tagData.list.concat(data);
		onSearchData();
		
		if ( data && data.length > 0 ) {
			findTagsAsync(asyncName, false);
		}
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
	tagData.idToIndex = {};

	for ( var i = 0 ; i < n ; ++i ) {
		var item = tagData.list[i];

		if ( tagData.idToIndex[item.ArtifactId] ) {
			continue;
		}

		tagData.idToIndex[item.ArtifactId] = i;
		rows += '<tr><td data-id="'+item.ArtifactId+'" '+
			'onclick="onTagClick('+item.ArtifactId+'); return false;" '+
			'title="['+item.ArtifactId+']'+(item.Note ? ' '+item.Note : '')+'">' +
			item.Name +(item.Disamb ? '<br/><span class="disamb">'+item.Disamb+'</span>' : '')+
			'</td></tr>';
	}

	$('#TagSearchRows').html(rows);

	$('#TagSearchRows td').mouseenter(function () {
		setTagSearchHighlight($(this).attr('data-id'));
	});

	$('#TagLayer .resultScroll').scrollTop(0);
	setTagSearchHighlight(tagData.activeId);
}

/*--------------------------------------------------------------------------------------------*/
function onTagSearchHighlightKey(pDir) {
	if ( tagData.list.length == 0 ) {
		return;
	}

	var tagI = 0;

	if ( tagData.activeId == null ) {
		tagData.activeId = tagData.list[0].ArtifactId;
	}
	else {
		tagI = tagData.idToIndex[tagData.activeId]+pDir;
	}

	if ( tagI < 0 || tagI >= tagData.list.length ) {
		return;
	}

	tagData.activeId = tagData.list[tagI].ArtifactId;
	setTagSearchHighlight(tagData.activeId, true);
}

/*--------------------------------------------------------------------------------------------*/
function setTagSearchHighlight(pArtId, pFromKey) {
	if ( pArtId == null ) {
		return;
	}

	tagData.activeId = pArtId;
	var activeTd = null;

	$('#TagSearchRows td').each(function() {
		var td = $(this);
		var act = (td.attr('data-id') == pArtId);
		td.attr('class', (act ? 'active' : ''));
		td.parent().attr('class', (act ? 'active' : ''));

		if ( act ) {
			activeTd = td;
		}
	});

	if ( activeTd == null || !pFromKey ) {
		return;
	}

	var rs = $('#TagLayer .resultScroll');
	var rsH = rs.height();
	var rsS = rs.scrollTop();

	var tr = activeTd.parent();
	var tdH = tr.height();
	var tdY = tr.offset().top-rs.offset().top;

	if ( tdY < 0 ) {
		rs.scrollTop(rsS+tdY);
	}
	else if ( tdY+tdH > rsH ) {
		rs.scrollTop(rsS+tdY-rsH+tdH);
	}
}

/*--------------------------------------------------------------------------------------------*/
function onTagClick(pArtId, pFromKey) {
	if ( pArtId == null ) {
		return;
	}

	cancelTagSearch(pFromKey);

	var data = {
		PhotoId: phoData.activePhotoId,
		ArtifactId: pArtId+"",
		PosX: tagData.spotRelX,
		PosY: tagData.spotRelY
	};
	
	$.post(tagData.addUrl, data, function() {
		console.log("Tag Added: pho="+phoData.activePhotoId+", art="+pArtId+
			" at (x="+tagData.spotRelX+", y="+tagData.spotRelY+")");
	});
}