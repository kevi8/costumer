using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using costumer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace costumer.Controllers;
// Name this anything you want with the word "Attribute" at the end
public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Find the session, but remember it may be null so we need int?
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        // Check to see if we got back null
        if(userId == null)
        {
            // Redirect to the Index page if there was nothing in session
            // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
            context.Result = new RedirectToActionResult("Register", "Home", null);
        }
    }
}



public class HomeController : Controller
{
    private MyContext _context; 
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger,MyContext context)
    {
        _logger = logger;
         _context = context;  
    }
[SessionCheck]
[HttpGet("")]
public IActionResult Index()
{
    ViewBag.userId = HttpContext.Session.GetInt32("UserId");
    ViewBag.Costumer = _context.Costumers;
    List<Costumer> costumers= _context.Costumers.ToList();
    return View("Index" , costumers);
}

[HttpGet("Register")]
public IActionResult Register()
{
    return View("Register");
}

[HttpPost("registration")]   
    public IActionResult CreateUser(Costumer newUser)    
    {        
        if(ModelState.IsValid)        
        {           
            //Save your user object to the database 
            _context.Add(newUser);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            return RedirectToAction("Index");       
        } else {
            return View("Register");  
        }   
    }
    [HttpPost("login")]
    public IActionResult Login(Login userSubmission)
    {    
        if(ModelState.IsValid)    
        {        
            // If initial ModelState is valid, query for a user with the provided email        
            Costumer? userInDb = _context.Costumers.FirstOrDefault(u => u.Email == userSubmission.LoginEmail);        
            // If no user exists with the provided email        
            if(userInDb == null)        
            {            
                // Add an error to ModelState and return to RedirectToAction!            
                ModelState.AddModelError("LoginEmail", "Invalid Email");            
                return View("Register");        
            }   
            HttpContext.Session.SetInt32("UserId", userInDb.UserId);
            
            return RedirectToAction( "Index");
        } else 
        {
            return View("Register");
        }
    }


        [HttpGet("Edit/{id}")]
    public IActionResult Edit(int id){
        ViewBag.userId = HttpContext.Session.GetInt32("UserId");
        Costumer? Costum = _context.Costumers.FirstOrDefault(e=> e.UserId == id );
        return View(Costum);
    }

     [HttpPost("EditCostumer/{id}")]
         public IActionResult EditCostumer(Costumer costumer, int id){
        if(ModelState.IsValid){
        Costumer? editCostumer = _context.Costumers.FirstOrDefault(e => e.UserId == id);
        editCostumer.FirstName = editCostumer.FirstName;
        _context.SaveChanges();
        int? idd = costumer.UserId;
        return RedirectToAction("Edit", new {id=idd});
        }
        else{
            costumer.UserId = id;
            return View("Edit", costumer);
        }
    }


       [SessionCheck]
    [HttpGet("Delete/{UserId}")]
    public IActionResult Delete(int UserId){
        Costumer costumer = _context.Costumers.FirstOrDefault(e=>e.UserId==UserId);
        _context.Remove(costumer);
        _context.SaveChanges();
        return View("Register");
    }
        [HttpPost("/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Register");
    }
}