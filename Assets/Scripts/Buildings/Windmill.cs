using UnityEngine;

public class Windmill : FactoryBuilding
{
    public float sailRotationsPerSecond;
    public GameObject sail;
    public GameObject mainBody;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (IsOperating)
        {
            sail.transform.Rotate(Vector3.forward, sailRotationsPerSecond * -360f * Time.deltaTime);

            mainBody.transform.localRotation = Quaternion.Inverse(WindController.Instance.WindRotation);
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
