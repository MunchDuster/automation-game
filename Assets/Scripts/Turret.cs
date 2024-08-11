using System;
using UnityEngine;
using Random = UnityEngine.Random;

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

    [Header("Shooting (Raycast)")]
    public float damage = 25f;
    public float maxRange = 1000;
    public ParticleSystem hitFX;
    public Transform shootPoint;
    public LayerMask layerMask;
    
    [Header("_recoil")]
    public float recoilMax = 0.1f;
    public float recoilMin = 0.1f;
    public float recoilPitchAngleMax = 6f;
    public float recoilPitchAngleMin = 3f;
    public float recoilYawAngleMax = 5f;
    public float recoilYawAngleMin = 1f;
    public float totalRecoilTime = 0.001f;
    public float recoilSpeed = 50;
    public AnimationCurve recoilCurve;
    private bool _recoiling;
    private float _curRecoilTime;
    
    [Header("Values")]
    public float verticalSlide;
    public float yaw;
    public float pitch;
    
    private float _recoil = 0f;
    private float _recoilYawAngleCurMax;
    private float _recoilPitchAngleCurMax;
    private float _recoilCurMax;
    private Vector3 _lastHitPoint;

    [Header("FX")]
    public ParticleSystem muzzleFlash;

    public AudioSource[] shotSounds;
    private int _shotSoundIndex;
    
    void Update()
    {
        verticalSlide = Mathf.Clamp(verticalSlide, verticalSliderMin, verticalSliderMax);
        verticalSlider.localPosition = verticalSliderAxis * verticalSlide;

        if (_recoiling)
        {
            Recoil();
        }
        else
        {
            Fire();
        }
    
        _recoilCurMax = Mathf.Lerp(recoilMin, recoilMax, Random.value);
        float recoilYaw = _recoilYawAngleCurMax * (_recoil / _recoilCurMax);
        float recoilPitch = _recoilPitchAngleCurMax * (_recoil / _recoilCurMax);
        
        yawRotator.localRotation = Quaternion.Euler(yawOffset) * Quaternion.Euler(yawAxis * (yaw - recoilYaw));
        pitchRotator.localRotation = Quaternion.Euler(pitchOffset) * Quaternion.Euler(pitchAxis * (pitch - recoilPitch));
        
        recoilSlider.localPosition = recoilSliderAxis * _recoil;
        boltSlider.localPosition = boltSliderAxis * -_recoil/2f;
    }

    public void Fire()
    {
        _recoiling = true;
        muzzleFlash.Play();
            
        AudioSource source = shotSounds[(++_shotSoundIndex)%shotSounds.Length];
        source.pitch = Random.Range(0.99f, 1.01f);
        source.Play();

        if (Physics.Raycast(shootPoint.position, shootPoint.forward, out RaycastHit hit, maxRange, layerMask))
        {
            hitFX.transform.position = hit.point;
            hitFX.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            
            Color color = hit.collider.GetComponent<MeshRenderer>().material.color;
            float brightnessShift = 0.05f;
            Color darkColor = new Color(color.r - brightnessShift, color.g - brightnessShift, color.b - brightnessShift, 1);
            Color brightColor = new Color(color.r + brightnessShift, color.g + brightnessShift, color.b + brightnessShift, 1);
            var main = hitFX.main;
            main.startColor = new ParticleSystem.MinMaxGradient(darkColor, brightColor);
            
            hitFX.Play();
            _lastHitPoint = hit.point;
            
            Debug.DrawRay(hit.point, hitFX.transform.rotation * Vector3.forward);

            Health health = hit.collider.gameObject.GetComponent<Health>();
            if (health)
            {
                health.TakeDamage(damage);
            }
        }
        
        _recoilYawAngleCurMax = Mathf.Lerp(recoilYawAngleMin, recoilYawAngleMax, Random.value);
        _recoilPitchAngleCurMax = Mathf.Lerp(recoilPitchAngleMin, recoilPitchAngleMax, Random.value);
    }

    private void Recoil()
    {
        _curRecoilTime += Time.deltaTime * recoilSpeed;
        if (_curRecoilTime > totalRecoilTime)
        {
            _recoiling = false;
            _curRecoilTime = 0;
            return;
        }
        _recoil = recoilCurve.Evaluate(_curRecoilTime / totalRecoilTime) * _recoilCurMax;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_lastHitPoint, 0.1f);
    }
}
