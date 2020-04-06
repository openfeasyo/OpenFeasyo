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
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;
using Microsoft.Win32;

namespace OpenFeasyo.Platform.Data
{
    public class Ict4Rehab
    {
        public const string GAMES = "/games";
        public const string PATIENTS_SETTINGS = "/patientseriousgamingsettings";
        public const string PATIENTS = "/patients/api?therapist=";
        //public const string PLANNEDGAMES = "/sessions/find=GamesBySession?sessionId=";
        public const string SESSIONS = "/sessions";
        public const string CONFIGURED_GAMES = "/configuredgames";
        public const string DATA_UPLOADINGS = "/seriousgamingdatauploadings";
        public const string DATA_UPLOADINGS_CONTENT = "/c3dcontents";
        public const string AUTHENTICATE_THERAPIST = "/applicationusers/api?authenticateTherapist";


        public string ServiceRoot { get; set; }

        private DataProvider _provider;

        public Ict4Rehab(DataProvider provider, string serviceRoot)
        {
            _provider = provider;
            ServiceRoot = serviceRoot;
        }

        public Game[] Games
        {
            get
            {
                return _provider.MakeRequest(ServiceRoot + GAMES, typeof(Game[])) as Game[];
            }
        }

        public PatientSettings[] AllPatientSettings { get { return _provider.MakeRequest(ServiceRoot + PATIENTS_SETTINGS, typeof(PatientSettings[])) as PatientSettings[]; } }


        public ExtendedPatient[] AllPatients
        {
            get
            {
                string name = LoadTherapistName();
                if (name == null) return new ExtendedPatient[0];
                return _provider.MakeRequest(ServiceRoot + PATIENTS + name, typeof(ExtendedPatient[])) as ExtendedPatient[];
            }
        }



        public Session[] AllSessions { get { return _provider.MakeRequest(ServiceRoot + SESSIONS, typeof(Session[])) as Session[]; } }

        public Session GetSession(int id)
        {
            return _provider.MakeRequest(ServiceRoot + SESSIONS + "/" + id, typeof(Session)) as Session;
        }

        public Session[] GetTodaySessionsForCurrentPatient()
        {
            if (SeriousGames.CurrentPatient == null || SeriousGames.CurrentPatient == null)
            {
                return new Session[0];
            }

            String today = DateTime.Today.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            Session[] result = _provider.MakeRequest(ServiceRoot + SESSIONS + "?patientId=" + SeriousGames.CurrentPatient.Id +
                                        "&hospitalId=" + SeriousGames.CurrentPatient.HospitalId +
                                        "&date=" + today,
                                        typeof(Session[])) as Session[];

            return result;
        }

        public ConfiguredGame[] AllConfiguredGames { get { return _provider.MakeRequest(ServiceRoot + CONFIGURED_GAMES, typeof(ConfiguredGame[])) as ConfiguredGame[]; } }

        public ConfiguredGame GetConfiguredGame(int id)
        {
            return _provider.MakeRequest(ServiceRoot + CONFIGURED_GAMES + "/" + id, typeof(ConfiguredGame)) as ConfiguredGame;
        }

        public DataUploading[] AllDataUploadings { get { return _provider.MakeRequest(ServiceRoot + DATA_UPLOADINGS, typeof(DataUploading[])) as DataUploading[]; } }

        public DataUploading GetDataUploading(int id)
        {
            return _provider.MakeRequest(ServiceRoot + DATA_UPLOADINGS + "/" + id, typeof(DataUploading)) as DataUploading;
        }

        public bool AuthenticateTherapist(string username, string password)
        {
            return _provider.MakePostRequest(ServiceRoot + AUTHENTICATE_THERAPIST, "username=" + username + "&password=" + password, "application/x-www-form-urlencoded");
        }

        public int CreateDataUplodingRecord(PartialDataUploading uploading)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(PartialDataUploading));
            MemoryStream stream = new MemoryStream();

            jsonSerializer.WriteObject(stream, uploading);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            string obj = sr.ReadToEnd();
            obj = obj.Replace("\\/", "/");
            return _provider.PostObject(ServiceRoot + DATA_UPLOADINGS, obj, "application/json");
        }

        public bool UploadFile(string c3dFile, int id)
        {
            return _provider.UploadFile(ServiceRoot + DATA_UPLOADINGS + DATA_UPLOADINGS_CONTENT + "/" + id, c3dFile);
        }

        public static void SaveTherapistName(String name)
        {
#if WINDOWS
            Registry.SetValue(RegistryElements.REGISTRY_SECTION, RegistryElements.REGISTRY_THERAPIST_TAG, Convert.ToBase64String(Encoding.UTF8.GetBytes(name)));
#endif
        }

        public static string LoadTherapistName()
        {
#if WINDOWS
            string therapist = (string)Registry.GetValue(RegistryElements.REGISTRY_SECTION, RegistryElements.REGISTRY_THERAPIST_TAG, null);
            if (therapist != null)
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(therapist));
            }
#endif
            return null;
        }
    }
}
