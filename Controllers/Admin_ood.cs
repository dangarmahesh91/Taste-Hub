    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
    using Microsoft.Data.SqlClient;
    using Taste_Hub.Models;

    namespace Taste_Hub.Controllers
    {
        public class Admin_ood : Controller
        {
            public string constr = "Data Source=LAPTOP-8NRV95P1;Initial Catalog=\"Taste Hub\";User ID=sa;Password=admin;Trust Server Certificate=True";
            public IActionResult Index()
            {
                return View();
            }
            [HttpPost]
            public IActionResult Index(Class_Admin_Register obj1)
            {
                SqlConnection con = new SqlConnection(constr);
                con.Open();
                SqlCommand cmd = new SqlCommand("select * from AdminRegister where Username=@u and Password=@p", con);
                cmd.Parameters.AddWithValue("@u", obj1.Username);
                cmd.Parameters.AddWithValue("@p", obj1.Password);
                SqlDataReader dr = cmd.ExecuteReader();
                if(dr.Read())
                {
                    HttpContext.Session.SetString("adminunm", obj1.Username);
                    ViewBag.Adminlog="Data Inserted Success";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ViewBag.Error = "Login Failed";
                    return View();
                }              
            }
            public IActionResult Register()
            {
                return View();
            }
            [HttpPost]
            public IActionResult Register(Class_Admin_Register obj)
            {
                SqlConnection con = new SqlConnection(constr);
                con.Open();
                SqlCommand cmd = new SqlCommand("insert into AdminRegister values ('" + obj.Username + "','" + obj.Password + "')", con);
                cmd.ExecuteNonQuery();
                //TempData["registermsg"] = "Registration Success......";
                con.Close();
                return RedirectToAction("Index","Admin_ood");
            }
            public IActionResult Dashboard()
            {
                string? username = HttpContext.Session.GetString("adminunm");
                ViewBag.adminnm = username;
                return View();
            }
            [HttpGet]
            public IActionResult Category()
            {
                List<Class_Admin_Category> lst = new List<Class_Admin_Category>();
                SqlConnection con = new SqlConnection(constr);
                con.Open();
                SqlCommand cmd = new SqlCommand("select * from Category", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    Class_Admin_Category obj = new Class_Admin_Category();
                    obj.Id = Convert.ToInt32(dr["Id"].ToString());
                    obj.Name = dr["Name"].ToString();
                    obj.Image = dr[2].ToString();
                    lst.Add(obj);
                }
                return View(lst);
            }
            [HttpPost]
            public IActionResult Category(Class_Admin_Category obj,IFormFile Image)
            {
                if (Image != null && Image.Length > 0)
                {
                    //1. Filename
                    string filename = Path.GetFileName(Image.FileName);
                    //2.Folder Name
                    string folder= Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", filename);
                    //3.Path
                    string filepath = Path.Combine(folder, filename);
                    using(var stream=new FileStream(folder,FileMode.Create))
                    {
                        Image.CopyTo(stream);
                    }
                        obj.Image = "/Images/" + filename;
                    SqlConnection con = new SqlConnection(constr);
                    con.Open();
                    SqlCommand cmd = new SqlCommand("insert into Category(Name,Image) values(@Name,@Image)", con);
                    cmd.Parameters.AddWithValue("@Name", obj.Name);
                    cmd.Parameters.AddWithValue("@Image", obj.Image);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                return RedirectToAction("Category");
            }
            [HttpPost]
            public IActionResult catdelete(Class_Admin_Category obj)
            {
                SqlConnection con = new SqlConnection(constr);
                con.Open();
                SqlCommand cmd = new SqlCommand("delete from Category where Id=@Id", con);
                cmd.Parameters.AddWithValue("@Id", obj.Id);
                cmd.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("Category");
            }
            [HttpPost]
            public IActionResult categoryupdate(Class_Admin_Category obj,IFormFile Image)
            {
                if(Image!=null && Image.Length>0)
                {
                    string filename= Path.GetFileName(Image.FileName);
                    string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", filename);
                    string filepath = Path.Combine(folder, filename);
                    string mainpath=Path.GetFullPath(folder);
                    using (var stream=new FileStream(folder,FileMode.Create))
                    {
                        Image.CopyTo(stream);
                    }
                    obj.Image = "/Images/" + filename;
                }
                SqlConnection con = new SqlConnection(constr);
                con.Open();
                SqlCommand cmd = new SqlCommand("update Category set Name='" + obj.Name + "',Image='" + obj.Image + "' where Id='" + obj.Id + "'", con);
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("Category");
            }
        
            [HttpGet]
            public IActionResult Food()
            {
                List<Class_Admin_Food> lst = new List<Class_Admin_Food>();
                SqlConnection con = new SqlConnection(constr);
                con.Open();
                SqlCommand cm = new SqlCommand("select * from Food", con);
                SqlDataReader dr = cm.ExecuteReader();
                while(dr.Read())
                {
                    Class_Admin_Food obj=new Class_Admin_Food();
                    obj.Id = Convert.ToInt32(dr[0].ToString());
                    obj.Name = dr[1].ToString();
                    obj.Description = dr[2].ToString();
                    obj.Category = dr[3].ToString();
                    obj.SubCategory = dr[4].ToString();
                    obj.Image = dr[5].ToString();
                    obj.Price = dr[6].ToString();
                    lst.Add(obj);
                }
                dr.Close();
                con.Close();
                return View(lst);
            }
            [HttpPost]
            public IActionResult Food(Class_Admin_Food obj,IFormFile Image)
            {
                if(Image!=null && Image.Length>0)
                {
                    //1.filename
                    string filename=Path.GetFileName(Image.FileName);
                    //2.folder
                    string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", filename);
                    //3.path
                    string filepath = Path.Combine(folder, filename);
                    using(var stream=new FileStream(folder,FileMode.Create))
                    {
                        Image.CopyTo(stream);
                    }
                    obj.Image = "/Images/" + filename;
                    SqlConnection con = new SqlConnection(constr);
                    con.Open();
                    SqlCommand cmd = new SqlCommand("insert into Food(Name,Description,Category,SubCategory,Image,Price) values(@Name,@Description,@Category,@SubCategory,@Image,@Price)", con);
                    cmd.Parameters.AddWithValue("@Name", obj.Name);
                    cmd.Parameters.AddWithValue("@Description", obj.Description);
                    cmd.Parameters.AddWithValue("@Category", obj.Category);
                    cmd.Parameters.AddWithValue("@SubCategory", obj.SubCategory);
                    cmd.Parameters.AddWithValue("@Image", obj.Image);
                    cmd.Parameters.AddWithValue("@Price", obj.Price);
                    cmd.ExecuteNonQuery();
                    TempData["foodinsert"]="Food Added Successfully...";
                    con.Close();
                }
                return RedirectToAction("Food");
            }
            [HttpPost]
            public IActionResult fooddelete(Class_Admin_Food obj)
            {
                SqlConnection con = new SqlConnection(constr);
                con.Open();
                SqlCommand cmd = new SqlCommand("delete from Food where Id='" + obj.Id + "'", con);
                cmd.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("Food");
            }
            [HttpPost]
            public IActionResult foodupdate(Class_Admin_Food obj, IFormFile Image)
            {
                    if(Image!=null && Image.Length>0)
                    {
                        string filename = Path.GetFileName(Image.FileName);
                        string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", filename);
                        string filepath = Path.Combine(folder, filename);
                        using (var stream = new FileStream(folder,FileMode.Create))
                        {
                            Image.CopyTo(stream);
                        }
                        obj.Image = "/Images/" + filename;
                    }
                    SqlConnection con = new SqlConnection(constr);
                    con.Open();
                    SqlCommand cmd = new SqlCommand("update Food set Name='" + obj.Name + "',Description='" + obj.Description + "',Category='" + obj.Category + "',SubCategory='" + obj.SubCategory + "',Image='" + obj.Image + "',Price='" + obj.Price + "' where Id='" + obj.Id + "'", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    return RedirectToAction("Food");
            }
            public IActionResult Users()
            {
                return View();
            }
            [HttpGet]
            public IActionResult Feedback()
            {
                List<Class_Feedback> lst = new List<Class_Feedback>();
                SqlConnection con = new SqlConnection(constr);
                con.Open();
                SqlCommand cmd = new SqlCommand("select * from Feedback", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    Class_Feedback obj = new Class_Feedback();
                    obj.Id = Convert.ToInt32(dr[0].ToString());
                    obj.Username = dr[1].ToString();
                    obj.Email = dr[2].ToString();
                    obj.Message = dr[3].ToString();
                    lst.Add(obj);
                }
                dr.Close();
                con.Close();
                return View(lst);
            }
            public IActionResult Logout()
            {
                HttpContext.Session.Remove("adminunm");
                return RedirectToAction("Index","Admin_ood");
            }
        }
    }
