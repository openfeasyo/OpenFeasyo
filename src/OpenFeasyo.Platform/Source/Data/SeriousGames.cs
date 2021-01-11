/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Lubos Omelina
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;

#if !INSTALLER
    using OpenFeasyo.Platform.Data.Offline;
    using Vub.Etro.IO;
    using System.Globalization;
    #if !ANDROID && !__MACOS__
        using System.Windows.Threading;
    #endif
#endif

namespace OpenFeasyo.Platform.Data
{
    public class FileUploadEventArgs : EventArgs {
        public enum UploadAction {
            UploadStart,
            UploadFinished,
            UploadError

        }

        private string _fileName;
        private string _message;
        private UploadAction _action;

        public string FileName { get { return _fileName; } }

        public UploadAction Action { get { return _action; } }
        
        public string Message { get { return _message; } }

        internal FileUploadEventArgs(string fileName, UploadAction action, string message = "") {
            _fileName = fileName;
            _action = action;
            _message = message;
        }

    }

    public static class SeriousGames
    {
        public const string HOME_FOLDER = "OpenFeasyo";

        public static string HomeLocation = "";

        public const string DIR_SEPARATOR =
#if ANDROID
            "/";
#else
            "\\";
#endif
        private static ExtendedPatient _currentPatient = null;
        public static ExtendedPatient CurrentPatient
        {
            get
            {
                if (_currentPatient == null)
                {
                    _currentPatient = new ExtendedPatient();
                    _currentPatient.BirthDate = "N/A";
                    _currentPatient.Gender = "N/A";
                    _currentPatient.HospitalId = "N/A";
                    _currentPatient.Id = "Default";
                }
                return _currentPatient;

            }
        }

        public static string SoftwareVersion { get {
                return "OpenFeasyo."+
#if ANDROID
                    "Android";
#else
                    "Windows";
#endif
            }
        }

        internal static string CheckAndCreateHomeFolder()
        {

            String PersonalFolder = HomeLocation != "" ? HomeLocation :
#if ANDROID
                Xamarin.Essentials.FileSystem.AppDataDirectory;
#else
                Environment.GetFolderPath(Environment.SpecialFolder.Personal);
#endif
            if (!Directory.Exists(PersonalFolder + DIR_SEPARATOR + HOME_FOLDER + DIR_SEPARATOR + "Patients"))
            {
                Directory.CreateDirectory(PersonalFolder + DIR_SEPARATOR + HOME_FOLDER);
                Directory.CreateDirectory(PersonalFolder + DIR_SEPARATOR + HOME_FOLDER + DIR_SEPARATOR + "Patients");
                Directory.CreateDirectory(PersonalFolder + DIR_SEPARATOR + HOME_FOLDER + DIR_SEPARATOR + "Patients" + DIR_SEPARATOR + "Default");
            }
            return PersonalFolder + DIR_SEPARATOR + HOME_FOLDER + DIR_SEPARATOR + "Patients";
        }

        public static string GetPatientDirectory(ExtendedPatient patient)
        {
            string path = CheckAndCreateHomeFolder() + DIR_SEPARATOR + patient.Id;
            DirectoryInfo info = Directory.CreateDirectory(path);
            if (!info.Exists)
            {
                Directory.CreateDirectory(CheckAndCreateHomeFolder() + DIR_SEPARATOR + patient.Id);
            }
            return path;
        }

        // Refactor the code 


        public static string DataDir { get;set; }
        public const string ID_FILE = "ID.txt";
        public const string PHOTO_FILE = "photo.png";
        public const string CONTEXT_FILE = "current_context.cfg";
        
        private static string _server = null;
        private static string _default_server = "https://my.feasymotion.com/ict4rehab-katka";
        public static string Server { 
            get {
                if (_server == null) {
#if WINDOWS
                    _server = (string)Registry.GetValue(RegistryElements.REGISTRY_ROOT_SECTION, RegistryElements.REGISTRY_SERVER, null);
#endif
                    if (_server == null) {
                        _server = _default_server;
                    }
                }
                return _server; 
            } 
            set { 
                _server = value; 
            } 
        }
        private static Ict4Rehab _sgData;

        private static Datapoint _localDatapoint
#if !INSTALLER
            = new OfflineDatapoint();
#else
        ;
#endif
        public static Datapoint LocalDatapoint {
            get { return _localDatapoint; }
            set { _localDatapoint = value;  }
        }
        
        //public static string SoftwareVersion { get; set; }

        private static Game [] _games = null;
        
        
        private static ConfiguredGame _currentGame;
        public static ConfiguredGame CurrentGame
        {
            get { return _currentGame; }
            set { _currentGame = value; }
        }

        
        //private static Account GetCurrentAccount() {
            //string currentContext = GetCurrentContext();
            //foreach (Account a in GetLocalAccounts()) {
            //    if (currentContext.StartsWith(a.Directory))
            //    {
            //        return a;
            //    }
            //}
        //    return null;
        //}

        //private static List<Account> LoadAccounts()
        //{
        //    CheckAndCreateHomeFolder();
        //    List<Account> list = new List<Account>();
        //    string [] dirs = Directory.GetDirectories(DataDir);
        //    foreach (string dir in dirs) 
        //    {
        //        Account a = new Account();
        //        a.Directory = dir;
        //        a.Name = dir.Substring(dir.LastIndexOf("\\") + 1);
                
        //        string idFilePath = dir + "\\" + ID_FILE;
        //        if (File.Exists(idFilePath)) {
        //            StreamReader reader = new StreamReader(idFilePath);
        //            a.Id = reader.ReadLine();
        //            reader.Close();
        //        }
        //        list.Add(a);
        //    }
        //    return list;
        //}


        //public static string GetPatientDirectory(ExtendedPatient patient) {
        //    string path = DataDir + "\\" + patient.Id;
        //    DirectoryInfo info = Directory.CreateDirectory(path);
        //    if(!info.Exists){
        //        CreatePatientDirectory(patient);
        //    }
        //    return path;
        //}

        public static bool CreatePatientDirectory(ExtendedPatient patient) 
        {
            DirectoryInfo info = Directory.CreateDirectory(DataDir+"\\"+ patient.Id);
            if (info != null) {
                // create id-file
                StreamWriter writer = new StreamWriter(info.FullName + "\\" + ID_FILE);
                writer.WriteLine(patient.Id);
                writer.WriteLine(patient.HospitalId);
                writer.Close();
                
                return true;
            }
            return false;
        }

        //public static void SetContextTo(Account account) 
        //{
        //    StreamWriter writer = new StreamWriter(CONTEXT_FILE);
        //    writer.WriteLine(account.Directory);
        //    writer.Close();
        //}

        public static DataUploading[] getAllUploadings() {
            if (_sgData == null)    
            {
                _sgData = new Ict4Rehab(new JsonProvider("etro", "ict4rehab"), Server);
            }
            return (DataUploading[])_sgData.AllDataUploadings;
        }

        public static bool UploadFile(String file, DataUploading uploading) {
            return _sgData.UploadFile(file, uploading.Id);
        }

        public static string GetCurrentContext()
        {
            string dir;
            TextReader reader = new StreamReader(CONTEXT_FILE);
            DirectoryInfo di = new DirectoryInfo(reader.ReadLine());
            dir = di.FullName + "\\";
            reader.Close();
            return dir;
        }


        //public static void UpdateAccounts(List<ExtendedPatient> accounts)
        //{
        //    if (_sgData == null)
        //    {
        //        _sgData = new Ict4Rehab(new JsonProvider("etro", "ict4rehab"), Server);
        //    }
        //    //PatientSettings[] settings = (PatientSettings[])_sgData.AllPatientSettings;
        //    ExtendedPatient[] patients = _sgData.AllPatients;
        //    if (patients != null)
        //        foreach (ExtendedPatient s in patients)
        //        {
        //            Account newA = null;
        //            foreach (Account a in accounts)
        //            {
        //                if (a.Id == s.Id)
        //                {
        //                    newA = a;
        //                    newA.Patient = s;
        //                    //newA.PatientSettings = s;
        //                    // copy remote data
        //                }
        //            }
        //            if (newA == null)
        //            {
        //                newA = new Account();
        //                newA.Id = s.Id;
        //                newA.Patient = s;
        //                accounts.Add(newA);
        //            }
        //        }
        //    //if(settings != null)
        //    //foreach (PatientSettings s in settings)
        //    //{
        //    //    Account newA = null;
        //    //    foreach (Account a in accounts)
        //    //    {
        //    //        if (a.Id == s.Patient.Id.Id)
        //    //        {
        //    //            newA = a;
        //    //            newA.PatientSettings = s;
        //    //            // copy remote data
        //    //        }
        //    //    }
        //    //    if (newA == null)
        //    //    {
        //    //        newA = new Account();
        //    //        newA.Id = s.Patient.Id.Id;
        //    //        accounts.Add(newA);
        //    //    }
        //    //}

        //}

        public static Game GetGameForFileName(string fileName) {
            if (_games == null) {
                _games = _sgData.Games;
            }

            foreach(Game g in _games){
                string packedName = fileName.Replace(" ", string.Empty).ToLower();
                if (packedName.StartsWith(g.Name.ToLower()))
                {
                    return g;
                }
            }
            return null;
        }


#region File Upload Events 

        public static event EventHandler<FileUploadEventArgs> FileUploadStarted;
        public static event EventHandler<FileUploadEventArgs> FileUploadFinished;
        public static event EventHandler<FileUploadEventArgs> FileUploadError;

        private static void OnFileUploadStarted(FileUploadEventArgs args)
        {
            if (FileUploadStarted != null) 
            {
                FileUploadStarted(null, args);
            }
        }

        private static void OnFileUploadFinished(FileUploadEventArgs args)
        {
            if (FileUploadFinished != null)
            {
                FileUploadFinished(null, args);
            }
        }

        private static void OnFileUploadError(FileUploadEventArgs args)
        {
            if (FileUploadError != null)
            {
                FileUploadError(null, args);
            }
        }
#endregion

#if !INSTALLER && !ANDROID && !__MACOS__
        public static bool UploadFiles(Datapoint dp, ExtendedPatient patient, Dispatcher dispatcher) {

            List<DataUploading> uploadings = dispatcher.Invoke(new Func< List<DataUploading> >(() => {
                return new List<DataUploading>(
                    dp.SelectAll<DataUploading>().Where(u => (u.Id == -1)));
            }));
            

            Patient p = null;
            if (patient != null)
            {
                p = new Patient();  
                p.Id = new PatientId();
                p.Id.Id = patient.Id;
                p.Id.Hospital = new Hospital();
                p.Id.Hospital.HospitalId = patient.HospitalId;
            }

            Game [] games = _sgData.Games;

            foreach (DataUploading du in uploadings) {
                if (!File.Exists(du.C3DFileId)) continue;

                PartialDataUploading data = new PartialDataUploading();
                data.Game = new PartialGame(du.GameID);
                data.Patient = p;
                data.PerformanceDate = du.PerformanceDate;
                data.Results = du.Results;
                
                du.Id = _sgData.CreateDataUplodingRecord(data);
                dispatcher.Invoke(() => dp.Update<DataUploading>(du));

                OnFileUploadStarted(new FileUploadEventArgs(du.C3DFileId, FileUploadEventArgs.UploadAction.UploadStart));
                if (SeriousGames.UploadFile(du.C3DFileId, du))
                {
                    du.Uploaded = 1;
                    dispatcher.Invoke(() => dp.Update<DataUploading>(du));
                    OnFileUploadFinished(new FileUploadEventArgs(du.C3DFileId, FileUploadEventArgs.UploadAction.UploadFinished));
                }
                else {
                    OnFileUploadError(new FileUploadEventArgs(du.C3DFileId, FileUploadEventArgs.UploadAction.UploadError));
                }

            }


            //if (p == null) return false;

            //foreach (string file in Directory.GetFiles(GetPatientDirectory(patient), "*.c3d"))
            //{
                
            //    string tagFile = file + ".uploaded";

            //    if (File.Exists(tagFile) || !InValid(file))
            //    {
            //        continue;
            //    }
            //    // create uploading
            //    PartialDataUploading data = new PartialDataUploading();

            //    data.Patient = p;
            //    data.Results = ExtractScore(file).ToString();
            //    DateTime fileCreatedDate = File.GetCreationTime(file);
            //    data.PerformanceDate = fileCreatedDate.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            //    Game game = GetGameForFileName(Path.GetFileName(file));
            //    if (game == null) {
            //        Console.Error.WriteLine("Unable to find Game DB item for: " + Path.GetFileName(file));
            //        OnFileUploadError(new FileUploadEventArgs(tagFile, FileUploadEventArgs.UploadAction.UploadError));
            //        continue;
            //    }

            //    OnFileUploadStarted(new FileUploadEventArgs(tagFile, FileUploadEventArgs.UploadAction.UploadStart));

            //    data.Game = new PartialGame(game.Id);
            //    // post uploading
            //    int uploadingId = _sgData.CreateDataUplodingRecord(data);

            //    // upload file
            //    DataUploading up = _sgData.GetDataUploading(uploadingId);
            //    if (SeriousGames.UploadFile(file, up))
            //    {
            //        OnFileUploadFinished(new FileUploadEventArgs(tagFile, FileUploadEventArgs.UploadAction.UploadFinished));
            //        using (File.Create(tagFile)) { }
            //    }
            //    else {
            //        OnFileUploadError(new FileUploadEventArgs(tagFile, FileUploadEventArgs.UploadAction.UploadError));
            //    }
                
            
            //}
            return true;
        }


        private static bool InValid(string c3dFile)
        {
            bool ret = false;
            C3dReader reader = new C3dReader();
            try
            {
                if (reader.Open(c3dFile))
                {

                    ret = reader.FramesCount > 0;
                    reader.Close();
                }
            }
            catch (ApplicationException e)
            {
                Console.Error.WriteLine(e.Message);
                ret = false;
            }
            return ret;
        }

        private static double ExtractScore(string c3dFile)
        {
            float ret = 0;
            C3dReader reader = new C3dReader();
            try
            {
                if (reader.Open(c3dFile))
                {
                    ret = reader.GetParameter<float>("SUBJECTS:GAME_SCORE");
                    reader.Close();
                }
            }
            catch (ApplicationException e){
                Console.Error.WriteLine(e.Message);
                ret = -1;
            }

            return ret;
       }
#endif
        //public static void CreateUploading(Account account) {
        //    if (_sgData == null)
        //    {
        //        _sgData = new Ict4Rehab(new JsonProvider("etro", "ict4rehab"), Server);
        //    }
        //    PartialDataUploading data = new PartialDataUploading();
        //    data.Patient = account.PatientSettings.Patient;
        //    data.Results = "";
        //    DateTime fileCreatedDate = File.GetCreationTime("C:\\Users\\bubo\\Documents\\ict4rehab\\Patients\\2012-000001\\HitTheBoxes_Trunk_Accelerometer_20130224_21-55-01-0740.c3d");
        //    data.PerformanceDate = fileCreatedDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
        //    Game game = GetGameForFileName(Path.GetFileName("C:\\Users\\bubo\\Documents\\ict4rehab\\Patients\\2012-000001\\HitTheBoxes_Trunk_Accelerometer_20130224_21-55-01-0740.c3d"));
        //    data.Game = new PartialGame(game.Id);
                
        //    _sgData.CreateDataUplodingRecord(data);
        //}

        public static bool AuthenticateTherapist(string name, string password) {
            if (_sgData == null)
            {
                _sgData = new Ict4Rehab(new JsonProvider("etro", "ict4rehab"), Server);
            }
            return _sgData.AuthenticateTherapist(name, password);
        }

        public static Session GetSession(int id)
        {
            if (_sgData == null)
            {
                _sgData = new Ict4Rehab(new JsonProvider("etro", "ict4rehab"), Server);
            }
            return _sgData.GetSession(id);
        }

        public static Session[] GetTodaySessionsForCurrentPatient()
        {

            if (_sgData == null) {
                _sgData = new Ict4Rehab(new JsonProvider("etro", "ict4rehab"), Server);
            }

            return _sgData.GetTodaySessionsForCurrentPatient();
        }

        public static string GetTherapistName()
        {
            if (_sgData == null)
            {
                _sgData = new Ict4Rehab(new JsonProvider("etro", "ict4rehab"), Server);
            }


            return Ict4Rehab.LoadTherapistName();
        }

        //private static void SynchronizeTable<T>(Datapoint dp, T[] remoteArray)
        //{
        //    List<T> localPatients = new List<T>(dp.SelectAll<T>());
        //    if (remoteArray == null) return;
        //    List<T> remoteCollection = new List<T>(remoteArray);
        //    foreach (T elem in remoteCollection)
        //    {
        //        T fullPatients = _sgData.GetConfiguredGame(g.ConfiguredGameId);
        //        List<T> existing = new List<T>(localGames.Where(l => l.ConfiguredGameId == fullGame.ConfiguredGameId));
        //        foreach (T found in existing)
        //        {
        //            localGames.Remove(found);
        //            if (!fullGame.Equals(found))
        //            {
        //                // update in local database
        //                dp.Update<T>(fullGame);
        //            }
        //        }
        //        if (existing.Count<T>() <= 0)
        //        {
        //            // insert to local database
        //            dp.Insert<ConfiguredGame>(fullGame);
        //        }
        //    }
        //    // remove from local database
        //    foreach (ConfiguredGame g in localGames)
        //    {
        //        dp.Remove<ConfiguredGame>(g);
        //    }
        //}


        public static int FetchMaxScore(ExtendedPatient patient, ConfiguredGame game) {
            

            return 0;
        }

        public static void SaveMaxScore(ExtendedPatient patient, ConfiguredGame game, int score) {
        
        }

        public static void SynchronizeTables(Datapoint dp)
        {
            if (_sgData == null)
            {
                _sgData = new Ict4Rehab(new JsonProvider("etro", "ict4rehab"), Server);
            }

            List<ConfiguredGame> localGames = new List<ConfiguredGame>(dp.SelectAll<ConfiguredGame>());
            ConfiguredGame[] remoteGamesArray = _sgData.AllConfiguredGames;
            if (remoteGamesArray == null) return;
            List<ConfiguredGame> remoteGames = new List<ConfiguredGame>(remoteGamesArray);
            foreach (ConfiguredGame g in remoteGames) {
                ConfiguredGame fullGame = _sgData.GetConfiguredGame(g.ConfiguredGameId);
                List<ConfiguredGame> existing = new List<ConfiguredGame>(localGames.Where(l => l.ConfiguredGameId == fullGame.ConfiguredGameId));
                foreach (ConfiguredGame found in existing) {
                    localGames.Remove(found);
                    if (!fullGame.Equals(found))
                    { 
                        // update in local database
                        dp.Update<ConfiguredGame>(fullGame);
                    }
                }
                if(existing.Count<ConfiguredGame>() <= 0){
                    // insert to local database
                    dp.Insert<ConfiguredGame>(fullGame);
                }
            }
            // remove from local database
            foreach (ConfiguredGame g in localGames) {
                if (g.ConfiguredGameId > 0) // negative ids mean that they need to be created remotely (TODO)
                {
                    dp.Remove<ConfiguredGame>(g);
                }
            }


            //
            //  Synchronizing Patients
            //
            List<ExtendedPatient> localPatients = new List<ExtendedPatient>(dp.SelectAll<ExtendedPatient>());
            ExtendedPatient[] remotePatientArray = _sgData.AllPatients;
            if (remotePatientArray == null) return;
            List<ExtendedPatient> remotePatients = new List<ExtendedPatient>(remotePatientArray);
            foreach (ExtendedPatient p in remotePatients)
            {
                List<ExtendedPatient> existing = new List<ExtendedPatient>(localPatients.Where(l => (l.Id == p.Id && l.HospitalId == p.HospitalId) ));
                foreach (ExtendedPatient found in existing)
                {
                    localPatients.Remove(found);
                }
                if (existing.Count<ExtendedPatient>() <= 0)
                {
                    // insert to local database
                    dp.Insert<ExtendedPatient>(p);
                }
            }
            // remove from local database
            foreach (ExtendedPatient p in localPatients)
            {
                dp.Remove<ExtendedPatient>(p);
            }

            //SynchronizeTable<ExtendedPatient>(dp, _sgData.AllPatients);


            
        }
    }
}
