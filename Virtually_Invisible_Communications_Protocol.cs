//Script made by Lelebees: Last update at 26-03-2022 16:12 UTC+1 (Central European Time)
//Virtually Invisible Communications Protocol V1.0

string antennaName = "Antenna";

IEnumerator<bool> stateMachine;
IMyRadioAntenna antenna;

public Program()
{
    // Initialize the script here
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    // Initialize an antenna so we can use the script
    antenna = GridTerminalSystem.GetBlockWithName(antennaName) as IMyRadioAntenna;
}

public void RunStateMachine()
{   
    // This is the code from the MDK Wiki. If you want to learn more about coroutines (and scripting in general), go there!
    if (stateMachine != null) 
    {
        bool hasMoreSteps = stateMachine.MoveNext();

        if (hasMoreSteps)
        {
            Runtime.UpdateFrequency |= UpdateFrequency.Once;
        } 
        else 
        {
            stateMachine.Dispose();
            stateMachine = null;
        }
    }
}

public void Main(string arg, UpdateType updateType)
{
    // If we've told the system to run a statemachine
    if ((updateType & UpdateType.Once) == UpdateType.Once) 
    {
        RunStateMachine();
    }
    else
    {
        // Run the normal script
        // This is an example on how to run the send method. you need both lines.
        stateMachine = Send("BroadcastChannel", "MessageText", TransmissionDistance.TransmissionDistanceMax);
        Runtime.UpdateFrequency |= UpdateFrequency.Once;
        // The line above will tell the system to run once more in the next tick, so we get to activate the if statement above
        // PLEASE NOTE: The code above will run every tick. You probably don't want that, but this is a working example.
    }
}

public IEnumerator<bool> Send(string broadcastChannel, string messageText, TransmissionDistance transmissionDistance) 
{
    // Do something here. You want to minimize the amount of things the script is doing while the antenna is on, so the signal is on for as little time as possible.
    
    //Everything has been prepared, we can now open the broadcasting channel
    antenna.EnableBroadcasting = true;
    yield return true; //wait* a tick
    //Send the message
    IGC.SendBroadcastMessage(broadcastChannel, messageText, transmissionDistance);  
    yield return true; //wait* a tick
    yield return true; //wait another tick so this fucking thing actually sends
    //Close the broadcast so detection is now impossible
    antenna.EnableBroadcasting = false;
    // Message has been sent and broadcast has stopped.
    
    // *The program doesn't actually wait. It stops and runs from that point on the next tick. 
    // this lowers the instruction count, making the script more preformance friendly, however, 
    // we're using it to make a delay inbetween  actions.
}

public void Receive(string broadcastChannel)
{
    // We create some IGC stuff here so we can receive stuff
    IGC.RegisterBroadcastListener(broadcastChannel);
    List<IMyBroadcastListener> listenerList = new List<IMyBroadcastListener>();
    IGC.GetBroadcastListeners(listenerList);

    if (listenerList[0].HasPendingMessage)
    {
        MyIGCMessage receivedMessage = new MyIGCMessage();
        receivedMessage = listenerList[0].AcceptMessage();
        // Take out the message data
        string receivedText = receivedMessage.Data.ToString();
        string receivedChannel = receivedMessage.Tag;
        long messageSender = receivedMessage.Source;
        // Do something with all of this stuff!
    }
}