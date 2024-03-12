using UnityEngine;

[CreateAssetMenu(fileName = "JobSpec", menuName = "Smolbean/Job Spec", order = 3)]
public class JobSpec : ScriptableObject
{
    public string jobName;
    public Texture2D thumbnail;
}
