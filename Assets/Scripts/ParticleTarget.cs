using Assets.Scripts;
using ParticlePlayground;
using UnityEngine;
using System.Collections.Generic;

public class ParticleTarget : MonoBehaviour {
    private Melody _melody;
    private List<MusicParticle> _musicParticles;
    public int MaxMusicParticles = 2;

    //public MusicParticle MusicParticle = new EnchantedOrb();
    public prefab : Enchanted Orb;
            
	public AudioSource AudioSource;

    // Use this for initialization
    void Start () {
        this._musicParticles = new List<MusicParticle>();
        for (var i = 0; i < MaxMusicParticles; i++)
        {
            var mParticle = Instantiate(prefab, this.transform.position, this.transform.rotation);

            this._musicParticles.Add(mParticle);
        }
        if (this.AudioSource == null)
		{
			this.AudioSource = GetComponent<AudioSource>();
			if (AudioSource == null)
			{
				this.AudioSource = gameObject.AddComponent<AudioSource>();
			}
        }
        this._musicParticles[0].setScatterSize(1.5f);
    }

    // Update is called once per frame
    void Update() {
        if (_melody == null)
        {
            return;
        }

        var time = Time.time % _melody.Duration;
        var averageMidiNoteForLastSeconds = _melody.GetAverageMidiForEachHand(1, time);
        var colorLeft = Color.HSVToRGB(averageMidiNoteForLastSeconds[0] / 127, 1, 1);
        var colorRight = Color.HSVToRGB(averageMidiNoteForLastSeconds[1] / 127, 1, 1);
        this._musicParticles[0].setColor(colorLeft);
        this._musicParticles[1].setColor(colorRight);
    }

    public Melody Melody
    {
	    get { return _melody; }
	    set
	    {
		    _melody = value;
            Melody.Analyze();
		    AudioSource.clip = Melody.Audio;
		    AudioSource.loop = true;
			AudioSource.Play();
	    }
    }
}