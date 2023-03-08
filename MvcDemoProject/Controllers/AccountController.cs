using MvcDemoProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MvcDemoProject.Controllers
{

    public class AccountController : Controller
    {
        SqlConnection con = new SqlConnection("Server=ELdRiDg3-PC\\SQLEXPRESS;Database=LoginRegistrationInMVC;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;");

        // GET: UserInfo
        //decorated with Authorize filter you can not able access this method without login
        [Authorize]
        public ActionResult UserInfo()
        {
            return View();
        }
        // GET:Login this Action method simple return the Login View
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        //Post:When user click on the submit button then this method will call
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel LoginViewModel)
        {
            if (IsValidUser(LoginViewModel.Email, LoginViewModel.Password))
            {
                FormsAuthentication.SetAuthCookie(LoginViewModel.Email, false);
                return RedirectToAction("UserInfo", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Your Email and password is incorrect");
            }
            return View(LoginViewModel);
        }
        // GET:Register return view
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        // Post:Register 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegistrationViewModel RegistrationViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!IsUserExist(RegistrationViewModel.Email))
                    {
                        string query = "select * from Users where Email=@email and Password=@Password";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Connection = con;
                            cmd.Parameters.AddWithValue("@Name", RegistrationViewModel.Name);
                            cmd.Parameters.AddWithValue("@Email", RegistrationViewModel.Email);
                            // encode password i'm using simple base64 you can use any more secure algo
                            cmd.Parameters.AddWithValue("@Password", Base64Encode(RegistrationViewModel.Password));
                            cmd.Parameters.AddWithValue("@ContactNo", RegistrationViewModel.ContactNo);
                            con.Open();
                            int i = cmd.ExecuteNonQuery();
                            con.Close();
                            if (i > 0)
                            {
                                FormsAuthentication.SetAuthCookie(RegistrationViewModel.Email, false);
                                return RedirectToAction("UserInfo", "Account");
                            }
                            else
                            {
                                ModelState.AddModelError("", "something went wrong try later!");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "User with same email already exist!");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Data is not correct");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return View();
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private bool IsUserExist(string email)
        {
            bool IsUserExist = false;
            string query = "select * from Users where Email=@email and Password=@Password";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@email", email);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    IsUserExist = true;
                }
            }
            return IsUserExist;
        }
        private bool IsValidUser(string email, string password)
        {
            var encryptpassowrd = Base64Encode(password);
            bool IsValid = false;
            string query = "select * from Users where Email=@email and Password=@Password";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", encryptpassowrd);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    IsValid = true;
                }
            }
            return IsValid;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}