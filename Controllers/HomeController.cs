using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Taste_Hub.Models;
using Microsoft.Data.SqlClient;
namespace Taste_Hub.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    public string constr = "Data Source=LAPTOP-8NRV95P1;Initial Catalog=\"Taste Hub\";Persist Security Info=True;User ID=sa;Password=admin;Trust Server Certificate=True";
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    public IActionResult Contact_Us()
    {
        return View();
    }
    public IActionResult About_Us()
    {
        return View();
    }
    public IActionResult Order()
    {
        return View();
    }
    public IActionResult Feedback()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Feedback(Class_Feedback obj)
    {
        SqlConnection con = new SqlConnection(constr);
        con.Open();
        SqlCommand cmd = new SqlCommand("insert into Feedback values('" + obj.Username + "','" + obj.Email + "','" + obj.Message + "')", con);
        cmd.ExecuteNonQuery();
        con.Close();
        return View();
    }
    public IActionResult Index()
    {
        string? username = HttpContext.Session.GetString("unm");
        ViewBag.Session=username;
        List<Class_Admin_Food> lst = new List<Class_Admin_Food>();  
        SqlConnection con=new SqlConnection(constr);
        con.Open();
        SqlCommand cmd = new SqlCommand("select * from Food", con);
        SqlDataReader dr = cmd.ExecuteReader();
        while(dr.Read())
        {
            Class_Admin_Food obj = new Class_Admin_Food();
            obj.Id = Convert.ToInt32(dr[0].ToString());
            obj.Name = dr[1].ToString();
            obj.Description = dr[2].ToString();
            obj.Category = dr[3].ToString();
            obj.SubCategory = dr[4].ToString();
            obj.Image = dr[5].ToString();
            obj.Price = dr[6].ToString();
            lst.Add(obj);
        }
        return View(lst);
    }

    public IActionResult view(int Id)
    {
        List<Class_Admin_Food> lst = new List<Class_Admin_Food>();
        SqlConnection con = new SqlConnection(constr);
        con.Open();
        SqlCommand cmd = new SqlCommand("select * from Food Where Id='" + Id + "'",con);
        SqlDataReader dr = cmd.ExecuteReader();
        if(dr.Read())
        {
            Class_Admin_Food obj = new Class_Admin_Food();
            obj.Id = Convert.ToInt32(dr[0].ToString());
            obj.Name = dr[1].ToString();
            obj.Description = dr[2].ToString();
            obj.Category = dr[3].ToString();
            obj.SubCategory = dr[4].ToString();
            obj.Image = dr[5].ToString();
            obj.Price = dr[6].ToString();
            lst.Add(obj);
        }
        return View(lst);
    }
    public IActionResult Product(string name)
    {
        List<Class_Admin_Food> lst = new List<Class_Admin_Food>();
        SqlConnection con = new SqlConnection(constr);
        con.Open();
        SqlCommand cmd = new SqlCommand("select * from Food where Category='"+ name + "'", con);
        SqlDataReader dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            Class_Admin_Food obj = new Class_Admin_Food();
            obj.Id = Convert.ToInt32(dr[0].ToString());
            obj.Name = dr[1].ToString();
            obj.Description = dr[2].ToString();
            obj.Category = dr[3].ToString();
            obj.SubCategory = dr[4].ToString();
            obj.Image = dr[5].ToString();
            obj.Price = dr[6].ToString();
            lst.Add(obj);
        }
        return View(lst);
    }
    public IActionResult Login(int? id,string cart)
    {
        if (HttpContext.Session.GetString("unm") == null)
        {
            return RedirectToAction("Login", new { cart = "cart" });
        }
        else
        {
            ViewBag.ProductId = id;
            return View();
        }
           
    }
    [HttpPost]
    public IActionResult Login(Class_Register obj,int? id, string cart)  
    {
      
        SqlConnection con = new SqlConnection(constr);
        con.Open();
        SqlCommand cmd = new SqlCommand("select * from Register where Username='" + obj.Username + "' and Password='" + obj.Password + "'", con);
        SqlDataReader dr = cmd.ExecuteReader();
        if(dr.Read())
        {
            HttpContext.Session.SetString("unm",obj.Username);
            ViewBag.ClientLogin = "Login Success";
            if(id.HasValue)
            {
                return RedirectToAction("AddToCart", new { id = id });
            }
            else if(cart=="cart")
            {
                return RedirectToAction("Cart");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        else
        {
            ViewBag.ClientLogError = "Invalid Username or Password";
            return View();
        }
    }
    public IActionResult Registration()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Registration(Class_Register obj)
    {
        SqlConnection con = new SqlConnection(constr);
        con.Open();
        SqlCommand cmd = new SqlCommand("insert into Register values ('" + obj.Username + "','" + obj.City + "','" + obj.Mobile + "','" + obj.Password + "')", con);
        cmd.ExecuteNonQuery();
        con.Close();
        return View("Registration");
    }
    public IActionResult AddToCart(int id)
    {
        if (HttpContext.Session.GetString("unm") == null)
        {
            return RedirectToAction("Login", new {id=id});
        }
        else
        {
            SqlConnection con = new SqlConnection(constr);
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Food where Id='" + id + "'", con);
            SqlDataReader dr = cmd.ExecuteReader();
            string name = "", desc = "", cate = "", subcate = "", imag = "", price = "";
            string user = HttpContext.Session.GetString("unm");
            if (dr.Read())
            {
                name = dr[1].ToString();
                desc = dr[2].ToString();
                cate = dr[3].ToString();
                subcate = dr[4].ToString();
                imag = dr[5].ToString();
                price = dr[6].ToString();
            }
            dr.Close();
            SqlCommand cmd2 = new SqlCommand("insert into Cart values(@user,@name,@desc,@cate,@subcate,@imag,@price)", con);
            cmd2.Parameters.AddWithValue("@user", user);
            cmd2.Parameters.AddWithValue("@name", name);
            cmd2.Parameters.AddWithValue("@desc", desc);
            cmd2.Parameters.AddWithValue("@cate", cate);
            cmd2.Parameters.AddWithValue("@subcate", subcate);
            cmd2.Parameters.AddWithValue("@imag", imag);
            cmd2.Parameters.AddWithValue("@price", price);
            cmd2.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("Cart");
        }
    }
    public IActionResult Cart(string Cart)
    {
        if(HttpContext.Session.GetString("unm") == null)
        {
            return RedirectToAction("Login", new {cart="cart"});
        }
        else
        {
            int total = 0;
            int cartcount = 0;
            SqlConnection con = new SqlConnection(constr);
            con.Open();
            List<Class_Cart> lst = new List<Class_Cart>();
            SqlCommand cmd = new SqlCommand("select * from Cart where Username='" + HttpContext.Session.GetString("unm") + "'", con);
            SqlDataReader dr2 = cmd.ExecuteReader();
            while(dr2.Read())
            {
                Class_Cart obj = new Class_Cart();
                obj.Id = Convert.ToInt32(dr2[0].ToString());
                obj.Username = dr2[1].ToString();
                obj.Name = dr2[2].ToString();
                obj.Description = dr2[3].ToString();
                obj.Category = dr2[4].ToString();
                obj.SubCategory = dr2[5].ToString();
                obj.Image = dr2[6].ToString();
                obj.Price = dr2[7].ToString();
                total = total + Convert.ToInt32(dr2[7].ToString());
                cartcount = cartcount + 1;
                lst.Add(obj);
            }
            ViewBag.Total = total;
            ViewBag.CartCount = cartcount;
            dr2.Close();
            con.Close();
            return View(lst);
        }
    }
    [HttpPost]
    public IActionResult cartdel(Class_Cart obj)
    {
        SqlConnection con = new SqlConnection(constr);
        con.Open();
        SqlCommand cmd=new SqlCommand("delete from Cart where Id='" + obj.Id + "'", con);
        cmd.ExecuteNonQuery();
        con.Close();
        return RedirectToAction("Cart");
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
