using UnityEngine;
using System.Collections.Generic;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance;
    
    [Header("Particle Effects")]
    public ParticleSystem ballTrailEffect;
    public ParticleSystem grassKickEffect;
    public ParticleSystem goalExplosionEffect;
    public ParticleSystem crowdConfettiEffect;
    public ParticleSystem rainEffect;
    public ParticleSystem snowEffect;
    
    [Header("Goal Effects")]
    public ParticleSystem[] goalCelebrationEffects;
    public GameObject goalTextEffect;
    public Light goalSpotlight;
    
    [Header("Ball Effects")]
    public ParticleSystem ballSmokeEffect;
    public ParticleSystem ballSparkEffect;
    public ParticleSystem ballWaterSplash;
    public TrailRenderer ballTrail;
    
    [Header("Player Effects")]
    public ParticleSystem runDustEffect;
    public ParticleSystem sweatEffect;
    public ParticleSystem tackleEffect;
    public ParticleSystem injuryEffect;
    
    [Header("Weather Effects")]
    public ParticleSystem[] weatherEffects;
    public GameObject lightningEffect;
    public AudioClip thunderSound;
    
    [Header("Stadium Effects")]
    public ParticleSystem[] fireworksEffects;
    public Light[] stadiumLights;
    public ParticleSystem crowdFlareEffect;
    
    [Header("UI Effects")]
    public ParticleSystem buttonClickEffect;
    public ParticleSystem scoreUpdateEffect;
    public ParticleSystem achievementEffect;
    
    private Dictionary<string, ParticleSystem> effectsCache = new Dictionary<string, ParticleSystem>();
    private Queue<ParticleSystem> particlePool = new Queue<ParticleSystem>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeEffects();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeEffects()
    {
        // Cachear efectos para mejor rendimiento
        CacheEffect("BallTrail", ballTrailEffect);
        CacheEffect("GrassKick", grassKickEffect);
        CacheEffect("GoalExplosion", goalExplosionEffect);
        CacheEffect("CrowdConfetti", crowdConfettiEffect);
        CacheEffect("RunDust", runDustEffect);
        CacheEffect("Sweat", sweatEffect);
        CacheEffect("Tackle", tackleEffect);
        
        // Configurar pool de partículas
        InitializeParticlePool();
    }
    
    void CacheEffect(string name, ParticleSystem effect)
    {
        if (effect != null)
        {
            effectsCache[name] = effect;
        }
    }
    
    void InitializeParticlePool()
    {
        // Crear pool de sistemas de partículas reutilizables
        for (int i = 0; i < 20; i++)
        {
            GameObject poolObject = new GameObject("PooledParticle");
            poolObject.transform.SetParent(transform);
            ParticleSystem ps = poolObject.AddComponent<ParticleSystem>();
            ps.Stop();
            particlePool.Enqueue(ps);
        }
    }
    
    ParticleSystem GetPooledParticle()
    {
        if (particlePool.Count > 0)
        {
            return particlePool.Dequeue();
        }
        else
        {
            // Crear nuevo si el pool está vacío
            GameObject poolObject = new GameObject("PooledParticle");
            poolObject.transform.SetParent(transform);
            return poolObject.AddComponent<ParticleSystem>();
        }
    }
    
    void ReturnToPool(ParticleSystem ps)
    {
        ps.Stop();
        ps.Clear();
        particlePool.Enqueue(ps);
    }
    
    // Efectos del balón
    public void PlayBallTrailEffect(Transform ballTransform)
    {
        if (ballTrailEffect != null)
        {
            ballTrailEffect.transform.position = ballTransform.position;
            ballTrailEffect.Play();
        }
        
        if (ballTrail != null)
        {
            ballTrail.enabled = true;
        }
    }
    
    public void StopBallTrailEffect()
    {
        if (ballTrailEffect != null)
        {
            ballTrailEffect.Stop();
        }
        
        if (ballTrail != null)
        {
            ballTrail.enabled = false;
        }
    }
    
    public void PlayGrassKickEffect(Vector3 position)
    {
        if (grassKickEffect != null)
        {
            grassKickEffect.transform.position = position;
            grassKickEffect.Play();
        }
    }
    
    public void PlayBallBounceEffect(Vector3 position, float intensity)
    {
        ParticleSystem effect = GetPooledParticle();
        effect.transform.position = position;
        
        var main = effect.main;
        main.startLifetime = 0.5f;
        main.startSpeed = intensity * 5f;
        main.maxParticles = Mathf.RoundToInt(intensity * 20f);
        
        effect.Play();
        
        // Retornar al pool después de usar
        Invoke("ReturnEffectToPool", 2f);
    }
    
    void ReturnEffectToPool()
    {
        // Implementar lógica para retornar efectos al pool
    }
    
    // Efectos de gol
    public void PlayGoalEffect(Vector3 position)
    {
        if (goalExplosionEffect != null)
        {
            goalExplosionEffect.transform.position = position;
            goalExplosionEffect.Play();
        }
        
        if (crowdConfettiEffect != null)
        {
            crowdConfettiEffect.Play();
        }
        
        // Reproducir múltiples efectos de celebración
        foreach (ParticleSystem effect in goalCelebrationEffects)
        {
            if (effect != null)
            {
                effect.transform.position = position;
                effect.Play();
            }
        }
        
        // Activar luz de gol
        if (goalSpotlight != null)
        {
            StartCoroutine(GoalLightEffect());
        }
        
        // Mostrar texto de gol
        if (goalTextEffect != null)
        {
            GameObject textInstance = Instantiate(goalTextEffect, position, Quaternion.identity);
            Destroy(textInstance, 3f);
        }
    }
    
    System.Collections.IEnumerator GoalLightEffect()
    {
        goalSpotlight.enabled = true;
        
        for (int i = 0; i < 5; i++)
        {
            goalSpotlight.intensity = 8f;
            yield return new WaitForSeconds(0.2f);
            goalSpotlight.intensity = 2f;
            yield return new WaitForSeconds(0.2f);
        }
        
        goalSpotlight.enabled = false;
    }
    
    // Efectos de jugadores
    public void PlayRunDustEffect(Vector3 position)
    {
        if (runDustEffect != null)
        {
            runDustEffect.transform.position = position;
            runDustEffect.Play();
        }
    }
    
    public void PlaySweatEffect(Transform playerTransform)
    {
        if (sweatEffect != null)
        {
            sweatEffect.transform.position = playerTransform.position + Vector3.up * 1.8f;
            sweatEffect.Play();
        }
    }
    
    public void PlayTackleEffect(Vector3 position)
    {
        if (tackleEffect != null)
        {
            tackleEffect.transform.position = position;
            tackleEffect.Play();
        }
    }
    
    public void PlayInjuryEffect(Vector3 position)
    {
        if (injuryEffect != null)
        {
            injuryEffect.transform.position = position;
            injuryEffect.Play();
        }
    }
    
    // Efectos climáticos
    public void SetWeatherEffect(WeatherType weather)
    {
        // Detener todos los efectos climáticos
        foreach (ParticleSystem effect in weatherEffects)
        {
            if (effect != null)
            {
                effect.Stop();
            }
        }
        
        // Activar efecto específico
        switch (weather)
        {
            case WeatherType.Rain:
                if (rainEffect != null)
                {
                    rainEffect.Play();
                }
                break;
                
            case WeatherType.Snow:
                if (snowEffect != null)
                {
                    snowEffect.Play();
                }
                break;
                
            case WeatherType.Storm:
                if (rainEffect != null)
                {
                    rainEffect.Play();
                    var main = rainEffect.main;
                    main.maxParticles = 200;
                }
                
                // Activar rayos ocasionales
                InvokeRepeating("PlayLightningEffect", 5f, Random.Range(10f, 30f));
                break;
        }
    }
    
    void PlayLightningEffect()
    {
        if (lightningEffect != null)
        {
            lightningEffect.SetActive(true);
            AudioManager.Instance.PlaySFX(thunderSound);
            
            // Desactivar después de un momento
            Invoke("HideLightningEffect", 0.2f);
        }
    }
    
    void HideLightningEffect()
    {
        if (lightningEffect != null)
        {
            lightningEffect.SetActive(false);
        }
    }
    
    // Efectos de estadio
    public void PlayFireworksEffect()
    {
        foreach (ParticleSystem firework in fireworksEffects)
        {
            if (firework != null)
            {
                firework.Play();
            }
        }
    }
    
    public void PlayCrowdFlareEffect()
    {
        if (crowdFlareEffect != null)
        {
            crowdFlareEffect.Play();
        }
    }
    
    public void FlashStadiumLights()
    {
        StartCoroutine(StadiumLightFlash());
    }
    
    System.Collections.IEnumerator StadiumLightFlash()
    {
        foreach (Light stadiumLight in stadiumLights)
        {
            if (stadiumLight != null)
            {
                stadiumLight.intensity *= 2f;
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        
        foreach (Light stadiumLight in stadiumLights)
        {
            if (stadiumLight != null)
            {
                stadiumLight.intensity /= 2f;
            }
        }
    }
    
    // Efectos de UI
    public void PlayButtonClickEffect(Vector3 position)
    {
        if (buttonClickEffect != null)
        {
            buttonClickEffect.transform.position = position;
            buttonClickEffect.Play();
        }
    }
    
    public void PlayScoreUpdateEffect(Vector3 position)
    {
        if (scoreUpdateEffect != null)
        {
            scoreUpdateEffect.transform.position = position;
            scoreUpdateEffect.Play();
        }
    }
    
    public void PlayAchievementEffect(Vector3 position)
    {
        if (achievementEffect != null)
        {
            achievementEffect.transform.position = position;
            achievementEffect.Play();
        }
    }
    
    // Efectos dinámicos
    public void PlayCustomEffect(string effectName, Vector3 position, Color color, float intensity = 1f)
    {
        if (effectsCache.ContainsKey(effectName))
        {
            ParticleSystem effect = effectsCache[effectName];
            effect.transform.position = position;
            
            var main = effect.main;
            main.startColor = color;
            main.maxParticles = Mathf.RoundToInt(main.maxParticles * intensity);
            
            effect.Play();
        }
    }
    
    // Efectos de cámara
    public void PlayCameraShake(float intensity, float duration)
    {
        StartCoroutine(CameraShakeEffect(intensity, duration));
    }
    
    System.Collections.IEnumerator CameraShakeEffect(float intensity, float duration)
    {
        Camera mainCamera = Camera.main;
        Vector3 originalPosition = mainCamera.transform.position;
        
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * intensity;
            mainCamera.transform.position = originalPosition + randomOffset;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        mainCamera.transform.position = originalPosition;
    }
    
    // Efectos de transición
    public void PlayFadeEffect(bool fadeIn, float duration = 1f)
    {
        StartCoroutine(FadeEffect(fadeIn, duration));
    }
    
    System.Collections.IEnumerator FadeEffect(bool fadeIn, float duration)
    {
        CanvasGroup fadeCanvas = GameObject.Find("FadeCanvas")?.GetComponent<CanvasGroup>();
        
        if (fadeCanvas != null)
        {
            float startAlpha = fadeIn ? 1f : 0f;
            float endAlpha = fadeIn ? 0f : 1f;
            
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                fadeCanvas.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);
                yield return null;
            }
            
            fadeCanvas.alpha = endAlpha;
        }
    }
    
    // Método para limpiar efectos
    public void ClearAllEffects()
    {
        foreach (var effect in effectsCache.Values)
        {
            if (effect != null)
            {
                effect.Stop();
                effect.Clear();
            }
        }
        
        foreach (ParticleSystem effect in weatherEffects)
        {
            if (effect != null)
            {
                effect.Stop();
            }
        }
        
        CancelInvoke();
    }
    
    // Método para ajustar calidad de efectos
    public void SetEffectsQuality(int quality)
    {
        float multiplier = quality switch
        {
            0 => 0.5f,  // Bajo
            1 => 0.75f, // Medio
            2 => 1f,    // Alto
            3 => 1.5f,  // Ultra
            _ => 1f
        };
        
        foreach (var effect in effectsCache.Values)
        {
            if (effect != null)
            {
                var main = effect.main;
                main.maxParticles = Mathf.RoundToInt(main.maxParticles * multiplier);
            }
        }
    }
}

public enum WeatherType
{
    Clear,
    Rain,
    Snow,
    Storm,
    Fog
}