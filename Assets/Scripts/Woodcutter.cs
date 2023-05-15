using UnityEngine;
using System.Linq;

public class Woodcutter : ResourceGatherer
{
    protected override GameObject GetTarget(Vector3 pos)
    {
        var candidates = Physics.OverlapSphere(pos, 500, LayerMask.GetMask(NatureLayer));

        return candidates
            .Select(c => c.gameObject)
            .Except(blacklist)
            .Where(go => go.GetComponent<SmolbeanTree>() != null)
            .OrderBy(go => Vector3.SqrMagnitude(go.transform.position - pos))
            .FirstOrDefault();
    }
}
