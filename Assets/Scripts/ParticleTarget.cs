using Assets.Scripts;
using ParticlePlayground;
using UnityEngine;
using System.Collections.Generic;

public class ParticleTarget : MonoBehaviour
{
    private Melody _melody;
    private List<MusicParticle> _musicParticles;
    private List<int> _activeParticleIndices;
    private Color black = new Color(0, 0, 0);
    public int MaxMusicParticles = 5;
    public Vector3 ParticleOffset;

    //public MusicParticle MusicParticle = new EnchantedOrb();
    public MusicParticle[] MP;
    public GameObject Pillar;

    public AudioSource AudioSource;

    // Use this for initialization
    void Start()
    {
        this._musicParticles = new List<MusicParticle>();
        this.ParticleOffset = new Vector3(0, 1.35f, 0);
        var pillar = Instantiate(this.Pillar, this.transform.position, this.transform.rotation) as GameObject;
        pillar.transform.localScale = new Vector3(0.2f, 0.1f, 0.2f);
        for (var i = 0; i < MaxMusicParticles; i++)
        {
            var mParticle = Instantiate(this.MP[i], this.transform.position + this.ParticleOffset, this.transform.rotation) as MusicParticle;
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
    void Update()
    {
        if (_melody == null)
        {
            return;
        }

        var time = Time.time % _melody.Duration;
        var averageMidiNoteForNow = _melody.GetAverageForEachHand(0f, time, "midi");
        var averageVelocityForNow = _melody.GetAverageForEachHand(0f, time, "velocity");
        var colorLeft = Color.HSVToRGB(averageMidiNoteForNow[0] / 61, 1f, Clamp(averageVelocityForNow[0] / 5f, 1f, 0f));
        var colorRight = Color.HSVToRGB(averageMidiNoteForNow[1] / 61, 1f, Clamp(averageVelocityForNow[1] / 5f, 1f, 0f));
        if (this._melody.AverageOctaveMidiLeftHand != -1)
        {
            var leftHandParticle = this._musicParticles[this._melody.AverageOctaveMidiLeftHand];
            //Debug.Log("colorLeft: " + colorLeft);
            if (!colorLeft.Equals(Color.black))
            {
                leftHandParticle.SetColor(colorLeft);
            }
            if (averageVelocityForNow[0] > 0)
            {
                leftHandParticle.Scale(averageVelocityForNow[0] + 0.01f);
            }
        }
        if (this._melody.AverageOctaveMidiRightHand != -1)
        {
            var rightHandParticle = this._musicParticles[this._melody.AverageOctaveMidiRightHand];
            //Debug.Log("colorRight: " + colorRight);
            if (!colorRight.Equals(Color.black))
            {
                rightHandParticle.SetColor(colorRight);
            }
            if (averageVelocityForNow[1] > 0)
            {
                rightHandParticle.Scale(averageVelocityForNow[1] + 0.01f);
            }
        }
    }

    public void SetMelody(Melody melody)
    {
        if (this._melody != null)
        {
            this.ToggleActiveParticles();
        }
        this._melody = melody;
        this.ToggleActiveParticles();

        this.AudioSource.clip = this._melody.Audio;
        this.AudioSource.loop = true;
        this.AudioSource.Play();
    }

    private void ToggleActiveParticles()
    {
        Debug.Log("LEFT: " + this._melody.AverageOctaveMidiLeftHand);
        if (this._melody.AverageOctaveMidiLeftHand != -1)
        {
            Debug.Log("left activated or deactivated: " + this._melody.AverageOctaveMidiLeftHand);
            this._musicParticles[this._melody.AverageOctaveMidiLeftHand].ToggleActivate();
        }

        Debug.Log("RIGHT: " + this._melody.AverageOctaveMidiRightHand);
        if (this._melody.AverageOctaveMidiRightHand != -1)
        {
            Debug.Log("right activated or deactivated: " + this._melody.AverageOctaveMidiRightHand);
            this._musicParticles[this._melody.AverageOctaveMidiRightHand].ToggleActivate();
        }
    }

    public static T Clamp<T>(T value, T max, T min)
    where T : System.IComparable<T>
    {
        T result = value;
        if (value.CompareTo(max) > 0)
            result = max;
        if (value.CompareTo(min) < 0)
            result = min;
        return result;
    }
}