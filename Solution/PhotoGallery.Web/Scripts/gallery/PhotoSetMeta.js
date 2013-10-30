/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
function PhotoSetMeta(pPhotoSet, pMetaUrl) {
	this.photoSet = pPhotoSet;
	this.metaUrl = pMetaUrl;
	this.events = new EventDispatcher('PhotoSetMeta');
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.loadData = function() {
	var onData = function(pScope) {
		return function(pData) {
			pScope.setMetas(pData);
		};
	};

	jQuery.post(this.metaUrl, null, onData(this));
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.setMetas = function(pMetas) {
	for ( var i = 0 ; i < pMetas.length ; ++i ) {
		var pm = pMetas[i];
		var d = this.photoSet.getData(pm.PhotoId);

		if ( d ) {
			d.setMeta(pm.Exposure, pm.FNumber, pm.FocalLen, pm.IsoSpeed, pm.UsesFlash);
		}
	}

	this.events.send('dataLoaded');
};


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.getValues = function(pProp) {
	var cl = this.photoSet.currentList();
	var vals = [];

	for ( var i = 0 ; i < cl.length ; ++i ) {
		var d = cl[i];
		var val = pProp(d);

		if ( val != null ) {
			vals.push(val);
		}
	}

	return vals;
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.getAvg = function(pValues) {
	var sum = 0.0;
	var count = 0;

	for ( var i = 0 ; i < pValues.length ; ++i ) {
		sum += pValues[i];
		count++;
	}

	return (count == 0 ? null : sum/count);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.getBuckets = function(pValues, pCount, pHistMin, pHistMax) {
	var min = pHistMin;
	var max = pHistMax;

	if ( min == null ) {
		min = 999999999999;
		max = -999999999999;

		for ( var i = 0 ; i < pValues.length ; ++i ) {
			min = Math.min(pValues[i], min);
			max = Math.max(pValues[i], max);
		}
	}

	if ( min == max ) {
		min--;
		max++;
	}

	var size = (max-min)/pCount;
	var buckets = [];

	for ( i = 0 ; i < pCount ; ++i ) {
		buckets.push({
			min: min+size*i,
			max: min+size*(i+1),
			count: 0
		});
	}
	
	for ( i = 0 ; i < pValues.length ; ++i ) {
		var val = pValues[i];
		var bi = Math.floor((val-min)/size);
		buckets[Math.min(bi, pCount-1)].count++;
	}

	return buckets;
};


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.getFNumberStr = function(pVal) {
	return (pVal == null ? '' : 'f/'+pVal.toFixed(1));
};
		
/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.getIsoSpeedStr = function(pVal) {
	return (pVal == null ? '' : pVal.toFixed(0));
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.getExposureStr = function(pVal) {
	if ( pVal == null ) {
		return '';
	}

	if ( pVal > 0.25 ) {
		return pVal.toFixed(2)+' sec';
	}

	return '1/'+(1/pVal).toFixed(0)+' sec';
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.getFocalLengthStr = function(pVal) {
	return (pVal == null ? '' : pVal.toFixed(1)+' mm');
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.getFlashUsageStr = function(pVal) {
	return (pVal == null ? '' : (pVal*100).toFixed(1)+'%');
};
