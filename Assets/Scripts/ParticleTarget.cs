using Assets.Scripts;
using ParticlePlayground;
using UnityEngine;

public class ParticleTarget : MonoBehaviour {
    private Melody _melody;
    private ParticleSystem _particleSystem;
    private ParticleSystem.MainModule _mainModule;

	public AudioSource AudioSource;

    // Use this for initialization
    void Start () {
        _particleSystem = GetComponent<ParticleSystem>();
        _mainModule = this._particleSystem.main;

		if (AudioSource == null)
		{
			AudioSource = GetComponent<AudioSource>();
			if (AudioSource == null)
			{
				AudioSource = gameObject.AddComponent<AudioSource>();
			}
		}
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
		    _mainModule.startLifetime = 1f;
			Melody.Analyze();
		    AudioSource.clip = Melody.Audio;
		    AudioSource.loop = true;
			AudioSource.Play();
	    }
    }
}
