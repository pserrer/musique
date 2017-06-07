using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class ParticleController : MonoBehaviour
{

	public Vector3 Center;
	public Vector3 ParticleOffset;

    public ParticleTarget ParticleTarget;
	public Transform ParticleParent;

    private int _targetCount;
	public int MaxTargetCount = 5;
	public float Radius = 20f;

	private List<ParticleTarget> _targets;
	public List<Melody> Melodies;

	// Use this for initialization
	void Start () {
		_targets = new List<ParticleTarget>();
		Melodies = new List<Melody>();

		if (ParticleParent == null)
		{
			ParticleParent = this.transform;
		}

		for (var i = 0; i < MaxTargetCount; i++)
		{
			var angle = ((float)i)/MaxTargetCount*360;
			var position = Center + Quaternion.AngleAxis(angle, Vector3.up) * (Vector3.forward*Radius) + ParticleOffset;
			var target = Instantiate(ParticleTarget, position, Quaternion.AngleAxis(angle, Vector3.up), ParticleParent);
			_targets.Add(target);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddMelody(Melody melody)
    {
        melody.Analyze();
        _targets[_targetCount++].Melody = melody;
        _targetCount %= MaxTargetCount;
		Melodies.Add(melody);
    }
}
