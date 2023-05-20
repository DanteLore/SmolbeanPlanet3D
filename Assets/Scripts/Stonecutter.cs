using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Stonecutter : ResourceGatherer
{
    protected override IEnumerable<GameObject> GetTargets(Vector3 pos)
    {
        var candidates = Physics.OverlapSphere(pos, 500, LayerMask.GetMask(NatureLayer));

        return candidates
            .Select(c => c.gameObject)
            .Except(blacklist)
            .Where(go => go.GetComponent<SmolbeanRock>() != null)
            .OrderBy(go => Vector3.SqrMagnitude(go.transform.position - pos));
    }
}
