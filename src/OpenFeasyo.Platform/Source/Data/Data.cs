using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace OpenFeasyo.Platform.Data
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class LocalDataMemberAttribute : Attribute
    {
        string name;
        bool isNameSetExplicit;
        int order = -1;
        bool isRequired;
        

        public LocalDataMemberAttribute()
        {
        }

        public string Name
        {
            get { return name; }
            set { name = value; isNameSetExplicit = true; }
        }

        internal bool IsNameSetExplicit
        {
            get { return isNameSetExplicit; }
        }

    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ForeignKeyAttribute : Attribute
    {
        public ForeignKeyAttribute()
        {
        }

        public Type Table { get; set; }
        
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class AutoIncrementAttribute : Attribute
    {
        public AutoIncrementAttribute()
        {
        }
        
    }

    [DataContract]
    public class Game
    {
        [DataMember(Name = "gameDescription")]
        public string Description { get; set; }

        [DataMember(Name = "gameId")]
        public int Id { get; set; }

        [DataMember(Name = "gameName")]
        public string Name { get; set; }

        public override string ToString()
        {
            return "{ " + Id + ", " + Name + "}";
        }
    }

    [DataContract]
    public class ConfiguredGame {

        [DataMember(Name = "name")]
        [LocalDataMember(Name = "name")]
        public string Name { get; set; }

        [Key]
        [DataMember(Name = "configuredGameId")]
        [LocalDataMember(Name = "configuredGameId")]
        public int ConfiguredGameId { get; set; }

        //[LocalDataMember(Name = "gameId")]
        //public int GameId { get; set; }

        [DataMember(Name = "color")]
        [LocalDataMember(Name = "color")]
        public string Color { get; set; }

        [DataMember(Name = "description")]
        [LocalDataMember(Name = "description")]
        public string Description { get; set; }
        
        [DataMember(Name = "icon")]
        [LocalDataMember(Name = "icon")]
        public string Icon { get; set; }
        
        [DataMember(Name = "repetitions")]
        [LocalDataMember(Name = "repetitions")]
        public int Repetitions { get; set; }

        [DataMember(Name = "params")]
        [LocalDataMember(Name = "params")]
        public string Parameters { get; set; }

        [DataMember(Name = "tags")]
        [LocalDataMember(Name = "tags")]
        public string[] Tags { get; set; }

        public override bool Equals(System.Object obj)
        {
            ConfiguredGame p = obj as ConfiguredGame;
            if ((object)p == null)
            {
                return false;
            }
            return p.Color == this.Color &&
                p.ConfiguredGameId == this.ConfiguredGameId &&
                //p.GameId == this.GameId &&
                p.Description == this.Description &&
                p.Icon == this.Icon &&
                p.Name == this.Name &&
                p.Parameters == this.Parameters &&
                p.Repetitions == this.Repetitions &&
                p.Tags.SequenceEqual(this.Tags);
        }
        public ConfiguredGame Clone() {
            ConfiguredGame p = new ConfiguredGame();
            p.Color = this.Color;
            p.ConfiguredGameId = -1;
            p.Description = this.Description;
            p.Icon = this.Icon;
            p.Name = this.Name;
            p.Parameters = this.Parameters;
            p.Repetitions = this.Repetitions;
            p.Tags = new string[] { };
            return p;
        }
    }

    [DataContract]
    public class PartialGame
    {
        public PartialGame(int id) {
            this.Id = id;
        }

        [DataMember(Name = "gameId")]
        public int Id { get; set; }
        
        public override string ToString()
        {
            return "{ " + Id + "}";
        }
    }

    [DataContract]
    public class PlanedGame
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "configuredGameId")]
        public int ConfiguredGameId { get; set; }

        [DataMember(Name = "gameId")]
        public int GameId { get; set; }

        [DataMember(Name = "gameName")]
        public string GameName { get; set; }

        [DataMember(Name = "gameDescription")]
        public string GameDescription { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "icon")]
        public string Icon { get; set; }

        [DataMember(Name = "color")]
        public string Color { get; set; }

        [DataMember(Name = "defaultConfig")]
        public bool IsDefault { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "gameConfig")]
        public string GameConfigurationXml { get; set; }

        [DataMember(Name = "params")]
        public string Parameters { get; set; }

        [DataMember(Name = "repetitions")]
        public int Repetitions { get; set; }

        [DataMember(Name = "weight")]
        public int Weight { get; set; }

        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }
    }
         

    [DataContract]
    public class Session
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
        
        [DataMember(Name = "hospitalId")]
        public string HospitalId { get; set; }

        [DataMember(Name = "patientId")]
        public string PatientId { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
     
        [DataMember(Name = "completion")]
        public double? Completion { get; set; }

        [DataMember(Name = "lastChange")]
        public string LastChange { get; set; }
        
        [DataMember(Name = "sessionStart")]
        public string StartDateTime { get; set; }

        [DataMember(Name = "sessionStop")]
        public string StopDateTime { get; set; }

        [DataMember(Name = "games")]
        public PlanedGame[] Games { get; set; }

        public DateTime SessionStart {
            get
            {
                return System.Convert.ToDateTime(StartDateTime);
            }
        }

        public DateTime SessionStop {
            get
            {
                return System.Convert.ToDateTime(StopDateTime);
            }
        }
    }

    [DataContract]
    public class Hospital {

        public Hospital() { }
        public Hospital(string hospitalId) {
            HospitalId = hospitalId;
        }

        [DataMember(Name = "hospitalId")]
        public string HospitalId { get; set; }

        public override string ToString()
        {
            return "{ " + HospitalId + " }";
        }
    }
    
    [DataContract]
    public class PatientId
    {
        public PatientId() {}
        public PatientId(string patientId, string hospitalId){
            Id = patientId;
            Hospital = new Hospital(hospitalId);
        }

        [DataMember(Name = "hospital")]
        public Hospital Hospital { get; set; }
        
        [DataMember(Name = "patientId")]
        public string Id { get; set; }

        public override string ToString()
        {
            return "{ " + Id + ", " + Hospital + " }";
        }
    }
    
    [DataContract]
    public class Patient
    {
        public Patient() { }
        public Patient(string patientId, string hospitalId) {
            Id = new PatientId(patientId,hospitalId);
        }

        [DataMember(Name = "id")]
        public PatientId Id { get; set; }

        public override string ToString()
        {
            return "{ " + Id + " }";
        }
    }

    [DataContract]
    public class ExtendedPatient
    {
        [DataMember(Name = "birthDate")]
        [LocalDataMember(Name = "birthDate")]
        public string BirthDate { get; set; }

	    [DataMember(Name = "gender")]
        [LocalDataMember(Name = "gender")]
        public string Gender { get; set; }
        
        [DataMember(Name = "hospitalId")]
        [LocalDataMember(Name = "hospitalId")]
        public string HospitalId { get; set; }

        [Key]
        [DataMember(Name = "patientId")]
        [LocalDataMember(Name = "patientId")]
        public string Id { get; set; }

        public override bool Equals(System.Object obj)
        {
            ExtendedPatient p = obj as ExtendedPatient;
            if ((object)p == null)
            {
                return false;
            }
            return p.Id == this.Id &&
                p.HospitalId == this.HospitalId &&
                p.Gender == this.Gender &&
                p.BirthDate == this.BirthDate;
        }
    }

    [DataContract]
    public class MaxScore {
        [Key]
        [LocalDataMember(Name = "Id")]
        public int Id { get; set; }

        [ForeignKey(Table=typeof(ExtendedPatient))]
        [LocalDataMember(Name = "patientId")]
        public string PatientId { get; set; }

        [LocalDataMember(Name = "hospitalId")]
        public string HospitalId { get; set; }

        [ForeignKey(Table = typeof(ConfiguredGame))]
        [LocalDataMember(Name = "configuredGameId")]
        public int ConfiguredGameId { get; set; }

        [LocalDataMember(Name = "maxScore")]
        public int Score { get; set; }

        [LocalDataMember(Name = "time")]
        public int Time { get; set; }

    }

    [DataContract]
    public class PatientSettings
    {
        [DataMember(Name = "patientSeriousGamingSettingId")]
        public int Id { get; set; }
        [DataMember(Name = "patient")]
        public Patient Patient { get; set; }
        [DataMember(Name = "settings")]
        public string Settings { get; set; }
        [DataMember(Name = "setupDate")]
        public string SetupDate { get; set; }
    }

    [DataContract]
    public class PatientGameSettings
    {
        [DataMember(Name = "patientGameSettingId")]
        public int Id { get; set; }
        [DataMember(Name = "game")]
        public Game Game { get; set; }
        [DataMember(Name = "patient")]
        public Patient Patient { get; set; }
        [DataMember(Name = "settings")]
        public string Settings { get; set; }
        [DataMember(Name = "setupDate")]
        public string SetupDate { get; set; }
    }

    [DataContract]
    public class DataUploading {

        [DataMember(Name = "seriousGamingDataUploadingId")]
        [LocalDataMember(Name = "seriousGamingDataUploadingId")]
        public int Id { get; set; }

        [DataMember(Name = "game")]
        public Game Game { get; set; }

        [LocalDataMember(Name = "game")]
        public int GameID { get; set; }

        [DataMember(Name = "patient")]
        public Patient Patient { get; set; }

        [LocalDataMember(Name = "hospitalId")]
        public string HospitalId {
            get { return Patient != null && Patient.Id != null && Patient.Id.Hospital != null ? Patient.Id.Hospital.HospitalId : null; }
            set {
                if (Patient != null)
                {
                    if (Patient.Id != null)
                    {
                        Patient.Id.Hospital = new Hospital(value);
                    }
                    else {
                        Patient.Id = new PatientId(null, value);
                    }
                }
                else {
                    Patient = new Patient(null, value);
                }
            }
        }

        [LocalDataMember(Name = "patientId")]
        public string PatientId {
            get { return Patient != null && Patient.Id != null ? Patient.Id.Id : null; }
            set {
                if (Patient != null)
                {
                    if (Patient.Id != null)
                    {
                        Patient.Id.Id = value;
                    }
                    else
                    {
                        Patient.Id = new PatientId(value, null);
                    }
                }
                else {
                    Patient = new Patient(value, null);
                }

            }
        }

        [DataMember(Name = "results")]
        [LocalDataMember(Name = "results")]
        public string Results { get; set; }

        [Key]
        [DataMember(Name = "c3DFileId")]
        [LocalDataMember(Name = "c3DFileId")]
        public string C3DFileId { get; set; }
    
        [DataMember(Name = "performanceDate")]
        [LocalDataMember(Name = "performanceDate")]
        public string PerformanceDate { get; set; }

        [LocalDataMember(Name = "uploaded")]
        public int Uploaded { get; set; }


        public override string ToString()
        {
            return "{ " + Id + ", " + Game + ", " + Patient + ", " + Results + ", " + C3DFileId + ", " + PerformanceDate + " }";
        }
    }

    [DataContract]
    public class PartialDataUploading
    {
        [DataMember(Name = "game")]
        public PartialGame Game { get; set; }

        [DataMember(Name = "patient")]
        public Patient Patient { get; set; }

        [DataMember(Name = "results")]
        public string Results { get; set; }

        [DataMember(Name = "performanceDate")]
        public string PerformanceDate { get; set; }

        public override string ToString()
        {
            return "{ " + Game + ", " + Patient + ", " + Results + ", " + PerformanceDate + " }";
        }
    }


    //public class Account
    //{
    //    private string _accountDirectory = null;
    //    private PatientSettings _patientSettings = null;
    //    public PatientSettings PatientSettings {get{return _patientSettings;} set {_patientSettings = value;}}
    //    public ExtendedPatient Patient { get; set; }
        

    //    public bool Local { get { return _accountDirectory != null; } }
    //    public bool Remote { get { return Patient != null; } }

    //    public string Name { get; set; }
    //    public string Id { get; set; }

    //    public string Directory { get { return _accountDirectory; } set { _accountDirectory = value; } }

    //    public override bool Equals(object obj)
    //    {
    //        if (obj == null || !(obj is Account) )
    //            return false;

    //        if (((Account)obj).Name == null || this.Name == null || ((Account)obj).Name != this.Name)
    //            return false;
    //        return true;
            
    //        //if (((Account)obj).Patient == null || this.Patient == null)
    //        //    return false;
    //        //
    //        //return ((Account)obj).Patient.Id == this.Patient.Id && ((Account)obj).Patient.HospitalId == this.Patient.HospitalId;
    //    }
    //}
}
        