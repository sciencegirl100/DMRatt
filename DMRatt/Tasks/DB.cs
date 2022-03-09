using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMRatt.Tasks
{
    class Licensee
    {
        public Int32 Id { get; set; }
        public string Callsign { get; set; }
        public string Status { get; set; }
        // 1   Submitted      
        // 2   Pending
        // A   A Granted
        // C   Consented To
        // D   Dismissed
        // E   Eliminate
        // G   Granted
        // H   History Only
        // I   Inactive
        // J   HAC Submitted
        // K   Killed
        // M   Consummated
        // N   Granted in Part
        // P   Pending Pack Filing
        // Q   Accepted
        // R   Returned
        // S   Saved
        // T   Terminated
        // U   Unprocessable
        // W   Withdrawn
        // X   NA
        // Y   Application has problems
        public DateTime GrantDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime CancelDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public string Suffix { get; set; }
        public string Title { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime LastActionDate { get; set; }
        public string[] RadioIds { get; set; }
        public string DMRFirstName { get; set; }
        public string DMRLastName { get; set; }
        public string DMRCity { get; set; }
        public string DMRState { get; set; }
        public string DMRCountry { get; set; }
    }

    class CFG
    {
        public Int32 Id { get; set; }
        public string Config { get; set; }
        public string Value { get; set; }
    }

    class DB
    {
        private LiteDatabase              db;
        public  ILiteCollection<Licensee> Licensees;
        public  ILiteCollection<CFG>      Configs;
        public void InitDatabase()
        {
            // make sure a database is in place and al tables and cols check out
            this.db = new LiteDatabase(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DMRatt.db");
            this.Licensees = db.GetCollection<Licensee>("Licensees");
            this.Configs = db.GetCollection<CFG>("Config");
        }

    }
    
}
