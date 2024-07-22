using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SearchForShootingSpotState : IState
{
    private const int MAX_COLLIDERS = 10;
    private static readonly RaycastHit[] hits = new RaycastHit[MAX_COLLIDERS];

    private readonly Hunter hunter;
    private readonly GridManager gridManager;
    private readonly float shootHeight;
    private readonly Vector3 targetPointOffset;
    private readonly string natureLayer;
    private readonly string groundLayer;
    private readonly float shotDistance;

    private const int MAX_TRIES = 360;
    private int tryCount = 0;
    private Vector3 shootPosition;
    private float rotation = 0f;

    public bool InProgress { get; private set; }
    public bool Found { get; private set; }

    public SearchForShootingSpotState(Hunter hunter, GridManager gridManager, float shootHeight, Vector3 targetPointOffset, string natureLayer, string groundLayer, float shotDistance)
    {
        this.hunter = hunter;
        this.gridManager = gridManager;
        this.shootHeight = shootHeight;
        this.targetPointOffset = targetPointOffset;
        this.natureLayer = natureLayer;
        this.groundLayer = groundLayer;
        this.shotDistance = shotDistance;
    }

    public void OnEnter()
    {
        InProgress = true;
        Found = false;

        rotation = 0f;
        tryCount = 0;
        shootPosition = hunter.transform.position;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        if(hunter.Prey == null)
        {
            InProgress = false;
        }
        else if(ShootingPositionValid(shootPosition, hunter.Prey.transform.position + targetPointOffset))
        {
            hunter.Target = shootPosition;
            hunter.Think("Stalking my prey");
            InProgress = false;
            Found = true;
        }
        else if(tryCount++ >= MAX_TRIES)
        {
            hunter.Think("Couldn't find shooting position");
            InProgress = false;
        }
        else
        {
            // Pick a better position for next time
            SelectShootPosition(hunter.Prey.transform.position + targetPointOffset);
        }
    }

    private void SelectShootPosition(Vector3 targetPosition)
    {
        // Rotate around a bit
        rotation += Random.Range(2f, 10f) % 360f; 
        var rot = Quaternion.AngleAxis(rotation, Vector3.up);
        Vector3 vectorFromTarget = (hunter.transform.position - targetPosition).normalized;
        Vector3 rotatedVector = rot * vectorFromTarget;
        float range = Random.Range(shotDistance * 0.3f, shotDistance * 0.9f);
        shootPosition = targetPosition + (rotatedVector * range);

        // Clamp to the ground
        float y = gridManager.GetGridHeightAt(shootPosition.x, shootPosition.z);
        shootPosition = new Vector3(shootPosition.x, y, shootPosition.z);

        // Find nearest spot on the navmesh
        if(NavMesh.SamplePosition(shootPosition, out var hit, 20f, NavMesh.AllAreas))
            shootPosition = hit.position;
    }

    private bool ShootingPositionValid(Vector3 shootPosition, Vector3 targetPosition)
    {
        var arrowStartPosition = shootPosition + Vector3.up * shootHeight;

        // Because the y coord will have changed, the shot distance check needs to have some leeway as we may be further 
        // away LOS than in pure x/z terms.  Also impose a minimum
        float dist = Vector3.Distance(targetPosition, arrowStartPosition);
        if(dist > shotDistance * 1.2f || dist < shotDistance * 0.5f)
            return false; // Too far away or too close

        // Check LOS
        return 0 == Physics.SphereCastNonAlloc(arrowStartPosition, 0.1f, (targetPosition - arrowStartPosition).normalized, hits, dist, LayerMask.GetMask(natureLayer, groundLayer));
    }
}
