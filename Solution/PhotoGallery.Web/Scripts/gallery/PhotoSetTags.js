/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
function PhotoSetTags(pPhotoSet) {
	this.photoSet = pPhotoSet;
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetTags.prototype.setTags = function(pTags) {
	this.tagList = [];
	this.tagMap = {};
	this.tagCount = 0;
	this.tagMaxCount = 0;

	for ( var i = 0 ; i < pTags.length ; ++i ) {
		var t = pTags[i];
		var count = t.PhotoIds.length;

		this.tagMap[t.Id] = this.tagList.length;
		this.tagList.push(t);
		
		this.tagCount += count;
		this.tagMaxCount = Math.max(count, this.tagMaxCount);

		for ( var j = 0 ; j < count ; ++j ) {
			var phoId = t.PhotoIds[j];
			var d = this.photoSet.getData(phoId);

			if ( d ) {
				d.addTagId(t.Id);
			}
		}
	}
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetTags.prototype.getTag = function(pTagId) {
	var index = this.tagMap[pTagId];
	return this.tagList[index];
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetTags.prototype.getTags = function() {
	return this.tagList;
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetTags.prototype.isPersonTag = function(pTagId) {
	var t = this.getTag(pTagId);
	return (t.Disamb == "female person" || t.Disamb == "male person");
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetTags.prototype.getTagWeight = function(pTagId) {
	var t = this.getTag(pTagId);
	return t.PhotoIds.length/this.tagMaxCount;
};
