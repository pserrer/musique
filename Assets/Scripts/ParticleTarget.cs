using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTarget : MonoBehaviour {
    public Melody melody;
    private ParticleSystem ps;

    // Use this for initialization
    void Start () {
        this.ps = GetComponent<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setMelody(Melody melody)
    {
        this.melody = melody;
        var ma = ps.main;
        ma.startLifetime = 1f;
        Color Color = Color.HSVToRGB(melody.AverageMidiNote / 127, 1, 1);
        ma.startColor = Color;
    }
}
