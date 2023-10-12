using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CastInfo {

}

/*
public struct BodyNeedsInfo {

	public BodyNeedsInfo(){

	}
}
*/

/*public struct SensesInfo {
	public List<Transform> _visibleObjects {
		get;
	}

	public List<Transform> _audibleObjects {
		get;
	}

	public SensesInfo(List<Transform> vO, List<Transform> aO) {
		this._visibleObjects = vO;
		this._audibleObjects = aO;
	}
}*/

public struct ViewCastInfo {
	public bool _hit;
	public Vector3 _point;
	public float _distance;
	public float _angle;

	public ViewCastInfo(bool hit, Vector3 point, float distance, float angle){
		_hit = hit;
		_point = point;
		_distance = distance;
		_angle = angle;
	}
}

public struct EdgeCastInfo {
	public Vector3 _pointA;
	public Vector3 _pointB;
	public EdgeCastInfo(Vector3 pointA, Vector3 pointB) {
		_pointA = pointA;
		_pointB = pointB;
	}
}
