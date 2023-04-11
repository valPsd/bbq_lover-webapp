using BBQLover.webapp.Models;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BBQLover.webapp.Controllers
{
    public class MenuController : Controller
    {
        List<Menu> menus;
        FirestoreDb db;

        public MenuController()
        {
            string filepath = AppDomain.CurrentDomain.BaseDirectory + @"bbq-lover.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            db = FirestoreDb.Create("bbq-lover-87318");
            menus = getDataAsync().GetAwaiter().GetResult();
        }

        public async Task<List<Menu>> getDataAsync()
        {
            List<Menu> list = new List<Menu>();
            Query resQuery = db.Collection("Menu");
            QuerySnapshot resQuerySnapshot = await resQuery.GetSnapshotAsync();
            if (resQuerySnapshot != null)
            {
                foreach (DocumentSnapshot documentSnapshot in resQuerySnapshot.Documents)
                {
                    Menu menu = new Menu();
                    Dictionary<string, object> reserve = documentSnapshot.ToDictionary();
                    menu.Id = documentSnapshot.Id;
                    foreach (KeyValuePair<string, object> pair in reserve)
                    {
                        if (pair.Key == "image")
                            menu.Image = pair.Value.ToString();
                        else if (pair.Key == "name")
                            menu.Name = pair.Value.ToString();
                        else if (pair.Key == "isPop")
                            menu.IsPop = (bool)pair.Value;
                        else if (pair.Key == "price")
                            menu.Price = pair.Value.ToString();
                        else if (pair.Key == "show")
                            menu.Show = (bool)pair.Value;
                        else if (pair.Key == "stock")
                            menu.Stock = pair.Value.ToString();
                        else if (pair.Key == "type")
                            menu.Type = pair.Value.ToString();
                    }
                    list.Add(menu);
                }
            }
            return list;
        }

        public ActionResult MenuPage()
        {
            var data = menus.ToList();
            return View(data);
        }

        [HttpPost]
        public async Task<string> Edit(string id, string mode, string price, string stock, string show)
        {
            if(mode == "edit")
            {
                if(Convert.ToInt32(price) < 0 || Convert.ToInt32(stock) < 0)
                {
                    return "ราคาหรือจำนวนสินค้าต้องไม่น้อยกว่า 0";
                }
                else
                {
                    DocumentReference resRef = db.Collection("Menu").Document(id);
                    Dictionary<string, object> updates = new Dictionary<string, object>
                    {
                        { "price", price },
                        { "stock", stock },
                    };
                    await resRef.UpdateAsync(updates);
                    return "แก้ไขข้อมูลสำเร็จแล้ว";
                }
            }
            else
            {
                DocumentReference resRef = db.Collection("Menu").Document(id);
                Dictionary<string, object> updates;
                if (show == "True")
                {
                    updates = new Dictionary<string, object>
                    {
                        { "show", false },
                    };
                }
                else
                {
                    updates = new Dictionary<string, object>
                    {
                        { "show", true },
                    };
                }
                await resRef.UpdateAsync(updates);
                return "เปลี่ยนสถานะสำเร็จแล้ว";
            }
        }
    }
}
