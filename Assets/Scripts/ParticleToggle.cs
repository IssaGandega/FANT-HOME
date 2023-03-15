using UnityEngine;

public class ParticleToggle : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        Pooler.instance.Depop(gameObject.name, gameObject);
    }
}
