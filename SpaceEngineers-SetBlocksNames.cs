public void Main(string argument)
{
    if (argument == "SETNAMES")
    {
        SetNames();
    }
    else if (argument == "SETHOME")
    {
        SetHome();
    }
}

private void SetNames()
{
    // Code for setting names goes here

        

        var gridNameCapitalLetters = GetShortGridName();

        // Get all the connectors on the grid
        var connectors = new List<IMyShipConnector>();
        GridTerminalSystem.GetBlocksOfType(connectors);
        connectors=connectors.Where(x => x.CubeGrid == Me.CubeGrid).ToList();

        // Rename the connectors
        foreach (var connector in connectors)
        {
            if (!connector.CustomName.Contains("."))
            {
                connector.CustomName = $"{gridNameCapitalLetters}.{connector.CustomName}";
            }
        }

        // Get all the turrets on the grid
        var turrets = new List<IMyLargeTurretBase>();
        GridTerminalSystem.GetBlocksOfType(turrets);
        turrets=turrets.Where(x => x.CubeGrid == Me.CubeGrid).ToList();

        // Rename the turrets
        foreach (var turret in turrets)
        {
            if (!turret.CustomName.Contains("."))
            {
                turret.CustomName = $"{gridNameCapitalLetters}.{turret.CustomName}";
            }
        }

        // // Create a new group for the turrets
        // var group = GridTerminalSystem.CreateBlockGroup();

        // // Set the group's name to "Turrets"
        // group.Name = "Turrets";

        // // Add the turrets to the group
        // //group.AddRange(turrets);
        // // Add the turrets to the group
        // foreach (var turret in turrets)
        // {
        //     group.AddBlock(turrets);
        // }

        // Get all the batteries on the grid
        var batteries = new List<IMyBatteryBlock>();
        GridTerminalSystem.GetBlocksOfType(batteries);
        batteries=batteries.Where(x => x.CubeGrid == Me.CubeGrid).ToList();

        // Rename the batteries
        foreach (var battery in batteries)
        {
            if (!battery.CustomName.Contains("."))
            {
                battery.CustomName = $"{gridNameCapitalLetters}.{battery.CustomName}";
            }
        }

        // Get all the remote controllers on the grid
        var remoteControllers = new List<IMyRemoteControl>();
        GridTerminalSystem.GetBlocksOfType(remoteControllers);
        remoteControllers=remoteControllers.Where(x => x.CubeGrid == Me.CubeGrid).ToList();

        // Rename the remote controllers
        foreach (var remoteController in remoteControllers)
        {
            // Check if the remote controller's name already has a dot
            if (!remoteController.CustomName.Contains("."))
            {
                remoteController.CustomName = $"{gridNameCapitalLetters}.{remoteController.CustomName}";
            }
        }

        // Get all the hydrogen tanks on the grid
        var hydrogenTanks = new List<IMyGasTank>();
        GridTerminalSystem.GetBlocksOfType(hydrogenTanks);
        hydrogenTanks=hydrogenTanks.Where(x => x.CubeGrid == Me.CubeGrid).ToList();

        // Rename the hydrogen tanks
        foreach (var hydrogenTank in hydrogenTanks)
        {
            if (!hydrogenTank.CustomName.Contains("."))
            {
                hydrogenTank.CustomName = $"{gridNameCapitalLetters}.{hydrogenTank.CustomName}";
            }
        }

        // Get all the programming blocks on the grid
        var programmingBlocks = new List<IMyProgrammableBlock>();
        GridTerminalSystem.GetBlocksOfType(programmingBlocks);
        programmingBlocks=programmingBlocks.Where(x => x.CubeGrid == Me.CubeGrid).ToList();

        // Rename the programming blocks
        foreach (var programmingBlock in programmingBlocks)
        {
            if (!programmingBlock.CustomName.Contains("."))
            {
                programmingBlock.CustomName = $"{gridNameCapitalLetters}.{programmingBlock.CustomName}";
            }
        }

        // Get all the timer blocks on the grid
        var timerBlocks = new List<IMyTimerBlock>();
        GridTerminalSystem.GetBlocksOfType(timerBlocks);
        timerBlocks=timerBlocks.Where(x => x.CubeGrid == Me.CubeGrid).ToList();

        // Rename the timer blocks
        foreach (var timerBlock in timerBlocks)
        {
        if (!timerBlock.CustomName.Contains("."))
            {
                timerBlock.CustomName = $"{gridNameCapitalLetters}.{timerBlock.CustomName}";
            }
        }

        // Get all the gyroscopes on the grid
        var gyroscopes = new List<IMyGyro>();
        GridTerminalSystem.GetBlocksOfType(gyroscopes);
        gyroscopes=gyroscopes.Where(x => x.CubeGrid == Me.CubeGrid).ToList();

        // Rename the gyroscopes
        foreach (var gyroscope in gyroscopes)
        {
            if (!gyroscope.CustomName.Contains("."))
            {
                gyroscope.CustomName = $"{gridNameCapitalLetters}.{gyroscope.CustomName}";
            }
        }

        // Get all the gatling guns on the grid
        var gatlingGuns = new List<IMyUserControllableGun>();
        GridTerminalSystem.GetBlocksOfType(gatlingGuns);
        gatlingGuns=gatlingGuns.Where(x => x.CubeGrid == Me.CubeGrid).ToList();

        // Rename the gatling guns
        foreach (var gatlingGun in gatlingGuns)
        {
        if (!gatlingGun.CustomName.Contains("."))
            {
                gatlingGun.CustomName = $"{gridNameCapitalLetters}.{gatlingGun.CustomName}";
            }
        }

        // Get all the cockpits on the grid
        var cockpits = new List<IMyCockpit>();
        GridTerminalSystem.GetBlocksOfType(cockpits);
        cockpits = cockpits.Where(x => x.CubeGrid == Me.CubeGrid).ToList();

        // Rename the cockpits
        foreach (var cockpit in cockpits)
        {
            if (!cockpit.CustomName.Contains("."))
            {
                cockpit.CustomName = $"{gridNameCapitalLetters}.{cockpit.CustomName}";
            }
        }

        // Get all the cameras on the grid
        var cameras = new List<IMyCameraBlock>();
        GridTerminalSystem.GetBlocksOfType(cameras);
        cameras = cameras.Where(x => x.CubeGrid == Me.CubeGrid).ToList();

        // Rename the cameras
        foreach (var camera in cameras)
        {
            if (!camera.CustomName.Contains("."))
            {
                camera.CustomName = $"{gridNameCapitalLetters}.{camera.CustomName}";
            }
        }
    
}

private void SetHome()
{
    // Code for setting home goes here

    
        // Get all the remote controllers on the grid
        var remoteControllers = new List<IMyRemoteControl>();
        IMyRemoteControl remoteController = null;
        GridTerminalSystem.GetBlocksOfType(remoteControllers);

        // Filter the list to include only remote controllers on the same grid as the programmable block
        remoteControllers = remoteControllers.Where(x => x.CubeGrid == Me.CubeGrid).ToList();

        // Get the first remote controller
        if (remoteControllers.Count > 0)
        {
            remoteController = remoteControllers[0];
            Echo($"remoteController.CustomName {remoteController.CustomName}");

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
            var waypoint2 = new MyWaypointInfo(gridNameCapitalLetters + ".Approach 30",
                remoteController.GetPosition() + remoteController.WorldMatrix.Up * 30);
            //waypoint2.Actions.Add(new MyWaypointAction("CollisionAvoidance_Off", remoteController.EntityId));
            remoteController.AddWaypoint(waypoint2);

            // Add a waypoint at the current position of the remote controller
            var waypoint3 = new MyWaypointInfo(gridNameCapitalLetters + ".Dock",
                remoteController.GetPosition());

            // Get the first timer block on the grid
            var timerBlocks = new List<IMyTimerBlock>();
            GridTerminalSystem.GetBlocksOfType(timerBlocks);
            var timerBlockDock = timerBlocks.FirstOrDefault();

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

                    Echo($"{timerBlockDock.InventoryCount}");

                    // Get the first connector on the grid
                    var connectors = new List<IMyShipConnector>();
                    GridTerminalSystem.GetBlocksOfType(connectors);
                    var connectorDock = connectors.FirstOrDefault();

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
            }


            remoteController.AddWaypoint(waypoint3);
        }
        else
        {
            Echo("No remote controllers found");
        }
}

string GetShortGridName()
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

        Echo(gridNameCapitalLetters);
        return gridNameCapitalLetters;
}
