using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance;

    [Header("Global Shake Force")]
    [SerializeField] private float globalShakeForce = 1f;
    [SerializeField] private CinemachineImpulseListener impulseListener;

    private CinemachineImpulseDefinition impulseDefinition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void CameraShake(CinemachineImpulseSource impulseSource)
    {
        if (impulseSource == null)
        {
            return;
        }
        impulseSource.GenerateImpulseWithForce(globalShakeForce);
    }

    public void ScreenShakeFromProfile(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        // Apply Settings!!  
        SetupScreenShakeSettings(profile, impulseSource);

        // Shake!!  
        impulseSource.GenerateImpulseWithForce(profile.impactForce);

    }

    private void SetupScreenShakeSettings(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        
        //impulse source settings
        impulseDefinition = impulseSource.ImpulseDefinition;
        impulseDefinition.ImpulseDuration = profile.impactTime;
        //impulseDefinition.DefaultVelocity = profile.defaultVelocity;
        impulseDefinition.CustomImpulseShape = profile.impulseCurve; // ✅ This is where your AnimationCurve goes

        //impulse listener settings
        impulseListener.ReactionSettings.AmplitudeGain = profile.listenerAmplitude;
        impulseListener.ReactionSettings.FrequencyGain = profile.listenerFrequency;
        impulseListener.ReactionSettings.Duration = profile.listenerDuration;



    }
}