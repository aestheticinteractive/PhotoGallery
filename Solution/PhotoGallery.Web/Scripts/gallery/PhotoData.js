/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function PhotoData(pPhotoId, pUrl, pCreated, pRatio) {
	this.photoId = pPhotoId;
	this.url = pUrl;
	this.created = pCreated;
	this.ratio = pRatio;

	this.tagIds = [];
	this.tagIdMap = {};
};

/*--------------------------------------------------------------------------------------------*/
PhotoData.prototype.setMeta = function(pExposure, pFNumber, pFocalLen, pIsoSpeed, pFlash) {
	this.exposure = pExposure;
	this.fNumber = pFNumber;
	this.focalLen = pFocalLen;
	this.isoSpeed = pIsoSpeed;
	this.flash = pFlash;
};

/*--------------------------------------------------------------------------------------------*/
PhotoData.prototype.setTagIds = function(pTagIds) {
	this.tagIds = (pTagIds ? pTagIds : []);
	this.tagIdMap = {};

	for ( var i = 0 ; i < this.tagIds.length ; ++i ) {
		var id = this.tagIds[i];
		this.tagIdMap[id] = true;
	}
};

/*--------------------------------------------------------------------------------------------*/
PhotoData.prototype.hasTag = function(pTagId) {
	return this.tagIdMap[pTagId];
};