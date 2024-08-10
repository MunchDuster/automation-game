using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Transform references")]
    public Transform verticalSlider;
    public Transform recoilSlider;
    public Transform pitchRotator;
    public Transform yawRotator;
    public Transform boltSlider;

    [Header("Axes")]
    public Vector3 verticalSliderAxis;
    public Vector3 recoilSliderAxis;
    public Vector3 pitchAxis;
    public Vector3 yawAxis;
    public Vector3 boltSliderAxis;
    [Space(10)]
    public Vector3 pitchOffset;
    public Vector3 yawOffset;

    [Header("Bounds")]
    public float verticalSliderMax = 1;
    public float verticalSliderMin = 0;
    
    [Header("_recoil")]
    public float recoilMax = 0.1f;
    public float recoilMin = 0.1f;
    public float recoilPitchAngleMax = 6f;
    public float recoilPitchAngleMin = 3f;
    public float recoilYawAngleMax = 5f;
    public float recoilYawAngleMin = 1f;
    public float recoilTolerance = 0.001f;
    public float recoilSpeed = 50;
    public float fireSpeed = 50;
    private bool _recoiling;
    private bool _firing;
    
    [Header("Values")]
    public float verticalSlide;
    public float yaw;
    public float pitch;
    public bool fire;
    
    private float _recoil = 0f;
    private float _recoilYawAngleCurMax;
    private float _recoilPitchAngleCurMax;

    [Header("FX")]
    public ParticleSystem muzzleFlash;

    public AudioSource[] shotSounds;
    private int _shotSoundIndex;
    
    void Update()
    {
        verticalSlide = Mathf.Clamp(verticalSlide, verticalSliderMin, verticalSliderMax);
        verticalSlider.localPosition = verticalSliderAxis * verticalSlide;

        float recoilAmount = Mathf.Lerp(recoilMin, recoilMax, Random.value);
        
        float _recoilYaw = _recoilYawAngleCurMax * (_recoil / recoilAmount);
        float _recoilPitch = _recoilPitchAngleCurMax * (_recoil / recoilAmount);

        if (_firing)
        {
            _recoil = Mathf.Lerp(_recoil, recoilAmount, Time.deltaTime * fireSpeed);
            if (recoilAmount - _recoil < 0.01f)
            {
                _firing = false;
                _recoiling = true;
            }
        }
        else
        {
            if (_recoiling)
            {
                Recoil();
            }
            else
            {
                Fire();
            }
        }
        
        yawRotator.localRotation = Quaternion.Euler(yawOffset) * Quaternion.Euler(yawAxis * (yaw - _recoilYaw));
        pitchRotator.localRotation = Quaternion.Euler(pitchOffset) * Quaternion.Euler(pitchAxis * (pitch - _recoilPitch));
        
        recoilSlider.localPosition = recoilSliderAxis * _recoil;
        boltSlider.localPosition = boltSliderAxis * -_recoil;
    }

    public void Fire()
    {
        _firing = true;
        muzzleFlash.Play();
            
        AudioSource source = shotSounds[(++_shotSoundIndex)%shotSounds.Length];
        source.pitch = Random.Range(0.99f, 1.01f);
        source.Play();
        
        _recoilYawAngleCurMax = Mathf.Lerp(recoilYawAngleMin, recoilYawAngleMax, Random.value);
        _recoilPitchAngleCurMax = Mathf.Lerp(recoilPitchAngleMin, recoilPitchAngleMax, Random.value);
    }

    private void Recoil()
    {
        _recoil *= 1f - recoilSpeed * Time.deltaTime;
        if (_recoil < recoilTolerance)
        {
            _recoiling = false;
        }
    }
}
