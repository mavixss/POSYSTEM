using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using MySql.Data.MySqlClient;


namespace POSYSTEM.Controllers
{
    public class HomeController : Controller
    {

        string connDB = "server=localhost;uid=root;database=possystem ";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }



        public ActionResult Loginn()
        {

            var data = new List<object>();
            var user = Request["username"];
            var pass = Request["password"];

            Session["clientusernme"] = user; // session

            using (var db = new MySqlConnection(connDB))
            {

                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM STAFF WHERE STAFF_USER = '" + user + "' AND STAFF_PASS = '" + pass + "'";
                    MySqlDataReader rd = cmd.ExecuteReader();

                    if (rd.Read())
                    {
                        data.Add(new
                        {
                            msg = 0
                        });
                    }
                    else
                    {
                        data.Add(new
                        {
                            msg = 1
                        });
                    }
                }
            }




            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ProductEntry()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProductEntry(FormCollection collection, HttpPostedFileBase uploadImg)
        {
            var itmcde = Convert.ToString(collection["txtCode"]);
            var itmname = Request["txtname"];
            var itmdesc = Request["txtDesc"];
            var itmprce = Request["txtprice"];
            var itmonhand = Request["txtonhand"];
            var date = Request["datepicker"];

            if (uploadImg != null)
            {
                try
                {
                    string filename = Path.GetFileName(uploadImg.FileName);
                    var checkextension = Path.GetExtension(uploadImg.FileName).ToUpper();
                    int filesize = uploadImg.ContentLength;
                    string logPath = "C:\\Uploads";
                    string filepath = Path.Combine(logPath, filename);
                    uploadImg.SaveAs(filepath);
                    using (var db = new MySqlConnection(connDB))
                    {
                        db.Open();
                        using (var cmd = db.CreateCommand())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = "INSERT INTO PRODTBL (ITMNUM,ITMNAME,ITMDESC,ITMIMG,ITMSIZE,ITMPRICE,ITMDATE)"
                                + " VALUES ("
                                + " @NUM,"
                                + " @NAME,"
                                + " @DESC,"
                                + " @IMG,"
                                + " @SIZE,"
                                + " @PRICE,"
                                + " @DATE)";
                            cmd.Parameters.AddWithValue("@NUM", itmcde);
                            cmd.Parameters.AddWithValue("@NAME", itmname);
                            cmd.Parameters.AddWithValue("@DESC", itmdesc);
                            cmd.Parameters.AddWithValue("@IMG", filename);
                            cmd.Parameters.AddWithValue("@SIZE", itmonhand);
                            cmd.Parameters.AddWithValue("@PRICE", itmprce);
                            cmd.Parameters.AddWithValue("@DATE", date);
                            var ctr = cmd.ExecuteNonQuery();
                            if (ctr >= 1)
                            {
                                Response.Write("<script>alert('Data Save')</script>");
                            }
                            else
                                Response.Write("<script>alert('Data Not Save')</script>");
                        }
                    }

                }
                catch (Exception ex)
                {

                }
            }

            return View();
        }


        [HttpGet]
        public FileResult Image(string filename)
        {
            var folder = "";
            var filepath = "";

            try
            {
                folder = "C:\\Uploads";
                filepath = Path.Combine(folder, filename);
                if (!System.IO.File.Exists(filepath))
                {
                    //image not found here
                }
            }
            catch (Exception)
            {
                //throw;
            }

            var mime = System.Web.MimeMapping.GetMimeMapping(Path.GetFileName(filepath));
            Response.Headers.Add("Content-Disposition", "Inline");

            return new FilePathResult(filepath, mime);
        }

        public ActionResult getItemCode()
        {
            var data = new List<object>();
            var itmcode = DateTime.Now.Year;
            using (var db = new MySqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT MAX(ITMNUM) as 'MAXID' FROM PRODTBL";
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        data.Add(new
                        {
                            itmcode = reader["MAXID"].ToString(),
                        });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        //public ActionResult getItemCode()
        //{
        //    var data = new List<object>();
        //    var itmcode = "";
        //    var year = DateTime.Now.Year;

        //    using (var db = new MySqlConnection(connDB))
        //    {
        //        db.Open();
        //        using (var cmd = db.CreateCommand())
        //        {
        //            cmd.CommandType = CommandType.Text;
        //            cmd.CommandText = "SELECT count(Id) as result FROM ITMTBLL";
        //            MySqlDataReader reader = cmd.ExecuteReader();


        //            if (reader.Read())
        //            {
        //                var result = year + "00" + reader["result"].ToString();
        //                Response.Write(result);

        //                cmd.CommandType = System.Data.CommandType.Text;
        //                cmd.CommandText = "INSERT INTO ITMTBLL (Id)"
        //                     + " VALUES ( @NUM)";
        //                cmd.Parameters.AddWithValue("@NUM", result);

        //                reader.Close();
        //                var ctr = cmd.ExecuteNonQuery();
        //                if (ctr >= 1)
        //                {
        //                    Response.Write("<script>alert('success')</script>");
        //                }
        //                else
        //                    Response.Write("<script>alert('Data Not Save')</script>");

        //            }



        //        }
        //    }
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult ProductList()
        {
            return View();
        }


        public ActionResult ListProd()
        {
            var data = new List<object>();
            var itmcode = Request["itemNum"].Trim();
            var itmname = Request["ItemName"].Trim();

            var price = Request["itemPrice"].Trim();
            var size = Request["ItemSize"].Trim();
            var image = Request["ItemImg"].ToString();
            var itmsdesc = Request["ItemDesc"].Trim();
            var date = DateTime.Now;

            var user = Session["clientusernme"].ToString();//session being pass from userlogin
            /*
                        //trial
                        Session["CartTotal"] = total; // session
                        Session["CartQty"] = qty; // session
                        Session["CartPrice"] = price; // session
                        Session["CartImage"] = image; // session
            */

            ////trial
            //var itemsleft = Request["ItemOnhand"].Trim(); // session to pass to update number of items on category
            //Session["ItemOnhand"] = itemsleft;



            using (var db = new MySqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO PRODTBLHS (ITMNUM,ITMNAME,ITMDESC,ITMIMG,ITMSIZE,ITMPRICE,ITMDATE)"
                                 + " VALUES ("
                                 + " @NUM,"
                                 + " @NAME,"
                                 + " @DESC,"
                                 + " @IMG,"
                                 + " @SIZE,"
                                 + " @PRICE,"
                                 + " @DATE)";
                    cmd.Parameters.AddWithValue("@NUM", itmcode);
                    cmd.Parameters.AddWithValue("@NAME", itmname);
                    cmd.Parameters.AddWithValue("@DESC", itmsdesc);
                    cmd.Parameters.AddWithValue("@IMG", image);
                    cmd.Parameters.AddWithValue("@SIZE", size);
                    cmd.Parameters.AddWithValue("@PRICE", price);
                    cmd.Parameters.AddWithValue("@DATE", date);



                    var ctr = cmd.ExecuteNonQuery();
                    if (ctr >= 1)
                    {
                        data.Add(new
                        {
                            msg = 0,
                            //amount = total
                        });
                    }
                    else
                    {
                        Response.Write("<script>alert('Data Not Save')</script>");

                    }

                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }




        public ActionResult EditDeleteProd()
        {
            return View();
        }





        public ActionResult DeleteProduct()
        {
            var data = new List<object>();
            var itemcode = Request["itemNo"].Trim();
            using (var db = new MySqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "delete from PRODTBL where itmnum ='" + itemcode + "'";
                    var ctr = cmd.ExecuteNonQuery();
                    if (ctr >= 1)
                    {
                        data.Add(new
                        {
                            msg = 0
                        });
                    }
                    else
                    {
                        data.Add(new
                        {
                            msg = 1
                        });
                    }



                    return Json(data, JsonRequestBehavior.AllowGet);

                }
            }
        }



        public ActionResult Edit()
        {

            var data = new List<object>();
            var itemcode = Request["itemCode"].Trim();
            Session["code"] = itemcode;

            using (var db = new MySqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select * from PRODTBL where itmnum ='" + itemcode + "'";
                    MySqlDataReader rd = cmd.ExecuteReader();

                    if (rd.Read())
                    {
                        data.Add(new
                        {
                            msg = 0
                        });
                    }
                    else
                    {
                        data.Add(new
                        {
                            msg = 1
                        });
                    }



                    return Json(data, JsonRequestBehavior.AllowGet);

                }
            }
        }





        public ActionResult UpdateProduct()
        {
            return View();
        }



        public ActionResult getcode()
        {
            var data = new List<object>();
            var code = Session["code"].ToString();



            data.Add(new
            {

                msg = 0,
                itmcode = code
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }



        public ActionResult SearchItem()
        {
            var data = new List<object>();
            var itmcode = Request["itemcode"];
            using (var db = new MySqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select * from PRODTBL where itmnum='" + itmcode + "'";
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        data.Add(new
                        {
                            mess = 0,
                            desc = reader["itmdesc"].ToString(),
                            price = reader["itmprice"].ToString(),
                            product = reader["itmname"].ToString(),
                            qty = reader["itmsize"].ToString(),
                        });
                    }
                    else
                    {
                        data.Add(new
                        {
                            mess = 1,
                        });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        public ActionResult UpdateItem()
        {
            var data = new List<object>();

            var itmcode = Request["itemcode"];
            var itmdesc = Request["itemdesc"];
            var itmprice = Request["itemprice"];
            var itmname = Request["itemname"];
            var itmqty = Request["itemqty"];


            using (var db = new MySqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE PRODTBL SET "
                        + " ITMNAME ='" + itmname + "',"
                        + " ITMDESC ='" + itmdesc + "',"
                        + " ITMSIZE ='" + itmqty + "',"
                        + " ITMPRICE ='" + itmprice + "'"
                        + " WHERE ITMNUM='" + itmcode + "'";
                    var ctr = cmd.ExecuteNonQuery();
                    if (ctr > 0)
                    {
                        data.Add(new
                        {
                            mess = 0,
                            code = itmcode
                        });
                    }
                    else
                    {
                        data.Add(new
                        {
                            mess = 1
                        });
                    }
                }
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        //for searching category
        public ActionResult HomeCategory()
        {
            var data = new List<object>();
            var selectedcategory = Request["select"];

            Session["category"] = selectedcategory;

            data.Add(new
            {
                msg = 0
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }




        public ActionResult Order()
        {
            var data = new List<object>();
            var prodcode = Request["ProdNum"].Trim();
            var prodname = Request["ProdName"].Trim();
            var category = Request["ProdCategory"].Trim();
            var price = Request["ProdPrice"].Trim();
            var size = Request["ProdSize"].Trim();

            //var image = Request["ItemImg"].ToString();
            //var productname = Request["ItemName"].Trim();
            //var user = Session["clientusernme"].ToString();//session being pass from userlogin
            var date = DateTime.Now;

            //Session["pcode"] = prodcode.ToString(); // session created
            //Session["name"] = prodname.ToString(); // session created
            //Session["category"] = category.ToString(); // session created
            //Session["price"] = price.ToString(); // session created
            //Session["size"] = size.ToString(); // session created
            //Session["date"] = date.ToString(); // session created

            var payment = "Done";


            using (var db = new MySqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO PRODORD (PRODNUM,PRODNAME,PRODCATGRY,PRODSIZE,PRODPRICE,PRODATEADDED)"
                                + " VALUES ("
                                + " @NUM,"
                                + " @NAME,"
                                + " @DESC,"
                                + " @SIZE,"
                                + " @PRICE,"
                                + " @DATE)";



                    cmd.Parameters.AddWithValue("@NUM", prodcode);
                    cmd.Parameters.AddWithValue("@NAME", prodname);
                    cmd.Parameters.AddWithValue("@DESC", category);
                    cmd.Parameters.AddWithValue("@SIZE", size);
                    cmd.Parameters.AddWithValue("@PRICE", price);
                    cmd.Parameters.AddWithValue("@DATE", date);

                    


                    var ctr = cmd.ExecuteNonQuery();

                    if (ctr >= 1)
                    {


                        data.Add(new
                        {
                            msg = 0,
                            //amount = total
                        });
                    }
                    else
                    {
                        Response.Write("<script>alert('Data Not Save')</script>");

                    }

                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Trial()
        {
            return View();
        }

        public ActionResult Payment()
        {
            return View();
        }

        public ActionResult Sales()
        {
            return View();
        }

        public ActionResult ManageAccount()
        {
            return View();
        }

        public ActionResult Discounts()
        {
            return View();
        }

        public ActionResult TotalBill()
        {
            var data = new List<object>();
            var total = Request["TotalBill"].Trim();
           

            //var image = Request["ItemImg"].ToString();
            //var productname = Request["ItemName"].Trim();
            //var user = Session["clientusernme"].ToString();//session being pass from userlogin
            var date = DateTime.Now;

            Session["totPayment"] = total; // session created
         

            var payment = "Done";
            //SELECT SUM(Quantity)
           // FROM OrderDetails

            using (var db = new MySqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT SUM (PRODPRICE) FROM PRODORD ";






                    var ctr = cmd.ExecuteNonQuery();
                    if (ctr >= 1)
                    {
                        data.Add(new
                        {
                            msg = 0,
                            //amount = total
                        });
                    }
                    else
                    {
                        Response.Write("<script>alert('Data Not Save')</script>");

                    }

                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult getTotalBill()
        {
            var data = new List<object>();
            var payment = "";
            Session["Totalpayemnt"] = payment; // session
            using (var db = new MySqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                //  cmd.CommandText = "SELECT SUM (PRODPRICE) as PAY FROM PRODORD ";

                    cmd.CommandText = "SELECT SUM(PRODPRICE) as PAY FROM PRODORD";
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        data.Add(new
                        {
                            payment = reader["PAY"].ToString(),
                        });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        public ActionResult sessionPayment()
        {
            var data = new List<object>();
            var sessiontotal = Session["Totalpayemnt"].ToString();
           



            data.Add(new
            {

                msg = 0,
                totpayment = sessiontotal
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }



        public ActionResult getTotalQty()
        {
            var data = new List<object>();
            var quantity = "";
            Session["TotalQTY"] = quantity; // session

            using (var db = new MySqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    //  cmd.CommandText = "SELECT SUM (PRODPRICE) as PAY FROM PRODORD ";

                    cmd.CommandText = "SELECT COUNT(DISTINCT PRODNUM) as qty FROM PRODORD";
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        data.Add(new
                        {
                            quantity = reader["qty"].ToString(),
                        });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        /*
                public ActionResult getTotal()
                {
                    var data = new List<object>();
                    var total = Session["totpayment"].ToString();



                    data.Add(new
                    {

                        msg = 0,
                        payment = total
                    });

                    return Json(data, JsonRequestBehavior.AllowGet);
                }

                */



        public ActionResult getOrder()
        {
            var data = new List<object>();





            var prodcode = Request["ProdNum"].Trim();
            var prodname = Request["ProdName"].Trim();
            var category = Request["ProdCategory"].Trim();
            var price = Request["ProdPrice"].Trim();
            var size = Request["ProdSize"].Trim();

            var date = DateTime.Now;




            var payment = "Done";


            using (var db = new MySqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO ORDR (PRODNUM,PRODNAME,PRODCATGRY,PRODSIZE,PRODPRICE,PRODATEADDED)"
                                + " VALUES ("
                                + " @NUM,"
                                + " @NAME,"
                                + " @DESC,"
                                + " @SIZE,"
                                + " @PRICE,"
                                + " @DATE)";



                    cmd.Parameters.AddWithValue("@NUM", prodcode);
                    cmd.Parameters.AddWithValue("@NAME", prodname);
                    cmd.Parameters.AddWithValue("@DESC", category);
                    cmd.Parameters.AddWithValue("@SIZE", size);
                    cmd.Parameters.AddWithValue("@PRICE", price);
                    cmd.Parameters.AddWithValue("@DATE", date);




                    var ctr = cmd.ExecuteNonQuery();
                    if (ctr >= 1)
                    {
                        data.Add(new
                        {
                            msg = 0,
                            //amount = total
                        });
                    }
                    else
                    {
                        Response.Write("<script>alert('Data Not Save')</script>");

                    }

                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }




        public ActionResult DeleteOrdd()
        {
            var data = new List<object>();
          //  var itemcode = Request["itemNo"].Trim();
            using (var db = new MySqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "DELETE * FROM PRODORD";

                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        data.Add(new
                        {
                            msg = 0
                        });
                    }


                    return Json(data, JsonRequestBehavior.AllowGet);

                }
            }
        }




        public ActionResult DeleteOrd()
        {
            var data = new List<object>();
          //  var quantity = "";
         //   Session["TotalQTY"] = quantity; // session

            using (var db = new MySqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    //  cmd.CommandText = "SELECT SUM (PRODPRICE) as PAY FROM PRODORD ";

                    cmd.CommandText = "DELETE FROM PRODORD";
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        data.Add(new
                        {
                            msg=0
                        });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }





        public ActionResult PaymentTrans()
        {
            var data = new List<object>();
            var totamount = Request["PymentTotAmnt"].Trim();
            var totdiscnt = Request["PaymentTotDscnt"].Trim();
            var moneyamount = Request["PaymentAmnt"].Trim();
            var cardID = Request["PaymentCardID"].Trim();
            var cardName = Request["PaymentCardName"].Trim();
            var change = Request["PaymentChange"].Trim();
            var paymntmethd = Request["PaymentMethd"].Trim();
            var dscntmethd = Request["PaymentDscntMethd"].Trim();

            //var image = Request["ItemImg"].ToString();
            //var productname = Request["ItemName"].Trim();
            //var user = Session["clientusernme"].ToString();//session being pass from userlogin
            var date = DateTime.Now;

            //Session["pcode"] = prodcode.ToString(); // session created
            //Session["name"] = prodname.ToString(); // session created
            //Session["category"] = category.ToString(); // session created
            //Session["price"] = price.ToString(); // session created
            //Session["size"] = size.ToString(); // session created
            //Session["date"] = date.ToString(); // session created

            var payment = "Done";


            using (var db = new MySqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO PAYMENTRANS (PYMNTAMNT,PYMNTDISCNT,PYMNTMONEYAMNT,PYMNTCARDID,PYMNTCARDNAME,PYMNTCHANGE,PYMNTMETHOD,PYMNTDSCNTMETHOD,PYMNTRPRTDATE)"
                                + " VALUES ("
                                + " @TOTAMNT,"
                                + " @DSCNT,"
                                + " @AMOUNT,"
                                + " @CARDID,"
                                + " @CARDNAME,"
                                + " @CHANGE,"
                                + " @PAYMNTMETHOD,"
                                + " @DISCMETHOD,"
                                + " @DATE)";



                    cmd.Parameters.AddWithValue("@TOTAMNT", totamount);
                    cmd.Parameters.AddWithValue("@DSCNT", totdiscnt);
                    cmd.Parameters.AddWithValue("@AMOUNT", moneyamount);
                    cmd.Parameters.AddWithValue("@CARDID", cardID);
                    cmd.Parameters.AddWithValue("@CARDNAME", cardName);
                    cmd.Parameters.AddWithValue("@CHANGE", change);
                    cmd.Parameters.AddWithValue("@PAYMNTMETHOD", paymntmethd);
                    cmd.Parameters.AddWithValue("@DISCMETHOD", dscntmethd);
                    cmd.Parameters.AddWithValue("@DATE", date);




                    var ctr = cmd.ExecuteNonQuery();

                    if (ctr >= 1)
                    {


                        data.Add(new
                        {
                            msg = 0,
                            //amount = total
                        });
                    }
                    else
                    {
                        Response.Write("<script>alert('Data Not Save')</script>");

                    }

                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
























    }


}