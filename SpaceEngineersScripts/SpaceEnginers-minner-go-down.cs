public void Main(string argument)
{
    if (argument == "DOWN")
    {
        MinerGoDown();
    }
    else if (argument == "GYROES")
    {
        ResetGyroes();
    }
    else if (argument == "GETWAYPOINT")
    {
        IMyRemoteControl remoteController = GetRemoteControl();

        List<MyWaypointInfo> waypoints = new List<MyWaypointInfo>();
        remoteController.GetWaypointInfo(waypoints);
        Vector3D currentPosition = remoteController.GetPosition();
        int i = 1;
        foreach(MyWaypointInfo waypoint in waypoints)
        {
            Echo($"Waypoint {i}:");
            Echo($"Name: {waypoint.Name}");
            Echo($"Coordinates: {waypoint.Coords.X}, {waypoint.Coords.Y}, {waypoint.Coords.Z}");
            double distance = (currentPosition - waypoint.Coords).Length();
            Echo($"Distance from current location: {distance}m");
            i++;
        }
    }

    AutopilotGyroesControll();
}


private void MinerGoDown(float metters = 20)
{
    IMyRemoteControl remoteController = GetRemoteControl();

    // Get the first remote controller
    if (remoteController != null)
    {
        remoteController.ClearWaypoints();

        // Set the speed limit of the remote controller to 0.5 m/s
        remoteController.SpeedLimit = 0.5f;

        // Set collision avoidance and docking mode to off
        remoteController.SetCollisionAvoidance(false);
        remoteController.SetDockingMode(true);

        // Set the remote controller's flight mode to OneWay
        remoteController.FlightMode = FlightMode.OneWay;

        // Add a waypoint meters below the current position of the remote controller
        remoteController.AddWaypoint((Vector3D)(remoteController.GetPosition() + (remoteController.WorldMatrix.Down * metters)),
            Me.CustomName + ".Waypoint");

        SetGyroesPower(0);

        // Turn on the autopilot
        remoteController.SetAutoPilotEnabled(true);

        //Enable programmable block to check if autopilot finished 
        this.Runtime.UpdateFrequency = UpdateFrequency.Update10;
    }
    else
    {
        Echo("No remote controllers found");
    }
}

private void AutopilotGyroesControll()
{
    IMyRemoteControl remoteController = GetRemoteControl();

    if (remoteController.IsAutoPilotEnabled)
    {
        List<MyWaypointInfo> waypoints = new List<MyWaypointInfo>();
        remoteController.GetWaypointInfo(waypoints);
        Vector3D currentPosition = remoteController.GetPosition();

        if(waypoints.Count == 1)
        {
            MyWaypointInfo waypoint = waypoints[0];
            double distance = (currentPosition - waypoint.Coords).Length();
            if(distance > 0.5)
            {
                remoteController.SetAutoPilotEnabled(true);
                Echo("DIGGING!");
                SetGyroesPower(0);
            }
            else
            {
                remoteController.SetAutoPilotEnabled(false);
                Echo("DIG finished reseting gyroes");
                ResetGyroes();
            }
        }
    }
    else
    {
        // Autopilot is not engaged
        ResetGyroes();
        Echo("Autopilot finished DIGGING reseting gyroes");
        this.Runtime.UpdateFrequency = UpdateFrequency.None;
    }
}
private void ResetGyroes()
{
    SetGyroesPower(1);
}

private void SetGyroesPower(float power)
{
        // Get all the gyroscopes on the grid
        var gyroscopes = new List<IMyGyro>();
        GridTerminalSystem.GetBlocksOfType(gyroscopes);
        gyroscopes=gyroscopes.Where(x => x.IsSameConstructAs(Me)).ToList();

        // Rename the gyroscopes
        foreach (var gyroscope in gyroscopes)
        {
            gyroscope.GyroPower=power;
        }
}

private IMyRemoteControl GetRemoteControl()
{
    // Get all the remote controllers on the grid
    var remoteControllers = new List<IMyRemoteControl>();
    GridTerminalSystem.GetBlocksOfType(remoteControllers);
    remoteControllers=remoteControllers.Where(x => x.IsSameConstructAs(Me)).ToList();

    // Get the first remote controller
    return remoteControllers.FirstOrDefault(x=>x.IsFunctional);
}