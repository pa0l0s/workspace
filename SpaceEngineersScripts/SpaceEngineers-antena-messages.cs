public void Main(string argument)
{
    // Get a list of all antenna blocks on the grid
    List<IMyRadioAntenna> antennas = new List<IMyRadioAntenna>();
    GridTerminalSystem.GetBlocksOfType(antennas);

    // Find the first antenna in the current construct
    IMyRadioAntenna antenna = antennas.Where(x => x.IsSameConstructAs(Me)).FirstOrDefault();

    // Make sure the antenna is valid
    if (antenna == null)
    {
        Echo("Antenna not found!");
        return;
    }

    // Set the antenna to receive mode
    antenna.EnableBroadcasting = false;
    //antenna.EnableTransmission = true;

    // Create a list to store the received messages
    List<MyTuple<string, string>> messages = new List<MyTuple<string, string>>();

    // Read all the incoming messages
    antenna.GetFullInbox(messages);

    // Print the count of received messages to the LCD panel
    Echo(messages.Count + " messages received:");

    // Loop through all the received messages and print them to the LCD panel
    for (int i = 0; i < messages.Count; i++)
    {
        Echo(messages[i].Item1 + ": " + messages[i].Item2);
    }
}
