using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

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
    public abstract IEnumerable<BuffInstance> ApplyTo(AnimalStats stats, float timeDelta);
}
