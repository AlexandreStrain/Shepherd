using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Senses : MonoBehaviour {

	//[Range (0f, 360f)]
	//public float _sightAngle; //could be measured by perception
	//public float _senseRadius; //could be measured by perception
	//public float _senseDelay = 0.2f; //could be measured by perception
	public CharacterBody _body;

	/*private float _currentDelay;
	public bool _sensesActive;

	//private const float _meshResolution = 1f;
	//private const float _edgeDistanceThreshold = 8f;

	//private const int _edgeResolveIterations = 4;

	private List<Transform> _visibleObjects = new List<Transform> ();
	private List<Transform> _audibleObjects = new List<Transform> ();

	//what is deemed worthy of attention (target) and what can be ignored (obstacle)
	public LayerMask _targetMask;
	public LayerMask _obstacleMask;

	//private Mesh _viewMesh;
	//public MeshFilter _viewMeshFilter;
*/
	public List<Transform> GetVisibleObjects() {
		return _body.GetVisibleObjects ();
	}

	public List<Transform> GetAudibleObjects() {
		return _body.GetAudibleObjects ();
	}

	/*NOT SURE IF NEEDED
	public float GetSenseRadius() {
		return _senseRadius;
	}
	public float GetSightAngle() {
		return _sightAngle;
	}*/

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal){
		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3 (Mathf.Sin (angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos (angleInDegrees * Mathf.Deg2Rad));
	}

	/*private void Start () {
		_viewMesh = new Mesh ();
		_viewMesh.name = "View Mesh";
		_viewMeshFilter.mesh = _viewMesh;
		_viewMeshFilter.gameObject.SetActive (true);
		StartCoroutine ("FindTargetsWithDelay", _senseDelay);
	}*/

	public void Init(CharacterBody body) {
		//_sightAngle = body.GetSightAngle ();
		//_senseRadius = body.GetSenseRadius ();
		//_senseDelay = body.GetSenseDelay ();
		_body = body;
		//_sensesActive = true;
		//_currentDelay = 0f;
		//_visibleObjects.Clear ();
		//_audibleObjects.Clear ();
	}

	/*public void Init(int perception) {
		if (perception == 0) {
			//blind
			_sightAngle = 0f;
			_senseRadius = 5f; //for now.. so they aren't deaf too!
			_senseDelay = 3f; //for now.. so they still hear pretty well
		} else {
			_sightAngle = 36f * Mathf.Clamp(perception, 0f, 10f); //HARDCODED, but should total 360 degree vision if perception is 10 
			_senseRadius = (0.75f * (Mathf.Clamp(perception, 0f, 10f) * Mathf.Clamp(perception, 0f, 10f))) + (2 * Mathf.Clamp(perception, 0f, 10f)) + 5f; //No more than 100 m away -- otherwise too taxing to system?
			_senseDelay = 5f / Mathf.Clamp(perception, 0.001f, 10f); 
		}
		_sensesActive = true;
		_currentDelay = 0f;
		_visibleObjects.Clear ();
		_audibleObjects.Clear ();
	}

	public void Tick(float delta) {
		if (_sensesActive) {
			if (_currentDelay >= _senseDelay) {
				SenseSurroundings ();
				_currentDelay = 0f;
			} else {
				_currentDelay += delta;
			}
		}
	}

	private void SenseSurroundings() {
		_visibleObjects.Clear ();
		_audibleObjects.Clear ();
		Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, _senseRadius, _targetMask);
		foreach (Collider target in targetsInViewRadius) {
			if (target.transform == this.gameObject.transform) {
				continue; //we hear/see ourselves, which doesn't matter
			}
			_audibleObjects.Add (target.transform);
			Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
			if (Vector3.Angle (transform.forward, dirToTarget) < _sightAngle / 2f) {
				float disToTarget = Vector3.Distance (transform.position, target.transform.position);
				if (!Physics.Raycast (transform.position, dirToTarget, disToTarget, _obstacleMask)) {
					_visibleObjects.Add (target.transform);
				}
			}
		}
	}

	/*private IEnumerator FindTargetsWithDelay(float delay) {
		while (true) {
			yield return new WaitForSeconds (delay);
			SenseSurroundings ();
		}
	}

	private void LateUpdate () {
		//DrawFieldOfView ();
	}

	private void DrawFieldOfView() {
		int stepCount = Mathf.RoundToInt(_sightAngle * _meshResolution);
		float stepAngleSize = _sightAngle / stepCount;

		List<Vector3> viewPoints = new List<Vector3> ();
		ViewCastInfo oldViewCast = new ViewCastInfo ();
		for (int i = 0; i <= stepCount; i++) {
			float angle = transform.eulerAngles.y - _sightAngle / 2.0f + stepAngleSize * i;
			ViewCastInfo newViewCast = ViewCast (angle);

			if (i > 0) {
				bool edgeDistanceThresholdExceeded = Mathf.Abs (oldViewCast._distance - newViewCast._distance) > _edgeDistanceThreshold;
				if (oldViewCast._hit != newViewCast._hit || (oldViewCast._hit && newViewCast._hit && edgeDistanceThresholdExceeded)) {
					EdgeCastInfo edge = FindEdge (oldViewCast, newViewCast);
					if (edge._pointA != Vector3.zero) {
						viewPoints.Add (edge._pointA);
					}

					if (edge._pointB != Vector3.zero) {
						viewPoints.Add (edge._pointB);
					}
				}
			}
			viewPoints.Add (newViewCast._point);
			oldViewCast = newViewCast;
		}
		int vertexCount = viewPoints.Count + 1;
		Vector3[] newVertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount - 2) * 3];
		newVertices [0] = Vector3.zero;
		for (int j = 0; j < vertexCount - 1; j++) {
			newVertices [j + 1] = transform.InverseTransformPoint(viewPoints [j]);
			if (j < vertexCount - 2) {
				triangles [j * 3] = 0;
				triangles [j * 3 + 1] = j + 1;
				triangles [j * 3 + 2] = j + 2;
			}
		}

		_viewMesh.Clear ();
		_viewMesh.vertices = newVertices;
		_viewMesh.triangles = triangles;
		_viewMesh.RecalculateNormals ();
	}

	private ViewCastInfo ViewCast(float globalAngle) {
		Vector3 dir = DirFromAngle (globalAngle, true);
		RaycastHit hit;
		if (Physics.Raycast (transform.position, dir, out hit, _senseRadius, _obstacleMask)) {
			return new ViewCastInfo (true, hit.point, hit.distance, globalAngle);
		}
		return new ViewCastInfo (false, transform.position + dir * _senseRadius, _senseRadius, globalAngle);
	}

	private EdgeCastInfo FindEdge(ViewCastInfo min, ViewCastInfo max) {
		float minAngle = min._angle;
		float maxAngle = max._angle;
		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		for (int i = 0; i < _edgeResolveIterations; i++) {
			float angle = (minAngle + maxAngle) / 2.0f;
			ViewCastInfo newViewCast = ViewCast (angle);
			bool edgeDistanceThresholdExceeded = Mathf.Abs (min._distance - newViewCast._distance) > _edgeDistanceThreshold;

			if (newViewCast._hit == min._hit && !edgeDistanceThresholdExceeded) {
				minAngle = angle;
				minPoint = newViewCast._point;
			} else {
				maxAngle = angle;
				maxPoint = newViewCast._point;
			}
		}
		return new EdgeCastInfo (minPoint, maxPoint);
	}*/
}