﻿using DAL.DContext;
using DAL.Repository;
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
    public class OcphDbContext : IDbContext, IDisposable
    {
        private string ConnectionString;
        private IDbConnection _Connection;

        public OcphDbContext()
        {

            
            if (File.Exists("database.db"))
            {
                this.ConnectionString = "Data Source=kata.s3db";
            }else
            {
                this.ConnectionString = "Data Source=kata.db;Version=3;New=True;Compress=True;";
                _Connection = new SQLiteContextConnection(ConnectionString);
                _Connection.Open();
                IDbCommand cmd = _Connection.CreateCommand();
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Kategori (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name VARCHAR(100))";
                IDataReader reader = cmd.ExecuteReader();
                reader.Close();
                

            }

        }

        public IRepository<Kategori> Categories { get { return new Repository<Kategori>(this); } }
        public IRepository<Kata> Words { get { return new Repository<Kata>(this); } }

        internal void IsExist<T>()
        {
            var name = typeof(T).Name;

        }

        public IDbConnection Connection
        {
            get
            {
                if (_Connection == null)
                {
                    _Connection = new SQLiteContextConnection(this.ConnectionString);
                    return _Connection;
                }
                else
                {
                    return _Connection;
                }
            }
        }

        public void Dispose()
        {
            if (_Connection != null)
            {
                if (this.Connection.State != ConnectionState.Closed)
                {
                    this.Connection.Close();
                }
            }
        }
    }
}