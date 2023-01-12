public void Main(string argument)
{
    if (argument == "DOWN")
    {
        MinerGoDown();
    }
        if (argument == "GYROES")
    {
        ResetGyroes();
    }
}


private void MinerGoDown(float metters = 20)
{
        // Get all the remote controllers on the grid
    var remoteControllers = new List<IMyRemoteControl>();
    GridTerminalSystem.GetBlocksOfType(remoteControllers);

    // Get the first remote controller
    if (remoteControllers.Count > 0)
    {
        IMyRemoteControl remoteController = remoteControllers[0];

        remoteController.ClearWaypoints();

        // Set the speed limit of the remote controller to 0.5 m/s
        remoteController.SpeedLimit = 0.5f;

        // Set collision avoidance and docking mode to off
        remoteController.SetCollisionAvoidance(false);
        remoteController.SetDockingMode(true);

        // Set the remote controller's flight mode to OneWay
        remoteController.FlightMode = FlightMode.OneWay;

        // Add a waypoint meters below the current position of the remote controller
        remoteController.AddWaypoint((Vector3D)(remoteController.GetPosition() + (remoteController.WorldMatrix.Down * metters)+remoteController.WorldMatrix.Forward),
            Me.CustomName + ".Waypoint");

        // Turn on the autopilot
        remoteController.SetAutoPilotEnabled(true);

                // Get all the gyroscopes on the grid
        var gyroscopes = new List<IMyGyro>();
        GridTerminalSystem.GetBlocksOfType(gyroscopes);
        gyroscopes=gyroscopes.Where(x => x.IsSameConstructAs(Me)).ToList();

        // Rename the gyroscopes
        foreach (var gyroscope in gyroscopes)
        {
            gyroscope.GyroPower=0;
        }
    }
    else
    {
        Echo("No remote controllers found");
    }
}

private void ResetGyroes()
{
                    // Get all the gyroscopes on the grid
        var gyroscopes = new List<IMyGyro>();
        GridTerminalSystem.GetBlocksOfType(gyroscopes);
        gyroscopes=gyroscopes.Where(x => x.IsSameConstructAs(Me)).ToList();

        // Rename the gyroscopes
        foreach (var gyroscope in gyroscopes)
        {
            gyroscope.GyroPower=1;
        }
}