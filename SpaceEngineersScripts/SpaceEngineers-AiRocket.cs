public Program()
{
    // Set the update frequency to every 10 seconds
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

// private IMyCockpit _cockpit;
// public IMyCockpit Cockpit
// {
//     get {
//         if(_cockpit==null)
//         {
//             _cockpit = GetBlock<IMyCockpit>();
//             ConfigureTextSurface(_cockpit.GetSurface(0));
//         }
//         return _cockpit;
//     }
// }

public Vector3D? LauchchCoords {get; set;}

public void Main(string argument)
{
    if (argument == "SET")
    {
        Set();
    }
    else if (argument == "LAUNCH")
    {
        Launch();
    }

    Manage();
}

private void Manage()
{
    //List<MyWaypointInfo> waypoints = new List<MyWaypointInfo>();
    //remoteController.GetWaypointInfo(waypoints);

    StringBuilder sbHudText = new StringBuilder();
    float distance = 10000f;

    if(LauchchCoords.HasValue)
    {
        var remoteController = GetBlocksOnSameGrid<IMyRemoteControl>().FirstOrDefault(x=>x.IsFunctional);
        if(remoteController!=null)
        {
            Vector3D currentPosition = remoteController.GetPosition();
            distance = (float)Vector3D.Distance(currentPosition,LauchchCoords.Value);
            Echo(string.Format("Autopilot distance: {0:0.00}m", distance));
            sbHudText.Append(string.Format("D: {0:0.00}m", distance+200));  
        } 
    }

    var tanks = GetBlocksOnSameGrid<IMyGasTank>();
    if(tanks.Any())
    {
        sbHudText.Append(string.Format(", Gt: {0:P}", GetAverageGasTanksFilledRatio(tanks)));
    }

    var batteries = GetBlocksOnSameGrid<IMyBatteryBlock>();
    if(batteries.Any())
    {
        sbHudText.Append(string.Format(", Bp: {0:P}", GetAverageCurrentStoredPower(batteries)));
    }

    SetAntennaMessage(sbHudText,distance);
}

private void Set()
{
    var tanks = GetBlocksOnSameGrid<IMyGasTank>();
    Echo($"Gas tanks: {tanks.Count}");

    foreach(var tank in tanks)
    {
        tank.Stockpile=true;
    }

    var batteries = GetBlocksOnSameGrid<IMyBatteryBlock>();
    foreach (var battery in batteries)
    {
        battery.ChargeMode = ChargeMode.Recharge;
    }

    var gyroes = GetBlocksOnSameGrid<IMyGyro>();
    foreach (var gyro in gyroes)
    {
        gyro.GyroOverride = true;
    }

    var thrusters = GetBlocksOnSameGrid<IMyThrust>();
    foreach (var thruster in thrusters)
    {
        thruster.ApplyAction("OnOff_Off");
    }
}

private void Launch()
{
    var tanks = GetBlocksOnSameGrid<IMyGasTank>();
    Echo($"Gas tanks: {tanks.Count}");

    foreach(var tank in tanks)
    {
        tank.Stockpile=false;
    }

    var batteries = GetBlocksOnSameGrid<IMyBatteryBlock>();
    foreach (var battery in batteries)
    {
        battery.ChargeMode = ChargeMode.Auto;
    }

    var gyroes = GetBlocksOnSameGrid<IMyGyro>();
    foreach (var gyro in gyroes)
    {
        gyro.GyroOverride = true;
    }

    var remoteController = GetBlocksOnSameGrid<IMyRemoteControl>().FirstOrDefault(x=>x.IsFunctional);
    if (remoteController!=null)
    {
        LauchchCoords = remoteController.GetPosition();
    }

    var timmer = GetBlocksOnSameGrid<IMyTimerBlock>().FirstOrDefault(x=>x.IsFunctional);
    if (timmer!=null)
    {
        //start timmer
        timmer.ApplyAction("Start");
    }

    var thrusters = GetBlocksOnSameGrid<IMyThrust>();
    foreach (var thruster in thrusters)
    {
        thruster.ThrustOverridePercentage = 1f;
        thruster.ApplyAction("OnOff_On");
    }
}

private List<T> GetBlocksOnSameGrid<T>() where T : class, IMyTerminalBlock
{
    var blocks = new List<T>();
    GridTerminalSystem.GetBlocksOfType(blocks);
    return blocks.Where(x => x.IsSameConstructAs(Me) && x.CubeGrid == Me.CubeGrid).ToList();
}

private void SetAntennaMessage(StringBuilder sbHudText, float antennaAttentionRadius=10000f)
{
            var antennas = GetBlocksOnSameGrid<IMyRadioAntenna>();
            var antenna = antennas.FirstOrDefault(x => x.IsFunctional);
            if (antenna != null)
            {
                antenna.SetValue("HudText", sbHudText);
                antenna.Radius = antennaAttentionRadius;
            }

}

private double GetAverageCurrentStoredPower(List<IMyBatteryBlock> batteries)
{
    double totalStoredPower = 0;
    foreach (var battery in batteries)
    {
        totalStoredPower += (battery.CurrentStoredPower/battery.MaxStoredPower);
    }
    return totalStoredPower / batteries.Count;
}

private double GetAverageGasTanksFilledRatio(List<IMyGasTank > gasTanks)
{
    double totalFilledRatio = 0;
    foreach (var gasTank in gasTanks)
    {
        totalFilledRatio += gasTank.FilledRatio;
    }
    return totalFilledRatio / gasTanks.Count;
}