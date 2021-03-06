﻿using Ocph.DAL.Repository;
using GameWordPuzzel.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWordPuzzel
{
    public class OcphDbContext :Ocph.DAL.Provider.SQLite.SQLiteDbConnection
    {

        public OcphDbContext()
        {
            if (File.Exists("kata.db"))
            {
                this.ConnectionString = "Data Source=kata.db";
            }else
            {
                this.ConnectionString = "Data Source=kata.db;Version=3;New=True;Compress=True;";
                //  _Connection = new SQLiteContextConnection(ConnectionString);
                this.Open();
                IDbCommand cmd = this.CreateCommand();
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Kategori (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name VARCHAR(100))";
                IDataReader reader = cmd.ExecuteReader();
                reader.Close();
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Katas (Id INTEGER PRIMARY KEY AUTOINCREMENT, KategoriId Integer(11), Nilai VARCHAR(100) )";
                reader = cmd.ExecuteReader();
                reader.Close();
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Soal (Id INTEGER PRIMARY KEY AUTOINCREMENT, Value Text, Level VARCHAR(100) )";
                reader = cmd.ExecuteReader();
                reader.Close();
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Pilihan (Id INTEGER PRIMARY KEY AUTOINCREMENT, SoalId Integer(11), Value Text, IsTrue VARCHAR(100) )";
                reader = cmd.ExecuteReader();
                reader.Close();
            }

        }

        public IRepository<Kategori> Categories { get { return new Repository<Kategori>(this); } }
        public IRepository<Kata> Words { get { return new Repository<Kata>(this); } }
        public IRepository<Soal> Soals { get { return new Repository<Soal>(this); } }
        public IRepository<Option> Options { get { return new Repository<Option>(this); } }

        internal void IsExist<T>()
        {
            var name = typeof(T).Name;

        }

      
    }
}
