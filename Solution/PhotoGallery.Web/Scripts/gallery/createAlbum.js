/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function CreateAlbum() {
};

/*--------------------------------------------------------------------------------------------*/
CreateAlbum.prototype.onFormSubmit = function(pAlbumUrl, pPhotoUrl, pEditAlbumId) {
	var form = $("#CreateAlbum");
	form.validate();

	if ( !form.valid() ) {
		return;
	}

	form.hide();

	this.photoUrl = pPhotoUrl;
	this.title = $('#Title').val();
	this.imagesOrig = $('#Files').get(0).files;
	this.imageCount = this.imagesOrig.length;
	this.uploadIndex = -1;
	this.uploadCount = 0;
	this.failCount = 0;

	var prog = $("#CreateAlbumProgress").show();
	var action = (pEditAlbumId == null ? "Creating" : "Editing");
	prog.find("#Title").html(action+' album "' + this.title + '":');
	this.onCreateAlbumProgress();

	var data = {
		Title: this.title
	};

	if ( pEditAlbumId == null ) {
		var successClosure = function(pScope) {
			return function(pData) {
				pScope.onAlbumTitleSuccess(pData);
			};
		};

		var failClosure = function(pScope) {
			return function(pData, pStatus) {
				pScope.onAlbumTitleFail(pData, pStatus);
			};
		};

		var jqxhr = $.post(pAlbumUrl, data, successClosure(this));
		jqxhr.fail(failClosure(this));
	}
	else {
		this.albumId = pEditAlbumId;
		this.createNextImage();
	}
};

/*--------------------------------------------------------------------------------------------*/
CreateAlbum.prototype.onAlbumTitleSuccess = function(pData) {
	this.albumId = Number(pData);
	this.createNextImage();
};

/*--------------------------------------------------------------------------------------------*/
CreateAlbum.prototype.onAlbumTitleFail = function(pData, pStatus) {
	alert("onAlbumTitleFail: " + pData + " // " + pStatus);
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
CreateAlbum.prototype.createNextImage = function() {
	this.onCreateAlbumProgress();
	
	var n = this.imageCount;
	var i = ++this.uploadIndex;

	if ( i >= n ) {
		this.onCreateAlbumComplete();
		return;
	}

	var file = this.imagesOrig[i];

	if ( !file.type.match(/image.*/) ) {
		this.failCount++;
		this.createNextImage();
		return;
	}

	var exifClosure = function(pScope, pFile) {
		return function(pEvent) {
			pScope.captureExif(pFile, pEvent.target.result);
		};
	};

	var reader = new FileReader();
	reader.onload = exifClosure(this, file);
	reader.readAsBinaryString(file);
};

/*--------------------------------------------------------------------------------------------*/
//Adapted from: http://stackoverflow.com/questions/10341685/
//  html-javascript-acces-exif-data-before-file-upload
CreateAlbum.prototype.captureExif = function(pFile, pBinaryData) {
	var exifData = EXIF.readFromBinaryFile(new BinaryFile(pBinaryData));
	delete exifData.MakerNote;
	delete exifData.UserComment;
	//console.log("EXIF: "+JSON.stringify(exifData));
	
	var uploadClosure = function(pScope, pClosureFile, pImg, pExif) {
		return function() {
			pScope.resizeAndUploadImage(pClosureFile, pImg, pExif);
		};
	};

	var img = document.createElement("img");
	img.onload = uploadClosure(this, pFile, img, exifData);
	img.src = window.URL.createObjectURL(pFile);
};

/*--------------------------------------------------------------------------------------------*/
//Adapted from: http://hacks.mozilla.org/2011/01/how-to-develop-a-html5-image-uploader
CreateAlbum.prototype.resizeAndUploadImage = function(pFile, pImg, pExifData) {
	var max = 1024;
	var w = pImg.width;
	var h = pImg.height;

	if ( w > h && w > max ) {
		h *= max/w;
		w = max;
	}
	else if ( h > w && h > max ) {
		w *= max/h;
		h = max;
	}

	var canvas = document.createElement("canvas");
	canvas.width = w;
	canvas.height = h;

	var ctx = canvas.getContext("2d");
	ctx.drawImage(pImg, 0, 0, w, h);

	var data = {
		AlbumId: this.albumId,
		Filename: pFile.name,
		ExifData: JSON.stringify(pExifData),
		ImageData: canvas.toDataURL("image/jpeg", 1.0),
		LastImage: (this.uploadIndex == this.imageCount-1)
	};
	
	var successClosure = function(pScope) {
		return function(pData) {
			pScope.onAlbumImageSuccess(pData);
		};
	};
	
	var failClosure = function(pScope) {
		return function(pData, pStatus) {
			pScope.onAlbumImageFail(pData, pStatus);
		};
	};

	var jqxhr = $.post(this.photoUrl, data, successClosure(this));
	jqxhr.fail(failClosure(this));
};

/*--------------------------------------------------------------------------------------------*/
CreateAlbum.prototype.onAlbumImageSuccess = function(pData) {
	//alert("onAlbumImageSuccess: "+pData+" // "+textStatus);
	++this.uploadCount;
	this.createNextImage();
};

/*--------------------------------------------------------------------------------------------*/
CreateAlbum.prototype.onAlbumImageFail = function(pData, pStatus) {
	alert("onAlbumImageFail: "+pData+" // "+pStatus);
	this.failCount++;
	this.createNextImage();
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
CreateAlbum.prototype.onCreateAlbumProgress = function() {
	var n = this.imageCount;
	var u = this.uploadCount;
	var f = this.failCount;

	var s = 'Uploading ' + n + ' image(s)...<br/>' +
		' &bull; Complete: ' + u + ' (' + Math.round(u / n * 100) + '%)<br/>' +
		' &bull; Failures: ' + f + ' (' + Math.round(f / n * 100) + '%)<br/>';

	$("#CreateAlbumProgress").find("#Info").html(s);
};

/*--------------------------------------------------------------------------------------------*/
CreateAlbum.prototype.onCreateAlbumComplete = function() {
};