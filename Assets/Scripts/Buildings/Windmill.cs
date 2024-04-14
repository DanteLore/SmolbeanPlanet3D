using UnityEngine;

public class Windmill : FactoryBuilding
{
    public float sailRotationsPerSecond;
    public GameObject sail;
    public GameObject mainBody;
    public float maxBodyRotationDegreesPerSecond = 5f;
    public float heightAtWhichMaxSpeed = 30f;
    public float maxSpeed = 4f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (IsOperating)
        {
            sail.transform.Rotate(Vector3.forward, sailRotationsPerSecond * Speed * -360f * Time.deltaTime);

            mainBody.transform.rotation = Quaternion.RotateTowards(mainBody.transform.rotation, WindController.Instance.WindRotation, maxBodyRotationDegreesPerSecond * Time.deltaTime);
        }
    }

    public override void StartProcessing()
    {
        base.StartProcessing();
        SetSpeed();
    }

    public override DropSpec StopProcessing()
    {
        return base.StopProcessing();
    }

    private void SetSpeed()
    {
        float y = transform.position.y;
        float t = Mathf.InverseLerp(0f, heightAtWhichMaxSpeed, y);
        Speed = Mathf.Lerp(1f, maxSpeed, t);
    }
}
