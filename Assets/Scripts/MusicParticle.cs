using Assets.Scripts;
using ParticlePlayground;
using UnityEngine;

public class MusicParticle : MonoBehaviour
{
    private PlaygroundParticlesC _particleSystem;
    private Gradient _gradient;
    private bool _active;
    private float _scatterSize;
    private float _particleScale;
    private GradientAlphaKey[] _alphaKeys = { new GradientAlphaKey(1, 0f), new GradientAlphaKey(0, 1f) };
    public PlaygroundTrails trailScript;
    //private ParticleSystem.MainModule _mainModule;

    // Use this for initialization
    void Start()
    {
        this._particleSystem = GetComponent<PlaygroundParticlesC>();
		this._particleSystem.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        this.trailScript = GetComponent<PlaygroundTrails>() as PlaygroundTrails;
        this._particleSystem.emit = false;
        this._active = false;
        this._scatterSize = 0.2f;
        this._particleScale = 0.2f;
        this.Scale(_scatterSize);
        this._gradient = new Gradient();
        this._gradient.SetKeys(_gradient.colorKeys, _alphaKeys);
        InvokeRepeating("Shrink", 0.0f, 0.01f);
        InvokeRepeating("FadeToGray", 0.0f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public bool ToggleActivate()
    {
        _particleSystem.emit = !_particleSystem.emit;
        this._active = !this._active;
        return this._active;
    }

    public void SetColor(Color color)
    {
        //Debug.Log("Color: " + color);
        _particleSystem.lifetimeColor = GetGradient(color);
        if (this.trailScript != null)
        {
            this.trailScript.lifetimeColor = GetGradient(color);
        }
    }

    public void SetParticleCount(int count)
    {
        _particleSystem.particleCount = count;
    }

    private Gradient GetGradient(Color color)
    {
        GradientColorKey[] colorKeys = { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) };
        this._gradient.SetKeys(colorKeys, this._gradient.alphaKeys);
        return this._gradient;
    }

    public void Scale(float size = 0.99f)
    {
        if (this._particleSystem.applySourceScatter)
        {
            this._scatterSize = this._scatterSize * size;
			this._scatterSize = Mathf.Clamp(this._scatterSize, 0.1f, 0.2f); //0.7f, 1.2f
            this._particleSystem.scatterScale = new Vector3(this._scatterSize, this._scatterSize, this._scatterSize);
        }
        else
        {
            this._particleScale = this._particleSystem.scale * size;
            this._particleScale = Mathf.Clamp(this._particleScale, 0.1f, 0.2f);
            this._particleSystem.scale = this._particleScale;
        }
    }

    private void Shrink()
    {
        if (this._particleSystem.applySourceScatter)
        {
            this._scatterSize = this._scatterSize * 0.993f;
			this._scatterSize = Mathf.Clamp(this._scatterSize, 0.1f, 0.2f); // 0.5f, 1.2f
            this._particleSystem.scatterScale = new Vector3(this._scatterSize, this._scatterSize, this._scatterSize);
        }
        else
        {
            this._particleScale = this._particleSystem.scale * 0.993f;
			this._particleScale = Mathf.Clamp(this._particleScale, 0.1f, 0.2f);
            this._particleSystem.scale = this._particleScale;
        }
    }

    private void FadeToGray()
    {
        var newColor = Color.Lerp(this._particleSystem.lifetimeColor.colorKeys[0].color, Color.black, 0.1f);
        GradientColorKey[] colorKeys = { new GradientColorKey(newColor, 0f), new GradientColorKey(newColor, 1f) };
        this._particleSystem.lifetimeColor.SetKeys(colorKeys, this._alphaKeys);
    }

    public float GetScatterSize()
    {
        return this._scatterSize;
    }
}
