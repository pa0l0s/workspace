public Program()
{
    // Set the update frequency to never
    Runtime.UpdateFrequency = UpdateFrequency.None;
}

public void Main(string argument)
{
    // If no argument was provided, do nothing
    if (string.IsNullOrEmpty(argument)) return;

    // Get all the antenna blocks on the grid
    var antennas = new List<IMyRadioAntenna>();
    GridTerminalSystem.GetBlocksOfType(antennas);

    // Broadcast the argument through all the antenna blocks
    foreach (var antenna in antennas)
    {
        antenna.TransmitMessage(argument);
    }
}