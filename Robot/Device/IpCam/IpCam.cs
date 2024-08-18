using MainRobot.Common;
using MainRobot.Common.Graphics;
using Robot;
using System.Net.Http.Headers;
using System.Text;

namespace MainRobot.Robot.Device.IpCam
{
    public class IpCam : IIpCam
    {
        public IpCam()
        {

        }

        public async Task<bool> IsDark() {
            var image = await this.GetSnapshot();
            var isDark =  ImageUtility.IsDarkImage(image);
            return isDark;
        }
        public async Task<RPoint> FindPointOfColor(Rgba32 color, Rgba32 tollerance)
        {
            var image = await this.GetSnapshot();
            var point = ImageUtility.PositionOfColor(image,
                                                    color.R,
                                                    color.G,
                                                    color.B,
                                                    tollerance.R,
                                                    tollerance.G,
                                                    tollerance.B);
            return point;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color">color rgb to find in image</param>
        /// <param name="tollerance">tollerance rgb to find a color</param>
        /// <param name="maxAttempts">max number of attempts , if no find color return nul</param>
        /// <returns></returns>
        public async Task<RPoint> TryFindPointOfColor(Rgba32 color, Rgba32 tollerance, short? maxAttempts=10) {
            short attempts = 0;
            RPoint point = null;
            while(point==null || attempts<maxAttempts)
            {
                point = await FindPointOfColor(color, tollerance);
            }
            return point;
        }

        public async Task<Image<Rgba32>> GetSnapshot()
        {
            var client = new HttpClient();

            var url = "http://" + Configuration.CAMERA_IP_URL + "/media/?action=snapshot";

            // Crea una richiesta GET
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Imposta l'header Authorization con le credenziali codificate in base64
            var username = Configuration.CAMERA_IP_USR;
            var password = Configuration.CAMERA_IP_PAS;
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);


            // Invia la richiesta e ottieni la risposta
            var response = await client.SendAsync(request);

            // Ottieni il byte array della risposta
            byte[] bytes = await response.Content.ReadAsByteArrayAsync();

            var img = Image.Load<Rgba32>(bytes); 
            return img;
        }
    }

}


