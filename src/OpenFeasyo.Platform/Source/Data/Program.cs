using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Data
{
    class Program
    {
        static void Main(string[] args)
        {
            Ict4Rehab sg = new Ict4Rehab(new JsonProvider("etro", "ict4rehab"), "https://www.feasymotion.com/ict4rehab-demo");
            Game[] games = sg.Games;
            //PatientSettings[] settings = sg.AllPatientSettings;

            Session[] sessions = sg.AllSessions;

            Session s = sg.GetSession(12047);

            bool login = sg.AuthenticateTherapist("TherapistUserName", "Password");
            Console.WriteLine(games.Length);

            
        }
    }
}
  