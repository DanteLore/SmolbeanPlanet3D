using UnityEngine;

public class Windmill : FactoryBuilding
{
    public float sailRotationsPerSecond;
    public GameObject sail;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if(IsOperating)
            sail.transform.transform.Rotate(-Vector3.forward, sailRotationsPerSecond * 360f * Time.deltaTime);
    }

    public override void StartProcessing()
    {
        base.StartProcessing();
    }

    public override DropSpec StopProcessing()
    {
        return base.StopProcessing();
    }
}