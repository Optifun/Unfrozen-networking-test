namespace Chat.Shared
{
    public enum WSMessage
    {
        ReceiveLast20,
        Receive,
        UserJoined,
        UserQuit,
        SendMessage,
        StreamAllUsers,
        StreamOnlineUsers
    }
}