using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTarget : MonoBehaviour {
    public Melody melody;
    private ParticleSystem ps;
    private ParticleSystem.MainModule ma;
    private float time;

    // Use this for initialization
    void Start () {
        this.ps = GetComponent<ParticleSystem>();
        this.ma = this.ps.main;
    }
	
	// Update is called once per frame
	void Update () {
        this.time = Time.time % melody.Duration;

        float averageMidiNoteForLastSeconds = melody.getAverageMidiNoteLastSeconds(0, this.time);
        Color Color = Color.HSVToRGB(averageMidiNoteForLastSeconds / 127, 1, 1);
        this.ma.startColor = Color;
    }

    public void setMelody(Melody melody)
    {
        this.melody = melody;
        this.ma.startLifetime = 1f;
    }
}
