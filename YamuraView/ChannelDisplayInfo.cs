using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YamuraView
{
    /// <summary>
    /// contains info for display of a channel in a view
    /// includes a reference to the master channel data
    /// </summary>
    public class ChannelDisplayInfo
    {
        // reference to channel data in logger file
        public DataChannel dataChannel = null;
        // reference to the channels axis
        public DataChannel axisChannel = null;
        // session channel is in
        int sessionIdx = 0;
        public int SessionIdx
        {
            get 
            { 
                return sessionIdx; 
            }
            set 
            {
                sessionIdx = value;
            }
        }
        // display channel in view
        bool showChannel = false;
        public bool ShowChannel
        {
            get { return showChannel; }
            set { showChannel = value; }
        }
        // trace color
        Color channelColor = Color.Red ;
        public Color ChannelColor
        {
            get { return channelColor; }
            set { channelColor = value; }
        }
        // axis offset 
        float[] axisOffsetX = new float[] { 0.0F, 0.0F };
        /// <summary>
        /// axis display offset values
        /// </summary>
        public float[] AxisOffsetX
        {
            get { return axisOffsetX; }
            set { axisOffsetX = value; }
        }
        /// <summary>
        /// axis offset value
        /// </summary>
        // axis offset 
        float[] axisOffsetY = new float[] { 0.0F, 0.0F };
        /// <summary>
        /// axis offset value
        /// </summary>
        public float[] AxisOffsetY
        {
            get { return axisOffsetY; }
            set { axisOffsetY = value; }
        }
        // path of points to display
        // generate on first display and if an axis changes
        GraphicsPath channelPath = new GraphicsPath();
        public GraphicsPath ChannelPath
        {
            get { return channelPath; }
            set { channelPath = value; }
        }
        public ChannelDisplayInfo(int session, DataChannel channelData, DataChannel axisData)
        {
            sessionIdx = session;
            dataChannel = channelData;
            axisChannel = axisData;
        }
        public override string ToString()
        {
            return dataChannel.ChannelName + "_" + SessionIdx.ToString() + " on " + axisChannel.ChannelName;
        }
    }
}
