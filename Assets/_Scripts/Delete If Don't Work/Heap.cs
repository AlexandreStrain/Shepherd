﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T>  where T : IHeapItem<T> {

	private T[] _items;
	private int _curItemCount;

	public Heap(int maxHeapSize) {
		_items = new T[maxHeapSize];
	}

	public void Add(T item){
		item.HeapIndex = _curItemCount;
		_items [_curItemCount] = item;
		SortUp (item);
		_curItemCount++;
	}

	public T RemoveFirst() {
		T firstItem = _items [0];
		_curItemCount--;

		_items [0] = _items [_curItemCount];
		_items [0].HeapIndex = 0;
		SortDown (_items [0]);
		return firstItem;
	}

	public bool Contains(T item) {
		return Equals (_items [item.HeapIndex], item);
	}

	public int getCount() {
		return _curItemCount;
	}

	public void UpdateItem(T item) {
		SortUp (item); //increase priority

		//sortdown would be decrease priority
	}

	private void SortDown(T item) {
		while (true) {
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;
			int swapIndex = 0;

			if (childIndexLeft < _curItemCount) {
				swapIndex = childIndexLeft;
				if (childIndexRight < _curItemCount) {
					if (_items [childIndexLeft].CompareTo (_items [childIndexRight]) < 0) {
						swapIndex = childIndexRight;
					}
				}

				if (item.CompareTo (_items [swapIndex]) < 0) {
					Swap (item, _items [swapIndex]);
				} else {
					return;
				}
			} else {
				return;
			}
		}
	}

	private void SortUp(T item) {
		int parentIndex = (item.HeapIndex - 1) / 2;
		while (true) {
			T parentItem = _items [parentIndex];
			if (item.CompareTo (parentItem) > 0) {
				Swap (item, parentItem);
			} else {
				break;
			}

			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	private void Swap(T itemA, T itemB) {
		_items [itemA.HeapIndex] = itemB;
		_items [itemB.HeapIndex] = itemA;
		int itemAIndex = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = itemAIndex;
	}
}

public interface IHeapItem<T> : IComparable<T> {
	int HeapIndex {
		get;
		set;
	}
}