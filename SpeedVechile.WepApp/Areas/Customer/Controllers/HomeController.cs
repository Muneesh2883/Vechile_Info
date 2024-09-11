using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using SpeedVechile.Application.Contracts.Presistence;
using SpeedVechile.Application.ExtensionMethods;
using SpeedVechile.Domain.Models;
using SpeedVechile.Domain.ViewModel;
using System.Diagnostics;


namespace SpeedVechile.WepApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? page,bool resetFilter=false)
        {
            IEnumerable<SelectListItem> brandList = _unitOfWork.Brand.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });

            IEnumerable<SelectListItem> vehicleTypeList = _unitOfWork.VehicleType.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });


            List<Post>posts;

            if (resetFilter)
            {
                TempData.Remove("FilteredPost");
                TempData.Remove("SelectedBrandId");
                TempData.Remove("SelectedVehicleTypeId");
            }
            if (TempData.ContainsKey("FilteredPost"))
            {
                posts = TempData.Get<List<Post>>("FilteredPost");
                TempData.Keep("FilteredPost");
            }
            else
            {
                posts = await _unitOfWork.Post.GetAllPost();
            }

            int pageSize = 6;
            int pageNumber = page ?? 1;
            int totalItems = posts.Count;
            int totalpages = (int)Math.Ceiling((double)totalItems / pageSize);

            ViewBag.TotalPages=totalpages;
            ViewBag.CurrentPage=pageNumber;
            var pagedposts=posts.Skip((pageNumber-1)*pageSize).Take(pageSize).ToList();

            HttpContext.Session.SetString("PreviousUrl", HttpContext.Request.Path);

            HomePostVM homePostVM = new HomePostVM
            {
                Posts=pagedposts,
                BrandList=brandList,
                VehicleTypeList=vehicleTypeList,
                BrandId = (Guid?)TempData["SelectedBrandId"],
                VehicleTypeId = (Guid?)TempData["SelectedVehicleTypeId"],
                
            };

            return View(homePostVM);
        }
        [HttpPost]
        public async Task<IActionResult> Index(HomePostVM homePostVM)
        {
            var post = await _unitOfWork.Post.GetAllPost(homePostVM.searchBox, homePostVM.BrandId, homePostVM.VehicleTypeId);

            TempData.Put("FilteredPost", post);
            TempData["SeletedBrandId"] = homePostVM.BrandId;
            TempData["SeletedVehicleTypeId"] = homePostVM.VehicleTypeId;

            return RedirectToAction("Index", new { page = 1, resetFilter = false });
            
        }
        [Authorize]
        public async Task<IActionResult> Details(Guid id,int? page)
        {
            Post post=await _unitOfWork.Post.GetPostById(id);
            List<Post> posts = new List<Post>();
            if (post != null)
            {
                posts = await _unitOfWork.Post.GetAllPost(post.Id,post.BrandId);
            }

            ViewBag.CurrentPage = page;

            CustomerDetailsVM customerDetailsVM = new CustomerDetailsVM
            {
                Post=post,
                Posts=posts
            };

            return View(customerDetailsVM);

            
        }
        public IActionResult GoBack(int? page)
        {
            string? previousUrl = HttpContext.Session.GetString("PreviousUrl");

            if (!string.IsNullOrEmpty(previousUrl))
            {
                if (page.HasValue)
                {
                    previousUrl = QueryHelpers.AddQueryString(previousUrl, "page", page.Value.ToString());
                }
                HttpContext.Session.Remove("previousUrl");
                return Redirect(previousUrl);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
