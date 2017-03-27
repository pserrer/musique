using Assets.Scripts;
using UnityEngine;

public class ParticleTarget : MonoBehaviour {
    private Melody _melody;
    private ParticleSystem _particleSystem;
    private ParticleSystem.MainModule _mainModule;

    // Use this for initialization
    void Start () {
        _particleSystem = GetComponent<ParticleSystem>();
        _mainModule = this._particleSystem.main;
    }
	
	// Update is called once per frame
	void Update () {
		if (_melody == null)
		{
			return;
		}

        var time = Time.time % _melody.Duration;
        var averageMidiNoteForLastSeconds = _melody.GetAverageMidiNoteLastSeconds(0, time);
        var color = Color.HSVToRGB(averageMidiNoteForLastSeconds / 127, 1, 1);
        _mainModule.startColor = color;
    }

    public Melody Melody
    {
	    get { return _melody; }
	    set
	    {
		    _melody = value;
		    this._mainModule.startLifetime = 1f;
	    }
    }
}
