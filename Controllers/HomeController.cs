using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;



namespace WebApplication7.Controllers
{
    public class HomeController : Controller
    {
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

        public ActionResult addUser(FormCollection fc)
        {
            String fname = fc["firstname"];
            String lname = fc["lastname"];
            String address = fc["address"];
            String email = fc["email"];
            String password = fc["password"];
            int role = 2;

            user use = new user();
            use.firstname = fname;
            use.lastname = lname;
            use.address = address;
            use.email = email;
            use.password = password;
            use.role_Id = role;

            CrudDBEntities crud = new CrudDBEntities();
            crud.users.Add(use);
            crud.SaveChanges();
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection gd)
        {
            string email = gd["email"];
            string password = gd["password"];

            using (CrudDBEntities dbContext = new CrudDBEntities())
            {
                var user = dbContext.users.FirstOrDefault(a => a.email == email && a.password == password);

                if (user != null)
                {
                    // Store user ID and role in session
                    Session["UserId"] = user.user_Id;
                    Session["Role"] = user.role_Id;

                    if (user.role_Id == 1)
                    {
                        // Redirect to Admin page
                        return RedirectToAction("AdminPage");
                    }
                    else if (user.role_Id == 2)
                    {
                        // Redirect to Profile page
                        return RedirectToAction("ProfilePage");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Invalid role for the user.";
                        return RedirectToAction("LoginFailed");
                    }
                }

                TempData["ErrorMessage"] = "Invalid login attempt";
                return RedirectToAction("LoginFailed");
            }
        }

        public ActionResult LoginFailed()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ProfilePage()
        {

            int userId = (int)Session["UserId"];

            using (var dbContext = new CrudDBEntities())
            {
                var user = dbContext.users.Include("role").FirstOrDefault(a => a.user_Id == userId);

                if (user != null)
                {
                    return View(user);
                }
            }
            return View();
        }

        public ActionResult AdminPage()
        {
            using (CrudDBEntities dbContext = new CrudDBEntities())
            {
                var users = dbContext.users.ToList();

                if (users.Any())
                {
                    return View(users);
                }
                else
                {
                    ViewBag.Message = "No users found in the database.";
                }
            }

            return RedirectToAction("LoginFailed");
        }


        public ActionResult Logout()
        {
            // Clear user session
            Session.Clear();

            // Redirect to the login page (assuming the login action is named "Login")
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Delete(int user_Id)
        {
            using (var dbContext = new CrudDBEntities())
            {
                var user = dbContext.users.Find(user_Id);

                if (user != null)
                {
                    dbContext.users.Remove(user);
                    dbContext.SaveChanges();

                    // Assuming you want to redirect to the admin page after deletion
                    return RedirectToAction("AdminPage");
                }
                else
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("AdminPage");
                }
            }
        }

        public ActionResult ViewUsers() {
            return View();
        }


    }
}