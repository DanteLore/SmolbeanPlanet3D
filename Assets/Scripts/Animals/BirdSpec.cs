using UnityEngine;

[CreateAssetMenu(fileName = "AnimalSpec", menuName = "Smolbean/Bird Spec", order = 2)]
public class BirdSpec : AnimalSpec
{
    [Header("Basic")]
    public DropSpec eggDropSpec;
    public GameObject eggLaidParticleSystem;

    [Header("Reproduction")]
    public float chickGestationSeconds;
}
