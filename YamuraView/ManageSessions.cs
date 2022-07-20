using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YamuraView
{
    public partial class ManageSessions : Form
    {
        public ManageSessions()
        {
            InitializeComponent();
            UpdateData();
        }

        private void addSessionMenuItem_Click(object sender, EventArgs e)
        {
            if(openLogFile.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            for (int fileIdx = 0; fileIdx < openLogFile.FileNames.Count(); fileIdx++)
            {
                if (openLogFile.FileNames[fileIdx].EndsWith("ylg", true, System.Globalization.CultureInfo.CurrentCulture))
                {
                    ReadYLGFile(openLogFile.FileNames[fileIdx]);
                }
                else if (openLogFile.FileNames[fileIdx].EndsWith("yl5", true, System.Globalization.CultureInfo.CurrentCulture))
                {
                    ReadYL5File(openLogFile.FileNames[fileIdx]);
                }
                else if (openLogFile.FileNames[fileIdx].EndsWith("txt", true, System.Globalization.CultureInfo.CurrentCulture))
                {
                    ReadTXTFile(openLogFile.FileNames[fileIdx]);
                }
            }
        }
        #region read various log file formats
        private void ReadTXTFile(String fileName)
        {
            #region create a temp file to write cleaned up data stream
            String tempLogFile = fileName.Replace(".txt", ".tmp");
            tempLogFile = tempLogFile.Replace(".TXT", ".TMP");
            StreamReader readLog = new StreamReader(fileName, true);
            StreamWriter writeLog = new StreamWriter(tempLogFile, false);
            String tmp_text = readLog.ReadToEnd();// readFile.ReadToEnd();
            StringBuilder gpx_text = new StringBuilder();
            foreach (char c in tmp_text)
            {
                if ((c != 0x01) && (c != 0x11) && (c != 0x0C))
                {
                    writeLog.Write(c);
                    gpx_text.Append(c);
                }
            }
            readLog.Close();
            writeLog.Close();
            #endregion

            StreamReader readTemp = new StreamReader(tempLogFile, true);
            String inputStr;
            String[] splitStr;
            int logSessionsIdx = 0;
            float priorLatVal = 0.0F;
            float priorLongVal = 0.0F;
            float latVal = 0.0F;
            float longVal = 0.0F;
            float gX = 0.0F;
            float gY = 0.0F;
            float gZ = 0.0F;
            ulong timestamp = 0;
            ulong timestampOffset = 0;
            float timestampSeconds = 0.0F;
            float mph = 0;
            float heading = 0;
            bool timestampOffsetValid = false;
            bool gpsDistanceValid = false;
            float gpsDist = 0.0F;

            StringBuilder strSessionsList = new StringBuilder();
            while (!readTemp.EndOfStream)
            {
                #region skip blanks
                inputStr = readTemp.ReadLine();
                if (inputStr.Length == 0)
                {
                    continue;
                }
                #endregion
                #region session start, add a new session to logger
                // found session start, create new data list in log events
                //                  new session data header
                //                  new display header
                if (String.Compare(inputStr, "Start", true) == 0)
                {
                    gpsDistanceValid = false;
                    gpsDist = 0.0F;
                    timestampOffsetValid = false;
                    timestampOffset = 0;
                    YamuraViewMain.dataLogger.sessionData.Add(new SessionData());
                    logSessionsIdx = YamuraViewMain.dataLogger.sessionData.Count - 1;
                    // set session file name in session data
                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].fileName = System.IO.Path.GetFullPath(fileName);
                    // add timestamp here, since it is always present
                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Time", "Timestamp", "Internal", 1.0F);
                    continue;
                }
                #endregion
                #region session end, skip just a marker
                else if ((String.Compare(inputStr, "Stop", true) == 0) ||
                         inputStr.StartsWith("GPS") ||
                         inputStr.StartsWith("Accel") ||
                         inputStr.StartsWith("Team Yamura"))
                {
                    continue;
                }
                #endregion
                #region break up the input string
                splitStr = inputStr.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                #endregion
                #region add timestamp
                timestamp = (ulong)BitConverter.ToUInt32(BitConverter.GetBytes(Convert.ToInt32(splitStr[0])), 0);
                if (!timestampOffsetValid)
                {
                    timestampOffset = timestamp;
                    timestampOffsetValid = true;
                }
                timestamp -= timestampOffset;
                timestampSeconds = Convert.ToSingle(timestamp) / 1000000.0F;
                YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Time"].AddPoint(timestampSeconds, timestampSeconds);
                #endregion
                #region GPS data
                // gps+accel form - 11 fields
                // 35990532 07/11/2019 12:44:39.00 42.446449 -83.456070  0.70    183.920000  7   0.306   0.005   0.947
                //
                // gps only - 8 fields
                // 35990532 07/11/2019 12:44:39.00 42.446449 -83.456070  0.70    183.920000  7
                //
                // accel only - 4 fields
                //36050376 0.41   0.00    0.94
                // timestamp is always first

                // gps only, gps+accelerometer - get gps portion
                if ((splitStr.Count() == 8) || (splitStr.Count() == 11))
                {
                    latVal = Convert.ToSingle(splitStr[3]);
                    longVal = Convert.ToSingle(splitStr[4]);
                    mph = Convert.ToSingle(splitStr[5]);
                    heading = Convert.ToSingle(splitStr[6]);
                    if (YamuraViewMain.dataLogger.sessionData[logSessionsIdx].dateStr.Length == 0)
                    {
                        YamuraViewMain.dataLogger.sessionData[logSessionsIdx].dateStr = splitStr[1];
                        YamuraViewMain.dataLogger.sessionData[logSessionsIdx].timeStr = splitStr[2];
                    }
                    // add channel (only if needed)
                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Latitude", "GPS Latitude", "GPS", 1.0F);
                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Longitude", "GPS Longitude", "GPS", 1.0F);
                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Speed-GPS", "GPS Speed", "GPS", 1.0F);
                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Heading-GPS", "GPS Heading", "GPS", 1.0F);
                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Distance-GPS", "GPS Distance", "GPS", 1.0F);
                    // add data to channel
                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Latitude"].AddPoint(timestampSeconds, latVal);
                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Longitude"].AddPoint(timestampSeconds, longVal);
                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Speed-GPS"].AddPoint(timestampSeconds, mph);
                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Heading-GPS"].AddPoint(timestampSeconds, heading);
                    if (!gpsDistanceValid)
                    {
                        YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Distance-GPS"].AddPoint(timestampSeconds, 0.0F);
                        priorLatVal = latVal;
                        priorLongVal = longVal;
                        gpsDistanceValid = true;
                    }
                    else
                    {
                        gpsDist += GPSDistance(priorLatVal, priorLongVal, latVal, longVal);
                        YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Distance-GPS"].AddPoint(timestampSeconds, gpsDist);
                    }
                }
                #endregion
                #region accelerometer data
                // accelerometer only, or gps+accelerometer - get accelerometer portion
                if ((splitStr.Count() == 4) || (splitStr.Count() == 11))
                {
                    latVal = 0;
                    longVal = 0;
                    int xValIdx = splitStr.Count() == 4 ? 1 : 8;
                    int yValIdx = splitStr.Count() == 4 ? 2 : 9;
                    int zValIdx = splitStr.Count() == 4 ? 3 : 10;

                    gX = Convert.ToSingle(splitStr[xValIdx]);
                    gY = Convert.ToSingle(splitStr[yValIdx]);
                    gZ = Convert.ToSingle(splitStr[zValIdx]);
                    // add channel (only if needed)
                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("gX", "X Axis Acceleration", "Accelerometer", 1.0F);
                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("gY", "Y Axis Acceleration", "Accelerometer", 1.0F);
                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("gZ", "Z Axis Acceleration", "Accelerometer", 1.0F);
                    // add data to channel
                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["gX"].AddPoint(timestampSeconds, gX);
                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["gY"].AddPoint(timestampSeconds, gY);
                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["gZ"].AddPoint(timestampSeconds, gZ);

                }
                #endregion
            }
            #region close and delete temp file
            // close and delete temp file
            readTemp.Close();
            System.IO.File.Delete(tempLogFile);
            #endregion
            UpdateData();
        }
        private void ReadYLGFile(String fileName)
        {
            int logSessionsIdx = 0;
            char[] b = new char[3];
            uint timeStamp = 0;
            uint timeStampOffset = 0;
            bool timeStampOffsetSet = false;
            float timestampSeconds = 0;
            float priorLatVal = 0.0F;
            float priorLongVal = 0.0F;
            float gpsDist = 0.0F;
            bool gpsDistanceValid = false;
            StringBuilder errStr = new StringBuilder();
            YamuraViewMain.dataLogger.sessionData.Add(new SessionData());

            logSessionsIdx = YamuraViewMain.dataLogger.sessionData.Count - 1;
            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Time", "Timestamp", "Internal", 1.0F);

            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].fileName = System.IO.Path.GetFullPath(fileName);


            Cursor = Cursors.WaitCursor;
            using (BinaryReader inFile = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                // check for EOF
                while (inFile.BaseStream.Position != inFile.BaseStream.Length)
                {
                    #region read 1 char, exception on EOF
                    try
                    {
                        b[0] = (char)inFile.ReadByte();
                    }
                    catch
                    {
                        continue;
                    }
                    #endregion
                    #region timestamp
                    // 'T', next 4 bytes are a unsigned long int
                    if ((char)b[0] == 'T')
                    {
                        timeStamp = inFile.ReadUInt32();
                        if (!timeStampOffsetSet)
                        {
                            timeStampOffset = timeStamp;
                            timeStampOffsetSet = true;
                        }
                        timeStamp -= timeStampOffset;
                        timestampSeconds = (float)timeStamp / 1000000.0F;
                        YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Time"].AddPoint(timestampSeconds, timestampSeconds);
                        continue;
                    }
                    #endregion
                    #region get channel type chars
                    try
                    {
                        b[1] = (char)inFile.ReadByte();
                        b[2] = (char)inFile.ReadByte();
                    }
                    catch
                    {
                        break;
                    }
                    #endregion
                    #region GPS
                    // GPS (gps device) returns NMEA strings
                    // 4 byte channel number followed by NMEA string
                    if ((b[0] == 'G') && (b[1] == 'P') && (b[2] == 'S'))
                    {
                        inFile.ReadUInt32();

                        float lat = 0.0F;
                        String ns = "";
                        float lng = 0.0F;
                        String ew = "";
                        float hd = 0.0F;
                        float speed = 0.0F;
                        int sat = 0;
                        String date = "";
                        int utcHr = 0;
                        int utcMin = 0;
                        Single utcSec = 0.0F;
                        if (ParseGPS_NMEA(inFile, out date, out utcHr, out utcMin, out utcSec, out lat, out ns, out lng, out ew, out hd, out speed, out sat, ref errStr))
                        {
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Latitude", "GPS Latitude", "GPS", 1.0F);
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Longitude", "GPS Longitude", "GPS", 1.0F);
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Speed-GPS", "GPS Speed", "GPS", 1.0F);
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Heading-GPS", "GPS Heading", "GPS", 1.0F);
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Distance-GPS", "GPS Distance", "GPS", 1.0F);
                            // add data to channel
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Latitude"].AddPoint(timestampSeconds, lat);
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Longitude"].AddPoint(timestampSeconds, lng);
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Speed-GPS"].AddPoint(timestampSeconds, speed);
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Heading-GPS"].AddPoint(timestampSeconds, hd);
                            if (!gpsDistanceValid)
                            {
                                YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Distance-GPS"].AddPoint(timestampSeconds, 0.0F);
                                priorLatVal = lat;
                                priorLongVal = lng;
                                gpsDistanceValid = true;
                            }
                            else
                            {
                                gpsDist += GPSDistance(priorLatVal, priorLongVal, lat, lng);
                                YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Distance-GPS"].AddPoint(timestampSeconds, gpsDist);
                            }
                        }
                    }
                    #endregion
                    #region ACC
                    //
                    // accel channel
                    // ACC (3 axis accelerometer) returns byte channel number followed by 3 float values
                    //
                    else if ((b[0] == 'A') && (b[1] == 'C') && (b[2] == 'C'))
                    {
                        inFile.ReadUInt32();
                        // add channel (only if needed)
                        YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("gX", "X Axis Acceleration", "Accelerometer", 1.0F);
                        YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("gY", "Y Axis Acceleration", "Accelerometer", 1.0F);
                        YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("gZ", "Z Axis Acceleration", "Accelerometer", 1.0F);
                        Single accelVal = 0.0F;
                        for (int valIdx = 0; valIdx < 3; valIdx++)
                        {
                            accelVal = inFile.ReadSingle();
                            // add data to channel
                            if (valIdx == 0)
                            {
                                YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["gX"].AddPoint(timestampSeconds, accelVal);
                            }
                            else if (valIdx == 1)
                            {
                                YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["gY"].AddPoint(timestampSeconds, accelVal);
                            }
                            else if (valIdx == 2)
                            {
                                YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["gZ"].AddPoint(timestampSeconds, accelVal);
                            }
                        }
                    }
                    #endregion
                    #region IMU
                    // not implemented
                    #endregion
                    #region A2D/DIG/CNT/RPM
                    // 
                    // analog channel
                    // 4 byte channel number followed by 1 float (value)
                    //
                    else if ((b[0] == 'A') && (b[1] == '2') && (b[2] == 'D'))
                    {
                        uint channelNum = inFile.ReadUInt32();
                        UInt32 channelVal = inFile.ReadUInt32();
                        float channelValF = (float)channelVal;
                        String channelName = "A2D" + channelNum.ToString();
                        YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel(channelName, "Analog to Digital channel " + channelName, "A2D", 1.0F);
                        YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels[channelName].AddPoint(timestampSeconds, channelValF);

                    }
                    else
                    {
                        errStr.AppendFormat("unexpected channel type - read {0}{1}{2}", b[0], b[1], System.Environment.NewLine);
                    }
                    #endregion
                }
                inFile.Close();
            }
            Cursor = Cursors.Default;

            //if (errStr.Length > 0)
            //{
            //    FileInfo errInfo = new FileInfo();
            //    errInfo.FileInfoText = errStr.ToString();
            //    errInfo.ShowDialog();
            //}
            UpdateData();
        }
        /// <summary>
        /// YamuraLog V5 with CAN ids as identifiers
        /// </summary>
        /// <param name="fileName"></param>
        private void ReadYL5File(String fileName)
        {
            int logSessionsIdx = 0;
            char[] b = new char[3];
            //float timestampSeconds = 0;
            uint absTimeInt = 0;
            float absTime = 0.0F;
            float offsetTime = -1.0F;
            float gpsDist = 0.0F;
            float priorLatVal = 0.0F;
            float priorLongVal = 0.0F;
            bool gpsDistanceValid = false;
            String channelName = "";
            StringBuilder errStr = new StringBuilder();
            YamuraViewMain.dataLogger.sessionData.Add(new SessionData());
            logSessionsIdx = YamuraViewMain.dataLogger.sessionData.Count - 1;
            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].sessionColor = YamuraViewMain.colors[logSessionsIdx % YamuraViewMain.colors.Count];
            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Time", "Timestamp", "Internal", 1.0F);
            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].fileName = System.IO.Path.GetFullPath(fileName);


            Cursor = Cursors.WaitCursor;
            using (BinaryReader inFile = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                while (true)
                {
                    try
                    {
                        Byte recordType = inFile.ReadByte();
                        // HUB node (0x10)
                        // no logged messages
                        // Control node (0x20)
                        // no logged messages
                        // AD node (0x30-0x3F)
                        if ((recordType >= 0x30) && (recordType <= 0x3F))
                        {
                            absTimeInt = inFile.ReadUInt32();
                            absTime = (float)absTimeInt / 1000.0F;
                            offsetTime = offsetTime < 0.0F ? absTime : offsetTime;
                            absTime -= offsetTime;
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Time"].AddPoint((float)absTime, (float)absTime);

                            Byte digitalVals = inFile.ReadByte();
                            UInt16[] a2d = new UInt16[8];
                            #region read the digital data
                            for (int idx = 0; idx < 8; idx++)
                            {
                                channelName = "D_" + ((recordType - 0x30) + idx).ToString();
                                if (!YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels.ContainsKey(channelName))
                                {
                                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel(channelName, "Digital channel " + channelName, "D", 1.0F);
                                }
                                YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels[channelName].AddPoint((float)absTime, (float)((digitalVals >> idx) & 0x01));
                            }
                            #endregion
                            #region read the a2d data
                            for (int idx = 0; idx < 8; idx++)
                            {
                                a2d[idx] = inFile.ReadUInt16();
                                channelName = "A2D_" + ((recordType - 0x30) + idx).ToString();
                                if (!YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels.ContainsKey(channelName))
                                {
                                    YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel(channelName, "Analog to Digital channel " + channelName, "A2D", 1.0F);
                                }
                                YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels[channelName].AddPoint((float)absTime, (float)a2d[idx]);
                            }
                            #endregion
                        }
                        // IMU/accelerometer node (0x40)
                        else if (recordType == 0x40)
                        {
                            absTimeInt = inFile.ReadUInt32();
                            absTime = (float)absTimeInt / 1000.0F;
                            offsetTime = offsetTime < 0.0F ? absTime : offsetTime;
                            absTime -= offsetTime;
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Time"].AddPoint((float)absTime, (float)absTime);

                            Int16 ax = inFile.ReadInt16();
                            Int16 ay = inFile.ReadInt16();
                            Int16 az = inFile.ReadInt16();
                            if (!YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels.ContainsKey("gX"))
                            {
                                YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("gX", "Accelerometer channel " + "gX", "G", 1.0F);
                            }
                            if (!YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels.ContainsKey("gY"))
                            {
                                YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("gY", "Accelerometer channel " + "gY", "G", 1.0F);
                            }
                            if (!YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels.ContainsKey("gZ"))
                            {
                                YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("gZ", "Accelerometer channel " + "gZ", "G", 1.0F);
                            }
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["gX"].AddPoint(absTime, ax);
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["gY"].AddPoint(absTime, ay);
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["gZ"].AddPoint(absTime, az);
                        }
                        // GPS node (0x50)
                        else if (recordType == 0x50)
                        {
                            absTimeInt = inFile.ReadUInt32();
                            absTime = (float)absTimeInt / 1000.0F;
                            offsetTime = offsetTime < 0.0F ? absTime : offsetTime;
                            absTime -= offsetTime;
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Time"].AddPoint((float)absTime, (float)absTime);

                            UInt16 gpsTimeYear = inFile.ReadUInt16();
                            Byte gpsTimeMonth = inFile.ReadByte();
                            Byte gpsTimeDay = inFile.ReadByte();
                            Byte gpsTimeHour = inFile.ReadByte();
                            Byte gpsTimeMinute = inFile.ReadByte();
                            Byte gpsTimeSecond = inFile.ReadByte();
                            float latitude = (float)inFile.ReadInt32() / 10000000.0F;
                            float longitude = (float)inFile.ReadInt32() / 10000000.0F;
                            float course = (float)inFile.ReadInt32();
                            float speed = (float)inFile.ReadInt32()/1000.0F;
                            Byte SIV = inFile.ReadByte();
                            if(gpsDistanceValid)
                            {
                                gpsDist += GPSDistance(priorLatVal, priorLongVal, latitude, longitude);
                            }
                            priorLatVal = latitude;
                            priorLongVal = longitude;
                            gpsDistanceValid = true;
                            if (!YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels.ContainsKey("Latitude"))
                            {
                                YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Latitude", "GPS Latitude", "GPS", 1.0F);
                            }
                            if (!YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels.ContainsKey("Longitude"))
                            {
                                YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Longitude", "GPS Longitude", "GPS", 1.0F);
                            }
                            if (!YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels.ContainsKey("Speed-GPS"))
                            {
                                YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Speed-GPS", "GPS Speed", "GPS", 1.0F);
                            }
                            if (!YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels.ContainsKey("Heading-GPS"))
                            {
                                YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Heading-GPS", "GPS Heading", "GPS", 1.0F);
                            }
                            if (!YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels.ContainsKey("Distance-GPS"))
                            {
                                YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel("Distance-GPS", "GPS Distance", "GPS", 1.0F);
                            }
                            // add data to channel
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Latitude"].AddPoint(absTime, latitude);
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Longitude"].AddPoint(absTime, longitude);
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Speed-GPS"].AddPoint(absTime, speed);
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Heading-GPS"].AddPoint(absTime, course);
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Distance-GPS"].AddPoint(absTime, gpsDist);
                        }
                        // IR Tire temp node (0x60-0x6F)
                        else if ((recordType >= 0x60) && (recordType <= 0x6F))
                        {
                            absTimeInt = inFile.ReadUInt32();
                            absTime = (float)absTimeInt / 1000.0F;
                            offsetTime = offsetTime < 0.0F ? absTime : offsetTime;
                            absTime -= offsetTime;
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Time"].AddPoint((float)absTime, (float)absTime);
                            //deltaTime = absTime - lastSample[recordType - 0x30];
                            //lastSample[recordType - 0x30] = absTime;
                            //UInt16 seq = inFile.ReadUInt16();

                            //float[] t = new float[8];
                            //int row = 0;
                            //int col = 0;
                            //switch (seq)
                            //{
                            //    case 0:
                            //        row = 0;
                            //        col = 0;
                            //        break;
                            //    case 1:
                            //        row = 0;
                            //        col = 8;
                            //        break;
                            //    case 2:
                            //        row = 1;
                            //        col = 0;
                            //        break;
                            //    case 3:
                            //        row = 1;
                            //        col = 8;
                            //        break;
                            //    case 4:
                            //        row = 2;
                            //        col = 0;
                            //        break;
                            //    case 5:
                            //        row = 2;
                            //        col = 8;
                            //        break;
                            //    case 6:
                            //        row = 3;
                            //        col = 0;
                            //        break;
                            //    case 7:
                            //        row = 3;
                            //        col = 8;
                            //        break;
                            //}
                            //for (int idx = 0; idx < 8; idx++)
                            //{
                            //    tempArray[row, col + idx] = inFile.ReadSingle();
                            //}
                            //if (seq == 7)
                            //{
                            //    deltaTime = absTime - lastSample[3];
                            //    lastSample[3] = absTime;
                            //    outStr.AppendFormat("0x{0:X02}\tIR\t{1}\t{2}", recordType,
                            //                                                    absTime,
                            //                                                    deltaTime);
                            //    for (row = 0; row < 4; row++)
                            //    {
                            //        for (col = 0; col < 16; col++)
                            //        {
                            //            outStr.AppendFormat("\t{0:F2}", tempArray[row, col]);
                            //        }

                            //    }
                            //    outStr.Append(System.Environment.NewLine);
                            //}
                        }
                        // Shock travel (0x70-0x7F)
                        else if ((recordType >= 0x70) && (recordType <= 0x7F))
                        {
                            absTimeInt = inFile.ReadUInt32();
                            absTime = (float)absTimeInt / 1000.0F;
                            offsetTime = offsetTime < 0.0F ? absTime : offsetTime;
                            absTime -= offsetTime;
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Time"].AddPoint(absTime, absTime);
                            //UInt32 speedVal = inFile.ReadUInt32();
                            //deltaTime = absTime - lastSample[recordType - 0x30];
                            //lastSample[recordType - 0x30] = absTime;
                            //// not implemented on hardware
                        }
                        // Wheel Speed node (4 groups - 0x80-0x83; 0x84-0x87; 0x88-0x8B; 0x8C-0x8F)
                        else if ((recordType >= 0x80) && (recordType <= 0x8F))
                        {
                            absTimeInt = inFile.ReadUInt32();
                            absTime = (float)absTimeInt / 1000.0F;
                            offsetTime = offsetTime < 0.0F ? absTime : offsetTime;
                            absTime -= offsetTime;
                            // conversion for 205/50R15 tires (874.18 revs/mile) with 4 magnets
                            // 1029.534994/interval
                            UInt32 speedVal = inFile.ReadUInt32();
                            float speedValF = 1029.534994F / (float)speedVal;
                            if(float.IsInfinity(speedValF))
                            {
                                continue;
                            }
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Time"].AddPoint(absTime, absTime);
                            channelName = "SPD_" + (recordType - 0x80).ToString();
                            if (!YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels.ContainsKey(channelName))
                            {
                                YamuraViewMain.dataLogger.sessionData[logSessionsIdx].AddChannel(channelName, "Wheelspeed channel " + channelName, "SPD", 1.0F);
                            }
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels[channelName].AddPoint((float)absTime, speedValF);
                        }
                        // Engine RPM (0x90)
                        else if (recordType == 0x90)
                        {
                            absTimeInt = inFile.ReadUInt32();
                            absTime = (float)absTimeInt / 1000.0F;
                            offsetTime = offsetTime < 0.0F ? absTime : offsetTime;
                            absTime -= offsetTime;
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Time"].AddPoint(absTime, absTime);
                            //UInt32 speedVal = inFile.ReadUInt32();
                            //deltaTime = absTime - lastSample[recordType - 0x30];
                            //lastSample[recordType - 0x30] = absTime;
                            //// not implemented on hardware
                        }
                        // CAN interface (0xA0)
                        else if (recordType == 0xA0)
                        {
                            absTimeInt = inFile.ReadUInt32();
                            absTime = (float)absTimeInt / 1000.0F;
                            offsetTime = offsetTime < 0.0F ? absTime : offsetTime;
                            absTime -= offsetTime;
                            YamuraViewMain.dataLogger.sessionData[logSessionsIdx].channels["Time"].AddPoint(absTime, absTime);
                            //UInt32 speedVal = inFile.ReadUInt32();
                            //deltaTime = absTime - lastSample[recordType - 0x30];
                            //lastSample[recordType - 0x30] = absTime;
                            //// not implemented on hardware
                        }
                        // unknown message
                        else
                        {
                            //outStr.AppendFormat("Unknown record type 0x{0:X02}{1}", (byte)recordType, System.Environment.NewLine);
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            Cursor = Cursors.Default;

            //if (errStr.Length > 0)
            //{
            //    FileInfo errInfo = new FileInfo();
            //    errInfo.FileInfoText = errStr.ToString();
            //    errInfo.ShowDialog();
            //}
            UpdateData();
        }
        public void UpdateData()
        {
            dataGridSessions.Rows.Clear();
            int sessionCount = 0;
            foreach(SessionData curSession in YamuraViewMain.dataLogger.sessionData)
            {
                dataGridSessions.Rows.Add();
                dataGridSessions.Rows[sessionCount].Cells[0].Value = sessionCount + 1;
                dataGridSessions.Rows[sessionCount].Cells[1].Value = curSession.dateStr + " " + curSession.timeStr;
                dataGridSessions.Rows[sessionCount].Cells[2].Value = curSession.channels["Time"].DataRange[0].ToString("0.000");
                dataGridSessions.Rows[sessionCount].Cells[3].Value = curSession.channels["Time"].DataRange[1].ToString("0.000");
                dataGridSessions.Rows[sessionCount].Cells[4].Value = curSession.channels.Count;
                dataGridSessions.Rows[sessionCount].Cells[5].Value = curSession.fileName;
                sessionCount++;
            }
        //    int channelIdx = 0;
        //    #region update display info
        //    for (int sessionIdx = initialSessionCount; sessionIdx < YamuraViewMain.dataLogger.sessionData.Count; sessionIdx++)
        //    {
        //        SessionData curSession = YamuraViewMain.dataLogger.sessionData[YamuraViewMain.dataLogger.sessionData.Count - 1];
        //        {
        //            channelIdx = 0;
        //            foreach (KeyValuePair<String, DataChannel> curChannel in curSession.channels)
        //            {
        //                if (curChannel.Value.DataRange[0] == curChannel.Value.DataRange[1])
        //                {
        //                    curChannel.Value.DataRange[1] += 1;
        //                }
        //                String axisName = curChannel.Key;
        //                for (int chartIdx = 0; chartIdx < chartControls.Count; chartIdx++)
        //                {
        //                    if (chartControls[chartIdx].ChartAxes.ContainsKey(axisName))
        //                    {
        //                        chartControls[chartIdx].ChartAxes[axisName].AxisValueRange[0] = chartControls[chartIdx].ChartAxes[axisName].AxisValueRange[0] < curChannel.Value.DataRange[0] ? chartControls[chartIdx].ChartAxes[axisName].AxisValueRange[0] : curChannel.Value.DataRange[0];
        //                        chartControls[chartIdx].ChartAxes[axisName].AxisValueRange[1] = chartControls[chartIdx].ChartAxes[axisName].AxisValueRange[1] > curChannel.Value.DataRange[1] ? chartControls[chartIdx].ChartAxes[axisName].AxisValueRange[1] : curChannel.Value.DataRange[1];
        //                    }
        //                    else
        //                    {
        //                        chartControls[chartIdx].ChartAxes.Add(axisName, new Axis());
        //                        chartControls[chartIdx].ChartAxes[axisName].AxisValueRange[0] = curChannel.Value.DataRange[0];
        //                        chartControls[chartIdx].ChartAxes[axisName].AxisValueRange[1] = curChannel.Value.DataRange[1];
        //                    }
        //                    chartControls[chartIdx].ChartAxes[axisName].AxisName = axisName;
        //                    chartControls[chartIdx].ChartAxes[axisName].AxisValueRange[2] = chartControls[chartIdx].ChartAxes[axisName].AxisValueRange[1] - chartControls[chartIdx].ChartAxes[axisName].AxisValueRange[0];
        //                    chartControls[chartIdx].ChartAxes[axisName].AxisDisplayRange[0] = chartControls[chartIdx].ChartAxes[axisName].AxisValueRange[0];
        //                    chartControls[chartIdx].ChartAxes[axisName].AxisDisplayRange[1] = chartControls[chartIdx].ChartAxes[axisName].AxisValueRange[1];
        //                    chartControls[chartIdx].ChartAxes[axisName].AxisDisplayRange[2] = chartControls[chartIdx].ChartAxes[axisName].AxisValueRange[2];

        //                    chartControls[chartIdx].ChartAxes[axisName].AssociatedChannels.Add((sessionIdx.ToString() + "-" + curChannel.Key), new ChannelInfo(sessionIdx, curChannel.Key));
        //                    chartControls[chartIdx].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].ChannelColor = penColors[channelIdx % penColors.Count];
        //                    chartControls[chartIdx].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisOffset[0] = 0.0F;
        //                    chartControls[chartIdx].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisOffset[1] = 0.0F;
        //                    chartControls[chartIdx].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisRange[0] = chartControls[chartIdx].ChartAxes[axisName].AxisValueRange[0];
        //                    chartControls[chartIdx].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisRange[1] = chartControls[chartIdx].ChartAxes[axisName].AxisValueRange[1];
        //                    chartControls[chartIdx].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisRange[2] = chartControls[chartIdx].ChartAxes[axisName].AxisValueRange[2];

        //                }
        //                #region strip chart control axes create/update
        //                //if (chartControls[0].ChartAxes.ContainsKey(axisName))
        //                //{
        //                //    chartControls[0].ChartAxes[axisName].AxisValueRange[0] = chartControls[0].ChartAxes[axisName].AxisValueRange[0] < curChannel.Value.DataRange[0] ? chartControls[0].ChartAxes[axisName].AxisValueRange[0] : curChannel.Value.DataRange[0];
        //                //    chartControls[0].ChartAxes[axisName].AxisValueRange[1] = chartControls[0].ChartAxes[axisName].AxisValueRange[1] > curChannel.Value.DataRange[1] ? chartControls[0].ChartAxes[axisName].AxisValueRange[1] : curChannel.Value.DataRange[1];
        //                //}
        //                //else
        //                //{
        //                //    chartControls[0].ChartAxes.Add(axisName, new Axis());
        //                //    chartControls[0].ChartAxes[axisName].AxisValueRange[0] = curChannel.Value.DataRange[0];
        //                //    chartControls[0].ChartAxes[axisName].AxisValueRange[1] = curChannel.Value.DataRange[1];
        //                //}
        //                //chartControls[0].ChartAxes[axisName].AxisName = axisName;
        //                //chartControls[0].ChartAxes[axisName].AxisValueRange[2] = chartControls[0].ChartAxes[axisName].AxisValueRange[1] - chartControls[0].ChartAxes[axisName].AxisValueRange[0];
        //                //chartControls[0].ChartAxes[axisName].AxisDisplayRange[0] = chartControls[0].ChartAxes[axisName].AxisValueRange[0];
        //                //chartControls[0].ChartAxes[axisName].AxisDisplayRange[1] = chartControls[0].ChartAxes[axisName].AxisValueRange[1];
        //                //chartControls[0].ChartAxes[axisName].AxisDisplayRange[2] = chartControls[0].ChartAxes[axisName].AxisValueRange[2];

        //                //chartControls[0].ChartAxes[axisName].AssociatedChannels.Add((sessionIdx.ToString() + "-" + curChannel.Key), new ChannelInfo(sessionIdx, curChannel.Key));
        //                //chartControls[0].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].ChannelColor = penColors[channelIdx % penColors.Count];
        //                //chartControls[0].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisOffset[0] = 0.0F;
        //                //chartControls[0].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisOffset[1] = 0.0F;
        //                //chartControls[0].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisRange[0] = chartControls[0].ChartAxes[axisName].AxisValueRange[0];
        //                //chartControls[0].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisRange[1] = chartControls[0].ChartAxes[axisName].AxisValueRange[1];
        //                //chartControls[0].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisRange[2] = chartControls[0].ChartAxes[axisName].AxisValueRange[2];
        //                #endregion
        //                #region traction circle control axes create/update
        //                //if (chartControls[1].ChartAxes.ContainsKey(axisName))
        //                //{
        //                //    chartControls[1].ChartAxes[axisName].AxisValueRange[0] = chartControls[1].ChartAxes[axisName].AxisValueRange[0] < curChannel.Value.DataRange[0] ? chartControls[1].ChartAxes[axisName].AxisValueRange[0] : curChannel.Value.DataRange[0];
        //                //    chartControls[1].ChartAxes[axisName].AxisValueRange[1] = chartControls[1].ChartAxes[axisName].AxisValueRange[1] > curChannel.Value.DataRange[1] ? chartControls[1].ChartAxes[axisName].AxisValueRange[1] : curChannel.Value.DataRange[1];
        //                //}
        //                //else
        //                //{
        //                //    chartControls[1].ChartAxes.Add(axisName, new Axis());
        //                //    chartControls[1].ChartAxes[axisName].AxisValueRange[0] = curChannel.Value.DataRange[0];
        //                //    chartControls[1].ChartAxes[axisName].AxisValueRange[1] = curChannel.Value.DataRange[1];
        //                //}
        //                //chartControls[1].ChartAxes[axisName].AxisName = axisName;
        //                //chartControls[1].ChartAxes[axisName].AxisValueRange[2] = chartControls[1].ChartAxes[axisName].AxisValueRange[1] - chartControls[1].ChartAxes[axisName].AxisValueRange[0];
        //                //chartControls[1].ChartAxes[axisName].AxisDisplayRange[0] = chartControls[1].ChartAxes[axisName].AxisValueRange[0];
        //                //chartControls[1].ChartAxes[axisName].AxisDisplayRange[1] = chartControls[1].ChartAxes[axisName].AxisValueRange[1];
        //                //chartControls[1].ChartAxes[axisName].AxisDisplayRange[2] = chartControls[1].ChartAxes[axisName].AxisValueRange[2];

        //                //chartControls[1].ChartAxes[axisName].AssociatedChannels.Add((sessionIdx.ToString() + "-" + curChannel.Key), new ChannelInfo(sessionIdx, curChannel.Key));
        //                //chartControls[1].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].ChannelColor = penColors[channelIdx % penColors.Count];
        //                //chartControls[1].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisOffset[0] = 0.0F;
        //                //chartControls[1].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisOffset[1] = 0.0F;
        //                //chartControls[1].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisRange[0] = chartControls[1].ChartAxes[axisName].AxisValueRange[0];
        //                //chartControls[1].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisRange[1] = chartControls[1].ChartAxes[axisName].AxisValueRange[1];
        //                //chartControls[1].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisRange[2] = chartControls[1].ChartAxes[axisName].AxisValueRange[2];
        //                #endregion
        //                #region track map control axes create/update
        //                //if (chartControls[2].ChartAxes.ContainsKey(axisName))
        //                //{
        //                //    chartControls[2].ChartAxes[axisName].AxisValueRange[0] = chartControls[2].ChartAxes[axisName].AxisValueRange[0] < curChannel.Value.DataRange[0] ? chartControls[2].ChartAxes[axisName].AxisValueRange[0] : curChannel.Value.DataRange[0];
        //                //    chartControls[2].ChartAxes[axisName].AxisValueRange[1] = chartControls[2].ChartAxes[axisName].AxisValueRange[1] > curChannel.Value.DataRange[1] ? chartControls[2].ChartAxes[axisName].AxisValueRange[1] : curChannel.Value.DataRange[1];
        //                //}
        //                //else
        //                //{
        //                //    chartControls[2].ChartAxes.Add(axisName, new Axis());
        //                //    chartControls[2].ChartAxes[axisName].AxisValueRange[0] = curChannel.Value.DataRange[0];
        //                //    chartControls[2].ChartAxes[axisName].AxisValueRange[1] = curChannel.Value.DataRange[1];
        //                //}
        //                //chartControls[2].ChartAxes[axisName].AxisName = axisName;
        //                //chartControls[2].ChartAxes[axisName].AxisValueRange[2] = chartControls[2].ChartAxes[axisName].AxisValueRange[1] - chartControls[2].ChartAxes[axisName].AxisValueRange[0];

        //                //chartControls[2].ChartAxes[axisName].AxisDisplayRange[0] = chartControls[2].ChartAxes[axisName].AxisValueRange[0];
        //                //chartControls[2].ChartAxes[axisName].AxisDisplayRange[1] = chartControls[2].ChartAxes[axisName].AxisValueRange[1];
        //                //chartControls[2].ChartAxes[axisName].AxisDisplayRange[2] = chartControls[2].ChartAxes[axisName].AxisValueRange[2];

        //                //chartControls[2].ChartAxes[axisName].AssociatedChannels.Add((sessionIdx.ToString() + "-" + curChannel.Key), new ChannelInfo(sessionIdx, curChannel.Key));
        //                //chartControls[2].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].ChannelColor = penColors[channelIdx % penColors.Count];
        //                //chartControls[2].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisOffset[0] = 0.0F;
        //                //chartControls[2].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisOffset[1] = 0.0F;
        //                //chartControls[2].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisRange[0] = chartControls[2].ChartAxes[axisName].AxisValueRange[0];
        //                //chartControls[2].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisRange[1] = chartControls[2].ChartAxes[axisName].AxisValueRange[1];
        //                //chartControls[2].ChartAxes[axisName].AssociatedChannels[(sessionIdx.ToString() + "-" + curChannel.Key)].AxisRange[2] = chartControls[2].ChartAxes[axisName].AxisValueRange[2];
        //                #endregion
        //                channelIdx++;
        //            }
        //            for (int chartIdx = 0; chartIdx < chartControls.Count; chartIdx++)
        //            {
        //                chartControls[chartIdx].chartViewForm.ChartOwner.ChartAxes = chartControls[chartIdx].ChartAxes;
        //                chartControls[chartIdx].chartPropertiesForm.ChartOwner.ChartAxes = chartControls[chartIdx].ChartAxes;
        //            }
        //            //chartControls[0].chartViewForm.ChartOwner.ChartAxes = chartControls[0].ChartAxes;
        //            //chartControls[0].chartPropertiesForm.ChartOwner.ChartAxes = chartControls[0].ChartAxes;
        //            //chartControls[1].chartViewForm.ChartOwner.ChartAxes = chartControls[1].ChartAxes;
        //            //chartControls[1].chartPropertiesForm.ChartOwner.ChartAxes = chartControls[1].ChartAxes;
        //            //chartControls[2].chartViewForm.ChartOwner.ChartAxes = chartControls[2].ChartAxes;
        //            //chartControls[2].chartPropertiesForm.ChartOwner.ChartAxes = chartControls[2].ChartAxes;

        //            //chartControls[0].Logger = YamuraViewMain.dataLogger;
        //            //chartControls[0].chartViewForm.Logger = YamuraViewMain.dataLogger;
        //            //chartControls[0].chartPropertiesForm.Logger = YamuraViewMain.dataLogger;

        //            //chartControls[1].Logger = YamuraViewMain.dataLogger;
        //            //chartControls[1].chartViewForm.Logger = YamuraViewMain.dataLogger;
        //            //chartControls[1].chartPropertiesForm.Logger = YamuraViewMain.dataLogger;

        //            //chartControls[2].Logger = YamuraViewMain.dataLogger;
        //            //chartControls[2].chartViewForm.Logger = YamuraViewMain.dataLogger;
        //            //chartControls[2].chartPropertiesForm.Logger = YamuraViewMain.dataLogger;
        //        }
        //    }
        //    #endregion
        }
        #endregion
        #region GPS functions
        /// <summary>
        /// very specific NMEA parser for the output from Sparkfun QWIIC GPS breakout
        /// see the Titan datasheet for more info
        /// 
        /// GGA - Time, position and fix type data.
        /// $GPGGA -
        /// $GNGGA -
        ///
        /// GSA - GNSS receiver operating mode, active satellites used in the position solution and DOP values.
        /// $GPGSA
        /// $GLGSA
        ///
        /// GSV - The number of GPS satellites in view satellite ID numbers, elevation, azimuth, and SNR values.
        /// $GPGSV
        /// $GLGSV
        ///
        /// RMC - Time, date, position, course and speed data. The recommended minimum navigation information.
        /// $GPRMC
        /// $GNRMC
        /// 
        /// Course and speed information relative to the ground.
        /// $GPVTG
        /// $GNVTG
        /// 
        /// </summary>
        /// <param name="inFile"></param>
        public bool ParseGPS_NMEA(BinaryReader inFile, out String date, out int hr, out int min, out float sec, out float lat, out String ns, out float lng, out String ew, out float hd, out float speed, out int sat, ref StringBuilder errStr)
        {
            bool rVal = false;
            int utcHour = -1;
            int utcMin = -1;
            int utcSec = -1;
            int utcmSec = -1;
            int latDeg = -1;
            int latMin = -1;
            int latMinDecimal = -1;
            int longDeg = -1;
            int longMin = -1;
            int longMinDecimal = -1;
            int fixType = -1;
            int satellites = -1;
            Single speedKnotsPH = 0.0F;
            Single speedKmPH = 0.0F;
            Single heading = 0.0F;
            String dateStr = "";
            String nsIndication = "";
            String ewIndication = "";
            String dataValid = "";
            lat = 0.0F;
            ns = "X";
            lng = 0.0F;
            ew = "X";
            hd = 0.0F;
            speed = 0.0F;
            sat = 0;
            date = "xx/xx/xxxx";
            hr = 0;
            min = 0;
            sec = 0F;


            char c;
            String dataSentence;
            // sentence always begins with '$', ends with 0x0D
            // except when it doesn't - sometimes the '$' gets dropped....
            while ((inFile.PeekChar() == '$') || (inFile.PeekChar() == 'G'))
            {
                #region read sentence
                dataSentence = "";
                c = inFile.ReadChar();
                while (c != 0x0D)
                {
                    dataSentence += c;
                    c = (char)inFile.ReadByte();
                }
                #region checksum
                // malformed, no * 
                if (dataSentence.IndexOf('*') < 0)
                {
                    errStr.AppendFormat("malformed NMEA sentance - missing '*' {0}{1}", dataSentence, System.Environment.NewLine);
                    continue;
                }
                int receivedChecksum = 0;
                // check for malformed, illegal char in hex value
                try
                {
                    receivedChecksum = Convert.ToInt32(dataSentence.Substring(dataSentence.IndexOf('*') + 1), 16);
                }
                catch
                {
                    errStr.AppendFormat("error reading checksum from NMEA sentance {0}{1}", dataSentence, System.Environment.NewLine);
                    continue;
                }
                // calculate checksum for characters between $ and *
                int calculatedChecksum = 0;
                int charIdx = 1;
                while (dataSentence[charIdx] != '*')
                {
                    calculatedChecksum ^= Convert.ToByte(dataSentence[charIdx]);
                    charIdx++;
                }
                // bad checksum - skip this sentence
                if (calculatedChecksum != receivedChecksum)
                {
                    errStr.AppendFormat("checksum mismatch read 0x{0:X} calculated 0x{1:X} for NMEA sentance {2}{3}", receivedChecksum, calculatedChecksum, dataSentence, System.Environment.NewLine);
                    continue;
                }
                #endregion

                String[] words = dataSentence.Split(new char[] { ',' });
                #endregion
                try
                {
                    #region PARSE GGA - Time, position and fix type data.
                    if ((dataSentence.StartsWith("$GPGGA")) || // GPS
                        (dataSentence.StartsWith("$GNGGA")))   // 
                    {
                        utcHour = Convert.ToInt32(words[1].Substring(0, 2));
                        utcMin = Convert.ToInt32(words[1].Substring(2, 2));
                        utcSec = Convert.ToInt32(words[1].Substring(4, 2));
                        utcmSec = Convert.ToInt32(words[1].Substring(7, 3));

                        latDeg = Convert.ToInt32(words[2].Substring(0, 2));
                        latMin = Convert.ToInt32(words[2].Substring(2, 2));
                        latMinDecimal = Convert.ToInt32(words[2].Substring(5, 4));

                        nsIndication = words[3];

                        longDeg = Convert.ToInt32(words[4].Substring(0, 3));
                        longMin = Convert.ToInt32(words[4].Substring(3, 2));
                        longMinDecimal = Convert.ToInt32(words[4].Substring(6, 4));

                        ewIndication = words[5];

                        fixType = Convert.ToInt32(words[6]);

                        satellites = Convert.ToInt32(words[7]);
                    }
                    #endregion
                    #region PARSE RMC - Time, date, position, course and speed data. The recommended minimum navigation information.
                    else if ((dataSentence.StartsWith("$GPRMC")) || // GPS
                             (dataSentence.StartsWith("$GNRMC"))) // GNSS
                    {
                        utcHour = Convert.ToInt32(words[1].Substring(0, 2));
                        utcMin = Convert.ToInt32(words[1].Substring(2, 2));
                        utcSec = Convert.ToInt32(words[1].Substring(4, 2));
                        utcmSec = Convert.ToInt32(words[1].Substring(7, 3));

                        dataValid = words[2];

                        latDeg = Convert.ToInt32(words[3].Substring(0, 2));
                        latMin = Convert.ToInt32(words[3].Substring(2, 2));
                        latMinDecimal = Convert.ToInt32(words[3].Substring(5, 4));

                        nsIndication = words[4];

                        longDeg = Convert.ToInt32(words[5].Substring(0, 3));
                        longMin = Convert.ToInt32(words[5].Substring(3, 2));
                        longMinDecimal = Convert.ToInt32(words[5].Substring(6, 4));

                        ewIndication = words[6];

                        speedKnotsPH = Convert.ToSingle(words[7]);
                        heading = Convert.ToSingle(words[8]);
                        dateStr = words[9];
                    }
                    #endregion
                    #region PARSE VTG - Course and speed information relative to the ground.
                    else if ((dataSentence.StartsWith("$GPVTG")) || // GPS
                             (dataSentence.StartsWith("$GNVTG"))) // GNSS
                    {
                        heading = Convert.ToSingle(words[1]);
                        speedKnotsPH = Convert.ToSingle(words[5]);
                        speedKmPH = Convert.ToSingle(words[7]);
                    }
                    #endregion
                    #region SKIP: GSA - GNSS receiver operating mode, active satellites used in the position solution and DOP values.
                    else if ((dataSentence.StartsWith("$GPGSA")) || // GPS, GNSS
                             (dataSentence.StartsWith("$GLGSA"))) // GPS+GLONASS
                    {

                    }
                    #endregion
                    #region SKIP: GSV - The number of GPS satellites in view satellite ID numbers, elevation, azimuth, and SNR values.
                    else if ((dataSentence.StartsWith("$GPGSV")) || // GPS, GNSS
                             (dataSentence.StartsWith("$GLGSV"))) // GPS + GLONASS
                    {

                    }
                    #endregion
                    #region SKIP unknown/deformed sentances - ignore
                    else
                    {
                        errStr.AppendFormat("ignored unknown/deformed NMEA sentance {2}{3}", receivedChecksum, calculatedChecksum, dataSentence, System.Environment.NewLine);
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    errStr.AppendFormat("ParseNMEA error reading sentence from {0} error: {1}{2}", dataSentence, e.Message, System.Environment.NewLine);
                }
            }
            if (latDeg == -1)
            {
                rVal = false;
            }
            else
            {
                rVal = true;
                if (dateStr.Length < 6)
                {
                    errStr.AppendFormat("ParseNMEA bad date string {0}{1}", dateStr, System.Environment.NewLine);
                    date = "xx/xx/xxxx";
                }
                else
                {
                    date = dateStr.Substring(2, 2) + "/" + dateStr.Substring(0, 2) + "/20" + dateStr.Substring(4, 2);
                }
                hr = utcHour;
                min = utcMin;
                sec = (Single)utcSec + (Single)utcmSec / 1000.0F;

                lat = (Single)latDeg + ((Single)latMin + ((Single)latMinDecimal / 10000.0F)) / 60.0F;
                ns = nsIndication;
                lng = (Single)longDeg + ((Single)longMin + ((Single)longMinDecimal / 10000.0F)) / 60.0F;
                ew = ewIndication;
                hd = heading;
                if ((speedKmPH == -1.0F) && (speedKnotsPH != -1.0F))
                {
                    speedKmPH = speedKnotsPH * 1.852F;
                }
                speed = speedKmPH;
                sat = satellites;
            }
            return rVal;
        }
        /// <summary>
        /// distance between 2 points of GPS data (lat/long)
        /// </summary>
        /// <param name="lat1Deg"></param>
        /// <param name="long1Deg"></param>
        /// <param name="lat2Deg"></param>
        /// <param name="long2Deg"></param>
        /// <returns></returns>
        private float GPSDistance(float lat1Deg, float long1Deg, float lat2Deg, float long2Deg)
        {
            // This uses the ‘haversine’ formula to calculate the great - circle distance between two points 
            // the shortest distance over the earth’s surface – giving an ‘as- the - crow - flies’ distance between the points 
            // (ignoring any hills they fly over, of course!).
            // Haversine formula:	a = sin²(deltaLat / 2) + cos lat1 ⋅ cos lat2 ⋅ sin²(deltaLong / 2)
            // c = 2 ⋅ atan2( √a, √(1−a) )
            // d = R ⋅ c
            // where   'lat' is latitude, 'long' is longitude, R is earth’s radius (mean radius = 6371km);

            double R = 6371e3F; // metres
            R = R * 3.28084; // feet
            double lat1 = DegreesToRadians(lat1Deg);
            double lat2 = DegreesToRadians(lat2Deg);
            double long1 = DegreesToRadians(lat1Deg);
            double long2 = DegreesToRadians(lat2Deg);
            double deltaLat = lat2 - lat1;
            double deltaLong = long2 - long1;

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                    Math.Cos(lat1) * Math.Cos(lat2) *
                    Math.Sin(deltaLong / 2) * Math.Sin(deltaLong / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double d = R * c;
            return (float)d;
        }
        /// <summary>
        /// convert degrees to radians
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        private float DegreesToRadians(double deg)
        {
            double rad = (deg * Math.PI) / 180.0;
            return (float)rad;
        }
        #endregion

        private void removeSessionsMenuItem_Click(object sender, EventArgs e)
        {
            for(int rowIdx = dataGridSessions.Rows.Count-1; rowIdx > 0; rowIdx--)
            {
                if (dataGridSessions.Rows[rowIdx].Selected == true)
                {
                    YamuraViewMain.dataLogger.sessionData.RemoveAt(rowIdx);
                }
            }
            UpdateData();
        }
    }
}
