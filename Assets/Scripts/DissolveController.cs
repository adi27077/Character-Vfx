using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class DissolveController : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMesh;
    public VisualEffect vfx;
    
    [Range(0, 0.1f)]
    public float dissolveRate = 0.0125f;
    [Range(0, 0.1f)]
    public float refreshRate = 0.025f;
    
    private Animator _animator;
    private Material[] _skinnedMeshMaterials;
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private static readonly int OnDie = Animator.StringToHash("Die");
    private static readonly int OnReset = Animator.StringToHash("Reset");
    
    private bool _alive = true;
    private bool _dissolving;

    // Start is called before the first frame update
    private void Start()
    {
        if (skinnedMesh != null)
        {
            _skinnedMeshMaterials = skinnedMesh.materials;
        }
        _animator = skinnedMesh.GetComponentInParent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Dissolve());
        }

        if (!Input.GetKeyDown(KeyCode.R) || _alive) return;
        if (_dissolving)
        {
            StopCoroutine(Dissolve());
            _dissolving = false;
        }
        StartCoroutine(Reset());
    }

    private IEnumerator Dissolve()
    {
        _alive = false;
        _dissolving = true;
        
        // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
        if (_animator != null)
        {
            _animator.SetTrigger(OnDie);
        }
        
        // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
        if (vfx != null)
        {
            vfx.Play();
        }
        
        if (_skinnedMeshMaterials.Length <= 0) yield break;
        float counter = 0;
        while (_skinnedMeshMaterials[0].GetFloat(DissolveAmount) < 1)
        {
            counter += dissolveRate;
            foreach (var material in _skinnedMeshMaterials)
            {
                material.SetFloat(DissolveAmount, counter);
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
    
    // ReSharper disable once Unity.IncorrectMethodSignature
    private IEnumerator Reset()
    {
        _alive = true;
        
        // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
        if (_animator != null)
        {
            _animator.SetTrigger(OnReset);
        }
        
        // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
        if (vfx != null)
        {
            vfx.Stop();
        }
        
        if (_skinnedMeshMaterials.Length <= 0) yield break;
        
        foreach (var material in _skinnedMeshMaterials)
        {
            material.SetFloat(DissolveAmount, 0);
        }

        yield return null;
    }
}
