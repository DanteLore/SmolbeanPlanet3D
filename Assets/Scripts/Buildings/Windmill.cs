using UnityEngine;

public class Windmill : FactoryBuilding
{
    public float sailRotationsPerSecond;
    public GameObject sail;
    public GameObject mainBody;
    public float maxBodyRotationDegreesPerSecond = 5f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (IsOperating)
        {
            sail.transform.Rotate(Vector3.forward, sailRotationsPerSecond * -360f * Time.deltaTime);

            mainBody.transform.localRotation = Quaternion.RotateTowards(mainBody.transform.localRotation, WindController.Instance.WindRotation, maxBodyRotationDegreesPerSecond * Time.deltaTime);
        }
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
