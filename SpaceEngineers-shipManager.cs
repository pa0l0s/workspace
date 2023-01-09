public Program()
{
    // Set the update frequency to every 10 seconds
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string argument)
{
    // Get the connector block
    var connector = GridTerminalSystem.GetBlockWithName("Connector AFV2") as IMyShipConnector;
    if (connector == null)
    {
        Echo("Connector not found");
        return;
    }
    Echo("Connector found");

    // Get all the battery blocks on the grid
    var batteries = new List<IMyBatteryBlock>();
    GridTerminalSystem.GetBlocksOfType(batteries);
    batteries=batteries.Where(x => x.CubeGrid == Me.CubeGrid).ToList();

    // Check if the connector is locked
    if (connector.Status == MyShipConnectorStatus.Connected)
    {
        // Set the charge mode to "Recharge" if any battery has a level below 90%
        foreach (var battery in batteries)
        {
            if (battery.CurrentStoredPower / battery.MaxStoredPower < 0.9)
            {
                battery.ChargeMode = ChargeMode.Recharge;
            }
        }
    }
    // If the connector is not locked, set the charge mode to "Auto"
    else
    {
        foreach (var battery in batteries)
        {
            battery.ChargeMode = ChargeMode.Auto;
        }
    }

    // Check if the battery level is below 50%
    var lowPower = false;
    foreach (var battery in batteries)
    {
        if (battery.CurrentStoredPower / battery.MaxStoredPower < 0.5)
        {
            lowPower = true;
            break;
        }
    }

    // If the battery level is below 50%, turn on the hydrogen engines
    if (lowPower)
    {
        // Get all the hydrogen engine blocks on the grid
        var hydrogenEngines = new List<IMyPowerProducer>();
        GridTerminalSystem.GetBlocksOfType(hydrogenEngines);
        hydrogenEngines=hydrogenEngines.Where(x => x.CubeGrid == Me.CubeGrid).ToList();
        // Turn on the hydrogen engines
        foreach (var hydrogenEngine in hydrogenEngines)
        {
            hydrogenEngine.Enabled = true;
        }
    }

    // Check if the battery level is below 20%
    if (batteries.Any(battery => battery.CurrentStoredPower / battery.MaxStoredPower < 0.2))
    {

        // Get the Timer Block named "AFV2.Timer Block Dock"
        var timerBlock = GridTerminalSystem.GetBlockWithName("AFV2.Timer Block Dock") as IMyTimerBlock;

        // Check if the Timer Block is active and counting
        if (timerBlock != null && timerBlock.IsCountingDown)
        {
            // Do not turn on the autopilot
            return;
        }

        // If the connector is not locked, turn on the autopilot
        if (connector.Status != MyShipConnectorStatus.Connected)
        {
            // Get the remote controller
            var remoteController = GridTerminalSystem.GetBlockWithName("Remote Control AFV2") as IMyRemoteControl;
            if (remoteController != null)
            {
                // Turn on the autopilot
                remoteController.SetAutoPilotEnabled(true);
            }
            else
            {
                Echo("No Remote Control AFV2");
            }
        }
    }
}
