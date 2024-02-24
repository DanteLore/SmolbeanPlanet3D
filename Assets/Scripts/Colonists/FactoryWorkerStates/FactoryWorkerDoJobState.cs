using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryWorkerDoJobState : IState
{
    private FactoryBuilding factory;
    private readonly DropController dropController;
    private FactoryWorker worker;
    private SoundPlayer soundPlayer;

    public FactoryWorkerDoJobState(FactoryWorker worker, FactoryBuilding factory, SoundPlayer soundPlayer, DropController dropController)
    {
        this.worker = worker;
        this.factory = factory;
        this.soundPlayer = soundPlayer;
        this.dropController = dropController;
    }

    public void OnEnter()
    {
        worker.Hide();
        soundPlayer.Play("Working");
        factory.StartProcessing();
    }

    public void OnExit()
    {
        worker.Show();
        soundPlayer.Stop("Working");
        factory.StopProcessing();

        var item = factory.recipe.createdItem;
        var qtty = factory.recipe.quantity;

        var drop = dropController.CreateInventoryItem(item, qtty);
        worker.Inventory.PickUp(drop);
    }

    public void Tick()
    {
        
    }
}
