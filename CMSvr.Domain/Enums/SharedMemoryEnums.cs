namespace CMSvr.Domain.Enums
{
    public enum UserMode
    {
        User = 0,
        Manager = 1,
        Developer = 2
    }

    public enum ConnectStatus
    {
        Not = 0,
        Pending = 1,
        Connected = 2
    }

    public enum PaRunStatus
    {
        Idle = 0,
        Run = 1,
        Unknown = 2,
        Error = 3,
        Pause = 4
    }

    public enum Axis
    {
        X = 0,
        Y = 1,
        Z = 2,
        A = 3,
        B = 4,
        Num = 5
    }

    public enum Coordinate
    {
        G53 = 0,
        G54 = 1,
        Num = 2
    }

    public enum RunMode
    {
        Stop = 0,
        ToStop = 1,
        ToRun = 2,
        Run = 3,
        Init = 4,
        Error = 5,
        Pause = 6
    }

    public enum NcFileState
    {
        Before = 0,
        Running = 1,
        Complete = 2,
        Stop = 3,
        Error = 4
    }

    public enum EN_IPC_COMMAND : byte
    {
        IPC_COMMAND_EMG = 1,
        IPC_COMMAND_RESET = 2,
        IPC_COMMAND_HOME = 3,
        IPC_COMMAND_MODE_SELECT = 4,
        IPC_COMMAND_OPEN = 5,
        IPC_COMMAND_CLOSE = 6,
        IPC_COMMAND_RUN = 7,
        IPC_COMMAND_PAUSE = 8,
        IPC_COMMAND_STOP = 9,
        IPC_COMMAND_OPERMODE = 10,
        IPC_COMMAND_MANUAL = 99
    }
}
