﻿using LiteDB;
using LiteDB.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiteDB.Demo
{
    public class ExampleStressTest : StressTest
    {
        public ExampleStressTest(string filename) : 
            base(new EngineSettings { Filename = filename })
        {
        }

        /// <summary>
        /// Use this method to initialize your stress test.
        /// You can drop existing collection, load initial data and run checkpoint before finish
        /// </summary>
        public override void OnInit(SqlDB db)
        {
            db.Insert("col1", new BsonDocument 
            { 
                ["_id"] = 1, 
                ["name"] = "John" 
            });

            //db.ExecuteScalar("CREATE INDEX idx_name ON col1(upper(name))");
        }

        [Task(Start = 0, Repeat = 10, Random = 10, Threads = 5)]
        public void Insert(SqlDB db)
        {
            db.Insert("col1", new BsonDocument
            {
                ["name"] = "John " + Guid.NewGuid(),
                ["r"] = "-".PadLeft(rnd.Next(6500, 9500), '-'),
                ["t"] = 0,
                ["active"] = false
            }); ;
        }

        [Task(Start = 2000, Repeat = 2000, Random = 1000, Threads = 2)]
        public void Update_Active(SqlDB db)
        {
            db.ExecuteScalar("UPDATE col1 SET active = true, r = null WHERE active = false"); 
        }

        [Task(Start = 5000, Repeat = 4000, Random = 500, Threads = 2)]
        public void Delete_Active(SqlDB db)
        {
            db.ExecuteScalar("DELETE col1 WHERE active = true");
        }

        [Task(Start = 100, Repeat = 75, Random = 25, Threads = 1)]
        public void QueryCount(SqlDB db)
        {
            db.Query("SELECT COUNT(*) FROM col1");
        }
    }
}