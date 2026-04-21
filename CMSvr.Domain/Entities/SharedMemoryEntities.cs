using System;
using System.Runtime.InteropServices;
using CMSvr.Domain.Enums;

namespace CMSvr.Domain.Entities
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public unsafe struct SNCFileInfo
    {
        public fixed byte id[26];
        public byte is_select;
        public byte is_select_updown;
        public byte state;
        public byte finish;
        public uint file_size;
        public uint total_lines;
        public uint machining_lines;
        public fixed byte reversed[3];
        public fixed byte start_time[20];
        public fixed byte work_time[20];
        public fixed byte file_name[257];
    }

    [StructLayout(LayoutKind.Explicit, Size = 936)]
    public unsafe struct SPAStatus
    {
        [FieldOffset(0)] public fixed int bInput[25];
        [FieldOffset(100)] public fixed int bOutput[24];
        [FieldOffset(196)] public int nMDCode;
        [FieldOffset(200)] public fixed byte szCurrentFileName[128];
        [FieldOffset(328)] public int nBlockNumber;
        [FieldOffset(332)] public int nLineNumber;
        [FieldOffset(336)] public int nGPLErrorCode;
        [FieldOffset(340)] public int nRunStatus;
        [FieldOffset(344)] public int nNumberProgramCycle;
        [FieldOffset(352)] public double fProgramCycleTime; 
        [FieldOffset(360)] public fixed double fPosition[5]; 
        [FieldOffset(400)] public fixed double fVelocity[5];
        [FieldOffset(440)] public fixed double fPositionTool[5];
        [FieldOffset(480)] public int nServoPower;
        [FieldOffset(484)] public int nServoHomeState;
        [FieldOffset(488)] public int nServoErrorState;
        [FieldOffset(492)] public int nZReadyState;
        [FieldOffset(496)] public int nIO_Board_Status;
        [FieldOffset(500)] public int nSpindle_Board_Status;
        [FieldOffset(504)] public int nSpindle2_Board_Status;
        [FieldOffset(508)] public int nCurrentToolNo;
        [FieldOffset(512)] public int nCurrentTool2No;
        [FieldOffset(516)] public int nCurrentCoordinateNo;
        [FieldOffset(520)] public double fCurrentToolLenght;
        [FieldOffset(528)] public double fCurrentTool2Lenght;
        [FieldOffset(536)] public int nSpindleRun;
        [FieldOffset(540)] public int nSpindle2Run;
        [FieldOffset(544)] public int nSpindleSpeed;
        [FieldOffset(548)] public int nSpindleSpeed2;
        [FieldOffset(552)] public int nSpindleSpeedSetting;
        [FieldOffset(556)] public int nSpindleOverride;
        [FieldOffset(560)] public int nSpindleSpeedWithOverride;
        [FieldOffset(564)] public int nMotorOverride;
        [FieldOffset(568)] public int nMotorFeedrate;
        [FieldOffset(572)] public int nMotorFeedrateWithOverride;
        [FieldOffset(576)] public int nRndErrorCode;
        [FieldOffset(580)] public fixed byte szRndErrorMessage[256];
        [FieldOffset(836)] public int nStreamStatusCode;
        [FieldOffset(840)] public int nStreamLineNumber;
        [FieldOffset(844)] public int nStreamBufferCount;
        [FieldOffset(848)] public int nToolLengthUpdateFlag;
        [FieldOffset(852)] public int nTool2LengthUpdateFlag;
        [FieldOffset(856)] public int nSpindle1ColletOpenFlag;
        [FieldOffset(860)] public int nSpindle2ColletOpenFlag;
        [FieldOffset(864)] public int nSpindle1PurgeAirOnFlag;
        [FieldOffset(868)] public int nSpindle2PurgeAirOnFlag;
        [FieldOffset(872)] public int nDuringToolChaneFlag;
        [FieldOffset(876)] public int nEMOButtonFlag;
        [FieldOffset(880)] public int nMotorMovingFlag;
        [FieldOffset(884)] public int bEMOStatus;
        [FieldOffset(888)] public int bLCDPassiveMode;
        [FieldOffset(892)] public int bLCDAlive;
        [FieldOffset(896)] public int bLCDConfirmed;
        [FieldOffset(900)] public int bLCDFinished;
        [FieldOffset(904)] public int bLCDEMOclicked;
        [FieldOffset(908)] public int bLCDStartClicked;
        [FieldOffset(912)] public int bLCDStopClicked;
        [FieldOffset(916)] public int bLCDReadyposClicked;
        [FieldOffset(920)] public int bLCDHomingClicked;
        [FieldOffset(924)] public int bLCDRefresh;
        }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
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
        public fixed byte szErrorType[64];
        public int nErrorTypeIsAlarm;
        public fixed byte szErrorCode[128];
        public fixed byte szErrorMessage[512];
        public int nLCDErrorCode;
        public fixed byte szLCDErrorCode[128];
        public fixed byte szResponseTerminalCommand_[128];
        public fixed byte szMotionProgVersion[128];
        public fixed byte szUIProgVersion[128];
        public fixed byte szFileReceiverVersion[128];
        public fixed byte szFileReceiverVersion2[128];
        public fixed byte szPAControllerVersion[128];
        public fixed byte szPA_IP_ARRD[32];
        public fixed byte szCANTOPS_IP_ADDR[32];
        public fixed byte szNEW_PA_IP_ADDR[32];
        public fixed byte szNEW_CANTOPS_IP_ADDR[32];
        public int bIsShowUserConfirmDlg;
        public int bIsShowCableConnectDlg;
        public int bIsShowOriginDlg;
        public fixed double fSoftLimit_[12];
        public uint dwTOTAL_LEFT_SPINDLE_RUN_TIME;
        public uint dwTOTAL_RIGHT_SPINDLE_RUN_TIME;
        public uint dwCLEAN_SPINDLE_RUN_TIME;
        public uint dwTOTAL_FILTER_TIME;
        public fixed byte szIpAddress[64];
        public int bIsFileOpening;
        public int nNcFileLoadingRate;
        public int nNcFileLoadingLine;
        public fixed byte szMaterialName[64];
        public fixed byte szBlockName[64];
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
        public fixed double fATCTest_MeasureResult[240];
        public int nIsConnectedPAController;
        public byte bConnectionFailed;
        public int nIsConnectedIOBoard;
        public int nBarcode_Data;
        public int bIsPauseAirLimit_;
        public int bCompleteResetOrigin_;
        public int bIsReceivedPNCID_;
        public fixed byte szPNCID_[64];
        public int bSaveSelectedLogFiles_;
        public fixed byte szSelectedLogFiles_[6144];
        public int nNumSelectLogFiles_;
        public int nLenSelectLogFiles_;
        public int bShowSetupDialog_;
        public int nJogSpeed_;
        public int bIsNCFileRun_;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct SToolData
    {
        public int bEnableToolUsageTime;
        public int bEnableRelatedTool;
        public double fTimeCountZAxisPos;
        public fixed byte hTool[11 * 24]; 
        public int nNumDataForRelatedTool;
        public fixed byte szRelateTool[12 * 64];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
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

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public unsafe struct SIpcCommCommand
    {
        [FieldOffset(0)] public byte m_cmd;
        [FieldOffset(1)] public byte s_cmd;
        [FieldOffset(1)] public fixed byte m_param[63];
        [FieldOffset(2)] public fixed byte s_param[62];
    }
}
