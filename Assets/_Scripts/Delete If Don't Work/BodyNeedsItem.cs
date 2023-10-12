using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyNeedsItem : IHeapItem<BodyNeedsItem> {
	
	public float _meter;
	public int _type;
	private int _heapIndex;

	public BodyNeedsItem(float start, int function) {
		this._meter = start;
		this._type = function;
	}

	public int HeapIndex {
		get {
			return _heapIndex;
		}
		set {
			this._heapIndex = value;
		}
	}

	public int CompareTo(BodyNeedsItem need) {
		//negative value means object comes before this in the sort order
		//positive value means object comes after this in sort order

		if (this._type < need._type) {
			if (this._meter > need._meter) {
				return 1;
			} else {
				return -1;
			}
		}

		if (this._type > need._type) {
			if (this._meter > need._meter) {
				return -1;
			} else {
				return 1;
			}
		}

		return 0;
	}

	public int getNeedType() {
		return this._type;
	}

	public void setNeedType(int bodyNeed) {
		this._type = bodyNeed;
	}

	public float getMeter() {
		return this._meter;
	}

	public void resetMeter() {
		this._meter = 0;
	}

	public void increaseMeter() {
		this._meter++;
		if (this._meter >= 100) {
			this._meter = 100;
		}
	}

	public void increaseMeter(float amount) {
		this._meter += amount;
		if (this._meter >= 100) {
			this._meter = 100;
		}
	}

	public void decreaseMeter() {
		this._meter--;
		if (this._meter <= 0) { 
			this._meter = 0;
		}
	}

	public void decreaseMeter(float amount) {
		this._meter -= amount;
		if (this._meter <= 0) { 
			this._meter = 0;
		}
	}
}