using BBQLover.webapp.Models;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace BBQLover.webapp.Controllers
{
    public class DeliveryController : Controller
    {
        List<Delivery> orders;
        FirestoreDb db;

        public DeliveryController()
        {
            string filepath = AppDomain.CurrentDomain.BaseDirectory + @"bbq-lover.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            db = FirestoreDb.Create("bbq-lover-87318");
            orders = getDataAsync().GetAwaiter().GetResult();
        }
        public async Task<List<Delivery>> getDataAsync()
        {
            List<Delivery> list = new List<Delivery>();
            Query deliQuery = db.Collection("Delivery");
            QuerySnapshot deliQuerySnapshot = await deliQuery.GetSnapshotAsync();
            if (deliQuerySnapshot != null)
            {
                foreach (DocumentSnapshot documentSnapshot in deliQuerySnapshot.Documents)
                {
                    Delivery delivery = new Delivery();
                    Dictionary<string, object> deli = documentSnapshot.ToDictionary();
                    delivery.Id = documentSnapshot.Id;
                    foreach (KeyValuePair<string, object> pair in deli)
                    {
                        if (pair.Key == "name")
                            delivery.Name = (string?)pair.Value;
                        else if (pair.Key == "address")
                            delivery.Address = (string?)pair.Value;
                        else if (pair.Key == "tel")
                            delivery.Tel = (string?)pair.Value;
                        else if (pair.Key == "more")
                            delivery.More = (string?)pair.Value;
                        else if (pair.Key == "order")
                        {
                            List<object> objects = (List<object>)pair.Value;
                            delivery.Orders = objects.Select(s => (string)s).ToList();
                        }
                        else if (pair.Key == "orderNum")
                            delivery.OrderNum = (string?)pair.Value;
                        else if (pair.Key == "payment")
                            delivery.Payment = (string?)pair.Value;
                        else if (pair.Key == "total")
                            delivery.Total = (int)(long)pair.Value;
                    }
                    list.Add(delivery);
                }
            }
            return list;
        }

        public IActionResult Deliver()
        {
            var data = orders.ToList();
            return View(data);
        }

        public IActionResult DeliDetails(string Id)
        {
            var data = orders.SingleOrDefault(p => p.Id == Id);
            return View(data);
        }

        [HttpPost]
        public async void Delete(string orderNum)
        {
            var data = orders.SingleOrDefault(p => p.OrderNum == orderNum);
            DocumentReference resRef = db.Collection("Delivery").Document(data.Id);
            await resRef.DeleteAsync();
        }
    }
}
