using Assets.Scripts;
using ParticlePlayground;
using UnityEngine;

public class ParticleTarget : MonoBehaviour {
    private Melody _melody;
    private PlaygroundParticlesC _particleSystem;
    private ParticleSystem.MainModule _mainModule;

	public AudioSource AudioSource;

    // Use this for initialization
    void Start () {
        _particleSystem = GetComponent<PlaygroundParticlesC>();

		if (AudioSource == null)
		{
			AudioSource = GetComponent<AudioSource>();
			if (AudioSource == null)
			{
				AudioSource = gameObject.AddComponent<AudioSource>();
			}
        }
        _particleSystem.lifetime = 1f;
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

        GradientColorKey[] colorKeys = { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) };
        GradientAlphaKey[] alphaKeys = { new GradientAlphaKey(1, 0f), new GradientAlphaKey(0, 1f) };
        var gradient = new Gradient();
        gradient.SetKeys(colorKeys, alphaKeys);
        _particleSystem.lifetimeColor = gradient;
    }

    public Melody Melody
    {
	    get { return _melody; }
	    set
	    {
		    _melody = value;
            _particleSystem.lifetimeMin = 2f;
            _particleSystem.lifetime = 4f;
            _particleSystem.particleCount = 1000;
            Melody.Analyze();
		    AudioSource.clip = Melody.Audio;
		    AudioSource.loop = true;
			AudioSource.Play();
	    }
    }
}
