/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function PhotoData(pPhotoId, pUrl, pCreated, pRatio) {
	this.photoId = pPhotoId;
	this.url = pUrl;
	this.created = pCreated;
	this.ratio = pRatio;

	this.tagList = [];
	this.idMap = {};
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
PhotoData.prototype.addTag = function(pTag) {
	this.idMap[pTag.Id] = this.tagList.length;
	this.tagList.push(pTag);
};

/*--------------------------------------------------------------------------------------------*/
PhotoData.prototype.hasTag = function(pTagId) {
	return (this.idMap[pTagId] >= 0);
};

/*--------------------------------------------------------------------------------------------*/
PhotoData.prototype.getTag = function(pTagId) {
	var id = this.idMap[pTagId];
	return this.tagList[id];
};