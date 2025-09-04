using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostlyGame.DataModel
{
    internal class DataConnection
    {
        private bool isLogged;
        private Subject currentSubject;

        public static DataConnection Instance { get; private set; }

        public DataConnection() {
            Instance = this;
        }


        public bool LogIn(String username, String password)
        {
            //TODO login request


            //currentSubject = new Subject(username, targetDuration);


            isLogged = false;

            return false;
        }

        public bool UploadFile() {

            return true;
        }
    }
}
