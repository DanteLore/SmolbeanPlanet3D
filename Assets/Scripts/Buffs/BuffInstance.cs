using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

[Serializable]
public abstract class BuffInstance
{
    public bool isExpired = false;
    public int buffSpecIndex;

    private BuffSpec buffSpec;

    [JsonIgnore]
    public BuffSpec Spec
    {
        get { return buffSpec; }
        set
        {
            buffSpec = value;
            Debug.Assert(BuffController.Instance.BuffSpecs.Contains(buffSpec), $"BuffSpec is not registered with the global BuffController: {buffSpec.name}");
            buffSpecIndex = Array.IndexOf(BuffController.Instance.BuffSpecs, buffSpec);
        }
    }

    public abstract IEnumerable<BuffInstance> ApplyTo(AnimalStats stats, AnimalSpec species, float timeDelta);

    public virtual bool GetThought(AnimalStats stats, float timeDelta, out string thought)
    {
        if (Spec.thoughts.Length > 0)
        {
            var thoughtRow = Spec.thoughts[Random.Range(0, Spec.thoughts.Length - 1)];

            // OK, OK, this means the probability is actually lower than the configured value... but YOLO
            float p = 1 / thoughtRow.probabilitySeconds * timeDelta;
            if (Random.Range(0.0f, 1.0f) < p)
            {
                thought = thoughtRow.thought;
                return true;
            }
        }
        thought = "";
        return false;
    }
}
