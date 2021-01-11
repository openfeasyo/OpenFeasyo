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
using System.Linq;
using System.Text;
//#if ANDROID
//    using Mono.Data.Sqlite;
//#else
using System.Data.SQLite;
//#endif

using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace OpenFeasyo.Platform.Data.Sqlite
{
    public class SqliteDatapoint : Datapoint
    {
        private const string HIT_THE_BOXES_CONFIG = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Configuration><devices><device name=\"Kinect\">" +
                "<param name=\"ShowPreview\" value=\"true\" /><analyzers><analyzer file=\"C3DSerializer.dll\" /></analyzers></device></devices>" +
                "<bindings><binding point=\"Horizontal\" zeroAngle=\"90.58064\" sensitivity=\"0.7419355\" device=\"Kinect\"><skeleton>" +
                "<type><BindingType>SingleBoneAngle</BindingType></type><firstBone><BoneMarkers>Spine</BoneMarkers></firstBone></skeleton>" +
                "</binding></bindings></Configuration>";

        private SQLiteConnection connection;
        private string _therapist;

        public SqliteDatapoint(string therapist)
        {
            _therapist = therapist;
        }   


        

        private void Open(string therapist)
        {
            
            SeriousGames.CheckAndCreateHomeFolder();
            // determine the path for the database file
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal)+
                "\\" + SeriousGames.HOME_FOLDER + "\\" + therapist + ".fdb";

            bool exists = File.Exists(dbPath);

            if (!exists){
                // Need to create the database before seeding it with some data
                Trace.WriteLine("Creating database: " + dbPath);
                SQLiteConnection.CreateFile(dbPath);
            }
            connection = InitializeTables(dbPath);
        }

        private void Close() {
            if (connection != null) { 
                connection.Close();
            }
        }

        public T[] SelectAll<T>() {
            List<T> objs = new List<T>();
            StringBuilder sb = new StringBuilder("SELECT ");
            Open(_therapist);
            
            AddListOfFields<T>(sb,false);
            sb.Append(" from [" + GetTableName(typeof(T))+ "];");
            
            using (var contents = connection.CreateCommand()) {
                contents.CommandText = sb.ToString();
                var r = contents.ExecuteReader();
            
                while (r.Read()) {
                    T o = (T)Activator.CreateInstance(typeof(T));
                    foreach (PropertyInfo p in GetOrderedColumns(typeof(T)))
                    {
                        if (null != p && p.CanWrite)
                        {
                            p.SetValue(o, GetValue(r[p.Name],p.PropertyType), null);
                        }
                    }
                    objs.Add(o);
                }
                r.Close();
            }
            Close();

            return objs.ToArray();
        }

        

        public void Remove<T>(T obj) {
            PropertyInfo pi = GetPrimaryKey<T>();
            if (pi == null) {
                throw new ApplicationException("No primary key fount for type " + typeof(T).Name);
            }
            string val = GetFieldValue(pi, obj,true);
            Open(_therapist);
            using (var c = connection.CreateCommand())
            {
                c.CommandText = "DELETE FROM " + GetTableName(typeof(T)) + " WHERE " + pi.Name + " = " + val;
                var rowcount = c.ExecuteNonQuery();
                Trace.WriteLine("\tExecuted: " + c.CommandText);
            }
            Close();
        }

        
        public void Update<T>(T obj) {
            PropertyInfo pi = GetPrimaryKey<T>();
            if (pi == null)
            {
                throw new ApplicationException("No primary key fount for type " + typeof(T).Name);
            }
            string val = GetFieldValue(pi, obj,true);
            
            // Lets build the query;
            StringBuilder sb = new StringBuilder("UPDATE " + GetTableName(typeof(T)) + " SET ");
            bool separator = false;
            foreach (PropertyInfo p in GetOrderedColumns(typeof(T)))
            {
                if (separator) sb.Append(",");
                sb.Append(GetFieldName(p) + "=" + GetFieldValue(p, obj,true));     // TODO - this is potentially dangerous - rework to use parameters
                separator = true;
            }
            sb.Append(" WHERE " + pi.Name + " = " + val);

            Open(_therapist);
            using (var c = connection.CreateCommand())
            {
                c.CommandText =  sb.ToString();

                var rowcount = c.ExecuteNonQuery();
                Trace.WriteLine("\tExecuted: " + c.CommandText);
            }
            Close();
        }

        public void Insert<T>(T obj, bool forcePrimaryKey) {
            StringBuilder sb = new StringBuilder("INSERT INTO [" + GetTableName(typeof(T)) + "] (");
            AddListOfFields<T>(sb,!forcePrimaryKey);
            sb.Append(") VALUES (");

            bool separator = false;
            PropertyInfo key = GetPrimaryKey<T>();
            foreach (PropertyInfo pi in GetOrderedColumns(typeof(T)))
            {
                if (!forcePrimaryKey && pi.Equals(key))
                    continue;
                if (separator) sb.Append(",");
                sb.Append(GetFieldValue(pi,obj,true));     // TODO - this is potentially dangerous
                separator = true;
            }
            sb.Append(");");

            Open(_therapist);
            using (var c = connection.CreateCommand())
            {
                c.CommandText = sb.ToString();
                var rowcount = c.ExecuteNonQuery();
                Trace.WriteLine("\tExecuted: " + sb.ToString());
            }
            Close();
        }


        private SQLiteConnection InitializeTables(string dbPath)
        {
            
            SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath,true);
            connection.Open();
            List<string> createCommands = new List<string>();

            var types = new[] { 
                typeof(ConfiguredGame), 
                typeof(ExtendedPatient), 
                typeof(DataUploading),
                typeof(MaxScore)};


            foreach (Type t in types){
                if (!tableExists(connection, t))
                {
                    createCommands.Add(CreateTableCommand(t));
                }
            }

            
                    //"CREATE TABLE [ConfiguredGames] (_id integer, name ntext, description ntext, params ntext);"};//,
            //        "INSERT INTO [ConfiguredGames] ([_id], [name], [description], [params]) VALUES (1, 'Hit the boxes','','"+HIT_THE_BOXES_CONFIG+"')"};
            // Open the database connection and create table with data
            
            foreach (var command in createCommands)
            {
                using (var c = connection.CreateCommand())
                {
                    c.CommandText = command;
                    var rowcount = c.ExecuteNonQuery();
                    Trace.WriteLine("\tExecuted " + command);
                }
            }
            return connection;
        }
        
        private bool tableExists(SQLiteConnection connection, Type t) {
            bool ret = false;
                using (var c = connection.CreateCommand())
                {
                    c.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='" + GetTableName(t) + "';";
                    var reader = c.ExecuteReader();
                    ret = reader.HasRows;
                    Trace.WriteLine("\tTABLE " + GetTableName(t) + (reader.HasRows ? " exists." : " doesn't exist!"));
                    reader.Close();
                }
            
            return ret;
        }

        private string CreateTableCommand(Type t) {
            StringBuilder sb = new StringBuilder("CREATE TABLE IF NOT EXISTS");
            sb.Append("[" + GetTableName(t) +"] (");

            bool separator = false;
            foreach (PropertyInfo pi in GetOrderedColumns(t))
            {
                if(separator) {
                    sb.Append(",");
                }else{
                    separator = true;
                }
                sb.Append(GetFieldName(pi) + " " + GetFieldType(pi));
            }
            sb.Append(");");
            return sb.ToString();
        }

        private void AddListOfFields<T>(StringBuilder sb, bool skipPrimaryKey) {
            bool separator = false;
            PropertyInfo key = GetPrimaryKey<T>();
            foreach (PropertyInfo pi in GetOrderedColumns(typeof(T)))
            {
                if (skipPrimaryKey && pi.Equals(key))
                    continue;

                if (separator)
                {
                    sb.Append(",");
                }
                else
                {
                    separator = true;
                }
                sb.Append("[" + GetFieldName(pi) +"]");
            }
        
        }

        private string GetFieldName(PropertyInfo pi) {
            return pi.Name;
        }

        private string GetFieldValue(PropertyInfo pi, object obj, bool escape = false) {
            if(pi.GetValue(obj) == null) 
                return "null";
            if (pi.PropertyType == typeof(string[])) {
                bool separator = false;
                string ret = "";
                foreach (string s in ((string[])pi.GetValue(obj))) {
                    if (separator) ret += ",";
                    ret += s;     // TODO - this is potentially dangerous
                    separator = true;
                }
                return escape? "'" + ret + "'": ret;
            }
            else if (pi.PropertyType == typeof(string)) {
                string ret = pi.GetValue(obj).ToString();
                return escape ? "'" + ret + "'" : ret;
            }
            else if (pi.PropertyType == typeof(bool)) {
                return (bool)pi.GetValue(obj) ? "1" : "0";
            }

            return pi.GetValue(obj).ToString();
        }

        private object GetValue(object obj, Type dest) {
            if (obj.GetType() == typeof(System.DBNull))
                return null;
            if (obj.GetType() == typeof(Int64))
                return Convert.ToInt32(obj);
            else if (   obj.GetType() == typeof(string) && 
                        dest == typeof(string[]))
                return ((string)obj) == ""? new string[] {} : ((string)obj).Split(',');
            return obj;
        }

        private PropertyInfo GetPrimaryKey<T>() {
            IEnumerable<PropertyInfo> infos = GetOrderedColumns(typeof(T)).Where(pi => Attribute.IsDefined(pi,
                typeof(System.ComponentModel.DataAnnotations.KeyAttribute)));
            return infos.Count<PropertyInfo>() <= 0? null : infos.First<PropertyInfo>();
        }

        private PropertyInfo GetPrimaryKey(Type t)
        {
            IEnumerable<PropertyInfo> infos = GetOrderedColumns(t).Where(pi => Attribute.IsDefined(pi,
                typeof(System.ComponentModel.DataAnnotations.KeyAttribute)));
            return infos.Count<PropertyInfo>() <= 0 ? null : infos.First<PropertyInfo>();
        }

        private string GetFieldType(PropertyInfo pi) {
            string role = Attribute.IsDefined(pi, 
                typeof(System.ComponentModel.DataAnnotations.KeyAttribute)) ? 
                    " primary key" : "";

            if (Attribute.IsDefined(pi, typeof(ForeignKeyAttribute))) {
                ForeignKeyAttribute attr = pi.GetCustomAttribute<ForeignKeyAttribute>();
                Type t = attr.Table;
                PropertyInfo p = GetPrimaryKey(t);
                role = " references " + t.Name + " (" + p.Name+ ")";
            }

            if( pi.PropertyType == typeof(string)) {
                return "ntext" + role;
            }
            else if (pi.PropertyType == typeof(string [])) {
                return "ntext" + role;
            }
            else if (pi.PropertyType == typeof(int)) {
                return "integer" + role;
            }
            else if (pi.PropertyType == typeof(bool)) {
                return "integer" + role;
            }
            else if (pi.PropertyType == typeof(long)) {
                return "integer" + role;
            }

            throw new ApplicationException("Type " + pi.PropertyType.Name + " is not supported by SQLite serialization mechanism!");
        }

        private IEnumerable<PropertyInfo> GetOrderedColumns(Type t)
        {
            return t
                .GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(LocalDataMemberAttribute)))
                .OrderBy(i => i.Name);
        }

        private string GetTableName(Type t)
        {
            return t.Name + "s";
        }
        
    }
}
