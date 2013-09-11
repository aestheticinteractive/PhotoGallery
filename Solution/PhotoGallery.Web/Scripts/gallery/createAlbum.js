/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />

var createAlbumObj;


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function submitCreateAlbum(albumUrl, photoUrl) {
	var form = $("#CreateAlbum");

	form.validate();

	if ( !form.valid() ) {
		return;
	}

	form.hide();

	createAlbumObj = {};
	createAlbumObj.photoUrl = photoUrl;
	createAlbumObj.title = $('#Title').val();
	createAlbumObj.imagesOrig = $('#Files').get(0).files;
	createAlbumObj.imageCount = createAlbumObj.imagesOrig.length;
	createAlbumObj.uploadIndex = -1;
	createAlbumObj.uploadCount = 0;
	createAlbumObj.failCount = 0;

	var prog = $("#CreateAlbumProgress").show();
	prog.find("#Title").html('Creating album "' + createAlbumObj.title + '":');
	onCreateAlbumProgress();

	var data = {
		Title: createAlbumObj.title
	};

	var jqxhr = $.post(albumUrl, data, onAlbumTitleSuccess);
	jqxhr.fail(onAlbumTitleFail);
}

/*--------------------------------------------------------------------------------------------*/
function onAlbumTitleSuccess(data) {
	createAlbumObj.albumId = Number(data);
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
	
	var n = createAlbumObj.imageCount;
	var i = ++createAlbumObj.uploadIndex;

	if ( i >= n ) {
		onCreateAlbumComplete();
		return;
	}

	var file = createAlbumObj.imagesOrig[i];

	if ( !file.type.match(/image.*/) ) {
		createAlbumObj.failCount++;
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
	console.log("EXIF: "+JSON.stringify(exifData));

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
		AlbumId: createAlbumObj.albumId,
		Filename: file.value,
		ExifData: JSON.stringify(exifData),
		ImageData: canvas.toDataURL("image/jpeg", 1.0)
	};
	
	var img2 = document.createElement("img");
	img2.src = data.ImageData;
	$("#CreateAlbumProgress").append("<hr/>");
	$("#CreateAlbumProgress").append(img2);
	$("#CreateAlbumProgress").append("<br/>");
	$("#CreateAlbumProgress").append(exifData);
	$.post(createAlbumObj.photoUrl, data, onAlbumImageSuccess);
}

/*--------------------------------------------------------------------------------------------*/
function onAlbumImageSuccess(data) {
	//alert("onAlbumImageSuccess: "+data+" // "+textStatus);
	++createAlbumObj.uploadCount;
	createNextImage();
}

/*--------------------------------------------------------------------------------------------*/
function onAlbumImageFail(data, textStatus) {
	alert("onAlbumImageFail: "+data+" // "+textStatus);
	createAlbumObj.failCount++;
	createNextImage();
}


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function onCreateAlbumProgress() {
	var n = createAlbumObj.imageCount;
	var u = createAlbumObj.uploadCount;
	var f = createAlbumObj.failCount;

	var s = 'Uploading ' + n + ' image(s)...<br/>' +
		' &bull; Complete: ' + u + ' (' + Math.round(u / n * 100) + '%)<br/>' +
		' &bull; Failures: ' + f + ' (' + Math.round(f / n * 100) + '%)<br/>';

	$("#CreateAlbumProgress").find("#Info").html(s);
}

/*--------------------------------------------------------------------------------------------*/
function onCreateAlbumComplete() {
	createAlbumObj = null;
}