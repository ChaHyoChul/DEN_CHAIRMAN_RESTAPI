using System;
using System.Runtime.InteropServices;
using CMSvr.Domain.Enums;

namespace CMSvr.Domain.Entities
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct SNCFileInfo
    {
        public fixed char id[26];
        public byte is_select;
        public byte is_select_updown;
        public byte state;
        public byte finish;
        public uint file_size;
        public uint total_lines;
        public uint machining_lines;
        public fixed byte reversed[3];
        public fixed char start_time[20];
        public fixed char work_time[20];
        public fixed char file_name[257];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct SPAStatus
    {
        public fixed int bInput[25];
        public fixed int bOutput[24];

        public int nMDCode;

        public fixed byte szCurrentFileName[128]; // char in C++ (ANSI)

        public int nBlockNumber;
        public int nLineNumber;
        public int nGPLErrorCode;
        public int nRunStatus;
        public int nNumberProgramCycle;
        public double fProgramCycleTime;

        public fixed double fPosition[5];
        public fixed double fVelocity[5];
        public fixed double fPositionTool[5];

        public int nServoPower;
        public int nServoHomeState;
        public int nServoErrorState;
        public int nZReadyState;
        public int nIO_Board_Status;
        public int nSpindle_Board_Status;
        public int nSpindle2_Board_Status;

        public int nCurrentToolNo;
        public int nCurrentTool2No;
        public int nCurrentCoordinateNo;
        public double fCurrentToolLenght;
        public double fCurrentTool2Lenght;

        public int nSpindleRun;
        public int nSpindle2Run;
        public int nSpindleSpeed;
        public int nSpindleSpeed2;
        public int nSpindleSpeedSetting;
        public int nSpindleOverride;
        public int nSpindleSpeedWithOverride;

        public int nMotorOverride;
        public int nMotorFeedrate;
        public int nMotorFeedrateWithOverride;

        public int nRndErrorCode;
        public fixed byte szRndErrorMessage[256];

        public int nStreamStatusCode;
        public int nStreamLineNumber;
        public int nStreamBufferCount;

        public int nToolLengthUpdateFlag;
        public int nTool2LengthUpdateFlag;
        public int nSpindle1ColletOpenFlag;
        public int nSpindle2ColletOpenFlag;
        public int nSpindle1PurgeAirOnFlag;
        public int nSpindle2PurgeAirOnFlag;
        public int nDuringToolChaneFlag;
        public int nEMOButtonFlag;
        public int nMotorMovingFlag;

        public int bEMOStatus;
        public int bLCDPassiveMode;
        public int bLCDAlive;
        public int bLCDConfirmed;
        public int bLCDFinished;
        public int bLCDEMOclicked;
        public int bLCDStartClicked;
        public int bLCDStopClicked;
        public int bLCDReadyposClicked;
        public int bLCDHomingClicked;
        public int bLCDRefresh;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct SThreadState
    {
        public ConnectStatus hConnectStatus;
        public RunMode hRunMode;
        public int bIsOpenNCFile;
        public SNCFileInfo hNCFileInfo;
        public int nNCFileInfo_FirstToolChageLine;
        public int nNCFileInfo_SecondToolChageLine;
        public uint dwRunningTime;
        public uint dwRunningTimeTickCount;
        public uint dwRunningTimeErrorCount;
        public uint dwRunningTimeTotal;
        public uint dwRunningTimeRemain;

        public int bIpcUpDownLoadComplete_;
        public int bIpcCmdComplete_;
        public int bIsOriginComplete_;
        public int bIsDemoMode_;
        public int bIsClientConnected_;
        public int bRemoteLock_;
        public int bRemoteAutoUpdate_;
        public int bSendRegistered_NCFileList_;
        public int bSendSDMemory_NCFileList_;
        public int bHideErrorMsgDialog_;
        public int bShowSetupToolDlg_;
        public int bUpdateNcFileList_;
        public int bUpdateNcFileListForSD_;

        public fixed uint dwLastUsedToolChangeLineNo_[100];
        public int nLastUsedToolChangeLineNoIndex_;

        public int nErrorType;
        public int nErrorCode;
        public fixed char szErrorType[64];
        public int nErrorTypeIsAlarm;
        public fixed char szErrorCode[128];
        public fixed char szErrorMessage[512];

        public int nLCDErrorCode;
        public fixed char szLCDErrorCode[128];

        public fixed byte szResponseTerminalCommand_[128];

        public fixed char szMotionProgVersion[128];
        public fixed char szUIProgVersion[128];
        public fixed char szFileReceiverVersion[128];
        public fixed char szFileReceiverVersion2[128];
        public fixed char szPAControllerVersion[128];

        public fixed char szPA_IP_ARRD[32];
        public fixed char szCANTOPS_IP_ADDR[32];
        public fixed byte szNEW_PA_IP_ADDR[32];
        public fixed byte szNEW_CANTOPS_IP_ADDR[32];

        public int bIsShowUserConfirmDlg;
        public int bIsShowCableConnectDlg;
        public int bIsShowOriginDlg;

        public fixed double fSoftLimit_[12]; // [6][2]

        public uint dwTOTAL_LEFT_SPINDLE_RUN_TIME;
        public uint dwTOTAL_RIGHT_SPINDLE_RUN_TIME;
        public uint dwCLEAN_SPINDLE_RUN_TIME;
        public uint dwTOTAL_FILTER_TIME;

        public fixed char szIpAddress[64];

        public int bIsFileOpening;
        public int nNcFileLoadingRate;
        public int nNcFileLoadingLine;
        public fixed char szMaterialName[64];
        public fixed char szBlockName[64];

        public int nBufferingLine;
        public int nStartingNCCodeStepNo;
        public int nCurrentNCCodeStepNo;
        public int nNumberOfPreparingStep;
        public int nRunMode_StepNo;
        public UserMode hUserMode;
        public int nPAYear;
        public int nPAMonth;
        public int nPADay;
        public int nSetupMode;
        public int nLCD_Start_Stop_AutoCal;

        public fixed int nAutoCal_CheckBoxState[10];
        public fixed int nAutoCal_CheckingItem[10];
        public byte bFullCalibration;
        public byte bFullAutoTeaching;
        public byte bIsEasyCalibration;
        public Axis hAutoCal_RotateAxis;
        public int nAutoCal_ConnectedCable;
        public int nAutoCal_ConnectedCable2;
        public IntPtr hWndSetupAutoCal;

        public fixed int nAutoTeach_CheckingItem[2];

        public int bNoNeedPassword;
        public int bOperationScreen_ToolButtonPressed;
        public int bOperationScreen_UsbMemory;
        public int bCheckStatus;
        public int nPauseByDoorOpen;

        public fixed int bATCTest_EnaTool[8];
        public int nATCTest_MeasureCount;
        public int nATCTest_WorkCount;
        public fixed double fATCTest_MeasureResult[240]; // [8][30]

        public int nIsConnectedPAController;
        public byte bConnectionFailed;
        public int nIsConnectedIOBoard;

        public int nBarcode_Data;
        public int bIsPauseAirLimit_;
        public int bCompleteResetOrigin_;
        public int bIsReceivedPNCID_;
        public fixed char szPNCID_[64];

        public int bSaveSelectedLogFiles_;
        public fixed byte szSelectedLogFiles_[6144]; // 1024*6
        public int nNumSelectLogFiles_;
        public int nLenSelectLogFiles_;
        public int bShowSetupDialog_;
        public int nJogSpeed_;
        public int bIsNCFileRun_;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct STool
    {
        public uint dwMaximumTime;
        public uint dwUsingTime;
        public double fUsingRate;
        public uint dwErrCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct SToolData
    {
        public int bEnableToolUsageTime;
        public int bEnableRelatedTool;
        public double fTimeCountZAxisPos;

        public fixed byte hTool[11 * 24]; // STool is 24 bytes, fixed can't be struct type. Manual offset needed.

        public int nNumDataForRelatedTool;
        public fixed byte szRelateTool[12 * 64];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct SConfigData
    {
        public fixed double fCoordOffset[10];
        public fixed double fTeachingPoint[55];
        public fixed double fOptionData[9];

        public int bUsingDetectBlock;
        public int nDelayGripBlock;
        public int bUsingAirLimitSensor;
        public int nAirLimitInterval;
        public int bUsingOpPanel;
        public int bUsingLCD;
        public int nSelectM28Operation;
        public int nToolErrorOccure_HandlingCode;

        public fixed int nToolTimesPerMilling[11];

        public int nEnableOperationLog;
        public int nEnableIpcCommLog;
        public int nEnableThreadModeLog;
        public int nEnableOpPenalLog;
        public int nEnableExtLog;
        public int nEnableErrLog;

        public int bUsingFlowSensor;
        public int nFlowSensorTimeout;
        public int nFlowSensorStartTimeout;
        public int bUsingWaterLevelSensor;
        public int nPurgeAirHoldTime;
        public int bCheckInvalidNcCode;
        public int bCheckNcFileTag;
        public int bCheckMachineID;
        public int bCheckSpindleOffset;
        public int bTransformNcFile;
        public int bCheckBlockSize;
        public int nCheckBlockSizeSpeed;
        public int nCheckBlockSizeTorque;
        public int nGraphicLCD_Port;
        public int bUsingSpindleAirPurge;
        public int nCleaningTimeout_Hour;
        public int nFilterTimeout_sec;
        public int bUsingNCFileAutoClose;
        public double fRunningTimePerLine;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct SMaintenanceData
    {
        public fixed int bEnableChecking[4];
        public fixed uint dwMaximumTime[4];
        public fixed long tmLastMaintenance[4];
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public unsafe struct SIpcCommCommand
    {
        [FieldOffset(0)]
        public byte m_cmd;

        [FieldOffset(1)]
        public byte s_cmd;

        [FieldOffset(1)]
        public fixed byte m_param[63];

        [FieldOffset(2)]
        public fixed byte s_param[62];
    }
}
