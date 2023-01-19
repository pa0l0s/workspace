private string _shortGridName;

public string ShortGridName
{
    get { 
    if(string.IsNullOrEmpty(_shortGridName))
    {
        _shortGridName = GetShortGridName();
    }
                
    return _shortGridName;
     }
}

private IMyCockpit _cockpit;
public IMyCockpit Cockpit
{
    get {
        if(_cockpit==null)
        {
            _cockpit = GetBlock<IMyCockpit>();
            ConfigureTextSurface(_cockpit.GetSurface(0));
        }
        return _cockpit;
    }
}

public Program()
{
    // Set the update frequency to every 10 seconds
    Runtime.UpdateFrequency = UpdateFrequency.Update10;


}

public void Main(string argument)
{

    if (argument == "SETNAMES")
    {
        Echo($"Phase {ShortGridName}. will be added in front of functional blocks.");
        SetNames(NamingFunctionSetNames);
    }
    else if (argument == "UNSETNAMES")
    {
        Echo($"Remove front namepart with dot for functional blocks.");
        SetNames(NamingFunctionUnsetNames);
    }
    else if (argument == "SETHOME")
    {
        SetHome();
    }
    else
    {
        ShipManagement();
    }
}

private void ShipManagement()
{
    var firstScreenText = new StringBuilder();
    //firstScreenText.Append("\n");

    // Get the connector block
    var connector = GetBlock<IMyShipConnector>();
    if (connector == null)
    {
        Echo("Connector not found");
        return;
    }
    //Echo($"Connector found {connector.CustomName}");

    // Get all the battery blocks on the grid
    var batteries = GetBlocksOnSameGrid<IMyBatteryBlock>(); 
    if (batteries.Count>1)
    {
        batteries.RemoveAt(0); //One battery remain ucontrolled tu support programming block work
    }
    double avgStoredPower = GetAverageCurrentStoredPower(batteries);
    Echo(string.Format("Battery average power: {0:P}", avgStoredPower));
    // Get the first timer block on the grid
    var timerBlocks = GetBlocksOnSameGrid<IMyTimerBlock>(); 

    firstScreenText.Append(string.Format("Bp: {0:P}\n", avgStoredPower));

    var remoteController = GetBlock<IMyRemoteControl>();
    if (remoteController.IsAutoPilotEnabled)
    {
        List<MyWaypointInfo> waypoints = new List<MyWaypointInfo>();
        remoteController.GetWaypointInfo(waypoints);
        Vector3D currentPosition = remoteController.GetPosition();

        if(waypoints.Count>0)
        {
            MyWaypointInfo waypoint = waypoints[waypoints.Count-1]; //Get last waypoint
            double distance = (currentPosition - waypoint.Coords).Length();

            Echo(string.Format("Autopilot distance: {0:0.00}m", distance));
            firstScreenText.Append(string.Format("Home: {0:0.00}m\n", distance));

        }
    }

    // Check if the connector is locked
    if (connector.Status == MyShipConnectorStatus.Connected)
    {
        // Set the charge mode to "Recharge" if any battery has a level below 90%
        if (avgStoredPower < 0.8)
        {
            foreach (var battery in batteries)
            {
                battery.ChargeMode = ChargeMode.Recharge;
            }
        }
        else if (avgStoredPower > 0.95)
        {
            foreach (var battery in batteries)
            {
                battery.ChargeMode = ChargeMode.Auto;
            }

            // Get all the hydrogen engine blocks on the grid
            var hydrogenEngines = GetBlocksOnSameGrid<IMyPowerProducer>().Where(x => x.CustomName.Contains("Engine")).ToList();
            // Turn on the hydrogen engines
            foreach (var hydrogenEngine in hydrogenEngines)
            {
                //hydrogenEngine.Enabled = false;
            }

            var reactors = GetBlocksOnSameGrid<IMyReactor>();
            foreach (var reactor in reactors)
            {
                reactor.Enabled = false;
            }
        }
    }
    // If the connector is not locked, set the charge mode to "Auto"
    else
    {
        foreach (var battery in batteries)
        {
            battery.Enabled = true;
            battery.ChargeMode = ChargeMode.Auto;
        }
    }

    // Check if the battery level is below 50%
    if (avgStoredPower < 0.5)
    {
        // Get all the hydrogen engine blocks on the grid
        var hydrogenEngines = GetBlocksOnSameGrid<IMyPowerProducer>();//.Where(x => x.CustomName.Contains("Engine")).ToList();
        // Turn on the hydrogen engines
        foreach (var hydrogenEngine in hydrogenEngines)
        {
            hydrogenEngine.Enabled = true;
        }

        var reactors = GetBlocksOnSameGrid<IMyReactor>();
        foreach (var reactor in reactors)
        {
            reactor.Enabled = true;
        }

    }

    // Check if the battery level is below 20%
    if (avgStoredPower < 0.2)
    {
        // Get the Timer Block 
        var timerBlockDock = GetBlock<IMyTimerBlock>(blocks: timerBlocks);

        // Check if the Timer Block is active and counting
        if (timerBlockDock != null && timerBlockDock.IsCountingDown)
        {
            // Do not turn on the autopilot
            return;
        }

        // If the connector is not locked, turn on the autopilot
        if (connector.Status != MyShipConnectorStatus.Connected)
        {
            if (remoteController != null)
            {
                // Turn on the autopilot
                remoteController.SetAutoPilotEnabled(true);
            }
            else
            {
                Echo("No Remote Control");
            }
        }

        firstScreenText.Append("LOW POWER\n");
    }

    var firstScreen = Cockpit?.GetSurface(0);
    if(firstScreen!=null)
    {
        firstScreen.WriteText(firstScreenText.ToString());
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

private void SetNames(Func<string, string> namingFunction)
{
    //rename all terminal blocks
    RenameBlocksByType<IMyTerminalBlock>(namingFunction, true);
}

private void SetHome()
{
    // Code for setting home goes here

    
        // Get all the remote controller on the grid
        var remoteController = GetBlock<IMyRemoteControl>();

        if (remoteController != null)
        {
            Echo($"Using remote controller: {remoteController.CustomName} to set docking waypoints. Additionnal actions net to be set manually. Torn off Collision Avoidance on Approach 40 and start timmer block Dock at final waypoit Dock.");


            if(!remoteController.CustomName.EndsWith("Dock"))
                {
                    // Rename the remoteController block
                    remoteController.CustomName = $"{remoteController.CustomName}Dock";

                    // Setup remoteController
                    remoteController.SetCollisionAvoidance(true);
                    remoteController.SetDockingMode(true);
                    remoteController.FlightMode = FlightMode.OneWay;
                    remoteController.SpeedLimit = 100;
                }

            var gridNameCapitalLetters = GetShortGridName();

            remoteController.ClearWaypoints();

                    // Add a waypoint 300m above the current position of the remote controller
            var waypoint1 = new MyWaypointInfo(gridNameCapitalLetters + ".Appropach 300",
                remoteController.GetPosition() + remoteController.WorldMatrix.Up * 300);

            //waypoint1.Actions.Add(new MyWaypointAction("CollisionAvoidance_On", remoteController.EntityId));
            remoteController.AddWaypoint(waypoint1);

            // Add a waypoint 20m above the current position of the remote controller
            var waypoint2 = new MyWaypointInfo(gridNameCapitalLetters + ".Approach 40",
                remoteController.GetPosition() + remoteController.WorldMatrix.Up * 40);
            //waypoint2.Actions.Add(new MyWaypointAction("CollisionAvoidance_Off", remoteController.EntityId));
            remoteController.AddWaypoint(waypoint2);

            // Add a waypoint at the current position of the remote controller
            var waypoint3 = new MyWaypointInfo(gridNameCapitalLetters + ".Dock",
                remoteController.GetPosition());

            var timerBlockDock = GetBlock<IMyTimerBlock>();

            // Check if the timer block's name does not already end with "Dock"
            if (timerBlockDock != null )
            {
                if(!timerBlockDock.CustomName.EndsWith("Dock"))
                {
                    // Rename the timer block
                    timerBlockDock.CustomName = $"{timerBlockDock.CustomName}Dock";

                    //Configure timmer block here
                    timerBlockDock.Silent = true;
                    timerBlockDock.TriggerDelay = 1; //1s

                    //Echo($"{timerBlockDock.InventoryCount}");
                }

                    // Get the first connector on the grid
                    var connectorDock = GetBlock<IMyShipConnector>();

                    // Check if the connector's name does not already end with "Dock"
                    if (connectorDock != null)
                    {
                        if (!connectorDock.CustomName.EndsWith("Dock"))
                        {
                        // Rename the connector
                        connectorDock.CustomName = $"{connectorDock.CustomName}Dock";


                            // Configure the connector here
                        }


                    }

                    // Add an action to lock the connector when the timer block's timer runs out
                    //timerBlockDock.AddAction("Lock", connectorDock);

                    //waypoint3.Actions.Add(new MyWaypointAction("Start", timerBlockDock.EntityId));

                    // Add the action to the waypoint
                    //waypoint3.AddAction("Activate Timer", timerBlockDock.CustomName);
                
            }


            remoteController.AddWaypoint(waypoint3);
        }
        else
        {
            Echo("No remote controllers found");
        }
}

private string GetShortGridName()
{
            // Get the grid name
        var gridName = Me.CubeGrid.CustomName;

        // Initialize an empty string to store the capital letters and numbers
        string gridNameCapitalLetters = "";

        // Loop through each character in the grid name
        foreach (char c in gridName)
        {
            // If the character is a capital letter or a number, add it to the string
            if (char.IsUpper(c) || char.IsNumber(c))
            {
                gridNameCapitalLetters += c;
            }
        }

        // If the string of capital letters is empty, use the original grid name
        if (string.IsNullOrEmpty(gridNameCapitalLetters))
        {
            gridNameCapitalLetters = gridName;
        }

        return gridNameCapitalLetters;
}

private T GetBlock<T>(string nameEnd = "Dock", List<T> blocks = null) where T : class, IMyTerminalBlock
{
    if (blocks == null)
    {
        blocks = GetBlocksOnSameGrid<T>();
    }
    if (blocks.Any(x => x.CustomName.EndsWith(nameEnd)))
    {
        return blocks.Where(x => x.CustomName.EndsWith(nameEnd)).FirstOrDefault();
    }

    return blocks.FirstOrDefault();
}

public void RenameBlocksByType<T>(Func<string, string> namingFunction, bool isFunctional = true) where T : class, IMyTerminalBlock
{
    List<T> blocks = new List<T>();
    GridTerminalSystem.GetBlocksOfType(blocks);
    blocks = blocks.Where(x => x.IsSameConstructAs(Me) && x.IsFunctional == isFunctional).ToList();

    foreach (T block in blocks)
    {
        block.CustomName = namingFunction(block.CustomName);
    }
}

private List<T> GetBlocksOnSameGrid<T>() where T : class, IMyTerminalBlock
{
    var blocks = new List<T>();
    GridTerminalSystem.GetBlocksOfType(blocks);
    return blocks.Where(x => x.IsSameConstructAs(Me) && !x.CustomName.Contains("Ignore")).ToList();
}


private string NamingFunctionSetNames(string oldCustomName)
{
    if (!oldCustomName.Contains("."))
    {
        var sortName = ShortGridName;
        return $"{sortName}.{oldCustomName}";
    }
    return oldCustomName;
}

private string NamingFunctionUnsetNames(string oldCustomName)
{
    if (oldCustomName.Contains("."))
    {
        return oldCustomName.Split('.')[1];
    }
    return oldCustomName;
}

public void ConfigureTextSurface(IMyTextSurface surface)
{
    surface.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    surface.Font = "DEBUG";
    surface.FontSize = 1.8f;
    surface.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
    //surface.Padding = new VRage.Game.GUI.TextPanel.LinePadding(0.1f, 0.1f, 0.1f, 0.1f);
    surface.TextPadding = 5;
    surface.FontColor = new Color(250,150,50);

    //string CurrentlyShownImage
    surface.ClearImagesFromSelection();
    surface.AddImageToSelection("Clear");
}