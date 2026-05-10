using System;
using System.Runtime.InteropServices;
using CMSvr.Domain.Enums;

namespace CMSvr.Domain.Entities
{
    [StructLayout(LayoutKind.Explicit, Size = 672)]
    public unsafe struct SNCFileInfo
    {
        [FieldOffset(0)] public fixed char id[26];
        [FieldOffset(52)] public char is_select;
        [FieldOffset(54)] public char is_select_updown;
        [FieldOffset(56)] public char state;
        [FieldOffset(58)] public char finish;
        [FieldOffset(60)] public uint file_size;
        [FieldOffset(64)] public uint total_lines;
        [FieldOffset(68)] public uint machining_lines;
        [FieldOffset(72)] public fixed char reversed[3];
        [FieldOffset(78)] public fixed char start_time[20];
        [FieldOffset(118)] public fixed char work_time[20];
        [FieldOffset(158)] public fixed char file_name[257];
    }

    /// <summary>
    /// NC 파일 목록을 관리하는 공유 메모리 구조체 (SNCFileInfo[100])
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 67212)]
    public unsafe struct SNCFileMgr
    {
        [FieldOffset(0)] public int nNumFiles;
        [FieldOffset(4)] public int nCurrentFileIndex;
        [FieldOffset(8)] public int nReversed;

        [FieldOffset(12)] private SNCFileInfo hNCFileInfo1;

        /// <summary>
        /// NC 파일 목록 배열 (index: 0~99)
        /// </summary>
        public SNCFileInfo* hNCFileInfo
        {
            get
            {
                fixed (SNCFileInfo* p = &hNCFileInfo1) return p;
            }
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 936)]
    public unsafe struct SPAStatus
    {
        [FieldOffset(0)] public fixed int bInput[25];      // 100 bytes (0-99)
        [FieldOffset(100)] public fixed int bOutput[24];   // 96 bytes (100-195)
        
        // nMDCode는 C++ 소스에 존재하지 않으므로 제거하고 오프셋을 4바이트씩 당깁니다.

        [FieldOffset(196)] public fixed byte szCurrentFileName[128]; // 196번지 시작
        [FieldOffset(324)] public int nBlockNumber;
        [FieldOffset(328)] public int nLineNumber;
        [FieldOffset(332)] public int nGPLErrorCode;
        [FieldOffset(336)] public int nRunStatus;
        [FieldOffset(340)] public int nNumberProgramCycle;
        [FieldOffset(348)] public double fProgramCycleTime; 
        [FieldOffset(356)] public fixed double fPosition[5]; 
        [FieldOffset(396)] public fixed double fVelocity[5];
        [FieldOffset(436)] public fixed double fPositionTool[5];
        [FieldOffset(476)] public int nServoPower;
        [FieldOffset(480)] public int nServoHomeState;
        [FieldOffset(484)] public int nServoErrorState;
        [FieldOffset(488)] public int nZReadyState;
        [FieldOffset(492)] public int nIO_Board_Status;
        [FieldOffset(496)] public int nSpindle_Board_Status;
        [FieldOffset(500)] public int nSpindle2_Board_Status;
        [FieldOffset(504)] public int nCurrentToolNo;
        [FieldOffset(508)] public int nCurrentTool2No;
        [FieldOffset(512)] public int nCurrentCoordinateNo;
        [FieldOffset(516)] public double fCurrentToolLenght;
        [FieldOffset(524)] public double fCurrentTool2Lenght;
        [FieldOffset(532)] public int nSpindleRun;
        [FieldOffset(536)] public int nSpindle2Run;
        [FieldOffset(540)] public int nSpindleSpeed;
        [FieldOffset(544)] public int nSpindleSpeed2;
        [FieldOffset(548)] public int nSpindleSpeedSetting;
        [FieldOffset(552)] public int nSpindleOverride;
        [FieldOffset(556)] public int nSpindleSpeedWithOverride;
        [FieldOffset(560)] public int nMotorOverride;
        [FieldOffset(564)] public int nMotorFeedrate;
        [FieldOffset(568)] public int nMotorFeedrateWithOverride;
        [FieldOffset(572)] public int nRndErrorCode;
        [FieldOffset(576)] public fixed byte szRndErrorMessage[256];
        [FieldOffset(832)] public int nStreamStatusCode;
        [FieldOffset(836)] public int nStreamLineNumber;
        [FieldOffset(840)] public int nStreamBufferCount;
        [FieldOffset(844)] public int nToolLengthUpdateFlag;
        [FieldOffset(848)] public int nTool2LengthUpdateFlag;
        [FieldOffset(852)] public int nSpindle1ColletOpenFlag;
        [FieldOffset(856)] public int nSpindle2ColletOpenFlag;
        [FieldOffset(860)] public int nSpindle1PurgeAirOnFlag;
        [FieldOffset(864)] public int nSpindle2PurgeAirOnFlag;
        [FieldOffset(868)] public int nDuringToolChaneFlag;
        [FieldOffset(872)] public int nEMOButtonFlag;
        [FieldOffset(876)] public int nMotorMovingFlag;
        [FieldOffset(880)] public int bEMOStatus;
        [FieldOffset(884)] public int bLCDPassiveMode;
        [FieldOffset(888)] public int bLCDAlive;
        [FieldOffset(892)] public int bLCDConfirmed;
        [FieldOffset(896)] public int bLCDFinished;
        [FieldOffset(900)] public int bLCDEMOclicked;
        [FieldOffset(904)] public int bLCDStartClicked;
        [FieldOffset(908)] public int bLCDStopClicked;
        [FieldOffset(912)] public int bLCDReadyposClicked;
        [FieldOffset(916)] public int bLCDHomingClicked;
        [FieldOffset(920)] public int bLCDRefresh;
    }

    [StructLayout(LayoutKind.Explicit, Size = 13432)]
    public unsafe struct SThreadState
    {
        [FieldOffset(0)] public ConnectStatus hConnectStatus;
        [FieldOffset(4)] public RunMode hRunMode;
        [FieldOffset(8)] public int bIsOpenNCFile;
        [FieldOffset(12)] public SNCFileInfo hNCFileInfo; 
        [FieldOffset(684)] public int nNCFileInfo_FirstToolChageLine;
        [FieldOffset(688)] public int nNCFileInfo_SecondToolChageLine;
        [FieldOffset(692)] public uint dwRunningTime;
        [FieldOffset(696)] public uint dwRunningTimeTickCount;
        [FieldOffset(700)] public uint dwRunningTimeErrorCount;
        [FieldOffset(704)] public uint dwRunningTimeTotal;
        [FieldOffset(708)] public uint dwRunningTimeRemain;
        [FieldOffset(712)] public int bIpcUpDownLoadComplete_;
        [FieldOffset(716)] public int bIpcCmdComplete_;
        [FieldOffset(720)] public int bIsOriginComplete_;
        [FieldOffset(724)] public int bIsDemoMode_;
        [FieldOffset(728)] public int bIsClientConnected_;
        [FieldOffset(732)] public int bRemoteLock_;
        [FieldOffset(736)] public int bRemoteAutoUpdate_;
        [FieldOffset(740)] public int bSendRegistered_NCFileList_;
        [FieldOffset(744)] public int bSendSDMemory_NCFileList_;
        [FieldOffset(748)] public int bHideErrorMsgDialog_;
        [FieldOffset(752)] public int bShowSetupToolDlg_;
        [FieldOffset(756)] public int bUpdateNcFileList_;
        [FieldOffset(760)] public int bUpdateNcFileListForSD_;
        [FieldOffset(764)] public fixed uint dwLastUsedToolChangeLineNo_[100];
        [FieldOffset(1164)] public int nLastUsedToolChangeLineNoIndex_;
        [FieldOffset(1168)] public int nErrorType;
        [FieldOffset(1172)] public int nErrorCode;
        [FieldOffset(1176)] public fixed char szErrorType[64];
        [FieldOffset(1304)] public int nErrorTypeIsAlarm;
        [FieldOffset(1308)] public fixed char szErrorCode[128];
        [FieldOffset(1564)] public fixed char szErrorMessage[512];
        [FieldOffset(2588)] public int nLCDErrorCode;
        [FieldOffset(2592)] public fixed char szLCDErrorCode[128];
        [FieldOffset(2848)] public fixed byte szResponseTerminalCommand_[128];
        [FieldOffset(2976)] public fixed char szMotionProgVersion[128];
        [FieldOffset(3232)] public fixed char szUIProgVersion[128];
        [FieldOffset(3488)] public fixed char szFileReceiverVersion[128];
        [FieldOffset(3744)] public fixed char szFileReceiverVersion2[128];
        [FieldOffset(4000)] public fixed char szPAControllerVersion[128];
        [FieldOffset(4256)] public fixed char szPA_IP_ARRD[32];
        [FieldOffset(4320)] public fixed char szCANTOPS_IP_ADDR[32];
        [FieldOffset(4384)] public fixed byte szNEW_PA_IP_ADDR[32];
        [FieldOffset(4416)] public fixed byte szNEW_CANTOPS_IP_ADDR[32];
        [FieldOffset(4448)] public int bIsShowUserConfirmDlg;
        [FieldOffset(4452)] public int bIsShowCableConnectDlg;
        [FieldOffset(4456)] public int bIsShowOriginDlg;
        [FieldOffset(4464)] public fixed double fSoftLimit_[12];
        [FieldOffset(7264)] public fixed byte szSelectedLogFiles_[6144];
        [FieldOffset(13408)] public int nNumSelectLogFiles_;
        [FieldOffset(13412)] public int nLenSelectLogFiles_;
        [FieldOffset(13416)] public int bShowSetupDialog_;
        [FieldOffset(13420)] public int nJogSpeed_;
        [FieldOffset(13424)] public int bIsNCFileRun_;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public unsafe struct STool
    {
        public uint dwMaximumTime;
        public uint dwUsingTime;
        public double fUsingRate;
        public uint dwErrCode;
    }

    [StructLayout(LayoutKind.Explicit, Size = 1056)]
    public unsafe struct SToolData
    {
        [FieldOffset(0)] public int bEnableToolUsageTime;
        [FieldOffset(4)] public int bEnableRelatedTool;
        [FieldOffset(8)] public double fTimeCountZAxisPos;

        [FieldOffset(16)] private STool hTool1;
        [FieldOffset(40)] private STool hTool2;
        [FieldOffset(64)] private STool hTool3;
        [FieldOffset(88)] private STool hTool4;
        [FieldOffset(112)] private STool hTool5;
        [FieldOffset(136)] private STool hTool6;
        [FieldOffset(160)] private STool hTool7;
        [FieldOffset(184)] private STool hTool8;
        [FieldOffset(208)] private STool hTool9;
        [FieldOffset(232)] private STool hTool10;
        [FieldOffset(256)] private STool hTool11;

        [FieldOffset(280)] public int nNumDataForRelatedTool;
        [FieldOffset(284)] public fixed byte szRelateTool[12 * 64];

        /// <summary>
        /// STool 배열에 접근하기 위한 포인터 속성 (index: 0~10)
        /// </summary>
        public STool* hTool
        {
            get
            {
                fixed (STool* p = &hTool1) return p;
            }
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 1104)]
    public unsafe struct SConfigData
    {
        [FieldOffset(0)] public fixed double fCoordOffset[10];
        [FieldOffset(80)] public fixed double fTeachingPoint[55];
        [FieldOffset(520)] public fixed double fOptionData[9];
        [FieldOffset(592)] public int bUsingDetectBlock;
        [FieldOffset(596)] public int nDelayGripBlock;
        [FieldOffset(600)] public int bUsingAirLimitSensor;
        [FieldOffset(604)] public int nAirLimitInterval;
        [FieldOffset(608)] public int bUsingOpPanel;
        [FieldOffset(612)] public int bUsingLCD;
        [FieldOffset(616)] public int nSelectM28Operation;
        [FieldOffset(620)] public int nToolErrorOccure_HandlingCode;
        [FieldOffset(624)] public fixed int nToolTimesPerMilling[11];
        [FieldOffset(668)] public int nEnableOperationLog;
        [FieldOffset(672)] public int nEnableIpcCommLog;
        [FieldOffset(676)] public int nEnableThreadModeLog;
        [FieldOffset(680)] public int nEnableOpPenalLog;
        [FieldOffset(684)] public int nEnableExtLog;
        [FieldOffset(688)] public int nEnableErrLog;
        [FieldOffset(692)] public int bUsingFlowSensor;
        [FieldOffset(696)] public int nFlowSensorTimeout;
        [FieldOffset(700)] public int nFlowSensorStartTimeout;
        [FieldOffset(704)] public int bUsingWaterLevelSensor;
        [FieldOffset(708)] public int nPurgeAirHoldTime;
        [FieldOffset(712)] public int bCheckInvalidNcCode;
        [FieldOffset(716)] public int bCheckNcFileTag;
        [FieldOffset(720)] public int bCheckMachineID;
        [FieldOffset(724)] public int bCheckSpindleOffset;
        [FieldOffset(728)] public int bTransformNcFile;
        [FieldOffset(732)] public int bCheckBlockSize;
        [FieldOffset(736)] public int nCheckBlockSizeSpeed;
        [FieldOffset(740)] public int nCheckBlockSizeTorque;
        [FieldOffset(744)] public int nGraphicLCD_Port;
        [FieldOffset(748)] public int bUsingSpindleAirPurge;
        [FieldOffset(752)] public int nCleaningTimeout_Hour;
        [FieldOffset(756)] public int nFilterTimeout_sec;
        [FieldOffset(760)] public int bUsingNCFileAutoClose;
        [FieldOffset(768)] public double fRunningTimePerLine;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public unsafe struct SMaintenanceData
    {
        public fixed int bEnableChecking[4];
        public fixed uint dwMaximumTime[4];
        public fixed long tmLastMaintenance[4];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct SIpcCommCommand
    {
        public byte m_cmd;
        public fixed byte param[63];
    }
}
