using MainRobot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRobot.Robot.Device.IpCam
{
    public interface IIpCam
    {
        /// <summary>
        /// get snapshot for ipCam configured
        /// </summary>
        /// <returns></returns>
        Task<Image<Rgba32>> GetSnapshot();
        /// <summary>
        /// check if image is dark
        /// </summary>
        /// <returns></returns>
        Task<bool> IsDark();
        /// <summary>
        /// find color into image get from ipcam image
        /// </summary>
        /// <param name="color"></param>
        /// <param name="tollerance"></param>
        /// <param name="maxAttempts"></param>
        /// <returns></returns>
        Task<RPoint> TryFindPointOfColor(Rgba32 color, Rgba32 tollerance, short? maxAttempts = 10);
    }
}
