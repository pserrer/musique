using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class ParticleController : MonoBehaviour
{

	public Vector3 Center;
	public Vector3 ParticleOffset;

	public ParticleTarget ParticleTarget;
	public int TargetCount = 5;
	public float Radius = 20f;

	private List<ParticleTarget> _targets;
	public List<Melody> _melodies;

	// Use this for initialization
	void Start () {
		_targets = new List<ParticleTarget>();
		_melodies = new List<Melody>();

		for (var i = 0; i < TargetCount; i++)
		{
			var angle = ((float)i)/TargetCount*360;
			var position = Center + Quaternion.AngleAxis(angle, Vector3.up) * (Vector3.forward*Radius) + ParticleOffset;
			var target = (ParticleTarget) Instantiate(ParticleTarget, position, Quaternion.AngleAxis(angle, Vector3.up));
			_targets.Add(target);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddMelody(Melody melody)
	{
		_melodies.Add(melody);
        melody.Analyze();
		if (_melodies.Count > TargetCount)
		{
			_melodies.RemoveAt(0);
		}
	}
}
