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
PhotoData.prototype.addTagId = function(pTagId) {
	this.tagIdMap[pTagId] = this.tagIds.length;
	this.tagIds.push(pTagId);
};

/*--------------------------------------------------------------------------------------------*/
PhotoData.prototype.hasTag = function(pTagId) {
	return (this.tagIdMap[pTagId] >= 0);
};