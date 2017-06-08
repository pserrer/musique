using Assets.Scripts;
using ParticlePlayground;
using UnityEngine;
using System.Collections.Generic;

public class ParticleTarget : MonoBehaviour {
    private Melody _melody;
    private List<MusicParticle> _musicParticles;
    public int MaxMusicParticles = 2;
    public Vector3 ParticleOffset;

    //public MusicParticle MusicParticle = new EnchantedOrb();
    public MusicParticle MP;
    public GameObject Pillar;

    public AudioSource AudioSource;

    // Use this for initialization
    void Start () {
        this._musicParticles = new List<MusicParticle>();
        this.ParticleOffset = new Vector3(0, 11, 0);
        var pillar = Instantiate(this.Pillar, this.transform.position, this.transform.rotation) as GameObject;
        for (var i = 0; i < MaxMusicParticles; i++)
        {
            var mParticle = Instantiate(this.MP, this.transform.position + this.ParticleOffset, this.transform.rotation) as MusicParticle;

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
        //var colorLeft = Color.HSVToRGB(averageMidiNoteForLastSeconds[0] / 64, 1, 1);
        //var colorRight = Color.HSVToRGB(averageMidiNoteForLastSeconds[1]-63 / 64, 1, 1);
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
            this._musicParticles[1].scaleScatterSize(0.8f);
        }
    }
}