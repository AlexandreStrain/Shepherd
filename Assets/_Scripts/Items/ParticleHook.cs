using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHook : MonoBehaviour {

	ParticleSystem[] _particles;
	private int _value = 1;

	public void Init() {
		_particles = GetComponentsInChildren<ParticleSystem> ();
	}

	public void EmitParticle(int value = 1) {
		if (_particles == null) {
			return;
		}

		for (int i = 0; i < _particles.Length; i++) {
			_particles [i].Emit (value);
		}
	}

	public void EmitParticle() {
		if (_particles == null) {
			return;
		}

		for (int i = 0; i < _particles.Length; i++) {
			_particles [i].Emit (_value);
		}
	}

	public void Cleanup() {
		Destroy (this.gameObject);
	}
}