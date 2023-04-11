using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using BBQLover.webapp.Models;

namespace BBQLover.webapp.Controllers
{
    public class ReservationController : Controller
    {
        List<Reservation> res;
        FirestoreDb db;

        public ReservationController()
        {
            string filepath = AppDomain.CurrentDomain.BaseDirectory + @"bbq-lover.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            db = FirestoreDb.Create("bbq-lover-87318");
            res = getDataAsync().GetAwaiter().GetResult();
        }

        public async Task<List<Reservation>> getDataAsync()
        {
            List<Reservation> list = new List<Reservation>();
            Query resQuery = db.Collection("Reservation");
            QuerySnapshot resQuerySnapshot = await resQuery.GetSnapshotAsync();
            if (resQuerySnapshot != null)
            {
                foreach (DocumentSnapshot documentSnapshot in resQuerySnapshot.Documents)
                {
                    Reservation reservation = new Reservation();
                    Dictionary<string, object> reserve = documentSnapshot.ToDictionary();
                    reservation.Id = documentSnapshot.Id;
                    foreach (KeyValuePair<string, object> pair in reserve)
                    {
                        if (pair.Key == "date")
                            reservation.Date = (string?)pair.Value;
                        else if (pair.Key == "name")
                            reservation.Name = (string?)pair.Value;
                        else if (pair.Key == "resNum")
                            reservation.ResNum = (string?)pair.Value;
                        else if (pair.Key == "tel")
                            reservation.Tel = (string?)pair.Value;
                        else if (pair.Key == "time")
                            reservation.Time = (string?)pair.Value;
                        else if (pair.Key == "totalpeep")
                            reservation.TotalPeople = (string?)pair.Value;
                        else if (pair.Key == "zone")
                            reservation.Zone = (string?)pair.Value;
                        else if (pair.Key == "table")
                            reservation.Table = pair.Value.ToString();
                    }
                    list.Add(reservation);
                }
            }
            return list;
        }

        public ActionResult Reserve()
        {
            var data = res.ToList();
            return View(data);
        }

        public ActionResult Details(string id)
        {
            var data = res.SingleOrDefault(p => p.Id == id);
            return View(data);
        }

        [HttpPost]
        public async Task<string> Update(string id, string[] newRes, string[] oldRes)
        {
            if (newRes[0] == oldRes[1] && newRes[1] == oldRes[2])
            {
                DocumentReference resRef = db.Collection("Reservation").Document(id);
                Dictionary<string, object> updates = new Dictionary<string, object>
                    {
                        { "totalpeep", newRes[2] },
                        { "zone", newRes[3] },
                    };
                await resRef.UpdateAsync(updates);
                return "ทำการอัพเดทข้อมูลเรียบร้อย";
            }
            else
            {
                bool isAvailable = true;
                string t = "1";
                for (var i = 1; i < 6; i++)
                {
                    DocumentReference docRef = db.Collection("Tables").Document(i.ToString()).Collection(newRes[0]).Document(newRes[1]);
                    DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                    if (snapshot.Exists)
                    {
                        isAvailable = false;
                    }
                    else
                    {
                        isAvailable = true;
                        t = i.ToString();
                        break;
                    }
                }

                if (isAvailable)
                {
                    string date = newRes[0];
                    string time = newRes[1];
                    string newRNum = newRes[4];
                    Console.WriteLine();
                    DocumentReference newTRef = db.Collection("Tables").Document(t).Collection(date).Document(time);
                    Dictionary<string, object> tableDic = new Dictionary<string, object>
                    {
                        { "resNum", newRNum },
                    };
                    await newTRef.SetAsync(tableDic);

                    DocumentReference tableRef = db.Collection("Tables").Document(oldRes[0]).Collection(oldRes[1]).Document(oldRes[2]);
                    await tableRef.DeleteAsync();

                    DocumentReference resRef = db.Collection("Reservation").Document(id);
                    Dictionary<string, object> updates = new Dictionary<string, object>
                    {
                        { "date", newRes[0] },
                        { "time", newRes[1] },
                        { "totalpeep", newRes[2] },
                        { "zone", newRes[3] },
                        { "table", t }
                    };
                    await resRef.UpdateAsync(updates);

                    return "ทำการอัพเดทข้อมูลเรียบร้อย";
                }
                else
                {
                    return "โต๊ะในวันหรือเวลาที่ท่านเลือกไม่ว่าง กรุณาเปลี่ยนวันหรือเวลา";
                }
            }
        }

        [HttpPost]
        public async void Delete(string id, string table, string date, string time)
        {
            DocumentReference resRef = db.Collection("Reservation").Document(id);
            await resRef.DeleteAsync();

            DocumentReference tableRef = db.Collection("Tables").Document(table).Collection(date).Document(time);
            await tableRef.DeleteAsync();
        }
    }
}
