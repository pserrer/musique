using Assets.Scripts;
using ParticlePlayground;
using UnityEngine;

public class MusicParticle : MonoBehaviour
{
    private PlaygroundParticlesC _particleSystem;
    private Gradient _gradient;
    private bool active;
    //private ParticleSystem.MainModule _mainModule;

    // Use this for initialization
    void Start ()
    {
        _particleSystem = GetComponent<PlaygroundParticlesC>();
        _particleSystem.emit = false;
        this.active = false;
        _gradient = new Gradient();
        GradientAlphaKey[] alphaKeys = { new GradientAlphaKey(1, 0f), new GradientAlphaKey(0, 1f) };
        _gradient.SetKeys(_gradient.colorKeys, alphaKeys);
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    public bool ToggleActivate()
    {
        _particleSystem.emit = !_particleSystem.emit;
        this.active = !this.active;
        return this.active;
    }

    public void SetColor(Color color)
    {
        //Debug.Log("Color: " + color);
        _particleSystem.lifetimeColor = GetGradient(color);
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

    public void ScaleScatterSize(float size)
    {
        var x = this._particleSystem.scatterScale.x;
        var y = this._particleSystem.scatterScale.y;
        var z = this._particleSystem.scatterScale.z;
        Vector3 newScale = new Vector3(size*x, size*y, size*z);
        this._particleSystem.scatterScale = newScale;
    }
}
