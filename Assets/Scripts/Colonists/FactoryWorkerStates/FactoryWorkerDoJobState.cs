using UnityEngine;

public class FactoryWorkerDoJobState : IState
{
    private readonly DropController dropController;
    private readonly FactoryWorker worker;
    private readonly SoundPlayer soundPlayer;

    protected FactoryBuilding Factory
    {
        get
        {
            Debug.Assert(worker.Job != null, "Should not get into this state if the colonist has no job!");
            return (FactoryBuilding)worker.Job.Building;
        }
    }

    public FactoryWorkerDoJobState(FactoryWorker worker, SoundPlayer soundPlayer, DropController dropController)
    {
        this.worker = worker;
        this.soundPlayer = soundPlayer;
        this.dropController = dropController;
    }

    public void OnEnter()
    {

        worker.Hide();
        soundPlayer.Play("Working");

        Factory.StartProcessing();
    }

    public void OnExit()
    {
        worker.Show();
        soundPlayer.Stop("Working");
        Factory.StopProcessing();

        var item = Factory.recipe.createdItem;
        var qtty = Factory.recipe.quantity;

        var drop = dropController.CreateInventoryItem(item, qtty);
        worker.Inventory.PickUp(drop);
    }

    public void Tick()
    {
        
    }
}
