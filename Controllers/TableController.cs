using BBQLover.webapp.Models;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace BBQLover.webapp.Controllers
{
    public class TableController : Controller
    {
        List<Table> tables;
        FirestoreDb db;
        public TableController()
        {
            string filepath = AppDomain.CurrentDomain.BaseDirectory + @"bbq-lover.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            db = FirestoreDb.Create("bbq-lover-87318");
            tables = new List<Table>();
        }
        public async Task<List<Table>> getDataAsync(string date, string time)
        {
            List<Table> list = new List<Table>();
            Query tableQuery = db.Collection("Tables");
            QuerySnapshot tableQuerySnapshot = await tableQuery.GetSnapshotAsync();
            if (tableQuerySnapshot != null)
            {
                foreach (DocumentSnapshot documentSnapshot in tableQuerySnapshot.Documents)
                {
                    Table table = new Table();
                    Dictionary<string, object> deli = documentSnapshot.ToDictionary();
                    table.TableNum = documentSnapshot.Id;
                    DocumentReference docRef = db.Collection("Tables").Document(documentSnapshot.Id).Collection(date).Document(time);
                    DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                    if (snapshot.Exists)
                    {
                        Dictionary<string, object> t = snapshot.ToDictionary();
                        foreach (KeyValuePair<string, object> pair in t)
                        {
                            if (pair.Key == "resNum")
                                table.ResNum = (string?)pair.Value;
                        }
                        table.Date = date;
                        table.Time = time;
                        table.Status = "ไม่ว่าง";
                    }
                    else
                    {
                        table.Date = date;
                        table.Time = time;
                        table.ResNum = "";
                        table.Status = "ว่าง";
                    }
                    list.Add(table);
                }
            }
            return list;
        }
        public IActionResult TableCheck()
        {
            return View();
        }

        [HttpPost]
        public List<Table> TableCheck(string date, string time)
        {
            tables = getDataAsync(date, time).GetAwaiter().GetResult();
            var data = tables.ToList();
            return data;
        }

        [HttpPost]
        public async void Update(string table, string date, string time)
        {
            DocumentReference docRef = db.Collection("Tables").Document(table).Collection(date).Document(time);
            Dictionary<string, object> t = new Dictionary<string, object>
            {
                { "resNum", "000000" },
            };
            await docRef.SetAsync(t);
        }

        [HttpPost]
        public async Task<string> Delete(string table, string date, string time)
        {
            DocumentReference resRef = db.Collection("Tables").Document(table).Collection(date).Document(time);
            await resRef.DeleteAsync();
            return "เปลี่ยนสถานะเรียบร้อย";
        }
    }
}
