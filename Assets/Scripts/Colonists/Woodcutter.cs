using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Woodcutter : ResourceGatherer
{
    protected override IEnumerable<GameObject> GetTargets(Vector3 pos)
    {
        var candidates = Physics.OverlapSphere(pos, 500, LayerMask.GetMask(natureLayer));

        return candidates
            .Select(c => c.gameObject)
            .Where(go => go.GetComponent<SmolbeanTree>() != null)
            .OrderBy(go => Vector3.SqrMagnitude(go.transform.position - pos));
    }

    protected override string GetGatheringTrigger()
    {
        return "StartChopping";
    }
}
