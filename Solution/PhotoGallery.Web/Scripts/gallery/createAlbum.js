/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />

var caData;


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function submitCreateAlbum(albumUrl, photoUrl) {
	var form = $("#CreateAlbum");

	form.validate();

	if ( !form.valid() ) {
		return;
	}

	form.hide();

	caData = {};
	caData.photoUrl = photoUrl;
	caData.title = $('#Title').val();
	caData.imagesOrig = $('#Files').get(0).files;
	caData.imageCount = caData.imagesOrig.length;
	caData.uploadIndex = -1;
	caData.uploadCount = 0;
	caData.failCount = 0;

	var prog = $("#CreateAlbumProgress").show();
	prog.find("#Title").html('Creating album "' + caData.title + '":');
	onCreateAlbumProgress();

	var data = {
		Title: caData.title
	};

	var jqxhr = $.post(albumUrl, data, onAlbumTitleSuccess);
	jqxhr.fail(onAlbumTitleFail);
}

/*--------------------------------------------------------------------------------------------*/
function onAlbumTitleSuccess(data) {
	caData.albumId = Number(data);
	createNextImage();
}

/*--------------------------------------------------------------------------------------------*/
function onAlbumTitleFail(data, textStatus) {
	alert("onAlbumTitleFail: " + data + " // " + textStatus);
}


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function createNextImage() {
	onCreateAlbumProgress();
	
	var n = caData.imageCount;
	var i = ++caData.uploadIndex;

	if ( i >= n ) {
		onCreateAlbumComplete();
		return;
	}

	var file = caData.imagesOrig[i];

	if ( !file.type.match(/image.*/) ) {
		caData.failCount++;
		createNextImage();
		return;
	}

	var reader = new FileReader();
	reader.onload = function(e) { captureExif(file, e.target.result); };
	reader.readAsBinaryString(file);
}

/*--------------------------------------------------------------------------------------------*/
//Adapted from: http://stackoverflow.com/questions/10341685/
//  html-javascript-acces-exif-data-before-file-upload
function captureExif(file, binaryData) {
	var exifData = EXIF.readFromBinaryFile(new BinaryFile(binaryData));
	delete exifData.MakerNote;
	delete exifData.UserComment;
	//console.log("EXIF: "+JSON.stringify(exifData));

	var img = document.createElement("img");
	img.onload = function() { resizeAndUploadImage(file, img, exifData); };
	img.src = window.URL.createObjectURL(file);
}

/*--------------------------------------------------------------------------------------------*/
//Adapted from: http://hacks.mozilla.org/2011/01/how-to-develop-a-html5-image-uploader
function resizeAndUploadImage(file, img, exifData) {
	var max = 1024;
	var w = img.width;
	var h = img.height;

	if ( w > h && w > max ) {
		h *= max/w;
		w = max;
	}
	else if ( h > w && h > max ) {
		w *= max/h;
		h = max;
	}

	var canvas = document.createElement("canvas");
	var ctx = canvas.getContext("2d");
	canvas.width = w;
	canvas.height = h;
	ctx.drawImage(img, 0, 0, w, h);

	var data = {
		AlbumId: caData.albumId,
		Filename: file.name,
		ExifData: JSON.stringify(exifData),
		ImageData: canvas.toDataURL("image/jpeg", 1.0),
		LastImage: (caData.uploadIndex == caData.imageCount-1)
	};
	
	/*var img2 = document.createElement("img");
	img2.src = data.ImageData;
	$("#CreateAlbumProgress").append("<hr/>");
	$("#CreateAlbumProgress").append(img2);
	$("#CreateAlbumProgress").append("<br/>");
	$("#CreateAlbumProgress").append(exifData);*/
	$.post(caData.photoUrl, data, onAlbumImageSuccess);
}

/*--------------------------------------------------------------------------------------------*/
function onAlbumImageSuccess(data) {
	//alert("onAlbumImageSuccess: "+data+" // "+textStatus);
	++caData.uploadCount;
	createNextImage();
}

/*--------------------------------------------------------------------------------------------*/
function onAlbumImageFail(data, textStatus) {
	alert("onAlbumImageFail: "+data+" // "+textStatus);
	caData.failCount++;
	createNextImage();
}


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function onCreateAlbumProgress() {
	var n = caData.imageCount;
	var u = caData.uploadCount;
	var f = caData.failCount;

	var s = 'Uploading ' + n + ' image(s)...<br/>' +
		' &bull; Complete: ' + u + ' (' + Math.round(u / n * 100) + '%)<br/>' +
		' &bull; Failures: ' + f + ' (' + Math.round(f / n * 100) + '%)<br/>';

	$("#CreateAlbumProgress").find("#Info").html(s);
}

/*--------------------------------------------------------------------------------------------*/
function onCreateAlbumComplete() {
	caData = null;
}